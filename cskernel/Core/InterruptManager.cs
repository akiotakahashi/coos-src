using System;
using CooS.Architectures;
using System.Runtime.InteropServices;

namespace CooS.Core {

	public static class InterruptManager {

		public static void Register(int intno, InterruptHandler handler) {
			if(intno<0 || 255<intno) throw new ArgumentOutOfRangeException();
			CooS.Architectures.IA32.InterruptManager.SetGate(0xE00, (byte)intno, handler);
		}

	}

}
