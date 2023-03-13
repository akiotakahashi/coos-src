using System;
using CooS.Reflection;
using System.Reflection;

namespace CooS.Wrap._System.Reflection {

	public class _FieldInfo {
	
		private static FieldInfo internal_from_handle(IntPtr handle) {
			return FieldInfoImpl.FindFieldFromHandle(handle);
		}
	
	}

}
