#include "TypeDef.h"
#include "Field.h"
#include "Method.h"
#include "../ilengine.h"
#include "../AssemblyManager.h"


namespace Reflection {

	void TypeDef::Init() {
		fieldCount = 0;
		methodCount = 0;
		staticSize = -1;
		instanceSize = -1;
		baseTypeDef = NULL;
		staticheap = NULL;
		initialized = false;
		fixedManagedType = false;
		elemTypeDef = NULL;
		pManagedType = NULL;
		arrayTypeDef = NULL;
		pointerTypeDef = NULL;
	}

	TypeDef::TypeDef(Assembly& parent, uint tableIndex, const Metadata::TypeDef& td, const Metadata::TypeDef* ptdnext) : Metadata::TypeDef(td), AssemblyItem(parent,tableIndex) {
		const Metadata::MainStream& ms = getRoot().getMainStream();
		const Metadata::StringStream& ss = getRoot().getStringStream();
		Init();
		name = ss.getString(td.Name);
		ns = ss.getString(td.Namespace);
		uint limitOfFieldIndex;
		uint limitOfMethodIndex;
		if(ptdnext==NULL) {
			limitOfFieldIndex = ms.getRowCount(TABLE_Field)+1;
			limitOfMethodIndex = ms.getRowCount(TABLE_Method)+1;
		} else {
			limitOfFieldIndex = ptdnext->FieldList;
			limitOfMethodIndex = ptdnext->MethodList;
		}
		fieldCount = limitOfFieldIndex-td.FieldList;
		methodCount = limitOfMethodIndex-td.MethodList;
	}
	
	TypeDef::TypeDef(const TypeDef& typdef, int dimension) : Metadata::TypeDef(typdef), AssemblyItem(typdef.getAssembly(),0) {
		Init();
		Flags = (Metadata::TypeAttributes)0;
		elemTypeDef = &typdef;
		name = typdef.getName()+L"[]";
		ns = typdef.getNamespace();
		baseTypeDef = AssemblyManager::FindTypeDef(L"mscorlib",L"System",L"Array");
		if(baseTypeDef==NULL) panic("TypeDef::TypeDef can't find System.Array");
	}
	
	TypeDef::TypeDef(const TypeDef& typdef) : Metadata::TypeDef(typdef), AssemblyItem(typdef.getAssembly(),0) {
		Init();
		Flags = (Metadata::TypeAttributes)0;
		elemTypeDef = &typdef;
		name = typdef.getName()+L"*";
		ns = typdef.getNamespace();
		baseTypeDef = getAssembly().ResolveType(L"ValueType",L"System");
		if(baseTypeDef==NULL) panic("TypeDef::TypeDef can't find System.ValueType");
		instanceSize = sizeof(void*);
	}

	TypeDef::~TypeDef() {
		delete arrayTypeDef;
		delete pointerTypeDef;
	}

	TypeDef* TypeDef::getTypeFromHandle(RuntimeTypeHandle handle) {
		if(handle==NULL) clrpanic("getTypeFromHandle: NULL of RuntimeTypeHandle");
		if(handle.TrackingField==NULL) clrpanic("getTypeFromHandle: Type is NULL");
		return (TypeDef*)handle.TrackingField;
	}

	RuntimeTypeHandle TypeDef::getHandleFromType() const {
		return RuntimeTypeHandle((void*)this);
	}

	Field& TypeDef::getField(uint index) const {
		return getAssembly().getField(FieldList+index-1);
	}

	Method& TypeDef::getMethod(uint index) const {
		return getAssembly().getMethod(MethodList+index-1);
	}

	uint TypeDef::getStaticSize() const {
		if(staticSize<0) const_cast<TypeDef*>(this)->DetermineStaticFieldLayout();
		return (uint)staticSize;
	}

	uint TypeDef::getInstanceSize() const {
		if(instanceSize<0) const_cast<TypeDef*>(this)->DetermineInstanceFieldLayout();
		return (uint)instanceSize;
	}

	uint TypeDef::getVariableSize() const {
		if(IsValueType()) {
			return getInstanceSize();
		} else {
			return sizeof(void*);
		}
	}

	bool TypeDef::IsValueType() const {
		static TypeDef* valuetype = NULL;
		if(valuetype==NULL) {
			valuetype = AssemblyManager::FindTypeDef(L"mscorlib",L"System",L"ValueType");
		}
		return IsSubclassOf(valuetype);
	}

	bool TypeDef::IsInterface() const {
		return 0!=(Flags&0x20);
	}

	bool TypeDef::IsRefType() const {
		return !IsInterface() && !IsValueType();
	}

	bool TypeDef::IsArray() const {
		const std::wstring& name = getName();
		return name.substr(name.length()-2)==L"[]";
	}

