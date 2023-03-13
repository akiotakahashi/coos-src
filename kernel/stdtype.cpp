#include "stdlib.h"


/*
	64 bit integer operations
*/

extern "C" __declspec(naked) extern void _allshl() {
	// [in,out] eax: 下位32bits
	// [in,out] edx: 上位32bits
	// [in] ecx: シフト数
	__asm {
		cmp cl, 32;
		jge L1;
		shld edx, eax, cl;
		shl eax, cl;
		ret;
L1:
		sub ecx, 32;
		mov edx, eax;
		shl edx, cl;
		xor eax, eax;
		ret;
	}
}

extern "C" __declspec(naked) extern void _allshr() {
	// [in,out] eax: 下位32bits
	// [in,out] edx: 上位32bits
	// [in] cl: シフト数
	__asm {
		cmp cl, 32;
		jge L1;
		shrd eax, edx, cl;
		sar edx, cl;
		ret;
L1:
		sub cl, 32;
		mov eax, edx;
		sar eax, cl;
		sar edx, 31;
		ret;
	}
}

extern "C" __declspec(naked) extern void _aullshr() {
	// [in,out] eax: 下位32bits
	// [in,out] edx: 上位32bits
	// [in] cl: シフト数
	__asm {
		cmp cl, 32;
		jge L1;
		shrd eax, edx, cl;
		shr edx, cl;
		ret;
L1:
		sub cl, 32;
		mov eax, edx;
		shr eax, cl;
		xor edx, edx;
		ret;
	}
}

extern "C" extern void __stdcall my_allmul(unsigned __int32 l1, unsigned __int32 h1, unsigned __int32 l2, unsigned __int32 h2) {
	if(h1|h2) {
		unsigned __int32 l, h;
		__asm {
			push ecx;
			mov eax, l1;
			mov ecx, l2;
			mul ecx;
			mov l, eax;
			mov h, edx;
			pop ecx;
		}
		h += l1*h2 + h1*l2;
		__asm {
			mov eax, l;
			mov edx, h;
		}
	} else {
		__asm {
			push ecx;
			mov eax, l1;
			mov ecx, l2;
			mul ecx;
			pop ecx;
		}
	}
}

static void __fastcall shld64(unsigned long long& lvalue, unsigned long long& rvalue) {
	__asm {
		push esi;
		push edi;
		push eax;
		mov edi, dword ptr [lvalue];
		mov esi, dword ptr [rvalue];
		mov eax, dword ptr [edi];
		shld dword ptr [edi+4], eax, 1;
		mov eax, dword ptr [esi+4];
		shld dword ptr [edi], eax, 1;
		mov eax, dword ptr [esi];
		shld dword ptr [esi+4], eax, 1;
		shl dword ptr [esi], 1;
		pop eax;
		pop edi;
		pop esi;
	}
}

extern "C" extern __declspec(naked) unsigned long long __stdcall my_aulldvrm(unsigned long long op1, unsigned long long op2) {
	// [in] op1: 被演算数
	// [in] op2: 演算数
	// [out] eax: 商下位32bit
	// [out] edx: 商上位32bit
	// [out] ecx: 余下位32bit
	// [out] ebx: 余上位32bit
	__asm {
		push ebp
		mov ebp, esp
		sub esp, __LOCAL_SIZE
	}
	unsigned long long res;
	unsigned long long tmp;
	{
		res = 0;
		tmp = 0;
		__asm {
			push edi;
			push esi;
		}
		for(int i=0; i<64; ++i) {
			res <<= 1;
			shld64(tmp, op1);
			if(tmp>=op2) {
				res |= 1;
				tmp -= op2;
			}
		}
		__asm {
			pop esi;
			pop edi;
		}
		__asm {
			mov eax, dword ptr [res];
			mov edx, dword ptr [res+4];
			mov ecx, dword ptr [tmp];
			mov ebx, dword ptr [tmp+4];
			leave;
			ret 16;
		}
	}
}

