#pragma once

#include "kernel.h"


struct Thread {
	int handle;
};

namespace Threading {

	extern void Initialize();
	extern void Reschedule();
	extern void SwitchThread(Context context);
	extern Thread CreateThread(void (*threadmain)(void* param), void* param, void* stacktop);

}
