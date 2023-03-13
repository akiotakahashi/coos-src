using System;
using System.IO;
using CooS.Wrap._System.IO;
using NUnit.Framework;

namespace CooS.Wrap._System.IO {

	[TestFixture]
	public class TestPath {

		private void CheckIsPathRooted(string path) {
			Assert.AreEqual(Path.IsPathRooted(path), _Path.IsPathRooted(path));
		}

		[Test]
		public void IsPathRooted_IsCompatible() {
			CheckIsPathRooted(@"\usr\bin");
			CheckIsPathRooted(@"/usr/bin");
			CheckIsPathRooted(@"usr\bin");
			CheckIsPathRooted(@"usr/bin");
			CheckIsPathRooted(@"");
		}

		[Test]
		public void IsPathRooted() {
			Assert.IsTrue(Path.IsPathRooted(@"c:"));
			Assert.IsTrue(Path.IsPathRooted(@"c:\"));
			Assert.IsTrue(_Path.IsPathRooted(@"cd0a:"));
			Assert.IsTrue(_Path.IsPathRooted(@"cd0a:/"));
		}

		[Test]
		public void GetDirectoryName_IsCorrect() {
			Assert.AreEqual(@"\usr", _Path.GetDirectoryName(@"\usr\bin"));
			Assert.AreEqual(@"/usr", _Path.GetDirectoryName(@"/usr/bin"));
			Assert.AreEqual(@"usr", _Path.GetDirectoryName(@"usr\bin"));
			Assert.AreEqual(@"usr", _Path.GetDirectoryName(@"usr/bin"));
			Assert.AreEqual(@"usr/bin", _Path.GetDirectoryName(@"usr/bin/"));
			Assert.AreEqual(@"usr\bin", _Path.GetDirectoryName(@"usr\bin\"));
		}

		private void CheckGetDirectoryName(string path) {
			Assert.AreEqual(Path.GetDirectoryName(path), _Path.GetDirectoryName(path));
		}

		[Test]
		public void GetDirectoryName_IsCompatible() {
			CheckGetDirectoryName(@"usr");
			CheckGetDirectoryName(@"usr");
			CheckGetDirectoryName(@"usr/");
			CheckGetDirectoryName(@"usr\");
			CheckGetDirectoryName(@"usr/bin");
			CheckGetDirectoryName(@"usr\bin");
		}

		[ExpectedException(typeof(ArgumentException))]
		public void GetDirectoryName_HasException() {
			_Path.GetDirectoryName(@"");
		}
	
	}

}
