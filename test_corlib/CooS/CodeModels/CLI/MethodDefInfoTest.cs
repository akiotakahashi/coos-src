using System;
using CooS.Reflection;
using NUnit.Framework;

namespace CooS.CodeModels.CLI {

	[TestFixture]
	public class MethodDefInfoTest {

		readonly AssemblyManager manager;
		readonly AssemblyBase assembly;

		public MethodDefInfoTest() {
			this.manager = TestUtility.CreateManager();
			this.assembly = manager.ResolveAssembly("test_corlib", true);
		}

		[Test]
		public void GetBaseDifinition() {
			Assert.AreSame(this.manager.ResolveMethod("System.Reflection.MemberInfo:get_DeclaringType",true)
				, this.manager.ResolveMethod("System.Type:get_DeclaringType",true).GetBaseDefinition());
		}

	}

}
