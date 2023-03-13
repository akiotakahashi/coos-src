using System;
using System.IO;
using NUnit.Framework;
using CooS.Formats.CLI.Metadata;
using CooS.Formats.CLI.Metadata.Rows;

namespace CooS.Formats.CLI.Signature {

	[TestFixture]
	public class MethodSigTest {

		[Test]
		public void Verify() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(MethodDefRow row in md.Tables.MethodDef) {
				MethodSig sig = new MethodSig(new SignatureReader(md.Blob[row.Signature]));
				Assert.AreEqual(sig.Generic, sig.GenericParameterCount>0);
				Assert.AreEqual(sig.ParameterCount, sig.Params.Length);
			}
		}

	}

}
