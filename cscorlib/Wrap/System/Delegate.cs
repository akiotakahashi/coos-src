using System;
using System.Reflection;
using CooS.Execution;
using CooS.Reflection;

namespace CooS.Wrap._System {

	class _Delegate {

		public static Delegate CreateDelegate_internal(Type type, object target, MethodInfo info) {
			//IntPtr fp = CodeManager.GetCallableCode((MethodInfoImpl)info).EntryPoint;
			IntPtr fp = ((MethodInfoImpl)info).GenerateCallableCode(CodeModels.CodeLevel.IL).EntryPoint;
			Delegate d = (Delegate)Memory.Allocate((TypeImpl)type);
			Assist.ConstructDelegate(d, target, fp);
			return d;
		}

	}

}
