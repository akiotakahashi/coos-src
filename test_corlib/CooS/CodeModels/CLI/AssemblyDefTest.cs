using System;
using System.IO;
using System.Reflection;
using CooS.Reflection;
using NUnit.Framework;

namespace CooS.CodeModels.CLI {

	class SampleClass {

		public int f1 = 1;

		public void m1(int x) {
		}

	}

	[TestFixture]
	public class AssemblyDefTest {

		readonly AssemblyManager manager;
		readonly AssemblyBase assembly;

		public AssemblyDefTest() {
			this.manager = TestUtility.CreateManager();
			this.assembly = manager.ResolveAssembly("test_corlib", true);
		}

		[Test]
		public void TestEnumAllType() {
			foreach(Type type in assembly.GetTypes(false)) {
				if(type.FullName=="CooS.CodeModels.CLI.SampleClass") {
					return;
				}
			}
			Assert.Fail("SampleClass not found");
		}

		[Test]
		public void TestEnumExportedTypeOnly() {
			foreach(Type type in assembly.GetTypes(true)) {
				Assert.IsFalse(type.FullName=="TestCooS.CooS.CodeModels.CLI.SampleClass");
			}
		}

		[Test]
		public void TestEnumMethod() {
		}

	}

}
