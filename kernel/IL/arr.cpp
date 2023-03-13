#include "stdafx.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_newarr(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) panic("newarr can't find TypeDef");
	unint count = stack.popn();
	stack.pusho(IL::NewArray(*typdef,count));
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "newarr " << stack << ":" << (uint16)typdef->getVariableSize() << "[" << (int)count << "]" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_ldlen(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	Array* array = (Array*)stack.popo();
	if(array==NULL) machine.panic("ldlen: null");
	stack.pushn(array->length);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "ldlen " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}
