#include <stdlib.h>
#include <stdio.h>
#include <setjmp.h>


extern "C" FILE *fopen( 
   const char *filename,
   const char *mode)
{
	::abort();
}

extern "C" int fclose( 
   FILE *stream)
{
	::abort();
}

extern "C" long ftell( 
   FILE *stream)
{
	::abort();
}

extern "C" int fseek( 
   FILE *stream,
   long offset,
   int origin)
{
	::abort();
}

extern "C" size_t fread( 
   void *buffer,
   size_t size,
   size_t count,
   FILE *stream)
{
	::abort();
}

extern "C" int ft_setjmp(int* env) {
	// NOP
	return 0;
}

extern "C" void ft_longjmp(int* env, int value) {
	::abort();
}
