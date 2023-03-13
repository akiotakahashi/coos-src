#include "stdafx.h"
#include "../AssemblyManager.h"


namespace IL {
	
	using namespace AssemblyManager;
	using namespace Reflection;

	static int OffsetToHandle = 4;	/* First 4 bytes is a field, [mscorlib] System.Type:_impl */

	static void SetHandleOfTypeObject(void* typeobj, const TypeDef& typdef) {
		*(void**)((byte*)typeobj+OffsetToHandle) = typdef.getHandleFromType();
	}

	static void ConstructType(METATYPE metatype, TypeDef& typdef) {
		IL::ILMachine& machine = ILMachine::getCurrent();
		const nint* ptop = &machine.stack.top();
		if(IL::IsNewProtocol(metatype)) {
			void* assembly = typdef.getAssembly().getManagedType();
			if(assembly==NULL) panic(L"Assembly is not loaded: "+typdef.getAssembly().getName());
			const TypeDef& mtype = GetMetaType(metatype);
			if(typdef.getTableIndex()>0) {
				machine.stack.pusho(assembly);
				machine.stack.pushi(typdef.getTableIndex());
				machine.stack.pusho(NULL);
				if(typdef.getStaticHeap()==NULL) {
					machine.stack.pusho(NULL);
				} else {
					machine.stack.pusho((byte*)typdef.getStaticHeap()-(int)&((IL::Array*)0)->start_elem);
				}
				IL::Execute(machine, *mtype.GetSingleMethod(L"Setup",true));
				void* type = machine.stack.popo();
				if(type==NULL) panic("Setup failed");
				typdef.setManagedType(type);
				machine.stack.pusho(type);	// Push instance to the top
			} else {
				void* type = IL::NewType(metatype)->getObject();
				SetHandleOfTypeObject(type, typdef);
				typdef.setManagedType(type);
				machine.stack.pusho(assembly);
				machine.stack.pusho(typdef.getElementType()->getManagedType());
				machine.stack.pusho(type);
				IL::Execute(machine, *mtype.GetSingleMethod(L".ctor",true));
				machine.stack.pusho(type);	// Push instance to the top
			}
			// この時点でスタックトップに生成したインスタンスが必要
			typdef.FixManagedType();
			SetHandleOfTypeObject((void*)machine.stack.top(), typdef);
			AssemblyManager::Execute(L"cscorlib",L"CooS.CodeModels.CLI",L"SuperType",L"CompleteSetup",machine);
		} else {
			void* type = IL::NewType(metatype)->getObject();
			SetHandleOfTypeObject(type, typdef);
			typdef.setManagedType(type);
			machine.stack.pushn(typdef.getHandleFromType());
			machine.stack.pushn(typdef.getTableIndex());
			machine.stack.pusho(IL::NewString(typdef.getName()));
			machine.stack.pusho(IL::NewString(typdef.getNamespace()));
			const TypeDef* baseType = typdef.getBaseType();
			if(baseType==NULL) {
				machine.stack.pusho(NULL);
			} else {
				machine.stack.pusho(baseType->getManagedType());
			}
			machine.stack.pusho(type);
			IL::Execute(machine, *GetMetaType(metatype).GetSingleMethod(L".ctor",true));
			if(typdef.IsArray()) {
				RuntimeTypeHandle handle = IL::GetHandleOfObject(type);
				TypeDef* metatype = TypeDef::getTypeFromHandle(handle);
				Method* method = metatype->GetCallableMethod(L"SetElementType",true);
				IL::ILMachine& machine = IL::ILMachine::getCurrent();
				machine.stack.pusho(type);
				machine.stack.pusho(typdef.getElementType()->getManagedType());
				IL::Execute(machine, *method);
			}
		}
		const nint* ptop2 = &machine.stack.top();
		if(ptop!=ptop2) panic("MACHINE STATCK DESTROYED");
	}

