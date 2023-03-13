using System;
using System.Reflection;
using CooS.CodeModels.CLI;

namespace CooS.Wrap._System {

	class _Engine {

		private static IntPtr GetMethodProxyCode(MethodInfo _mi) {
			MethodDefInfo mi = (MethodDefInfo)_mi;
			string fn = mi.ReflectedType.Assembly.FullName;
			return Engine.GetMethodProxyCode(fn.Split(',')[0], mi.RowIndex);
		}

	}

}
