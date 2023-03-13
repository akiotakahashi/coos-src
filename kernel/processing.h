#pragma once

#include "stdtype.h"


namespace Processing {

	// Sync with CooS.ExecutionSystem.RunState
	struct RunState {
		void* thread;
		//void* context;
		//void* appdomain;
	};

}
