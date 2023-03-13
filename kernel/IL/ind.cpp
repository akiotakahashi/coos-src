#include "stdafx.h"

using namespace Reflection;
using namespace IL;



extern Method* execute_ldind_i(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(nint*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.i " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_i1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(char*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.i1 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_i2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(int16*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.i2 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_i4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	if(address==NULL) clrpanic("ldind.i4> Operand is null");
	stack.pushn(*(int32*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.i4 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_i8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushl(*(int64*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.i8 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_r4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushr4(*(float*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.r4 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_r8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushr8(*(double*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.r8 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_ref(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushp(*(void**)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.ref " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_u(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(unint*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.u " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_u1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(byte*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.u1 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_u2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(uint16*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.u2 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldind_u4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint address = stack.popn();
	stack.pushn(*(uint32*)address);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldind.u4 " << stack << ", dst=" << address << endl;
ILDEBUG_END;
#endif
	return NULL;
}

template < typename T >
static void stind(ILStack& stack) {
	T value;
	stack.popmem(&value, sizeof(T));
	void* address = (void*)stack.popn();
	*(T*)address = value;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "stind value=" << value << ", dst=" << address << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_stind_i(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<nint>(machine.stack);
	return NULL;
}

extern Method* execute_stind_i1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<int8>(machine.stack);
	return NULL;
}

extern Method* execute_stind_i2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<int16>(machine.stack);
	return NULL;
}

extern Method* execute_stind_i4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<int32>(machine.stack);
	return NULL;
}

extern Method* execute_stind_i8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<int64>(machine.stack);
	return NULL;
}

extern Method* execute_stind_r4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<float>(machine.stack);
	return NULL;
}

extern Method* execute_stind_r8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<double>(machine.stack);
	return NULL;
}

extern Method* execute_stind_ref(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stind<void*>(machine.stack);
	return NULL;
}
