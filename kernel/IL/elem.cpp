#include "stdafx.h"

using namespace Reflection;
using namespace IL;


/*
DUMMY_INST(ldelem_r4);
DUMMY_INST(ldelem_r8);
DUMMY_INST(ldelem_ref);
*/

template < typename ST, typename ET >
static void stelem(ILMachine& machine) {
	ILStack& stack = machine.stack;
	ST value;
	stack.popmem(&value, sizeof(value));
	int32 index = stack.popi();
	Array* array = (Array*)stack.popo();
	if(array==NULL) machine.panic("array is null");
	if(index<0 || (int)array->length<=index) machine.panic("index is out of range");
	((ET*)&array->start_elem)[index] = (ET)value;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "stelem." << (int)sizeof(ET) << " value=" << value << ", array=" << array << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_stelem_i(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<nint,nint>(machine);
	return NULL;
}

extern Method* execute_stelem_i1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<nint,int8>(machine);
	return NULL;
}

extern Method* execute_stelem_i2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<nint,int16>(machine);
	return NULL;
}

extern Method* execute_stelem_i4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<nint,int32>(machine);
	return NULL;
}

extern Method* execute_stelem_i8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<int64,int64>(machine);
	return NULL;
}

extern Method* execute_stelem_r4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<double,float>(machine);
	return NULL;
}

extern Method* execute_stelem_r8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<double,double>(machine);
	return NULL;
}

extern Method* execute_stelem_ref(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	stelem<void*,void*>(machine);
	return NULL;
}

extern Method* execute_ldelema(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	uint32 index = stack.popi();
	Array* array = (Array*)stack.popo();
	if(array==NULL) clrpanic("ldelema> array is null");
	if(array->length<=index) clrpanic(("ldelema> index ("+itos<char,10>(index)+") is out of range [0,"+itos<char,10>(array->length)+"]").c_str());
	byte* p = (byte*)&array->start_elem;
	stack.pushp(p+array->elemsize*index);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldelema " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

template < typename ST, typename ET, ELEMENT_TYPE datatype >
static void ldelem(ILMachine& machine) {
	ILStack& stack = machine.stack;
	int index = (int)stack.popi();
	Array* array = (Array*)stack.popo();
	if(array==NULL) machine.panic("array is null");
	if(index<0 || (int)array->length<=index) {
		getConsole().MakeNewLine();
		getConsole() << "Range: [0, " << (int)array->length-1 << "]" << endl;
		getConsole() << "Index: " << index << endl;
		machine.panic("ldelem: index is out of range");
	}
	stack.pushmem((ET*)&array->start_elem+index, datatype, array->elemsize);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldelem." << (int)sizeof(ET) << " " << stack << ", array=" << array << endl;
ILDEBUG_END;
#endif
}

extern Method* execute_ldelem_i(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,nint,ELEMENT_TYPE_I>(machine);
	return NULL;
}

extern Method* execute_ldelem_i1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,int8,ELEMENT_TYPE_I1>(machine);
	return NULL;
}

extern Method* execute_ldelem_i2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,int16,ELEMENT_TYPE_I2>(machine);
	return NULL;
}

extern Method* execute_ldelem_i4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,int32,ELEMENT_TYPE_I4>(machine);
	return NULL;
}

extern Method* execute_ldelem_i8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<int64,int64,ELEMENT_TYPE_I8>(machine);
	return NULL;
}

extern Method* execute_ldelem_r4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<float,float,ELEMENT_TYPE_R4>(machine);
	return NULL;
}

extern Method* execute_ldelem_r8(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<double,double,ELEMENT_TYPE_R8>(machine);
	return NULL;
}

extern Method* execute_ldelem_ref(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<void*,void*,ELEMENT_TYPE_BYREF>(machine);
	return NULL;
}

extern Method* execute_ldelem_u1(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,uint8,ELEMENT_TYPE_U1>(machine);
	return NULL;
}

extern Method* execute_ldelem_u2(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,uint16,ELEMENT_TYPE_U2>(machine);
	return NULL;
}

extern Method* execute_ldelem_u4(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ldelem<nint,uint32,ELEMENT_TYPE_U4>(machine);
	return NULL;
}
