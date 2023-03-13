using System;
using NUnit.Framework;
using CooS.Formats.CLI.Metadata;
using CooS.Formats.CLI.Metadata.Rows;

namespace CooS.Formats.CLI.Signature {

	[TestFixture]
	public class TestDefRowTest {

		[Test]
		public void TypeName() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(TypeDefRow row in md.Tables.TypeDef) {
				Assert.Greater(md[row.TypeName].Length, 0);
			}
		}

		[Test]
		public void TypeNamespace() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(TypeDefRow row in md.Tables.TypeDef) {
				Assert.GreaterOrEqual(md[row.TypeNamespace].Length, 0);
			}
		}

		[Test]
		public void FieldList() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(TypeDefRow row in md.Tables.TypeDef) {
				Assert.Greater(row.MethodList.Value, 0);
				Assert.LessOrEqual(row.FieldList.Value, md.Tables.FieldDef.RowCount+1);
			}
		}

		[Test]
		public void MethodList() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = MetadataRootTest.LoadMetadata();
			foreach(TypeDefRow row in md.Tables.TypeDef) {
				Assert.Greater(row.MethodList.Value, 0);
				Assert.LessOrEqual(row.MethodList.Value, md.Tables.MethodDef.RowCount+1);
			}
		}

	}

}
