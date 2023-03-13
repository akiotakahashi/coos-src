using System;

namespace CooS.Wrap._System {
	
	class _Buffer {

		static unsafe bool BlockCopyInternal(Array _src, int srcOffset, Array _dst, int dstOffset, int count) {
			SzArrayData* src = (SzArrayData*)Kernel.ObjectToValue(_src).ToPointer();
			SzArrayData* dst = (SzArrayData*)Kernel.ObjectToValue(_dst).ToPointer();
			Tuning.Memory.Copy(&dst->first+dstOffset, &src->first+srcOffset, count);
			return true;
		}

	}

}
