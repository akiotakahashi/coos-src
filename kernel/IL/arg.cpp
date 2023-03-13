#include "stdafx.h"

using namespace Reflection;
using namespace IL;


static void ldarg(ILMachine& machine, Frame& frame, uint index) {
	ILStack& stack = machine.stack;
	int offset = frame.method.getArgumentOffset(index);
	uint size = frame.method.getArgumentSize(index);
	ELEMENT_TYPE type = frame.method.getArgumentElemType(index);
	const void* p = frame.abp+offset;
	stack.pushmem(p, type, size);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldarg." << (int)index << " " << stack << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_ldarg_0(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldarg(machine, frame, 0);
	return NULL;
}

extern Method* execute_ldarg_1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldarg(machine, frame, 1);
	return NULL;
}

extern Method* execute_ldarg_2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldarg(machine, frame, 2);
	return NULL;
}

extern Method* execute_ldarg_3(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldarg(machine, frame, 3);
	return NULL;
}

extern Method* execute_ldarg(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ldarg(machine, frame, *(uint*)operand);
	return NULL;
}

extern Method* execute_ldarg_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ldarg(machine, frame, *(byte*)operand);
	return NULL;
}

static void ldarga(ILMachine& machine, Frame& frame, uint16 index) {
	ILStack& stack = machine.stack;
	int offset = frame.method.getArgumentOffset(index);
	stack.pushp(frame.abp+offset);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldarga." << (int)index << " " << stack << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_ldarga(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 3;
	ldarga(machine, frame, *(uint16*)operand);
	return NULL;
}

extern Method* execute_ldarga_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ldarga(machine, frame, *(byte*)operand);
	return NULL;
}

static void starg(ILMachine& machine, Frame& frame, uint16 index) {
	ILStack& stack = machine.stack;
	int offset = frame.method.getArgumentOffset(index);
	uint size = frame.method.getArgumentSize(index);
	stack.popmem(frame.abp+offset, size);
}

extern Method* execute_starg(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 4;
	ILStack& stack = machine.stack;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "starg." << (int)*(uint16*)operand << " " << stack << endl;
ILDEBUG_END;
#endif
	starg(machine, frame, *(uint16*)operand);
	return NULL;
}

extern Method* execute_starg_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ILStack& stack = machine.stack;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "starg.s." << (int)*(byte*)operand << " " << stack << endl;
ILDEBUG_END;
#endif
	starg(machine, frame, *(byte*)operand);
	return NULL;
}
