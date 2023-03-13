using System;
using System.Runtime.InteropServices;

namespace CooS.Toolchains.IA32 {

	[StructLayout(LayoutKind.Sequential)]
	public struct RegisterSet {
		int edi;
		int esi;
		int ebp;
		int esp;
		int ebx;
		int edx;
		int ecx;
		int eax;
	}

}