	bool TypeDef::IsPointer() const {
		const std::wstring& name = getName();
		return name.substr(name.length()-1)==L"*";
	}

	const TypeDef* TypeDef::getBaseType() const {
		if(baseTypeDef==NULL) {
			if(Extends.index==0) return NULL;
			const TypeDef* typdef = getAssembly().ResolveTypeDef(Extends);
			if(typdef==NULL) panic("TypeDef::getBaseType can't resolve the base type.");
			baseTypeDef = typdef;
		}
		return baseTypeDef;
	}

	const TypeDef* TypeDef::getElementType() const {
		return elemTypeDef;
	}

	bool TypeDef::IsSubclassOf(const TypeDef* typdef) const {
		if(typdef==NULL) return false;
		const TypeDef* pbase = this->getBaseType();
		while(pbase!=NULL) {
			if(pbase==typdef) {
				return true;
			}
			pbase = pbase->getBaseType();
		}
		return false;
	}

	bool TypeDef::IsInterfaceImpl(TableId table, uint index) const {
		const Metadata::MainStream& ms = getRoot().getMainStream();
		for(uint i=1; i<ms.getRowCount(TABLE_InterfaceImpl); ++i) {
			Metadata::InterfaceImpl* ifimpl = (Metadata::InterfaceImpl*)ms.getRow(TABLE_InterfaceImpl, i);
			if(ifimpl->Class==getTableIndex()) {
				if(ifimpl->Interface.index==index && ifimpl->Interface.table==table) {
					delete ifimpl;
					return true;
				}
			}
			delete ifimpl;
		}
		return false;
	}

	void TypeDef::FixManagedType() const {
		fixedManagedType = true;
	}

	void TypeDef::ReloadManagedType(bool newload) {
		if(newload || pManagedType!=NULL) {
			if(fixedManagedType && pManagedType!=NULL) {
#if defined(ILWARN)
				getConsole() << "{TYPEDEF:RELOAD}";
#endif
				//panic("Tried to reload but fixed");
			} else {
				IL::METATYPE metatype;
				if(IsInterface()) {
					metatype = IL::METATYPE_INTERFACE;
				} else if(IsArray()) {
					metatype = IL::METATYPE_SZARRAY;
				} else if(IsValueType()) {
					metatype = IL::METATYPE_VALUETYPE;
					if(getNamespace()==L"System") {
						metatype = IL::METATYPE_PRIMITIVE;
						if(getName()==L"Void") {
						} else if(getName()==L"Boolean") {
						} else if(getName()==L"Char") {
						} else if(getName()==L"Byte") {
						} else if(getName()==L"Int16") {
						} else if(getName()==L"Int32") {
						} else if(getName()==L"Int64") {
						} else if(getName()==L"SByte") {
						} else if(getName()==L"UInt16") {
						} else if(getName()==L"UInt32") {
						} else if(getName()==L"UInt64") {
						} else if(getName()==L"Single") {
						} else if(getName()==L"Double") {
						} else if(getName()==L"IntPtr") {
						} else if(getName()==L"UIntPtr") {
						} else {
							metatype = IL::METATYPE_VALUETYPE;
						}
					}
				} else {
					metatype = IL::METATYPE_CLASSTYPE;
					if(getNamespace()==L"System") {
						if(getName()==L"String") {
							metatype = IL::METATYPE_STRING;
						} else if(getName()==L"Delegate") {
							metatype = IL::METATYPE_DELEGATE;
						} else if(getName()==L"MulticastDelegate") {
							metatype = IL::METATYPE_DELEGATE;
						}
					}
				}
				IL::IntroduceNewType(metatype, *const_cast<TypeDef*>(this));
				// Assertion
				if(pManagedType==NULL) clrpanic("TypeDef::ReloadManagedType> pManagedType is still NULL.");
			}
		}
		/*
		// Derived Types
		if(arrayTypeDef!=NULL) {
			if(arrayTypeDef->ReadyManagedType() && !arrayTypeDef->IsFixedManagedType()) {
				arrayTypeDef->ReloadManagedType(newload);
			}
		}
		*/
	}

	void TypeDef::setManagedType(void* obj) {
		if(obj==NULL) panic("Managed Type can't be set to NULL.");
		if(pManagedType==obj) return;
		if(pManagedType) {
			if(fixedManagedType) clrpanic("This reflecting ManagedType has been fixed already.");
			uint count = IL::ReplaceExistingType(pManagedType, obj);
			if(count>0) {
				getConsole() << "[" << getAssembly().getName() << "] ";
				getConsole() << getName() << ": ";
				getConsole() << pManagedType << " > " << obj;
				getConsole() << " (" << (int)count << ")" << endl;
			}
		}
		pManagedType = obj;
		// Derived Types
		if(arrayTypeDef!=NULL) {
			if(arrayTypeDef->ReadyManagedType()) {
				arrayTypeDef->ReloadManagedType(false);
			}
		}
	}

