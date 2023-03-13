#include <memory.h>


extern void memclr(void* p, unsigned int sz) {
	__asm {
		mov	eax, 0;
		mov	edi, p;
		mov	ecx, sz;
		shr	ecx, 2;
		rep stosd;
		mov	ecx, sz;
		and	ecx, 0x3;
		rep stosb;
	}
}
