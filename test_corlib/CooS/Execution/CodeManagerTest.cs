using System;
using NUnit.Framework;

namespace CooS.Execution {

	[TestFixture]
	public class CodeManagerTest {

		CodeManager manager = new CodeManager();

		public CodeManagerTest() {
			TestUtility.Prepare();
		}

		[SetUp]
		public void Setup() {
			manager = new CodeManager();
		}

		[TearDown]
		public void TearDown() {
		}

		[Test]
		public void PrepareOrdinalMethod() {
			manager.Prepare("CooS.CodeModels.Assembler:AllocateObjectImpl");
		}

		[Test]
		public void PrepareSpecialNamedMethod() {
			manager.Prepare("System.Reflection.MethodBase:get_IsSpecialName");
		}

		[Test]
		public void PrepareConstructor() {
			manager.Prepare("CooS.CodeModels.DLL.RVA:.ctor");
		}

	}

}
