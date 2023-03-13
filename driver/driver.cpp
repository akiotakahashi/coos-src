#include "stdafx.h"
#include <memory.h>
#include <string.h>
#include <stdio.h>
#include <math.h>
#include "driver.h"

#pragma unmanaged
#include "../kernel/kernel.h"
#pragma managed

using namespace System::Runtime::InteropServices;

#define max(a,b)            (((a) > (b)) ? (a) : (b))

namespace Graphics {

	/*
#pragma unmanaged

	class _Graphics {
		ModeInfoBlock mib;
	public:
		int bytesPerPixel;
		int bytesPerLine;
		db* vram;
	public:
		_Graphics(const ModeInfoBlock& _mib) {
			mib = _mib;
			bytesPerPixel = (mib.BitsPerPixel+7)/8;
			bytesPerLine = bytesPerPixel*mib.XResolution;
		}
	public:
		db* getMemoryAt(int x, int y) {
			return ((db*)0xF00000) + y*bytesPerLine + x*bytesPerPixel;
		}
		db* getVideoMemoryAt(int x, int y) {
			return mib.PhysBasePtr + y*bytesPerLine + x*bytesPerPixel;
		}
	public:
		void Refresh() {
			memcpy(getVideoMemoryAt(0,0),getMemoryAt(0,0),bytesPerLine*mib.YResolution);
		}
		void FillRectangle(int x0, int y0, int x1, int y1, db r, db g, db b) {
			db* p = getMemoryAt(x0,y0);
			for(int y=y0; y<y1; ++y) {
				for(int x=x0; x<x1; ++x) {
					p[0] = b;
					p[1] = g;
					p[2] = r;
					p += bytesPerPixel;
				}
				p -= bytesPerPixel*(x1-x0);
				p += bytesPerLine;
			}
		}
		void FillPixel(int x, int y, db r, db g, db b) {
			db* p = getMemoryAt(x,y);
			p[0] = b;
			p[1] = g;
			p[2] = r;
		}
	};

	static void native(char n) {

		const VbeInfoBlock& vib = *reinterpret_cast<VbeInfoBlock*>(0x800);
		const ModeInfoBlock& mib = *reinterpret_cast<ModeInfoBlock*>(0x900);

		_Graphics g(mib);
		g.FillRectangle(0, 0, mib.XResolution, mib.YResolution, 0, 0, 0);

		int costbl[360*2];
		for(int r=0; r<360; ++r) {
			costbl[r] = (int)(0x10000*(1+cos(3.14159265358979*r/180))/2);
			costbl[r+360] = costbl[r];
		}

		int halfx = mib.XResolution/2;
		int halfy = mib.YResolution/2;
		for(int _t=0; true; ++_t) {
			if(_t==100) _t=0;
			double t = _t/20.0;
			int rmax = (int)(120*t);
			int gmax = (int)(120*t);
			int bmax = (int)(120*t);
			int rmin = rmax-255;
			int gmin = gmax-255;
			int bmin = bmax-255;
			if(rmin<0) rmin=0;
			if(gmin<0) gmin=0;
			if(bmin<0) bmin=0;
			if(rmax>255) rmax=255;
			if(gmax>255) gmax=255;
			if(bmax>255) bmax=255;
			int rd = rmax-rmin;
			int gd = gmax-gmin;
			int bd = bmax-bmin;
			for(int y=0; y<=mib.YResolution; ++y) {
				for(int x=0; x<mib.XResolution; ++x) {
					int dx = x-halfx;
					int dy = y-halfy;
					int be
						= costbl[(int)(+t*120+dx*3+(360*costbl[(int)(x*2-t*120)%360+360]>>16))%360+360]
						+ costbl[(int)(-t* 73+dy*2+(360*costbl[(int)(y*3-t* 73)%360+360]>>16))%360+360];
					//re *= rd;
					//ge *= gd;
					be >>= 1+8;
					be *= bd;
					int rv = rmin;//+(re>>16);
					int gv = gmin;//+(ge>>16);
					int bv = (be>>8)+bmin;
					if(rv>255) rv=255;
					if(gv>255) gv=255;
					if(bv>255) bv=255;
					g.FillPixel(x,y,rv,gv,bv);
				}
			}
			g.Refresh();
		}

	};

#pragma managed
	
	*/

}

#pragma optimize(disable)

int __stdcall f(int x, int y) {
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	System::Console::WriteLine(S"D");
	return x-y;
}

public __gc class T {
public:
	static void Main() {
		int n = System::Int32::Parse(System::Console::ReadLine());
		System::Console::WriteLine(__box(f(n, 5)));
	}
};
