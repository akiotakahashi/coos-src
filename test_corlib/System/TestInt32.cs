using System;
using NUnit.Framework;

namespace TestCooS.System {

	[TestFixture]
	public class TestInt32 {

		[Test]
		public void TestParse() {
			Assert.AreEqual(12345, int.Parse("12345"));
		}

	}

}
