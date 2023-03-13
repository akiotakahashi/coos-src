#include "stdafx.h"

using namespace Reflection;
using namespace IL;


static void idc_i4(ILMachine& machine, int num) {
	machine.stack.pushi(num);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "idc_i4 " << num << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_ldc_i4_0(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 0);
	return NULL;
}

extern Method* execute_ldc_i4_1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 1);
	return NULL;
}

extern Method* execute_ldc_i4_2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 2);
	return NULL;
}

extern Method* execute_ldc_i4_3(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 3);
	return NULL;
}

extern Method* execute_ldc_i4_4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 4);
	return NULL;
}

extern Method* execute_ldc_i4_5(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 5);
	return NULL;
}

extern Method* execute_ldc_i4_6(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 6);
	return NULL;
}

extern Method* execute_ldc_i4_7(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 7);
	return NULL;
}

extern Method* execute_ldc_i4_8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, 8);
	return NULL;
}

extern Method* execute_ldc_i4_m1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	idc_i4(machine, -1);
	return NULL;
}

extern Method* execute_ldc_i4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	idc_i4(machine, *(int*)operand);
	return NULL;
}

extern Method* execute_ldc_i4_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	idc_i4(machine, *(int8*)operand);
	return NULL;
}

extern Method* execute_ldc_i8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 9;
	ILStack& stack = machine.stack;
	stack.pushl(*(int64*)operand);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "idc.i8 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldc_r4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "idc.r4 " << *(uint32*)operand << endl;
ILDEBUG_END;
#endif
	stack.pushr4(*(float*)operand);
	return NULL;
}

extern Method* execute_ldc_r8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 9;
	ILStack& stack = machine.stack;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "idc.r8 " << *(uint64*)operand << endl;
ILDEBUG_END;
#endif
	stack.pushr8(*(double*)operand);
	return NULL;
}
