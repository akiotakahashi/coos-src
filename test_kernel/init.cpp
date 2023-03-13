#include "stdafx.h"
#include "../kernel/kernel.h"
#include "../kernel/stdlib.h"
#include "../kernel/memory.h"


#pragma warning(disable: 4073)
#pragma init_seg(lib)

extern mspace g_mspace = NULL;
extern "C" mspace create_mspace_with_base(void* base, size_t capacity, int locked);

struct MEMINIT {
	MEMINIT() {
		const int knlmemsz = 32*1024*1024;
		/*
		MemoryManager* p = (MemoryManager*)::malloc(sizeof(MemoryManager));
		p->Initialize(::malloc(knlmemsz), knlmemsz);
		setMemoryManager(*p);
		*/
		g_mspace = create_mspace_with_base(::malloc(knlmemsz), knlmemsz, 0);
	}
} ___meminit;


namespace stlpmtx_std {
	extern void __stl_throw_length_error(const char*) {
		panic("__stl_throw_length_error");
	}
}
