using System;
using NUnit.Framework;
using CooS.Formats.CLI.Metadata;
using CooS.Formats.CLI.Metadata.Rows;

namespace CooS.Formats.CLI.Signature {

	[TestFixture]
	public class MethodDefRowTest {

		[Test]
		public void Name() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(MethodDefRow row in md.Tables.MethodDef) {
				Assert.Greater(md[row.Name].Length, 0);
			}
		}

		[Test]
		public void ParamList() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(MethodDefRow row in md.Tables.MethodDef) {
				Assert.Greater(row.ParamList.Value, 0);
				Assert.LessOrEqual(row.ParamList.Value, md.Tables.Param.RowCount+1);
			}
		}

		[Test]
		public void Signature() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(MethodDefRow row in md.Tables.MethodDef) {
				Assert.Less(row.Signature.RawIndex, md.Blob.HeapSize);
			}
		}

	}

}
