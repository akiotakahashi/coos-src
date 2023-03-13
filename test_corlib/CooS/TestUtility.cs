using System;
using System.Reflection;
using CooS.Reflection;
using CooS.Management;

namespace CooS {
	
	public class TestUtility {

		static bool prepared = false;

		static TestUtility() {
			Prepare();
		}

		public static void Prepare() {
			if(!prepared && !Engine.Infrastructured) {
				AssemblyResolver.Manager = CreateManager();
				prepared = true;
			}
		}

		public static AssemblyManager CreateManager() {
			AssemblyManager manager = new AssemblyManager();
			manager.LoadAssembly(@"D:\Repository\clios\cdimage\mscorlib.dll");
			manager.LoadAssembly(@"cskorlib.dll");
			manager.LoadAssembly(@"cscorlib.dll");
			manager.LoadAssembly(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
			manager.Setup();
			return manager;
		}

	}

}
