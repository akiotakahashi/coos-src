#include "stl.h"
#include "threading.h"
#include "console.h"


/*
extern "C" extern long __stdcall InterlockedExchange(long volatile* Target, long Value) {
	long v = Value;
	__asm {
		mov ecx, [Target];
		mov eax, Value;
		xchg [ecx], eax;
		mov Value, eax;
	}
	if(&getConsole()!=NULL) {
		// Almost Target=0, Value=1
		if(Value!=1 && Value!=0) {
			getConsole() << newline << "Target=" << (uint)Value << " [" << (void*)Target << "]" << ", Value=" << (uint)v << endl;
			//panic("InterlockedExchange");
		}
	}
	return Value;
}

extern "C" extern void __stdcall Sleep(unsigned long dwMilliseconds) {
	// This may indicate asynchronous operation.
	panic("stlport:Sleep");
}
*/

/*
exception::exception() _STLP_NOTHROW {
	panic("stlport:exception");
}

exception::exception(const char* const &) _STLP_NOTHROW {
	panic("stlport:exception");
}

exception::~exception() _STLP_NOTHROW {
}

const char* exception::what() const _STLP_NOTHROW {
	return "class exception";
}
*/