	void* TypeDef::getManagedType() const {
		if(pManagedType==NULL) {
			const_cast<TypeDef*>(this)->ReloadManagedType(true);
		}
		return pManagedType;
	}

	void TypeDef::MakeClassInitialized() {
		if(!IsClassInitialized()) {
			Method* cctor = GetSingleMethod(L".cctor",false);
			MarkClassInitialized(true);
			if(cctor!=NULL) {
				getConsole() << "Initialize class: " << this->getName() << endl;
				IL::Execute(IL::ILMachine::getCurrent(), *cctor);
			}
		}
	}
	
	TypeDef* TypeDef::CreateArrayType() const {
		if(arrayTypeDef==NULL) {
			arrayTypeDef = new TypeDef(*this, 1);
			IL::IntroduceNewType(IL::METATYPE_SZARRAY, *arrayTypeDef);
		}
		return arrayTypeDef;
	}

	TypeDef* TypeDef::CreatePointerType() const {
		if(pointerTypeDef==NULL) {
			pointerTypeDef = new TypeDef(*this);
			IL::IntroduceNewType(IL::METATYPE_BYVALPTR, *pointerTypeDef);
			void* metaobj = pointerTypeDef->getManagedType();
			RuntimeTypeHandle handle = IL::GetHandleOfObject(metaobj);
			TypeDef* metatype = TypeDef::getTypeFromHandle(handle);
			Method* method = metatype->GetCallableMethod(L"SetElementType", true);
			IL::ILMachine& machine = IL::ILMachine::getCurrent();
			machine.stack.pusho(pointerTypeDef->getManagedType());
			machine.stack.pusho(getManagedType());
			IL::Execute(machine, *method);
		}
		return pointerTypeDef;
	}

	void TypeDef::NotifyLoadingType(void* obj) {
		if(getStaticSize()>0) {
			IL::ILMachine& machine = IL::ILMachine::getCurrent();
			machine.stack.pusho(obj);
			machine.stack.pusho((byte*)getStaticHeap()-(int)&((IL::Array*)0)->start_elem);
			AssemblyManager::Execute(L"cscorlib",L"CooS.CodeModels.CLI.Metatype",L"ConcreteType",L"AssigneStaticHeap", machine);
		}
	}

	byte* TypeDef::getStaticHeap() const {
		if(staticSize<0) const_cast<TypeDef*>(this)->DetermineStaticFieldLayout();
		return staticheap;
	}

	void TypeDef::setStaticHeap(void* p) {
		staticheap = (byte*)p;
	}

	void TypeDef::DetermineInstanceFieldLayout() {
		// 静的変数に自己のインスタンスを使う場合があるので、
		// インスタンスのサイズを先に求める。
		if(instanceSize<0) {
			uint offset_i = 0;
			if(getBaseType()!=NULL) {
				offset_i = getBaseType()->getInstanceSize();
			}
			Metadata::ClassLayout* layout = NULL;
			switch(this->Flags & Metadata::TypeAttribute_LayoutMask) {
			case Metadata::TypeAttribute_ExplicitLayout:
				if(getFieldCount()>0) {
					getConsole() << "ExplicitLayout: " << this->getFullName() << endl;
					panic("Don't support ExplicitLayout");
				}
				// go down
			case Metadata::TypeAttribute_SequentialLayout:
				{
				const Metadata::MainStream& ms = getRoot().getMainStream();
				for(uint i=1; i<=ms.getRowCount(TABLE_ClassLayout); ++i) {
					Metadata::ClassLayout* row = (Metadata::ClassLayout*)ms.getRow(TABLE_ClassLayout,i);
					if(row->Parent==this->getTableIndex()) {
						//getConsole() << "Layouted Class: " << this->getFullName() << endl;
						layout = row;
						break;
					}
					delete row;
				}
				break;
				}
			default:
				break;
			}
			for(uint i=1; i<=getFieldCount(); ++i) {
				Field& field = getField(i);
				if(!field.IsStatic()) {
					if(field.HasFieldRVA()) {
						panic("Instance field having RVA detected");
					}
					field.setOffset(offset_i);
					uint size = field.getSize();
					offset_i += size;
				}
			}
			instanceSize = offset_i;
			if(layout!=NULL) {
				if(layout->ClassSize>0) {
					if(instanceSize>(int)layout->ClassSize) {
						getConsole() << "Target Class    : " << getFullName() << endl;
						getConsole() << "Calculated Size : " << instanceSize << endl;
						getConsole() << "ClassLayout Size: " << layout->ClassSize << endl;
						panic("Class size is larger than the specified size in ClassLayout");
					}
					instanceSize = layout->ClassSize;
				} else if(layout->PackingSize>0) {
					uint packing = layout->PackingSize;
					if(packing>4) {
						instanceSize = (instanceSize+7)&~7;
					} else if(packing>2) {
						instanceSize = (instanceSize+3)&~3;
					} else if(packing>1) {
						instanceSize = (instanceSize+1)&~1;
					}
				}
			}
			delete layout;
		}
	}

