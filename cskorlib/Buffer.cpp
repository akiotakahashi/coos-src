#include "stdafx.h"
#include "cskorlib.h"
#include <memory.h>

using namespace System;


#include "Array.cpp"

namespace CooS {
namespace Wrap {
namespace System {

	/*
	public __gc class _Buffer {
	public:
		static bool BlockCopyInternal(Array* _src, int srcOffset, Array* _dst, int dstOffset, int count) {
			_Array* src = static_cast<_Array*>(static_cast<Object*>(_src));
			_Array* dst = static_cast<_Array*>(static_cast<Object*>(_dst));
			_Array __pin* ps = src;
			_Array __pin* pd = dst;
			memcpy(&pd->start_elem+dstOffset, &ps->start_elem+srcOffset, count);
			return true;
		}
	};
	*/

}
}
}