extern "C" extern __declspec(naked) long long __stdcall my_alldvrm(long long op1, long long op2) {
	// [in] op1: 被演算数
	// [in] op2: 演算数
	// [out] eax: 商下位32bit
	// [out] edx: 商上位32bit
	// [out] ecx: 余下位32bit
	// [out] ebx: 余上位32bit
	__asm {
		push ebp
		mov ebp, esp
		sub esp, __LOCAL_SIZE
		push esi;
	}
	/*
	The relationship between the multiplicative operators is given by the identity
	(e1 / e2) * e2 + e1 % e2 == e1.
	*/
	if(op1>=0) {
		if(op2>=0) {
			my_aulldvrm((unsigned long long)op1, (unsigned long long)op2);
		} else {
			my_aulldvrm((unsigned long long)op1, (unsigned long long)-op2);
			__asm {
				neg eax;
				adc edx, 0;
				neg edx;
			}
		}
	} else {
		if(op2>=0) {
			my_aulldvrm((unsigned long long)-op1, (unsigned long long)op2);
			__asm {
				neg eax;
				adc edx, 0;
				neg edx;
			}
			__asm {
				neg ecx;
				adc ebx, 0;
				neg ebx;
			}
		} else {
			my_aulldvrm((unsigned long long)-op1, (unsigned long long)-op2);
			__asm {
				neg ecx;
				adc ebx, 0;
				neg ebx;
			}
		}
	}
	__asm {
		pop esi;
		leave;
		ret 16;
	}
}

extern "C" extern long long __stdcall my_alldiv(long long op1, long long op2) {
	bool minus = false;
	if(op1<0) { op1=-op1; minus=!minus; }
	if(op2<0) { op2=-op2; minus=!minus; }
	if(minus) {
		return -(signed long long)((unsigned long long)op1/(unsigned long long)op2);
	} else {
		return (signed long long)((unsigned long long)op1/(unsigned long long)op2);
	}
}

extern "C" extern long long __stdcall my_allrem(long long op1, long long op2) {
	long long div = op1/op2;
	return op1-op2*div;
}

extern "C" extern unsigned long long __stdcall my_aulldiv(unsigned long long op1, unsigned long long op2) {
	unsigned long long ret = 0;
	unsigned long long tmp = 0;
	for(int i=0; i<64; ++i) {
		ret <<= 1;
		shld64(tmp, op1);
		if(tmp>=op2) {
			ret |= 1;
			tmp -= op2;
		}
	}
	return ret;
}

extern "C" extern unsigned long long __stdcall my_aullrem(unsigned long long op1, unsigned long long op2) {
	unsigned long long div = op1/op2;
	return op1-op2*div;
}

extern "C" extern char _fltused = 0;

extern "C" extern __declspec(naked) void __cdecl _allmul(unsigned __int32 l1, unsigned __int32 h1, unsigned __int32 l2, unsigned __int32 h2) {
	__asm { jmp my_allmul }
}

extern "C" extern __declspec(naked) unsigned long long __cdecl _alldiv(long long op1, long long op2) {
	__asm { jmp my_alldiv }
}

extern "C" extern __declspec(naked) unsigned long long __cdecl _allrem(long long op1, long long op2) {
	__asm { jmp my_allrem }
}

extern "C" extern __declspec(naked) unsigned long long __cdecl _aulldiv(unsigned long long op1, unsigned long long op2) {
	__asm { jmp my_aulldiv }
}

extern "C" extern __declspec(naked) unsigned long long __cdecl _aullrem(unsigned long long op1, unsigned long long op2) {
	__asm { jmp my_aullrem }
}

extern "C" extern __declspec(naked) void __cdecl _alldvrm(long long op1, long long op2) {
	__asm { jmp my_alldvrm }
}

extern "C" extern __declspec(naked) void __cdecl _aulldvrm(unsigned long long op1, unsigned long long op2) {
	__asm { jmp my_aulldvrm }
}
