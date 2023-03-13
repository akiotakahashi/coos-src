#include "stdafx.h"
#include "cskorlib.h"
#include <stdlib.h>

using namespace System;


#pragma unmanaged

extern int main(int argc, char* argv[]) {
	abort();
}

#pragma managed


namespace CooS {
namespace Wrap {
namespace _CooS {
        
	public __gc class _Kernel {
	private:
		static void* ValueToObject(void* value) {
			return value;
		}
		static IntPtr ValueToObject(IntPtr value) {
			return value;
		}
		static Object* ObjectToValue(Object* obj) {
			return obj;
		}
	};

	public __gc class _Engine {
	public:
		static bool get_Infrastructured() {
			return true;
		}
	};

}
}
}
