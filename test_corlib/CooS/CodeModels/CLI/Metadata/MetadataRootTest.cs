using System;
using System.IO;
using System.Reflection;
using CooS.CodeModels.CLI.Metadata;
using NUnit.Framework;

namespace CooS.CodeModels.CLI.Metadata {

	class SampleClass {

		public int f1 = 0;

		public void m1(int x) {
		}

	}

	[TestFixture]
	public class MetadataRootTest {

		readonly byte[] buf;
		readonly MetadataRoot root;

		public MetadataRootTest() {
			Console.WriteLine(Assembly.GetExecutingAssembly().CodeBase);
			using(Stream stream = new FileStream(Assembly.GetExecutingAssembly().CodeBase.Substring(8), FileMode.Open, FileAccess.Read)) {
				buf = new byte[stream.Length];
				stream.Read(buf, 0, (int)stream.Length);
			}
			root = Engine.AnalyzePEImage(buf);
		}

		[Test]
		public void ReadTypeDef() {
			Table table = root.Tables[TableId.TypeDef];
			for(int i=1; i<=table.RowCount; ++i) {
				TypeDefRow row = (TypeDefRow)table[i];
				if(root.Strings[row.Name]=="SampleClass" && root.Strings[row.Namespace]=="CooS.CodeModels.CLI.Metadata") {
					return;
				}
			}
			Assert.Fail("SampleClass not found");
		}

	}

}
