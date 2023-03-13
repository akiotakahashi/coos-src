using System;
using CooS.Reflection;
using CooS.Execution;
using CooS.Toolchains;

namespace CooS.Compile {

	class Program {

		public static readonly string MscorlibPath = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll";
		public static readonly string RootPath = @"D:\Repository\clios\";
		public static readonly string Format		= RootPath+@"csformat\bin\Debug\csformat.dll";
		public static readonly string Reflection	= RootPath+@"csreflection\bin\Debug\csreflection.dll";
		public static readonly string Execution		= RootPath+@"csexecution\bin\Debug\csexecution.dll";
		public static readonly string Korlib		= RootPath+@"cskorlib\Release\cskorlib.dll";
		public static readonly string Kernel		= RootPath+@"cskernel\bin\Debug\cskernel.exe";
		public static readonly string Utility		= RootPath+@"csutility\bin\Debug\csutility.dll";
		public static readonly string Test1Path		= RootPath+@"test1\bin\Debug\test1.dll";
		public static readonly string Test2Path		= RootPath+@"test2\bin\Debug\test2.exe";

		static void Dump(TypeBase type) {
			Console.WriteLine(">>> {0} : {1} in {2}", type, type.BaseType, type.EnclosingType);
			Console.WriteLine(type.ContainsGenericParameters);
			Console.WriteLine(type.IsClosedGeneric);
			foreach(FieldBase field in type.EnumFields()) {
				Console.WriteLine("f: {0} {1}", field.ReturnType.FullName, field.Name);
			}
			foreach(MethodBase method in type.EnumMethods()) {
				Console.Write("m: {0} {1} << ", method.ReturnType.FullName, method.Name);
				foreach(ParamBase p in method.EnumParameterInfo()) {
					Console.Write(", {0} {1}", method.GetParameterType(p.Position).FullName, p.Name);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		static void Main(string[] args) {
			Console.WriteLine("Making a world...");
			World world = new CooS.Execution.CLI.WorldImpl(6);
			Domain domain = new Domain();
			world.LoadAssembly("CLI", MscorlibPath);
			world.LoadAssembly("CLI", Format);
			world.LoadAssembly("CLI", Reflection);
			world.LoadAssembly("CLI", Execution);
			world.LoadAssembly("CLI", Korlib);
			world.LoadAssembly("CLI", Utility);
			AssemblyBase assembly = world.LoadAssembly("CLI", Kernel);
			if(true) {
				TypeBase c = world.ResolveType("C`1","CooS");
				Dump(c);
				TypeBase d = c.FindNestedType("D`1");
				Dump(d);
				c = c.Specialize(new TypeBase[] { world.Int32 });
				Dump(c);
				d = c.FindNestedType("D`1").Specialize(new TypeBase[] { world.Int32, world.Int64 });
				Dump(d);
				//c = c.Instantiate(c);
				//Dump(c);
				//d = d.Instantiate(d);
				//Dump(d);
			}
			Console.WriteLine("Counting methods...");
			int count = 0;
			foreach(TypeBase type in assembly.EnumTypes()) {
				foreach(MethodBase method in type.EnumMethods()) {
					++count;
				}
			}
			Engine engine = new Engine(world);
			Compiler compiler = new CLI.CompilerImpl(engine, domain);
			int index = 0;
			Console.Write(">");
			int skip = int.Parse(Console.ReadLine());
			foreach(TypeBase type in assembly.EnumTypes()) {
				foreach(MethodBase method in type.EnumMethods()) {
					Console.WriteLine("[{0,"+count.ToString().Length+"}/{1}] {2}", ++index, count, method.FullName);
					if(index<skip) { continue; }
					CodeInfo code;
					if(method.IsBlank) {
						TypeBase t = world.ResolveType("_"+type.Name, "CooS.Wrap._"+type.Namespace);
						MethodBase m = method.FindWrappingMethod();
						if(m!=null) {
							Console.WriteLine("# redirected to {0}", m.FullName);
							code = compiler.Compile(m);
						} else {
							Console.WriteLine("# blank");
							code = null;
						}
					} else if(method.ContainsGenericParameters) {
						Console.WriteLine("# generic");
						code = null;
					} else {
						try {
							code = compiler.Compile(method);
						} catch(AssemblyNotFoundException ex) {
							Console.WriteLine("FAILED TO RESOLVE EXTERNAL ASSEMBLY: {0}", ex.Message);
							code = null;
						}
					}
					Console.WriteLine(code);
				}
			}
		}

	}

}