	static void IntroduceMetaType(METATYPE metatype, const std::wstring& assem, const std::wstring& ns, const std::wstring& name) {
		TypeDef* typdef = FindAssembly(assem)->FindTypeDef(name,ns);
		if(typdef==NULL) panic((L"IntroduceNewType:["+assem+L"] "+ns+L"."+name).c_str());
		IL::SetMetaType(metatype, *typdef);
		BlankType* btype = IL::NewBlankType(IL::GetMetaType(METATYPE_METATYPE).getInstanceSize());
		SetHandleOfTypeObject(btype->getObject(), *typdef);
		typdef->setManagedType(btype->getObject());
		btype->type = IL::GetMetaType(METATYPE_METATYPE).getManagedType();
	}

	static void InitMetaType(METATYPE metatype) {
		TypeDef& typdef = const_cast<TypeDef&>(IL::GetMetaType(metatype));
		ConstructType(METATYPE_METATYPE, typdef);
	}

	extern void IntroduceNewType(METATYPE metatype, TypeDef& typdef) {
		ConstructType(metatype, typdef);
	}

	static bool Setup(IKernel* kernel) {
		AssemblyManager::LinkAssembly(L"cscorlib");
		AssemblyManager::LinkAssembly(L"cskorlib");
		AssemblyManager::LinkAssembly(L"csbridge");
		// ルートメタタイプの情報を取得
		const TypeDef& rootType = *FindAssembly(L"cscorlib")->FindTypeDef(L"TypeImpl",L"CooS.Reflection");
		OffsetToHandle = rootType.getField(1).getOffset();
		// ルートメタタイプをロード
		IntroduceMetaType(METATYPE_METATYPE, L"cscorlib",L"CooS.Reflection",L"PseudoType");
		IntroduceMetaType(METATYPE_TYPEBASE, L"cscorlib",L"CooS.Reflection",L"TypeImpl");
		/*
		IntroduceMetaType(METATYPE_STRING, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_DELEGATE, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_VALUETYPE, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_PRIMITIVE, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_INTERFACE, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_SZARRAY, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_MNARRAY, L"cscorlib",L"CooS.Types",L"PseudoType");
		IntroduceMetaType(METATYPE_POINTER, L"cscorlib",L"CooS.Types",L"PseudoType");
		*/
		//
		SetMetaType(METATYPE_CLASSTYPE, GetMetaType(METATYPE_METATYPE));
		SetMetaType(METATYPE_STRING, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_DELEGATE, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_VALUETYPE, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_PRIMITIVE, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_INTERFACE, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_SZARRAY, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_MNARRAY, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_BYREFPTR, GetMetaType(METATYPE_CLASSTYPE));
		SetMetaType(METATYPE_BYVALPTR, GetMetaType(METATYPE_CLASSTYPE));
		/*/ ルートメタタイプを初期化
		FixupMetaType(METATYPE_METATYPE);
		FixupMetaType(METATYPE_TYPEBASE);
		/*
		FixupMetaType(METATYPE_STRING);
		FixupMetaType(METATYPE_DELEGATE);
		FixupMetaType(METATYPE_VALUETYPE);
		FixupMetaType(METATYPE_PRIMITIVE);
		FixupMetaType(METATYPE_INTERFACE);
		FixupMetaType(METATYPE_SZARRAY);
		FixupMetaType(METATYPE_MNARRAY);
		FixupMetaType(METATYPE_POINTER);
		*/
		// ルートメタタイプを初期化
		InitMetaType(METATYPE_METATYPE);
		InitMetaType(METATYPE_TYPEBASE);
		/*

		InitMetaType(METATYPE_STRING);
		InitMetaType(METATYPE_DELEGATE);
		InitMetaType(METATYPE_VALUETYPE);
		InitMetaType(METATYPE_PRIMITIVE);
		InitMetaType(METATYPE_INTERFACE);
		InitMetaType(METATYPE_SZARRAY);
		InitMetaType(METATYPE_MNARRAY);
		InitMetaType(METATYPE_POINTER);
		*/
		// カーネルブリッジを設定
		ILMachine& machine = ILMachine::getCurrent();
		machine.stack.pushn(kernel);
		AssemblyManager::Execute(L"csbridge",L"System",L"Bridge",L"setKernel",machine);
		AssemblyManager::Execute(L"cscorlib",L"CooS",L"Initializer",L"Setup",machine);
		return true;
	}

