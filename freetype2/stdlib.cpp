#include <stdio.h>

using namespace System;


extern int my_sprintf(char* buf, char const* format, ... array<Object^>^ values ) {
	return 0;
}

extern void* my_memmove(void* _dst, const void* _src, unsigned int count) {
	unsigned char* dst = (unsigned char*)_dst;
	const unsigned char* src = (const unsigned char*)_src;
	if(src<dst) {
		while(count>0) {
			--count;
			dst[count] = src[count];
		}
	} else {
		unsigned int i = 0;
		while(i<count) {
			dst[i] = src[i];
			++i;
		}
	}
	return _dst;
}

extern int my_strncmp(const char* s1, const char* s2, size_t size) {
	while(size--) {
		unsigned char q1 = *(const unsigned char*)s1++;
		unsigned char q2 = *(const unsigned char*)s2++;
		if(q1<q2) return -1;
		if(q1>q2) return +1;
		if(!q1 || !q2) return (!q2-!q1);
	}
	return 0;
}

extern char* my_strncpy(char* _dst, const char* src, size_t size) {
	char* dst = _dst;
	while(size--) {
		if(!(*dst=*src)) break;
	}
	return _dst;
}

extern char* my_strrchr(const char* s, int ch) {
	const char* p = NULL;
	while(*s) {
		if(*(s++)==ch) p=s;
	}
	return const_cast<char*>(p);
}

extern int my_isdigit(int ch) {
	return ('0'<=ch && ch<='9');
}

extern int my_isalnum(int ch) {
	return my_isdigit(ch) || ('a'<=ch && ch<='z') || ('A'<=ch && ch<='Z');
}

extern int my_isxdigit(int ch) {
	return my_isdigit(ch) || ('a'<=ch && ch<='f') || ('A'<=ch && ch<='F');
}

extern void my_qsort(void* arr, unsigned int, unsigned int , int (*)(const void*, const void*)) {
	throw gcnew NotImplementedException();
}

extern void my_exit(int ret) {
	throw gcnew SystemException(ret.ToString());
}

extern long my_atol(const char* p) {
	String^ s = gcnew String(p);
	return Int32::Parse(s);
}

/*
extern "C" extern void* memcpy(void* _dst, const void* _src, size_t count) {
	__asm {
		mov	ecx, count
		mov	esi, _src
		mov	edi, _dst
		rep movsb
	}
	return _dst;
}

extern "C" extern void* memmove(void* _dst, const void* _src, size_t count) {
	unsigned char* dst = (unsigned char*)_dst;
	const unsigned char* src = (const unsigned char*)_src;
	if(src<dst) {
		while(count>0) {
			--count;
			dst[count] = src[count];
		}
	} else {
		unsigned int i = 0;
		while(i<count) {
			dst[i] = src[i];
			++i;
		}
	}
	return _dst;
}

extern void memclr(void* p, size_t sz) {
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
*/

/*
extern "C" extern size_t strlen(const char* s) {
	int c = 0;
	while(*s) {
		++c;
		++s;
	}
	return c;
}

extern "C" extern int puts(const char *string) {
	Console::Write(new String(string));
	return -1;
}

extern "C" extern void abort() {
	__asm {
		cli;
begin:
		hlt;
		jmp	begin;
	}
}

extern "C" extern int _purecall() {
	Console::Write(S"PURE FUNCTION CALLED");
	abort();
	return 0;
}

extern "C" int atexit(void(*func)()) {
	return 1;
}
*/
