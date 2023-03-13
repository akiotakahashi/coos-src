#include "AssemblyManager.h"
#include "ilengine.h"


typedef std::vector<Reflection::TypeDef*> TypeDefList;
typedef std::map<std::string,TypeDefList> TypeNameMap;

#define g_typenames (*p_typenames)
static TypeNameMap* p_typenames;


namespace AssemblyManager {

	using namespace ::Reflection;

	struct AssemblyData {
		PE::Image* pefile;
		Metadata::MetadataRoot* metadata;
		Reflection::Assembly* assembly;
		AssemblyData() {
			pefile = NULL;
			metadata = NULL;
			assembly = NULL;
		}
		void Release() {
			delete pefile;
			delete metadata;
			delete assembly;
		}
	};

	static std::vector<AssemblyData>* assemblies = NULL;

	extern void Finalize() {
		if(assemblies!=NULL) {
			for(uint i=0; i<assemblies->size(); ++i) {
				getConsole() << "Desposing " << assemblies->at(i).assembly->getName() << endl;
				assemblies->at(i).assembly->Dispose();
			}
			for(uint i=0; i<assemblies->size(); ++i) {
				getConsole() << "Destroying " << assemblies->at(i).assembly->getName() << endl;
				assemblies->at(i).Release();
			}
			delete assemblies;
		}
		if(p_typenames!=NULL) {
			delete p_typenames;
		}
	}

	extern int GetAssemblyCount() {
		if(assemblies==NULL) return 0;
		return assemblies->size();
	}

	extern Assembly* LoadAssembly(const void* buf) {
		if(p_typenames==NULL) p_typenames = new TypeNameMap();
		if(assemblies==NULL) assemblies = new std::vector<AssemblyData>();
		Console& console = getConsole();
		AssemblyData ad;
		ad.pefile = new PE::Image(buf);
		ad.metadata = Metadata::MetadataRoot::FromPEImage(*ad.pefile);
		const Metadata::MainStream& ms = ad.metadata->getMainStream();
		const Metadata::StringStream& ss = ad.metadata->getStringStream();
		if(ms.getRowCount(TABLE_Assembly)!=1) {
			console << "Failed to load Assembly because it doesn't have only one assembly row." << endl;
		} else {
			/*
			Metadata::InterfaceImpl* pt = (Metadata::InterfaceImpl*)ms.getRow(TABLE_InterfaceImpl,1);
			getConsole() << "Row: " << pt->Interface << endl;
			*/
			const char* assemname = NULL;
			Metadata::Assembly& assem = *(Metadata::Assembly*)ms.getRow(TABLE_Assembly,1);
			assemname = ss+assem.Name;
			//*
			console << "Assembly: "<< assemname << "(" << assem.Name << ")"
				<< " " << (int)assem.MajorVersion << "." << (int)assem.MinorVersion
				<< "." << (int)assem.BuildNumber << "." << (int)assem.RevisionNumber
				<< " (" << assem.Flags << ")\r\n";
			//*/
			delete &assem;
			if(strlen(assemname)==0) {
				console << "This assembly doesn't have Assembly Name." << endl;
			} else {
				std::wstring aname;
				aname.assign(assemname, assemname+strlen(assemname));
				if(NULL!=AssemblyManager::FindAssembly(aname)) {
					console << "Already registered assembly '" << aname << "'" << endl;
				} else {
					ad.assembly = new Assembly(assemname, *ad.metadata, ad.pefile->getImageBase());
					assemblies->push_back(ad);
					return ad.assembly;
				}
			}
		}
		delete ad.assembly;
		delete ad.metadata;
		delete ad.pefile;
		panic("CAN'T LOAD ASSEMBLY");
	}

	extern Assembly* GetAssembly(int index) {
		return assemblies->at(index).assembly;
	}

	extern Assembly* FindAssembly(const std::wstring& name) {
		for(int i=0; i<GetAssemblyCount(); ++i) {
			Assembly* assem = GetAssembly(i);
			if(assem->getName()==name) {
				return assem;
			}
		}
		return NULL;
	}

	extern TypeDef* FindTypeDef(const std::wstring& asmname, const std::wstring& ns, const std::wstring& name) {
		Assembly* assembly = FindAssembly(asmname);
		if(assembly==NULL) return NULL;
		return assembly->FindTypeDef(name,ns);
	}

