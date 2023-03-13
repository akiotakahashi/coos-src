#include "stdafx.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_br(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	frame.pc += *(int32*)operand;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "br" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_br_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	frame.pc += *(int8*)operand;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "br.s" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#if defined(ILWARN)
#define brwarn(msg) { getConsole()<<msg; /*machine.dump();*/ }
#else
#define brwarn(msg)
#endif

#define branch1(comparator, OffsetType, T4, T8) \
	frame.pc += 1+sizeof(OffsetType); \
	ILStack& stack = machine.stack; \
	switch(stack.getDataType()) { \
	case STACK_TYPE_V: \
		/*brwarn("{BR1:VALUETYPE}");*/ \
	case STACK_TYPE_I4: \
		{ \
		int32 v1; \
		v1 = stack.popi(); \
		if(v1 comparator 0) frame.pc+=*(OffsetType*)operand; \
		} \
		break; \
	case STACK_TYPE_I8: \
		{ \
		int64 v1; \
		v1 = stack.popl(); \
		if(v1 comparator 0) frame.pc+=*(OffsetType*)operand; \
		} \
		break; \
	case STACK_TYPE_N: \
		{ \
		nint v1; \
		v1 = stack.popn(); \
		if(v1 comparator 0) frame.pc+=*(OffsetType*)operand; \
		} \
		break; \
	case STACK_TYPE_R4: \
		{ \
		float v1; \
		v1 = stack.popr4(); \
		if(v1 comparator 0) frame.pc+=*(OffsetType*)operand; \
		} \
		break; \
	case STACK_TYPE_R8: \
		{ \
		double v1; \
		v1 = stack.popr8(); \
		if(v1 comparator 0) frame.pc+=*(OffsetType*)operand; \
		} \
		break; \
	case STACK_TYPE_O: \
	case STACK_TYPE_P: \
		{ \
		void* v1; \
		v1 = stack.popp(); \
		if(v1 comparator 0) frame.pc+=*(OffsetType*)operand; \
		} \
		break; \
	default: \
		panic("branch1"); \
		break; \
	}

extern Method* execute_brfalse(ILMachine& machine, Frame& frame, const byte* operand) {
	branch1(==,int32,int32,int64);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "brfalse" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_brfalse_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch1(==,int8,int32,int64);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "brfalse.s" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_brtrue(ILMachine& machine, Frame& frame, const byte* operand) {
	branch1(!=,int32,int32,int64);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "brtrue" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_brtrue_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch1(!=,int8,int32,int64);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "brtrue.s" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

#if defined(ILDEBUG)
#define BRANCH_DEBUG(text) \
	ILDEBUG_BEGIN; \
	getConsole() << "cond.br " << v1 << text << v2 << endl; \
	ILDEBUG_END;
#else
#define BRANCH_DEBUG(text) ;
#endif

#define branch(comparator, OffsetType, T4, T8, TN) \
	frame.pc += 1+sizeof(OffsetType); \
	ILStack& stack = machine.stack; \
	switch(stack.getDataType()) { \
	case STACK_TYPE_V: \
		/*brwarn("{BR2:VALUETYPE}");*/ \
	case STACK_TYPE_I4: \
		{ \
		T4 v1; \
		T4 v2; \
		v2 = (T4)stack.popi(); \
		v1 = (T4)stack.popi(); \
		if(v1 comparator v2) frame.pc+=*(OffsetType*)operand; \
		BRANCH_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_I8: \
		{ \
		T8 v1; \
		T8 v2; \
		v2 = (T8)stack.popl(); \
		v1 = (T8)stack.popl(); \
		if(v1 comparator v2) frame.pc+=*(OffsetType*)operand; \
		BRANCH_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_N: \
		{ \
		TN v1; \
		TN v2; \
		v2 = (TN)stack.popn(); \
		v1 = (TN)stack.popn(); \
		if(v1 comparator v2) frame.pc+=*(OffsetType*)operand; \
		BRANCH_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_R4: \
		{ \
		float v1; \
		float v2; \
		v2 = stack.popr4(); \
		v1 = stack.popr4(); \
		if(v1 comparator v2) frame.pc+=*(OffsetType*)operand; \
		BRANCH_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_R8: \
		{ \
		double v1; \
		double v2; \
		v2 = stack.popr8(); \
		v1 = stack.popr8(); \
		if(v1 comparator v2) frame.pc+=*(OffsetType*)operand; \
		BRANCH_DEBUG(#comparator); \
		} \
		break; \
	case STACK_TYPE_O: \
	case STACK_TYPE_P: \
		{ \
		void* v1; \
		void* v2; \
		v2 = stack.popp(); \
		v1 = stack.popp(); \
		if(v1 comparator v2) frame.pc+=*(OffsetType*)operand; \
		BRANCH_DEBUG(#comparator); \
		} \
		break; \
	default: \
		machine.panic("branch2"); \
		break; \
	}

extern Method* execute_beq(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(==,int32,int32,int64,nint);
	return NULL;
}

extern Method* execute_beq_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(==,int8,int32,int64,nint);
	return NULL;
}

extern Method* execute_bne_un(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(!=,int32,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_bne_un_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(!=,int8,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_bge(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>=,int32,int32,int64,nint);
	return NULL;
}

extern Method* execute_bge_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>=,int8,int32,int64,nint);
	return NULL;
}

extern Method* execute_bge_un(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>=,int32,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_bge_un_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>=,int8,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_bgt(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>,int32,int32,int64,nint);
	return NULL;
}

extern Method* execute_bgt_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>,int8,int32,int64,nint);
	return NULL;
}

extern Method* execute_bgt_un(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>,int32,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_bgt_un_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(>,int8,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_ble(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<=,int32,int32,int64,nint);
	return NULL;
}

extern Method* execute_ble_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<=,int8,int32,int64,nint);
	return NULL;
}

extern Method* execute_ble_un(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<=,int32,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_ble_un_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<=,int8,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_blt(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<,int32,int32,int64,nint);
	return NULL;
}

extern Method* execute_blt_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<,int8,int32,int64,nint);
	return NULL;
}

extern Method* execute_blt_un(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<,int32,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_blt_un_s(ILMachine& machine, Frame& frame, const byte* operand) {
	branch(<,int8,uint32,uint64,unint);
	return NULL;
}

extern Method* execute_switch(ILMachine& machine, Frame& frame, const byte* operand) {
	int32* p = (int32*)operand;
	uint32 n = *(uint32*)p++;
	frame.pc += 1+sizeof(uint32)+sizeof(int32)*n;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "switch";
	for(uint i=0; i<n; ++i) {
		getConsole() << " " << (uint16)(frame.pc+p[i]);
	}
	getConsole() << endl;
ILDEBUG_END;
#endif
	uint32 value = (uint32)machine.stack.popi();
	if(value<n) {
		frame.pc += p[value];
	}
	return NULL;
}
