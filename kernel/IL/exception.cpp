#include "stdafx.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_leave(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	frame.pc += *(int32*)operand;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "leave" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_leave_s(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	frame.pc += *(int8*)operand;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "leave.s" << endl;
ILDEBUG_END;
#endif
	return NULL;
}
