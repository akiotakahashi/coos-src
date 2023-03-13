#include "stdafx.h"
#include "cskorlib.h"
#include <memory.h>
#include <string.h>

using namespace System;


namespace CooS {

	namespace Tuning {
        
		public __gc class Memory {
		public:

			static void Clear(void* p, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memset(p, 0, size);
			}

			static void Clear(IntPtr p, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memset(p.ToPointer(), 0, size);
			}

			static void Fill(void* p, unsigned char data, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memset(p, data, size);
			}

			static void Fill(IntPtr p, unsigned char data, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memset(p.ToPointer(), data, size);
			}

			static void Copy(void* dst, const void* src, unsigned int size) {
				memcpy(dst, src, size);
			}

			static void Copy(void* dst, const void* src, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memcpy(dst, src, (unsigned int)size);
			}

			static void Copy(IntPtr dst, IntPtr src, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memcpy(dst.ToPointer(), src.ToPointer(), (unsigned int)size);
			}

			static void Move(void* dst, const void* src, unsigned int size) {
				memmove(dst, src, size);
			}

			static void Move(void* dst, const void* src, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memmove(dst, src, size);
			}

			static void Move(IntPtr dst, IntPtr src, int size) {
				if(size<0) throw new ArgumentOutOfRangeException();
				memmove(dst.ToPointer(), src.ToPointer(), size);
			}

		};

	}

}
