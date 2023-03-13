#include "kernel.h"
#include "io.h"
#include "pic.h"
#include "timer.h"
#include "threading.h"
#include "interrupt.h"


namespace Timer {

	static TIMERHANDLER* handler = NULL;

	extern TIMERHANDLER* GetTimerHandler() {
		return handler;
	}

	extern void SetTimerHandler(TIMERHANDLER* fp) {
		handler = fp;
	}

	static void handleTimerInterrupt() {
		TIMERHANDLER* fp = handler;
		if(fp) fp();
	}

	static __declspec(naked) void __stdcall raw_timer_handler() {
		pushreg;
		pushseg;
		_InterruptHandler_Prologue;
		handleTimerInterrupt();
		PIC::NotifyEndOfInterrupt(0);
		_InterruptHandler_Epilogue;
		__asm call Threading::SwitchThread;
		popseg;
		popreg;
		__asm iretd;
	}

	#define PIT_MODE   0x43
	#define PIT_COUNT0 0x40

	extern void Initialize(int freqency) {
		// setup timer interval
		ulong timer_count = 1193181 / freqency;
		outp8(PIT_MODE, 0x36);
		outp8(PIT_COUNT0, (byte)(timer_count & 0xff));
		outp8(PIT_COUNT0, (byte)(timer_count >> 8));
		// register interrupt handler
		PIC::RegisterInterruptHandler(0, raw_timer_handler);
		PIC::EnableInterrupt(0);
	}

}
