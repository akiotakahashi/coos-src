#include "stdafx.h"
#include "../kernel.h"
#include "../AssemblyManager.h"


static long InterlockedExchange(long volatile* Target, long Value) {
	__asm {
		mov ecx, [Target];
		mov eax, Value;
		xchg [ecx], eax;
		mov Value, eax;
	}
	return Value;
}

namespace IL {

	static ILMachine* active_machine = NULL;

	void ILMachine::init() {
		this->next_machine = (ILMachine*)InterlockedExchange((volatile long*)&active_machine, (long)this);
	}

	ILMachine::ILMachine(uint stacksize) : stack(stacksize) {
		init();
	}

	ILMachine::ILMachine() : stack(1024) {
		init();
	}

	ILMachine::~ILMachine() {
		if(active_machine!=this) panic("Machine chain is destroyed.");
		active_machine = this->next_machine;
	}

	ILMachine* ILMachine::getPrevious() {
		return this->next_machine;
	}

	ILMachine& ILMachine::getCurrent() {
		if(active_machine==NULL) ::panic("NO MACHINE");
		return *active_machine;
	}

	static void Output(ILMachine& machine, const wchar_t* msg) {
		machine.stack.pushn(msg);
		AssemblyManager::Execute(L"cscorlib",L"CooS",L"Assist",L"Output",machine);
	}

	void ILMachine::dump() {
		ILMachine* prev = getPrevious();
		if(prev!=NULL) {
			prev->dump();
			Output(ILMachine(), L"--- End of Outer Machine --------------------------------------------\r\n");
		}
		FrameList::iterator it = frames.begin();
		while(it!=frames.end()) {
			Frame& frame = *it;
			std::wstring buf = L"[";
			buf += itos<wchar_t,16>(frame.pc,3);
			buf += L"] ";
			buf += frame.method.getFullName();
			buf += L"\r\n";
			Output(ILMachine(), buf.c_str());
			++it;
		}
	}

	void ILMachine::panic(const char* msg) {
		dump();
#if defined(WIN32)
		::panic(msg);
#else
		getConsole() << msg;
		halt;
#endif
	}

	void ILMachine::panic(const wchar_t* msg) {
		dump();
#if defined(WIN32)
		::panic(msg);
#else
		getConsole() << msg;
		halt;
#endif
	}

	void ILMachine::panic(const std::wstring& msg) {
		dump();
#if defined(WIN32)
		::panic(msg);
#else
		getConsole() << msg;
		halt;
#endif
	}

}
