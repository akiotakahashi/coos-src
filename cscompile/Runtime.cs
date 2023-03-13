using System;
using System.Runtime.CompilerServices;

namespace CooS {

	public static class Runtime {
	
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern unsafe object ValueToObject(void* value);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object ValueToObject(IntPtr value);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ObjectToValue(object value);

	}

}
