#include "stdafx.h"

using namespace Reflection;
using namespace IL;

#define stack machine.stack

#if defined(ILWARN)
#define convmsg(msg) getConsole() << msg;
#else
#define convmsg(msg)
#endif

#define conv_sn(suffix, type) \
	switch(stack.getDataType()) { \
	case STACK_TYPE_V: \
		convmsg("{conv.sn." #suffix ":VALUETYPE}"); \
	case STACK_TYPE_N: \
		stack.push ## suffix((type)stack.popn()); \
		break; \
	case STACK_TYPE_I4: \
		stack.push ## suffix((type)stack.popi()); \
		break; \
	case STACK_TYPE_I8: \
		stack.push ## suffix((type)stack.popl()); \
		break; \
	case STACK_TYPE_R4: \
		stack.push ## suffix((type)stack.popr4()); \
		break; \
	case STACK_TYPE_R8: \
		stack.push ## suffix((type)stack.popr8()); \
		break; \
	case STACK_TYPE_O: \
		stack.push ## suffix((type)(nint)stack.popo()); \
		break; \
	case STACK_TYPE_P: \
		stack.push ## suffix((type)(nint)stack.popp()); \
		break; \
	default: \
		convmsg("{conv." #suffix ":UNK}"); \
		stack.push ## suffix((type)stack.popn()); \
	}

#define conv_un(suffix, type) \
	switch(stack.getDataType()) { \
	case STACK_TYPE_V: \
		convmsg("{conv.un." #suffix ":VALUETYPE}"); \
	case STACK_TYPE_N: \
		stack.push ## suffix((type)(unint)stack.popn()); \
		break; \
	case STACK_TYPE_I4: \
		stack.push ## suffix((type)(uint32)stack.popi()); \
		break; \
	case STACK_TYPE_I8: \
		stack.push ## suffix((type)(uint64)stack.popl()); \
		break; \
	case STACK_TYPE_R4: \
		stack.push ## suffix((type)stack.popr4()); \
		break; \
	case STACK_TYPE_R8: \
		stack.push ## suffix((type)stack.popr8()); \
		break; \
	case STACK_TYPE_O: \
		stack.push ## suffix((type)(unint)stack.popo()); \
		break; \
	case STACK_TYPE_P: \
		stack.push ## suffix((type)(unint)stack.popp()); \
		break; \
	default: \
		convmsg("{conv." #suffix ":UNK}"); \
		stack.push ## suffix((type)stack.popn()); \
	}

extern Method* execute_conv_i(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(n, nint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.i " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_u(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_un(n, unint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.u " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_i1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(i, int8);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.i1 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_u1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_un(i, uint8);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.u1 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_i2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(i, int16);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.i2 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_u2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_un(i, uint16);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.u2 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_i4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(i, int32);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.i4 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_u4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_un(i, uint32);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.u4 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_i8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(l, int64);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.i8 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_u8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_un(l, uint64);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.u8 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_r4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(r4, float);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.r4 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_r8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	conv_sn(r8, double);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.r8 " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_conv_r_un(ILMachine& machine, Frame& frame, const byte* operand) {
	//TODO: treat as unsigned
	frame.pc += 1;
	conv_sn(r8, double);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "conv.r.un " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#define DUMMY_CONV_OVF(type) \
	extern Method* execute_conv_ovf_ ## type(ILMachine& machine, Frame& frame, const byte* operand) \
	{ return execute_conv_ ## type(machine, frame, operand); }

DUMMY_CONV_OVF(i);
DUMMY_CONV_OVF(u);
DUMMY_CONV_OVF(i1);
DUMMY_CONV_OVF(i2);
DUMMY_CONV_OVF(i4);
DUMMY_CONV_OVF(u1);
DUMMY_CONV_OVF(u2);
DUMMY_CONV_OVF(u4);

#define DUMMY_CONV_OVF_UN(type) \
	extern Method* execute_conv_ovf_ ## type ## _un(ILMachine& machine, Frame& frame, const byte* operand) \
	{ return execute_conv_ ## type(machine, frame, operand); }

DUMMY_CONV_OVF_UN(i);
DUMMY_CONV_OVF_UN(u);
DUMMY_CONV_OVF_UN(i1);
DUMMY_CONV_OVF_UN(i2);
DUMMY_CONV_OVF_UN(i4);
DUMMY_CONV_OVF_UN(u1);
DUMMY_CONV_OVF_UN(u2);
DUMMY_CONV_OVF_UN(u4);
