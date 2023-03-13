#include "stdafx.h"
#include "../AssemblyManager.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_ldnull(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	machine.stack.pushp(NULL);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldnull" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldstr(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	if(token.table!=0x70) panic("ldstr attempted to load a string from non-US table");
	const MemoryRegion mem = frame.assembly.getRoot().getUserStringStream().getData(token.index);
	IL::String* s = IL::NewString(mem.size()/sizeof(wchar_t));
	memcpy(&s->start_char, mem+0, mem.size()&~1);	// if size is odd, buffer is not perfect.
	stack.pusho(s);
	AssemblyManager::Execute(L"mscorlib",L"System",L"String",L"Intern",machine);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldstr " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldtoken(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	const void* handle;
	Metadata::MetadataToken token = *(uint32*)operand;
	switch(token.table) {
	case TABLE_TypeDef:
	case TABLE_TypeRef:
	case TABLE_TypeSpec:
		{
			const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
			const_cast<TypeDef*>(typdef)->MakeClassInitialized();
			handle = typdef->getHandleFromType();
		}
		break;
	case TABLE_Field:
		//panic("ldtoken tried for MemberRef");
		{
		Field* field = frame.assembly.ResolveField(token);
		if(field==NULL) panic("ldtoken failed to resolve FieldDef");
		handle = field->getStaticValue();
		//getConsole() << handle << " " << field->getName() << endl;
		machine.stack.pusho(IL::NewString(field->getAssembly().getName()));
		machine.stack.pushi(field->getTableIndex());
		machine.stack.pushn(handle);
		AssemblyManager::Execute(L"cscorlib",L"CooS",L"Initializer",L"PreloadFieldHandle",machine);
		}
		break;
	case TABLE_Method:
		//panic("ldtoken tried for MemberRef");
		handle = frame.assembly.ResolveMethod(token);
		if(handle==NULL) panic("ldtoken can't find MethodDef");
		break;
	case TABLE_MemberRef:
	//	case TABLE_FieldRef:
	//	case TABLE_MethodRef:
		panic("ldtoken tried for MemberRef");
	default:
		panic("ldtoken");
	}
	if(handle==NULL) panic("ldtoken can't find TypeDef");
	stack.pushn(handle);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldtoken " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldftn(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 6;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	Method* method = frame.assembly.ResolveMethod(token);
	if(method==NULL) machine.panic("ldftn> can't find target method");
	const void* proxycode = method->CreateProxyCode();
	stack.pushn((nint)proxycode);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldftn " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldvirtftn(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 6;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	Method* method = frame.assembly.ResolveMethod(token);
	if(method==NULL) machine.panic("ldvirtftn> can't find target method");
	void* obj = machine.stack.popo();
	RuntimeTypeHandle handle = IL::GetHandleOfObject(obj);
	const TypeDef* realtype = TypeDef::getTypeFromHandle(handle);
	Method* realmethod = realtype->ResolveOverrideMethod(*method, true);
	const void* proxycode = realmethod->CreateProxyCode();
	stack.pushn((nint)proxycode);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldvirtftn " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}
