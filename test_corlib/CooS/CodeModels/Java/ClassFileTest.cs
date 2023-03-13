using System;
using System.IO;
using NUnit.Framework;

namespace CooS.Formats.Java {

	[TestFixture]
	public class ClassFileTest {

		readonly string path = @"D:\Repository\clios\test_corlib\res\teppan.class";
		ClassFile clsfile;

		[SetUp]
		public void Setup() {
			using(Stream stream = new FileStream(path,FileMode.Open)) {
				clsfile = new ClassFile(stream);
			}
		}

		[Test]
		public void Dump() {
			this.clsfile.Dump();
		}

	}

}
