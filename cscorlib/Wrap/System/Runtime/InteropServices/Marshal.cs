using System;
using CooS.Reflection;

namespace CooS.Wrap._System.Runtime.InteropServices {

	public class _Marshal {

		public static int SizeOf(Type t) {
			return ((TypeImpl)t).InstanceSize;
		}

	}

}
