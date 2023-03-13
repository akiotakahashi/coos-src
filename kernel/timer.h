#pragma once

#include "console.h"


namespace Timer {

	typedef void TIMERHANDLER();

	extern void Initialize(int freqency);
	extern TIMERHANDLER* GetTimerHandler();
	extern void SetTimerHandler(TIMERHANDLER* fp);

}