	void TypeDef::DetermineStaticFieldLayout() {
		// 静的領域サイズを計算
		if(staticSize<0) {
			if(staticheap!=NULL) panic("Static field layout is already determined.");
			uint offset_s = 0;
			for(uint i=1; i<=getFieldCount(); ++i) {
				Field& field = getField(i);
				if(field.IsStatic()) {
					field.setOffset(offset_s);
					offset_s += field.getSize();
				}
			}
			staticSize = offset_s;
			if(staticSize==0) {
				staticheap = NULL;
			} else {
				IL::Array* arr = IL::NewArray(*AssemblyManager::FindTypeDef(L"mscorlib",L"System",L"Byte"), staticSize);
				staticheap = &arr->start_elem;
				memclr(staticheap, staticSize);
			}
		}
		//
		MakeClassInitialized();
	}

	Field* TypeDef::ResolveField(const std::wstring& name, bool nofail) const {
		for(uint i=1; i<=fieldCount; ++i) {
			Field& field = getField(i);
			if(field.getName()==name) {
				return &field;
			}
		}
		if(getBaseType()!=NULL) return getBaseType()->ResolveField(name,nofail);
		if(nofail) panic(L"Failed to resolve field: "+name);
		return NULL;
	}

	Method* TypeDef::GetSingleMethod(const std::wstring& name, bool nofail) const {
		for(uint i=1; i<=methodCount; ++i) {
			Method& method = getMethod(i);
			if(method.getName()==name) {
				return &method;
			}
		}
		if(nofail) panic(L"Failed to find one method: "+name);
		return NULL;
	}

	Method* TypeDef::GetCallableMethod(const std::wstring& name, bool nofail) const {
		const TypeDef* typdef = this;
		while(typdef!=NULL) {
			Method* method = typdef->GetSingleMethod(name,false);
			if(method!=NULL) return method;
			typdef = typdef->getBaseType();
		}
		if(nofail) panic(L"Failed to find a callable method: "+name);
		return NULL;
	}

	Method* TypeDef::ResolveOverrideMethod(const Method& method, bool nofail) const {
		const TypeDef& methodtype = method.getTypeDef();
		const std::wstring& methodname = method.getName();
		const Signature::ParamSigList& params = method.getSignature().Params;
		//
		const_cast<Signature::ParamSigList&>(params).Link(method.getAssembly());
		// はじめにインターフェイス指定のオーバーライドを探す
		if(method.getTypeDef().IsInterface()) {
			std::wstring fqname = methodtype.getFullName()+L"."+methodname;
			const TypeDef* typdef = this;
			while(typdef!=NULL) {
				Method* realmethod = typdef->ResolveMethod(fqname, params);
				if(realmethod!=NULL) return realmethod;
				typdef = typdef->getBaseType();
			}
		}
		// 次にシグネチャによるオーバーライドを探す
		const TypeDef* typdef = this;
		while(typdef!=NULL) {
			Method* realmethod = typdef->ResolveMethod(methodname, params);
			if(realmethod!=NULL) return realmethod;
			typdef = typdef->getBaseType();
		}
		//
		getConsole().MakeNewLine();
		getConsole() << "Method: " << method.getFullName() << endl;
		getConsole() << "Object: " << this->getFullName() << endl;
		if(nofail) {
			/*
			typdef = realtype;
			while(typdef!=NULL) {
				realmethod = typdef->ResolveMethod(methodname, params);
				if(realmethod!=NULL) break;
				typdef = typdef->getBaseType();
			}
			*/
			panic("ResolveOverrideMethod can't find method maching signature");
		}
		return NULL;
	}

	using namespace Signature;

	Method* TypeDef::ResolveMethod(const std::wstring& name, const ParamSigList& params) const {
		if(!params.IsLinked()) panic("ParameterList must be linked.");
		for(uint i=1; i<=methodCount; ++i) {
			Method& method = getMethod(i);
			ParamSigList& params2 = method.getSignature().Params;
			params2.Link(method.getAssembly());
			if(method.getName()==name && params.Match(params2)) {
				return &method;
			}
		}
		return NULL;
	}

}
