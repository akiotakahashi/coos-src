using System;
using System.Collections;
using System.Runtime.InteropServices;
using CooS.Reflection;

namespace CooS.Wrap._System.Runtime.InteropServices {

	public class _GCHandle {

		//static Hashtable table = new Hashtable();

		private static int GetTargetHandle(object obj, int handle, GCHandleType type) {
			if(handle!=0) throw new NotImplementedException("Resetting handle not supported");
			handle = Kernel.ObjectToValue(obj).ToInt32();
			return handle;
		}

		private static IntPtr GetAddrOfPinnedObject(int handle) {
			object obj = GetTarget(handle);
			TypeImpl type = (TypeImpl)obj.GetType();
			return new IntPtr(handle+type.OffsetToContents);
		}
 
		private static void FreeHandle(int handle) {
		}

		private static bool CheckCurrentDomain(int handle) {
			return true;
		}

		private static object GetTarget(int handle) {
			return Kernel.ValueToObject(new IntPtr(handle));
		}

	}

}
