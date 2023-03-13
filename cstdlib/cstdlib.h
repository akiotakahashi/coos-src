#pragma once

#if !defined(EOF)
#define EOF (-1)
#endif


extern void memclr(void* p, unsigned int size);
extern "C" extern void* memcpy(void *dst, const void *src, unsigned int count);
extern "C" extern void* memmove(void *dst, const void *src, unsigned int count);

extern int stoi(const char* s);
extern int stoi(const wchar_t* s);

extern "C" extern unsigned int strlen(const char* s);
extern "C" extern unsigned int wcslen(const wchar_t* s);
extern bool streql(const char* s1, const char* s2);
