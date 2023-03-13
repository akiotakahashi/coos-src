using System;

namespace CooS.Wrap._System {

	public class _Object {

		public _Object() {
		}

		internal static int InternalGetHashCode(object obj) {
			return Kernel.ObjectToValue(obj).GetHashCode();
		}

		public new unsafe Type GetType() {
			IntPtr p = Kernel.ObjectToValue(this);
			IntPtr* pp = (IntPtr*)p.ToPointer();
			return (Type)Kernel.ValueToObject(*(pp-1));
		}

	}

}
