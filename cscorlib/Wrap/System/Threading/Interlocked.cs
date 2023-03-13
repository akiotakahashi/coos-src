using System;

namespace CooS.Wrap._System.Threading {

	public class _Interlocked {
	
		public static int Increment(ref int location) {
			return ++location;
		}

	}

}
