using System;
using System.Collections;
using System.Reflection;
using CooS.Collections;
using CooS.Reflection;

namespace CooS.Management {
	using Enumerable = Inttable.HashValues;

	public class AssemblyResolver {

		static readonly Version EmptyVersion = new Version(0,0,0,0);
		static AssemblyManager manager = new AssemblyManager();
		static bool associate = false;

		private AssemblyResolver() {
		}

		public static AssemblyManager Manager {
			get {
				return manager;
			}
			set {
				manager = value;
			}
		}

		internal static void AssociateAssemblies() {
			foreach(AssemblyBase assembly in manager.EnumAssemblies()) {
				assembly.RealAssembly = new AssemblyImpl(assembly);
			}
			associate = true;
		}

		public static AssemblyBase RegisterAssembly(AssemblyBase assembly) {
			if(associate) {
				assembly.RealAssembly = new AssemblyImpl(assembly);
			}
			manager.LoadAssembly(assembly);
			return assembly;
		}

		public static AssemblyBase GetAssemblyInfo(int aid) {
			if(manager==null) throw new UnexpectedException("Manager is absent!");
			return manager.GetAssembly(aid, true);
		}

		public static AssemblyBase FindAssemblyInfo(string asmname) {
			return manager.ResolveAssembly(asmname, false);
		}

		public static Enumerable EnumAssemblies() {
			return manager.EnumAssemblies();
		}

		public static AssemblyImpl ResolveAssembly(string name, bool throwOnMiss) {
			AssemblyName asmname = new AssemblyName();
			asmname.Name = name;
			asmname.Version = EmptyVersion;
			return manager.ResolveAssembly(asmname, throwOnMiss).RealAssembly;
		}
		
		public static AssemblyImpl ResolveAssembly(AssemblyName asmname, bool throwOnMiss) {
			return manager.ResolveAssembly(asmname, throwOnMiss).RealAssembly;
		}

	}

}
