using System;
using NUnit.Framework;

namespace CooS.Formats.DLL {

	[TestFixture]
	public class PEImageTest {

		public static readonly string MscorlibPath	= @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll";
		public static readonly string RootPath		= @"D:\Repository\clios\";
		public static readonly string Format		= RootPath+@"csformat\bin\Debug\csformat.dll";
		public static readonly string Reflection	= RootPath+@"csreflection\bin\Debug\csreflection.dll";
		public static readonly string Execution		= RootPath+@"csexecution\bin\Debug\csexecution.dll";
		public static readonly string Korlib		= RootPath+@"cskorlib\Release\cskorlib.dll";
		public static readonly string Kernel		= RootPath+@"cskernel\bin\Debug\cskernel.exe";
		public static readonly string Utility		= RootPath+@"csutility\bin\Debug\csutility.dll";
		public static readonly string Test1Path		= RootPath+@"test1\bin\Debug\test1.dll";
		public static readonly string Test2Path		= RootPath+@"test2\bin\Debug\test2.exe";

		private PEImage Load(string filename) {
			return new PEImage(filename);
		}

		[Test]
		public void Load() {
			Load(MscorlibPath);
		}

		[Test]
		public void SectionCount() {
			using(PEImage image = Load(MscorlibPath)) {
				Assert.AreEqual(image.CoffHeader.NumberOfSections, image.Sections.Count);
			}
		}

		[Test]
		public void RVAToSection() {
			using(PEImage image = Load(MscorlibPath)) {
				foreach(Section sec in image.Sections.Values) {
					Assert.AreSame(sec, image.RVAToSection(sec.VirtualAddress));
				}
			}
		}

		[Test]
		public void RVAToVA() {
			using(PEImage image = Load(MscorlibPath)) {
				foreach(Section sec in image.Sections.Values) {
					Assert.AreEqual(sec.PointerToRawData, image.RVAToLocation(sec.VirtualAddress));
				}
			}
		}

	}

}
