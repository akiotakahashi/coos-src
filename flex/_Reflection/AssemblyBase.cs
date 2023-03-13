using System;
using CooS.Reflection;
using System.Reflection;

namespace CooS.Reflection {

	public abstract class AssemblyBase {

		//private AssemblyManager manager;
		//private AssemblyImpl assembly;
		private int id = 0;

		public int Id {
			get {
				return this.id;
			}
			set {
				if(this.id!=0) throw new InvalidOperationException("ID is already set");
				this.id = value;
			}
		}

		public abstract MethodInfo EntryPoint {get;}
		public abstract AssemblyName GetName(bool copiedName);

		/*
		public abstract int TypeDefCount {get;}
		public abstract int TypeRefCount {get;}
		public abstract Type GetTypeRef(int rowIndex);
		*/
		public abstract string GetTypeName(int rowIndex);
		public abstract TypeImpl GetTypeInfo(int rowIndex);
		public abstract FieldInfoImpl GetFieldInfo(int rowIndex);
		public abstract MethodInfoImpl GetMethodInfo(int rowIndex);
		//public abstract MethodInfoImpl GetMethodRef(int rowIndex);
		public abstract string LoadString(int rowIndex);
		public abstract TypeImpl FindType(string fullname, bool throwOnMiss);

		// THESE TWO METHOD ARE CALLED VIA SUBSTITUTE BROKER.
		public abstract Type[] GetTypes(bool exportedOnly);
		//public abstract Type InternalGetType(Module module, string name, bool throwOnError, bool ignoreCase);

		public static explicit operator AssemblyImpl(AssemblyBase assembly) {
			return assembly.RealAssembly;
		}

		public static explicit operator AssemblyBase(Assembly assembly) {
			AssemblyImpl impl = (AssemblyImpl)assembly;
			return impl.AssemblyInfo;
		}

	}

}
