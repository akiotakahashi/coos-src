using System;
using NUnit.Framework;

namespace CooS.Formats.CLI.Metadata {

	[TestFixture]
	public class MetadataRootTest {

		//public static readonly string MscorlibPath	= @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll";
		public static readonly string MscorlibPath	= @"D:\Repository\clios\cdimage\mscorlib.dll";
		public static readonly string RootPath		= @"D:\Repository\clios\";
		public static readonly string Format		= RootPath+@"csformat\bin\Debug\csformat.dll";
		public static readonly string Reflection	= RootPath+@"csreflection\bin\Debug\csreflection.dll";
		public static readonly string Execution		= RootPath+@"csexecution\bin\Debug\csexecution.dll";
		public static readonly string Korlib		= RootPath+@"cskorlib\Release\cskorlib.dll";
		public static readonly string Kernel		= RootPath+@"cskernel\bin\Debug\cskernel.exe";
		public static readonly string Utility		= RootPath+@"csutility\bin\Debug\csutility.dll";
		public static readonly string Test1Path		= RootPath+@"test1\bin\Debug\test1.dll";
		public static readonly string Test2Path		= RootPath+@"test2\bin\Debug\test2.exe";

		public static MetadataRoot LoadMetadata() {
			return MetadataRoot.LoadMetadataFromFile(MscorlibPath);
		}

		[Test]
		public void LoadMetadataFromFile() {
			LoadMetadata();
		}

		[Test]
		public void EnumTables() {
			MetadataRoot md = LoadMetadata();
			foreach(ITable table in md.Tables) {
				Assert.AreSame(table, md.Tables[table.TableId]);
			}
		}

		[Test]
		public void CountOfRows() {
			MetadataRoot md = LoadMetadata();
			foreach(ITable table in md.Tables) {
				int count = 0;
				foreach(object row in table) {
					++count;
				}
				Assert.AreEqual(table.RowCount, count);
			}
		}

		[Test]
		public void TypeDefName() {
			MetadataRoot md = LoadMetadata();
			foreach(Rows.TypeDefRow row in md.Tables.TypeDef) {
				Assert.Greater(md.Strings[row.TypeName].Length, 0);
			}
		}

	}

}
