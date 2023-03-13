#include "stdafx.h"

using namespace Reflection;
using namespace IL;


#if defined(ILDEBUG)
#define COMPARE_DEBUG(text) \
	ILDEBUG_BEGIN; \
	console << v1 << text << v2 << endl; \
	ILDEBUG_END;
#else
#define COMPARE_DEBUG(text)
#endif

#define compareop(comparator, T4, T8, TN) \
	frame.pc += 2; \
	ILStack& stack = machine.stack; \
	switch(stack.getDataType()) { \
	case STACK_TYPE_I4: \
		{ \
		T4 v1; \
		T4 v2; \
		v2 = (T4)stack.popi(); \
		v1 = (T4)stack.popi(); \
		stack.pushi(v1 comparator v2); \
		COMPARE_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_I8: \
		{ \
		T8 v1; \
		T8 v2; \
		v2 = (T8)stack.popl(); \
		v1 = (T8)stack.popl(); \
		stack.pushi(v1 comparator v2); \
		COMPARE_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_N: \
		{ \
		TN v1; \
		TN v2; \
		v2 = (TN)stack.popn(); \
		v1 = (TN)stack.popn(); \
		stack.pushi(v1 comparator v2); \
		COMPARE_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_R4: \
		{ \
		float v1; \
		float v2; \
		v2 = stack.popr4(); \
		v1 = stack.popr4(); \
		stack.pushi(v1 comparator v2); \
		COMPARE_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_R8: \
		{ \
		double v1; \
		double v2; \
		v2 = stack.popr8(); \
		v1 = stack.popr8(); \
		stack.pushi(v1 comparator v2); \
		COMPARE_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_O: \
	case STACK_TYPE_P: \
		{ \
		void* v1; \
		void* v2; \
		v2 = stack.popp(); \
		v1 = stack.popp(); \
		stack.pushi(v1 comparator v2); \
		COMPARE_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_V: \
	default: \
		panic("compareop"); \
		break; \
	}

#define compare(comparator) compareop(comparator,int32,int64,nint) 

#define compare_un(comparator) compareop(comparator,uint32,uint64,unint) 

extern Method* execute_ceq(ILMachine& machine, Frame& frame, const byte* operand) {
	Console& console = getConsole();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	console << "ceq ";
ILDEBUG_END;
#endif
	compare(==);
	return NULL;
}

extern Method* execute_clt(ILMachine& machine, Frame& frame, const byte* operand) {
	Console& console = getConsole();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	console << "clt ";
ILDEBUG_END;
#endif
	compare(<);
	return NULL;
}

extern Method* execute_clt_un(ILMachine& machine, Frame& frame, const byte* operand) {
	Console& console = getConsole();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	console << "clt_un ";
ILDEBUG_END;
#endif
	compare_un(<);
	return NULL;
}

extern Method* execute_cgt(ILMachine& machine, Frame& frame, const byte* operand) {
	Console& console = getConsole();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	console << "cgt ";
ILDEBUG_END;
#endif
	compare(>);
	return NULL;
}

extern Method* execute_cgt_un(ILMachine& machine, Frame& frame, const byte* operand) {
	Console& console = getConsole();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	console << "cgt_un ";
ILDEBUG_END;
#endif
	compare_un(>);
	return NULL;
}
