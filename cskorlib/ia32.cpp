#include "stdafx.h"
#include "cskorlib.h"
#include "../kernel/io.h"

using namespace System;
using namespace System::Runtime::InteropServices;


static unsigned short __native_ldcs() {
	unsigned short rCS;
	__asm { mov rCS, cs }
	return rCS;
}

static void __native_int3() {
	__asm { int 3 }
}

static void __native_hlt() {
	__asm hlt;
}

static void __native_cli() {
	__asm cli;
}

static void __native_sti() {
	__asm sti;
}

static uint64 __native_rdtsc() {
	uint32 h,l;
	__asm {
		rdtsc;
		mov h,edx;
		mov l,eax;
	}
	return ((uint64)h<<32) | l;
}

static void __native_lidt(void* pidt) {
	__asm {
		mov eax, dword ptr [pidt];
		lidt [eax];
	}
}

static void __native_sidt(void* pidt) {
	__asm {
		mov eax, dword ptr [pidt];
		sidt [eax];
	}
}

static void* __native_getReturnAddress(int depth) {
	void** rip;
	__asm { mov rip, ebp }
	while(depth-->0) {
		rip = (void**)*rip;
	}
	return *(rip+1);
}

#define __NATIVE_INT_IMPL(a, b, c) static void __native_int_ ## a ## b ## c() { __asm { int 0 ## a ## b ## c } }

#define __NATIVE_INT_IMPL_8(a, b) \
	__NATIVE_INT_IMPL(a, b, 0) \
	__NATIVE_INT_IMPL(a, b, 1) \
	__NATIVE_INT_IMPL(a, b, 2) \
	__NATIVE_INT_IMPL(a, b, 3) \
	__NATIVE_INT_IMPL(a, b, 4) \
	__NATIVE_INT_IMPL(a, b, 5) \
	__NATIVE_INT_IMPL(a, b, 6) \
	__NATIVE_INT_IMPL(a, b, 7)

#define __NATIVE_INT_IMPL_64(a) \
	__NATIVE_INT_IMPL_8(a, 0) \
	__NATIVE_INT_IMPL_8(a, 1) \
	__NATIVE_INT_IMPL_8(a, 2) \
	__NATIVE_INT_IMPL_8(a, 3) \
	__NATIVE_INT_IMPL_8(a, 4) \
	__NATIVE_INT_IMPL_8(a, 5) \
	__NATIVE_INT_IMPL_8(a, 6) \
	__NATIVE_INT_IMPL_8(a, 7)

#pragma warning(disable: 4793)
__NATIVE_INT_IMPL_64(0);
__NATIVE_INT_IMPL_64(1);
__NATIVE_INT_IMPL_64(2);
__NATIVE_INT_IMPL_64(3);
#pragma warning(default: 4793)

#define __NATIVE_INT_CASE(a, b, c) case 0 ## a ## b ## c: __native_int_ ## a ## b ## c(); break;

#define __NATIVE_INT_CASE_8(a, b) \
	__NATIVE_INT_CASE(a, b, 0) \
	__NATIVE_INT_CASE(a, b, 1) \
	__NATIVE_INT_CASE(a, b, 2) \
	__NATIVE_INT_CASE(a, b, 3) \
	__NATIVE_INT_CASE(a, b, 4) \
	__NATIVE_INT_CASE(a, b, 5) \
	__NATIVE_INT_CASE(a, b, 6) \
	__NATIVE_INT_CASE(a, b, 7)

#define __NATIVE_INT_CASE_64(a) \
	__NATIVE_INT_CASE_8(a, 0) \
	__NATIVE_INT_CASE_8(a, 1) \
	__NATIVE_INT_CASE_8(a, 2) \
	__NATIVE_INT_CASE_8(a, 3) \
	__NATIVE_INT_CASE_8(a, 4) \
	__NATIVE_INT_CASE_8(a, 5) \
	__NATIVE_INT_CASE_8(a, 6) \
	__NATIVE_INT_CASE_8(a, 7)

namespace CooS {

	namespace Architectures {

		namespace IA32 {

			[StructLayoutAttribute(LayoutKind::Sequential, Pack=1)]
			public __value struct InterruptDescriptorTable {
				uint16 limit;
				uint32 start;
			};
			
			[StructLayoutAttribute(LayoutKind::Sequential, Pack=1)]
			public __value struct InterruptDescriptor {
				uint16 handler_l;
				uint16 segment;
				uint16 flags;
				/*
				uint16 flags		: 13;
				uint16 dpl			: 2;
				uint16 p			: 1;
				*/
				uint16 handler_h;
			};

			public __gc class Instruction {
			public:
				static void int3() {
					__native_int3();
				}
				static void hlt() {
					__native_hlt();
				}
			public:
				static void sti() {
					__native_sti();
				}
				static void cli() {
					__native_cli();
				}
				static void intn(int iv) {
					switch(iv) {
					__NATIVE_INT_CASE_64(0)
					__NATIVE_INT_CASE_64(1)
					__NATIVE_INT_CASE_64(2)
					__NATIVE_INT_CASE_64(3)
					}
				}
			public:
				static unsigned short ldcs() {
					return __native_ldcs();
				}
			public:
				static void lidt(InterruptDescriptorTable idt) {
					InterruptDescriptorTable __pin* p = &idt;
					__native_lidt(p);
				}
				static void sidt([Out] InterruptDescriptorTable* pidt) {
					InterruptDescriptorTable __pin* p = pidt;
					__native_sidt(p);
				}
			public:
				static void outb(unsigned short port, unsigned __int8 data) {
					outp8(port, data);
				}
				static void outw(unsigned short port, unsigned __int16 data) {
					outp16(port, data);
				}
				static void outd(unsigned short port, unsigned __int32 data) {
					outp32(port, data);
				}
				static unsigned __int8 inb(unsigned short port) {
					return inp8(port);
				}
				static unsigned __int16 inw(unsigned short port) {
					return inp16(port);
				}
				static unsigned __int32 ind(unsigned short port) {
					return inp32(port);
				}
				static void inw(unsigned short port, void* p, int count, unsigned short watchport, unsigned char mask, unsigned char value) {
					inp16(port, p, count, watchport, mask, value);
				}
			public:
				static uint64 rdtsc() {
					return __native_rdtsc();
				}
			public:
				static IntPtr GetReturnAddress(int depth) {
					return System::IntPtr(__native_getReturnAddress(depth+2));
				}
			};
		}
	}
}
