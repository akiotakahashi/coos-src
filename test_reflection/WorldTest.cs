using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace CooS.Reflection.Test {

	[TestFixture]
	public class WorldTest {

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

		public WorldTest() {
			WorldImpl.RegisterTypeSystem("CLI", new CooS.Reflection.CLI.LoaderImpl());
		}

		public World LoadAssembly() {
			World world = new WorldImpl(6);
			world.LoadAssembly("CLI", MscorlibPath);
			world.LoadAssembly("CLI", Format);
			world.LoadAssembly("CLI", Reflection);
			world.LoadAssembly("CLI", Execution);
			world.LoadAssembly("CLI", Korlib);
			world.LoadAssembly("CLI", Utility);
			world.LoadAssembly("CLI", System.Reflection.Assembly.GetExecutingAssembly().Location);
			return world;
		}

		[Test]
		public void LoadAssemblyTest() {
			LoadAssembly();
		}

	}

}
