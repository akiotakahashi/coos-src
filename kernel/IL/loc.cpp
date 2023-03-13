#include "stdafx.h"

using namespace Reflection;
using namespace IL;


static void ldloc(ILMachine& machine, Frame& frame, uint index) {
	ILStack& stack = machine.stack;
	int offset = frame.method.getLocalVarOffset(index);
	void* p = frame.vbp+offset;
	ELEMENT_TYPE type = frame.method.getLocalVarElemType(index);
	stack.pushmem(p, type, frame.method.getLocalVarSize(index));
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldloc." << (int)index << " " << stack << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_ldloc_0(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldloc(machine, frame, 0);
	return NULL;
}

extern Method* execute_ldloc_1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldloc(machine, frame, 1);
	return NULL;
}

extern Method* execute_ldloc_2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldloc(machine, frame, 2);
	return NULL;
}

extern Method* execute_ldloc_3(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldloc(machine, frame, 3);
	return NULL;
}

extern Method* execute_ldloc_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ldloc(machine, frame, *operand);
	return NULL;
}

extern Method* execute_ldloc(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ldloc(machine, frame, *(uint*)operand);
	return NULL;
}

static void ldloca(ILMachine& machine, Frame& frame, uint index) {
	ILStack& stack = machine.stack;
	int offset = frame.method.getLocalVarOffset(index);
	stack.pushp(&frame.vbp[offset]);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldloca " << stack << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_ldloca(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ldloca(machine, frame, *(uint*)operand);
	return NULL;
}

extern Method* execute_ldloca_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ldloca(machine, frame, *(byte*)operand);
	return NULL;
}

static void stloc(ILMachine& machine, Frame& frame, uint index) {
	ILStack& stack = machine.stack;
	int offset = frame.method.getLocalVarOffset(index);
	uint size = frame.method.getLocalVarSize(index);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "stloc." << (int)index << " " << stack << ":" << (int)size << endl;
ILDEBUG_END;
#endif
	stack.popmem(&frame.vbp[offset], size);
}

extern Method* execute_stloc_0(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stloc(machine, frame, 0);
	return NULL;
}

extern Method* execute_stloc_1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stloc(machine, frame, 1);
	return NULL;
}

extern Method* execute_stloc_2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stloc(machine, frame, 2);
	return NULL;
}

extern Method* execute_stloc_3(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stloc(machine, frame, 3);
	return NULL;
}

extern Method* execute_stloc(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	stloc(machine, frame, *(uint*)operand);
	return NULL;
}

extern Method* execute_stloc_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	stloc(machine, frame, *(byte*)operand);
	return NULL;
}
