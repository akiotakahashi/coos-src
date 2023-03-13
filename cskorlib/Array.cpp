#include "stdafx.h"
#include "cskorlib.h"
#include <memory.h>

using namespace System;


namespace CooS {
namespace Wrap {
namespace System {

	/*
	public __gc class _Array {
	public:
		Type* type;
		int elemsize;
		int length;
		unsigned char start_elem;
	private:
		static void ClearInternal(Array* a, int index, int count) {
			_Array* arr = static_cast<_Array*>(static_cast<Object*>(a));
			int size = arr->elemsize;
			_Array __pin* p = arr;
			memset(&p->start_elem+size*index, 0, size*count);
		}
		static bool FastCopy(Array* source, int source_idx, Array* dest, int dest_idx, int length) {
			_Array* src = static_cast<_Array*>(static_cast<Object*>(source));
			_Array* dst = static_cast<_Array*>(static_cast<Object*>(dest));
			if(src->type!=dst->type) return false;
			_Array __pin* psrc = src;
			_Array __pin* pdst = dst;
			memcpy(
				(char*)&pdst->start_elem+src->elemsize*dest_idx,
				(char*)&psrc->start_elem+dst->elemsize*source_idx,
				src->elemsize*length);
			return true;
		}
	};
	*/

}
}
}