	static void ReplaceMetaType(METATYPE metatype, const std::wstring& asmname, const std::wstring& ns, const std::wstring& name) {
		IntroduceMetaType(metatype, asmname, ns, name);
		SetNewProtocol(metatype);
		InitMetaType(metatype);
	}

	static void SetNewMetaType(METATYPE metakind, void* metatype) {
		SetMetaType(metakind, *TypeDef::getTypeFromHandle(GetHandleOfType(metatype)));
		SetNewProtocol(metakind);
	}

	static void ReplaceType(METATYPE metatype, const std::wstring& asmname, const std::wstring& ns, const std::wstring& name) {
		TypeDef& typdef = *AssemblyManager::FindAssembly(asmname)->FindTypeDef(name,ns);
		if(!typdef.IsFixedManagedType()) ConstructType(metatype, typdef);
	}

	struct MetaTypeDesc {
		void* mt_metatype;
		void* mt_basetype;
		void* mt_class;
		void* mt_string;
		void* mt_delegate;
		void* mt_valuetype;
		void* mt_primitive;
		void* mt_interface;
		void* mt_szarray;
		void* mt_mnarray;
		void* mt_byrefptr;
		void* mt_byvalptr;
	};

	static bool Setup2() {
		// Load metatypes
		ILMachine& machine = ILMachine::getCurrent();
		const nint* saved = &machine.stack.top();
		//
		MetaTypeDesc desc;
		AssemblyManager::Execute(L"cscorlib",L"CooS",L"Initializer",L"GetMetaTypes", machine);
		machine.stack.popmem(&desc, sizeof(desc));
		//
		//MetaTypeDesc desc;
		machine.stack.pusho(AssemblyManager::FindAssembly(L"cscorlib")->getManagedType());
		AssemblyManager::Execute(L"cscorlib",L"CooS",L"Initializer",L"LoadMetaTypes", machine);
		machine.stack.popmem(&desc, sizeof(desc));
		//
		if(saved!=&machine.stack.top()) panic("MACHINE STACK CORRUPTED");
		//
		SetNewMetaType(METATYPE_METATYPE,	desc.mt_metatype);
		SetNewMetaType(METATYPE_TYPEBASE,	desc.mt_basetype);
		SetNewMetaType(METATYPE_CLASSTYPE,	desc.mt_class);
		SetNewMetaType(METATYPE_STRING,		desc.mt_string);
		SetNewMetaType(METATYPE_DELEGATE,	desc.mt_delegate);
		SetNewMetaType(METATYPE_VALUETYPE,	desc.mt_valuetype);
		SetNewMetaType(METATYPE_PRIMITIVE,	desc.mt_primitive);
		SetNewMetaType(METATYPE_INTERFACE,	desc.mt_interface);
		SetNewMetaType(METATYPE_SZARRAY,	desc.mt_szarray);
		SetNewMetaType(METATYPE_MNARRAY,	desc.mt_mnarray);
		SetNewMetaType(METATYPE_BYREFPTR,	desc.mt_byrefptr);
		SetNewMetaType(METATYPE_BYVALPTR,	desc.mt_byvalptr);

		/*
		const wchar_t* MetatypeNS = L"CooS.CodeModels.CLI.Metatype";
		// Reset meta-types
		ReplaceMetaType(METATYPE_METATYPE,	L"cscorlib", MetatypeNS, L"ClassType");
		ReplaceMetaType(METATYPE_TYPEBASE,	L"cscorlib", L"CooS.Reflection", L"TypeImpl");
		IL::SetMetaType(METATYPE_CLASSTYPE, IL::GetMetaType(METATYPE_METATYPE));
		IL::SetNewProtocol(METATYPE_CLASSTYPE);
		//ReplaceMetaType(METATYPE_CLASSTYPE,	L"cscorlib", MetatypeNS, L"ClassType");
		ReplaceMetaType(METATYPE_STRING,	L"cscorlib", MetatypeNS, L"StringType");
		ReplaceMetaType(METATYPE_DELEGATE,	L"cscorlib", MetatypeNS, L"DelegateType");
		ReplaceMetaType(METATYPE_VALUETYPE,	L"cscorlib", MetatypeNS, L"StructType");
		ReplaceMetaType(METATYPE_PRIMITIVE,	L"cscorlib", MetatypeNS, L"PrimitiveType");
		ReplaceMetaType(METATYPE_INTERFACE,	L"cscorlib", MetatypeNS, L"InterfaceType");
		ReplaceMetaType(METATYPE_SZARRAY,	L"cscorlib", MetatypeNS, L"SzArrayType");
		ReplaceMetaType(METATYPE_MNARRAY,	L"cscorlib", MetatypeNS, L"MnArrayType");
		ReplaceMetaType(METATYPE_BYREFPTR,	L"cscorlib", MetatypeNS, L"ByRefPointerType");
		ReplaceMetaType(METATYPE_BYVALPTR,	L"cscorlib", MetatypeNS, L"ByValPointerType");
		*/

		// Reload primary types
		getConsole() << "> begin to replace Object" << endl;
		ReplaceType(METATYPE_CLASSTYPE	, L"mscorlib", L"System", L"Object");
		getConsole() << "> finished replacing Object" << endl;

		SupressTypeReplacing();
		ReplaceType(METATYPE_STRING		, L"mscorlib", L"System", L"String");
		ReplaceType(METATYPE_DELEGATE	, L"mscorlib", L"System", L"Delegate");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Boolean");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Char");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Byte");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"SByte");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Int16");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Int32");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Int64");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"UInt16");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"UInt32");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"UInt64");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"IntPtr");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"UIntPtr");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Single");
		ReplaceType(METATYPE_PRIMITIVE, L"mscorlib", L"System", L"Double");
		FlushTypeReplacing();

		// Reload all types
		SupressTypeReplacing();
		for(int i=0; i<AssemblyManager::GetAssemblyCount(); ++i) {
			Assembly& assembly = *AssemblyManager::GetAssembly(i);
			for(uint j=1; j<=assembly.getTypeDefCount(); ++j) {
				TypeDef& typdef = assembly.getTypeDef(j);
				if(typdef.ReadyManagedType() && !typdef.IsFixedManagedType()) {
					typdef.ReloadManagedType(true);
				}
			}
		}
		FlushTypeReplacing();

		//
		IL::Synchronize();
		return true;
	}

	static void IntroduceAssemblyIntoManaged(void* (*reader)(const wchar_t*, int*), void (*befree)(void*), const wchar_t* filename, const wchar_t* asmname) {
		int sz;
		void* p = reader(filename, &sz);
		AssemblyManager::IntroduceAssemblyIntoManaged(asmname, p, sz);
		befree(p);
	}

	extern void Install(IKernel* kernel, void* (*reader)(const wchar_t*, int*), void (*befree)(void*)) {

		Console& console = getConsole();
		IL::ILMachine machine(1*1024*1024);

		//----------

		console << "--- Read Core CLI Assemblies ---" << endl;
		void* p = NULL;
		int sz = 0;
		AssemblyManager::LoadAssembly(p=reader(L"mscorlib.dll",&sz));	befree(p);
		AssemblyManager::LoadAssembly(p=reader(L"csbridge.dll",&sz));	befree(p);
		AssemblyManager::LoadAssembly(p=reader(L"cskorlib.dll",&sz));	befree(p);
		AssemblyManager::LoadAssembly(p=reader(L"cscorlib.dll",&sz));	befree(p);

		//----------

		console << "--- Setup CLI Environment ---" << endl;
		IL::Setup(kernel);

		//----------

		console << "--- Install Core Components ---" << endl;
		IntroduceAssemblyIntoManaged(reader, befree, L"mscorlib.dll", L"mscorlib");
		IntroduceAssemblyIntoManaged(reader, befree, L"cskorlib.dll", L"cskorlib");
		IntroduceAssemblyIntoManaged(reader, befree, L"cscorlib.dll", L"cscorlib");

		//----------

		console << "--- Phase Shift ---" << endl;
		AssemblyManager::Execute(L"cscorlib", L"CooS", L"Initializer", L"PrepareSetup", machine);
		IL::Setup2();

		//----------
		
		console << "--- Final Setup ---" << endl;
		AssemblyManager::Execute(L"cscorlib", L"CooS", L"Initializer", L"FinalizeSetup", machine);
	
		//----------
		
		console << "---> All initialization completed." << endl;

	}

}
