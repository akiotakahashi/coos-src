using System;
using NUnit.Framework;
using CooS.Formats.CLI.Metadata;
using CooS.Formats.CLI.Metadata.Rows;

namespace CooS.Formats.CLI.Signature {

	[TestFixture]
	public class FieldDefRowTest {

		[Test]
		public void Name() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(FieldRow row in md.Tables.FieldDef) {
				Assert.Greater(md[row.Name].Length, 0);
			}
		}

		[Test]
		public void Signature() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(FieldRow row in md.Tables.FieldDef) {
				Assert.Less(row.Signature.RawIndex, md.Blob.HeapSize);
			}
		}

	}

}
