#pragma once

#include "..\ilengine.h"


namespace IL {

	extern void AssignMemory(void* address, uint size);

}

#if defined(WIN32)
#define ILTRACE		// Enable ILTRACE to show verbose.
#define ILDEBUG		// Enable ILDEBUG to show details.
//#define ILWARN		// Enable ILWARN to show warning.
#else
#define ILTRACE		// Enable ILTRACE to show verbose.
#define ILDEBUG		// Enable ILDEBUG to show details.
//#define ILWARN		// Enable ILWARN to show warning.
#endif

#define ILDEBUG_BEGIN	if(!::IL::IsDebugEnabled()) {} else {
#define ILDEBUG_END		}