	extern bool LinkAssembly(const wchar_t* assemblyname) {
		Console& console = getConsole();
		using Reflection::Assembly;
		using Reflection::TypeDef;
		using Reflection::Method;
		Assembly* korlib = NULL;
		for(int i=0; i<AssemblyManager::GetAssemblyCount(); ++i) {
			Assembly& assem = *AssemblyManager::GetAssembly(i);
			if(assem.getName()==assemblyname) {
				korlib = &assem;
			}
		}
		if(korlib==NULL) {
			console << assemblyname << " not found." << endl;
			return false;
		} else {
			for(uint i=1; i<=korlib->getTypeDefCount(); ++i) {
				const TypeDef& typdef = korlib->getTypeDef(i);
				std::wstring ns = typdef.getNamespace();
				if(ns.substr(0,10)!=L"CooS.Wrap.") continue;
				ns = ns.substr(10);
				//ñºëOãÛä‘ñºÇÃêÊì™ÇÕ _ Ç≈Ç†ÇÈÇ◊Ç´ÅB
				if(ns.substr(0,1)!=L"_") {
					console << "Beginning of Namespace must be an underscore: [" << assemblyname << "] " << typdef.getFullName() << endl;
				} else {
					ns = ns.substr(1);
				}
				std::wstring typname = typdef.getName().substr(1);
				if(typdef.getName()[0]!='_') {
					console << "ClassName must begin with underscore: " << typdef.getFullName() << endl;
					continue;
				}
				bool hit = false;
				for(int i=0; i<AssemblyManager::GetAssemblyCount(); ++i) {
					Assembly& assem = *AssemblyManager::GetAssembly(i);
					TypeDef* typdef2 = assem.FindTypeDef(typname, ns);
					if(typdef2==NULL) {
					} else {
						hit = true;
						for(uint m=1; m<=typdef.getMethodCount(); ++m) {
							Method& method = typdef.getMethod(m);
							if(method.getName()==L".ctor" && method.getParamCount()==0) continue;
							if(method.getName()==L".cctor") continue;
							method.getSignature().Params.Link(*korlib);
							Method* method2 = typdef2->ResolveMethod(method.getName(), method.getSignature().Params);
							if(method2==NULL) {
								console << method.getFullName() << " -> (*UNRESOLVED*)" << endl;
							} else if(method.HasThis()!=method2->HasThis()) {
								console << method.getFullName() << " -> (*MIS-CALLSPEC*)" << endl;
							} else {
								Method* patched = assem.GetPatchedMethodBy(method2);
								if(patched==NULL) {
									patched = method2;
								} else {
									//console << "Repatch " << patched->getFullName() << " patched by " << method.getFullName() << endl;
								}
								assem.ReplaceMethodWith(patched->getTableIndex(), &method);
							}
						}
					}
				}
				if(!hit) console << typdef.getFullName() << " -> (*STRAYCAT*)" << endl;
			}
			return true;
		}
	}

	extern bool IntroduceAssemblyIntoManaged(const std::wstring& name, const void* data, int size) {
		IL::ILMachine& machine = IL::ILMachine::getCurrent();
		machine.stack.pushn(size);
		machine.stack.pushn(data);
		if(!AssemblyManager::Execute(L"cscorlib",L"CooS",L"Initializer",L"LoadPEImage",machine)) {
			return 1;
		} else {
			Reflection::Assembly* assem = AssemblyManager::FindAssembly(name);
			assem->setManagedType(machine.stack.popo());
			assem->LinkVTableFixups();
			return 0;
		}
		panic("Failed IntroduceAssemblyIntoManaged()");
	}

	extern bool Execute(const std::wstring& assemblyName, const std::wstring& Namespace, const std::wstring& typeName, const std::wstring& methodName, IL::ILMachine& machine) {
		Reflection::Assembly* assembly = AssemblyManager::FindAssembly(assemblyName);
		if(assembly==NULL) return false;
		for(uint i=1; i<=assembly->getTypeDefCount(); ++i) {
			const Reflection::TypeDef& td = assembly->getTypeDef(i);
			if(td.getName()!=typeName || td.getNamespace()!=Namespace) continue;
			for(uint j=1; j<=td.getMethodCount(); ++j) {
				const Reflection::Method& m = td.getMethod(j);
				if(m.getName()==methodName) {
					IL::Execute(machine, m);
					return true;
				}
			}
		}
		machine.panic(L"AssemblyManager::Execute failed: "+methodName);
	}

}
