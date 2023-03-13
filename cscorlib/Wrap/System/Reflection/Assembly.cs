using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Wrap._System.Reflection {

	public class _Assembly {

		internal Type InternalGetType(Module module, string name, bool throwOnError, bool ignoreCase) {
			if(module!=null) throw new NotSupportedException("Module is not null.");
			if(ignoreCase) throw new NotSupportedException("IgnoreCase options is not supported.");
			AssemblyImpl impl = (AssemblyImpl)(object)this;
			return impl.AssemblyInfo.Manager.ResolveType(name, throwOnError);
		}

		public static Assembly GetExecutingAssembly() {
			AssemblyBase assembly = (AssemblyBase)Engine.GetExecutingAssembly(0);
			if(assembly==null) return null;
			return assembly.RealAssembly;
		}

		public static Assembly GetCallingAssembly() {
			AssemblyBase assembly = (AssemblyBase)Engine.GetExecutingAssembly(1);
			if(assembly==null) return null;
			return assembly.RealAssembly;
		}
	
	}

}
