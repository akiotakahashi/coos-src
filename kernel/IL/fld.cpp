#include "stdafx.h"

using namespace Reflection;
using namespace IL;

#define stack machine.stack


extern Method* execute_stfld(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Field* field = frame.assembly.ResolveField(Metadata::MetadataToken(*(uint32*)operand));
	if(field==NULL) panic("execute_ldfld can't find field.");
	int offset = field->getOffset();
	int size = field->getSize();
	byte* obj = (byte*)stack[ILStack::getLengthOnStack(size)];
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "stfld " << stack << ", obj=" << obj << "+" << offset << endl;
ILDEBUG_END;
#endif
	stack.popmem(obj+offset, size);
	stack.popo();
	return NULL;
}

extern Method* execute_ldfld(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Field* field = frame.assembly.ResolveField(Metadata::MetadataToken(*(uint32*)operand));
	if(field==NULL) panic("execute_ldfld can't find field.");
	int offset = field->getOffset();
	if(stack.getDataType()!=STACK_TYPE_V) {
		byte* obj = (byte*)stack.popo();
		if(obj==NULL) {
			machine.panic(L"ldfld> obj is null");
		}
#if defined(WIN32)
		if(obj==(void*)0xCDCDCDCD) {
			machine.panic(L"ldfld> obj is invalid");
		}
#endif
		void* p = obj+offset;
		stack.pushmem(p, field->getSignature().Type.ElementType, field->getSize());
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldfld " << stack << " from " << obj << "+" << offset << ":" << (int)field->getSize() << endl;
ILDEBUG_END;
#endif
	} else {
		panic("ldfld for valuetype");
	}
	return NULL;
}

extern Method* execute_ldflda(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Field* field = frame.assembly.ResolveField(Metadata::MetadataToken(*(uint32*)operand));
	if(field==NULL) panic("execute_ldfld can't find field.");
	int offset = field->getOffset();
	int size = field->getSize();
	byte* obj = (byte*)stack.popo();
	stack.pushp(obj+offset);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldflda " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_stsfld(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Field* field = frame.assembly.ResolveField(Metadata::MetadataToken(*(uint32*)operand));
	if(field==NULL) panic("stfld can't find field.");
	int size = field->getSize();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "stsfld " << stack << ", adr=" << field->getStaticValue() << endl;
ILDEBUG_END;
#endif
	stack.popmem((void*)field->getStaticValue(), size);
	return NULL;
}

extern Method* execute_ldsfld(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Field* field = frame.assembly.ResolveField(Metadata::MetadataToken(*(uint32*)operand));
	if(field==NULL) panic("execute_ldfld can't find field.");
	//TODO: Don't use offset because static value is held by Field.
	void* p = (void*)field->getStaticValue();
	stack.pushmem(p, field->getSignature().Type.ElementType, field->getSize());
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldsfld " << stack << " from " << p << ":" << (int)field->getSize() << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldsflda(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Field* field = frame.assembly.ResolveField(Metadata::MetadataToken(*(uint32*)operand));
	if(field==NULL) panic("execute_ldfld can't find field.");
	int offset = field->getOffset();
	int size = field->getSize();
	stack.pushp((void*)field->getStaticValue());
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldsflda " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}
