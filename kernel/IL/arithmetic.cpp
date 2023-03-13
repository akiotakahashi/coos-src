#include "stdafx.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_neg(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint* p = (nint*)&stack;
	switch(stack.getDataType()) {
	case STACK_TYPE_N:	stack.pushn(-stack.popn());	break;
	case STACK_TYPE_I4:	stack.pushi(-stack.popi());	break;
	case STACK_TYPE_I8:	stack.pushl(-stack.popl());	break;
	case STACK_TYPE_R4:	stack.pushr4(-stack.popr4());	break;
	case STACK_TYPE_R8:	stack.pushr8(-stack.popr8());	break;
	case STACK_TYPE_O:	stack.pusho((void*)~stack.popn());	break;
	case STACK_TYPE_P:	stack.pushp((void*)~stack.popn());	break;
	default:	machine.panic("neg");
	}
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "neg " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_not(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	nint* p = (nint*)&stack;
	switch(stack.getDataType()) {
	case STACK_TYPE_V:
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
		getConsole() << "{NOT:V}";
ILDEBUG_END;
#endif
	case STACK_TYPE_N:	stack.pushn(~stack.popn());	break;
	case STACK_TYPE_I4:	stack.pushi(~stack.popi());	break;
	case STACK_TYPE_I8:	stack.pushl(~stack.popl());	break;
	case STACK_TYPE_R4:	stack.pushi(~stack.popi());	break;
	case STACK_TYPE_R8:	stack.pushl(~stack.popl());	break;
	case STACK_TYPE_O:	stack.pusho((void*)~stack.popn());	break;
	case STACK_TYPE_P:	stack.pushp((void*)~stack.popn());	break;
	default:
		machine.panic("not");
	}
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "not " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#define stack machine.stack

#if defined(ILWARN)
#define ariwarn(msg) { getConsole()<<msg; /*machine.dump();*/ }
#else
#define ariwarn(msg)
#endif

#define arithmetictypedop(OPERATOR, TI, TL, TN) \
	switch(stack.getDataType()) { \
	default: ariwarn("{AOP:UNK}"); \
	case STACK_TYPE_N:	{ TN v2=(TN)stack.popn(); TN v1=(TN)stack.popn(); stack.pushn(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_I4:	{ TI v2=(TI)stack.popi(); TI v1=(TI)stack.popi(); stack.pushi(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_I8:	{ TL v2=(TL)stack.popl(); TL v1=(TL)stack.popl(); stack.pushl(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_R4:	{ float v2=stack.popr4(); float v1=stack.popr4(); stack.pushr4(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_R8:	{ double v2=stack.popr8(); double v1=stack.popr8(); stack.pushr8(v1 OPERATOR v2);	}	break; \
	case STACK_TYPE_O:	{ nint v2=stack.popn(); nint v1=stack.popn(); stack.pusho((void*)(v1 OPERATOR v2));	}	break; \
	case STACK_TYPE_P:	{ nint v2=stack.popn(); nint v1=stack.popn(); stack.pushp((void*)(v1 OPERATOR v2));	}	break; \
	}

#define arithmeticop(OPERATOR) arithmetictypedop(OPERATOR,int32,int64,nint)

#define arithmeticop_un(OPERATOR) arithmetictypedop(OPERATOR,uint32,uint64,unint)

extern Method* execute_add(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	arithmeticop(+);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "add " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_sub(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	arithmeticop(-);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "sub " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_mul(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	arithmeticop(*);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "mul " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_div(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	arithmeticop(/);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "div " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_div_un(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	arithmeticop_un(/);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "div.un " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#define shiftop(OPERATOR, TI, TL, TN) \
	uint32 shiftAmount = stack.popi(); \
	switch(stack.getDataType()) { \
	default: machine.panic("{SHIFT:UNEXPECTED}"); \
	case STACK_TYPE_V:	\
	case STACK_TYPE_I4:	stack.pushi((TI)stack.popi() OPERATOR shiftAmount);	break; \
	case STACK_TYPE_I8:	stack.pushl((TL)stack.popl() OPERATOR shiftAmount);	break; \
	case STACK_TYPE_N:	stack.pushn((TN)stack.popn() OPERATOR shiftAmount);	break; \
	case STACK_TYPE_O:	stack.pusho((void*)(stack.popn() OPERATOR shiftAmount));	break; \
	case STACK_TYPE_P:	stack.pushp((void*)(stack.popn() OPERATOR shiftAmount));	break; \
	case STACK_TYPE_Unk: getConsole() << "{SHIFT:OPERAND TYPE UNKNOWN|" << "}"; \
	}

extern Method* execute_shl(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	shiftop(<<, int32, int64, nint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "shl " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_shr(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	shiftop(>>, int32, int64, nint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "shr " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_shr_un(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	shiftop(>>, uint32, uint64, nint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "shr.un " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#define remop(OPERATOR, TI, TL, TN) \
	switch(stack.getDataType()) { \
	case STACK_TYPE_O: \
	case STACK_TYPE_P: \
	case STACK_TYPE_N:	{ TN rem=(TN)stack.popn(); stack.pushn((TN)stack.popn() OPERATOR rem); }	break; \
	case STACK_TYPE_I4:	{ TI rem=(TI)stack.popi(); stack.pushi((TI)stack.popi() OPERATOR rem); }	break; \
	case STACK_TYPE_I8:	{ TL rem=(TL)stack.popl(); stack.pushl((TL)stack.popl() OPERATOR rem); }	break; \
	/* \
	case STACK_TYPE_R4:	{ float rem=stack.popr4(); stack.pushr4(stack.popr4() OPERATOR rem); }		break; \
	case STACK_TYPE_R8:	{ double rem=stack.popr8(); stack.pushr8(stack.popr8() OPERATOR rem); }		break; \
	*/ \
	default:	panic("rem:0x"+itos<char,16>((int)stack.getDataType())); \
	}

extern Method* execute_rem(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	remop(%, int32, int64, nint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "rem " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_rem_un(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	remop(%, uint32, uint64, nint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "rem.un " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#define bitwiseop(OPERATOR, TI, TL, TN) \
	switch(stack.getDataType()) { \
	default: panic("bitwise" #OPERATOR); \
	case STACK_TYPE_V: \
	case STACK_TYPE_Unk: /*ariwarn("{BITWISE}");*/ \
	case STACK_TYPE_N:	{ TN v2=(TN)stack.popn(); TN v1=(TN)stack.popn(); stack.pushn(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_I4:	{ TI v2=(TI)stack.popi(); TI v1=(TI)stack.popi(); stack.pushi(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_I8:	{ TL v2=(TL)stack.popl(); TL v1=(TL)stack.popl(); stack.pushl(v1 OPERATOR v2);	}		break; \
	case STACK_TYPE_O:	{ nint v2=stack.popn(); nint v1=stack.popn(); stack.pusho((void*)(v1 OPERATOR v2));	}	break; \
	case STACK_TYPE_P:	{ nint v2=stack.popn(); nint v1=stack.popn(); stack.pushp((void*)(v1 OPERATOR v2));	}	break; \
	}

extern Method* execute_and(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	bitwiseop(&,uint32,uint64,unint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "and " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_or(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	bitwiseop(|,uint32,uint64,unint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "or " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_xor(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	bitwiseop(^,uint32,uint64,unint);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "xor " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_add_ovf(ILMachine& machine, Frame& frame, const byte* operand) {
	return execute_add(machine, frame, operand);
}

extern Method* execute_sub_ovf(ILMachine& machine, Frame& frame, const byte* operand) {
	return execute_sub(machine, frame, operand);
}

extern Method* execute_mul_ovf(ILMachine& machine, Frame& frame, const byte* operand) {
	return execute_mul(machine, frame, operand);
}

extern Method* execute_shl_ovf(ILMachine& machine, Frame& frame, const byte* operand) {
	machine.panic(__FUNCTION__);
}

extern Method* execute_shr_un_ovf(ILMachine& machine, Frame& frame, const byte* operand) {
	machine.panic(__FUNCTION__);
}
