using System;
using CooS.FileSystem;
using NUnit.Framework;

namespace TestCooS.CooS.FileSystem {

	[TestFixture]
	public class TestPathInfo {

		PathInfo pi1 = new PathInfo(@"cd0a@host:/usr/bin");
		PathInfo pi2 = new PathInfo(@"c:\usr\bin");
		PathInfo pi3 = new PathInfo(@"/usr/bin");

		[Test]
		public void TestDrive() {
			Assert.AreEqual("cd0a", pi1.Drive);
			Assert.AreEqual("c", pi2.Drive);
			Assert.AreEqual(null, pi3.Drive);
		}

		[Test]
		public void TestHost() {
			Assert.AreEqual("host", pi1.Host);
			Assert.AreEqual(null, pi2.Host);
			Assert.AreEqual(null, pi3.Host);
		}

		[Test]
		public void TestLocation() {
			Assert.AreEqual(@"/usr/bin", pi1.Location);
			Assert.AreEqual(@"\usr\bin", pi2.Location);
			Assert.AreEqual(@"/usr/bin", pi3.Location);
		}

	}

}
