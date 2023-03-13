using System;
using CooS.Architectures.IA32;
using System.Runtime.CompilerServices;

namespace System {

	public class Kernel {

		public static void Panic(string msg) {
			Console.WriteLine("*** PANIC ***");
			Console.WriteLine(msg);
			while(true) Instruction.hlt();
		}

		public static void Delay(int miliseconds, int microseconds, int nanoseconds) {
			microseconds += miliseconds*1000;
			nanoseconds += microseconds*1000;
			nanoseconds /= 1000;
			for(int i=0; i<=nanoseconds; ++i) {
				//NOP
			}
		}

		public static void Halt() {
			Instruction.hlt();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern unsafe object ValueToObject(void* value);
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object ValueToObject(IntPtr value);
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ObjectToValue(object value);

	}

}
