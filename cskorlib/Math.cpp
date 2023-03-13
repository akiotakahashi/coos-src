#include "stdafx.h"
#include "cskorlib.h"
#include <math.h>

using namespace System;


#pragma unmanaged

static double myfloor(double x) {
	return ::floor(x);
}

static double mypow(double x, double y) {
#if 0
	return ::pow(x, y);
#else
	if(x<0) return 0;
	/*
	x^y
	= 2^(y * log2(x))
	= 2^(fyl2x(x,y))
	= f2xm1(fyl2x(x,y))+1
	*/
	__asm {
		fld	y;
		fld	x;
		fyl2x;
		//fstp y;
	}
	// �����ŁA2^y ���v�Z�ł���΂悢
	// y �������Ɛ����ɕ�������
	__asm {
		//fld y;
		fst st(1);
		frndint;
		fsub st(1), st(0);
		fstp y;
		fstp x;
	}
	// �����ŁAy �͐����Ax �͏����ɂȂ�
	// �����ɂ��Čv�Z����
	__asm {
		fld y;
		fld1;
		fscale;
		//fst y;
	}
	// �����ɂ��Čv�Z����
	__asm {
		fld x;
		f2xm1;
		fld1;
		fadd;
		//fst x;
	}
	// ��������
	__asm {
		//fld y;
		//fld x;
		fmul;
		fstp y;
	}
	return y;
#endif
}

static double mysqrt(double x) {
	return ::sqrt(x);
}

static double mysin(double a) {
	return ::sin(a);
}

static double mycos(double a) {
	return ::cos(a);
}

#pragma managed


namespace CooS {
namespace Wrap {
namespace _System {

	public __gc class _Math {
		// �l�C�e�B�u�𒼐ڌĂڂ��Ƃ���� System.Math �ւ̌Ăяo���ɕϊ�����Ă��܂��̂ŁA
		// �������񉴊֐�����邱�ƂŃl�C�e�B�u�̃��[�`�����Ăяo���Ă���B
	private:
		static double Floor(double value) {
			return ::myfloor(value);
		}
		static double Pow(double x, double y) {
			return ::mypow(x, y);
		}
		static double Sqrt(double x) {
			return ::mysqrt(x);
		}
		static double Sin(double a) {
			return ::mysin(a);
		}
		static double Cos(double a) {
			return ::mycos(a);
		}
	};

}
}
}
