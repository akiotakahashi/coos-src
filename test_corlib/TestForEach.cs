using System;
using NUnit.Framework;

namespace TestCooS
{

	[TestFixture]
	public class TestForEach
	{

		[Test]
		public void TestSzArray()
		{
			string result = "";
			string[] list = new string[]{"1","2","3","4","5"};
			foreach(string e in list) {
				result += e;
			}
			Assert.AreEqual(12345, int.Parse(result));
		}

	}

}
