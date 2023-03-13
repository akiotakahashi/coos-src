#include "threading.h"
#include "stdlib.h"
#include "stl.h"
#include "pic.h"
#include "interrupt.h"


/*
	スレッド切り替えは非常にシビアであり、コンパイラの最適化に敏感である。
	ここのコードはアセンブラからの推察により記述されていたりするので、
	コードだけレビューして不用意に改変しないこと。
*/

namespace Threading {

	struct SwitchContext {
		uint32 esp;
		uint32 ebp;
		SwitchContext(uint32 sp, uint32 bp) {
			esp = sp;
			ebp = bp;
		}
	};

	typedef std::vector<SwitchContext> SwitchContextList;

	static volatile uint currentThreadId = 0;
	static SwitchContextList* contexts = NULL;

	static __declspec(naked) void __stdcall raw_switch_handler() {
		pushreg;
		pushseg;
		__asm call SwitchThread;
		popseg;
		popreg;
		__asm iretd;
	}

	extern void Initialize() {
		contexts = new SwitchContextList();
		contexts->push_back(SwitchContext(0,0));
		Interrupt::RegisterInterruptGate(0x80, raw_switch_handler);
	}

	extern void Reschedule() {
		__asm int 0x80;
	}

	extern void SwitchThread(Context context) {
		uint32 esp_, ebp_;
		__asm mov esp_, esp;
		__asm mov ebp_, ebp;
		if(contexts->size()>1) {
			contexts->at(currentThreadId) = SwitchContext(esp_,ebp_);
			//getConsole() << "saved esp=" << esp_ << ", ebp=" << ebp_ << endl;
			if(++currentThreadId >= contexts->size()) {
				currentThreadId = 0;
			}
			esp_ = contexts->at(currentThreadId).esp;
			ebp_ = contexts->at(currentThreadId).ebp;
			//if(esp_>0x010000 || ebp_>0x010000) panic("Incorrect Thread Stack");
			__asm {
				mov eax, ebp_;
				mov esp, esp_;
				mov ebp, eax;
			}
		}
	}

	static __declspec(naked) void kickThread() {
		InterruptHandler_Prologue;
		//PIC::NotifyEndOfInterrupt(0);
		InterruptHandler_Epilogue;
	}

	static void disposeThread() {
		DisableInterrupt();
		contexts->erase(contexts->begin()+currentThreadId);
		EnableInterrupt();
	}

	extern Thread CreateThread(void (*threadmain)(void* param), void* param, void* stacktop) {
		uint16 cs_, ds_, ss_;
		uint32 eflags_;
		__asm {
			mov cs_, cs;
			mov ds_, ds;
			mov ss_, ss;
			pushfd;
			pop eflags_;
		}
		uint32* stack = (uint32*)stacktop;
		*--stack = (uint32)param;
		*--stack = (uint32)disposeThread;
		*--stack = (uint32)eflags_;
		*--stack = (uint32)cs_;
		*--stack = (uint32)threadmain;
		*--stack = (uint32)kickThread;
		*--stack = 0;	// ebp
		DisableInterrupt();
		contexts->push_back(SwitchContext((uint32)stack,(uint32)stack));
		EnableInterrupt();
	}

}
