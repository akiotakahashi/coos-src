using System;
using System.IO;
using System.Reflection;
using CooS;
using CooS.Reflection;

namespace CooS.Precompile {
	using IA32Toolkit.Assembler;

	class Precompiler {
	
		static void Main(string[] args) {
			if(args.Length!=3) {
				if(args.Length==0) {
					try {
						Engage("cscorlib.dll", "System.Engine:GenerateNativeCode");
					} catch(Exception ex) {
						Console.Error.WriteLine(ex);
					}
				} else {
					Console.WriteLine("precompile <assembly file> <class name>:<method name>");
				}
			} else if(!File.Exists(args[0])) {
				Console.WriteLine("File not found");
			} else {
				try {
					Engage(args[0], args[1]);
				} catch(Exception ex) {
					Console.WriteLine(ex);
				}
			}
		}

		static void Engage(string filename, string methodname) {
			AssemblyManager manager = new AssemblyManager();
			manager.LoadAssembly(@"D:\Repository\clios\cdimage\mscorlib.dll");
			AssemblyBase assembly = manager.LoadAssembly(filename);
			manager.Setup();
			Console.WriteLine("--- Dump all types and methods");
			foreach(TypeImpl type in assembly.GetTypes(false)) {
				Console.Write(type.FullName);
				if(type.BaseType!=null) {
					Console.Write(" : "+type.BaseType.FullName);
				}
				Console.WriteLine();
				/*
				foreach(MethodInfo method in type.GetMethodsImpl()) {
					Console.Write(type.FullName+":"+method.Name+"(");
					foreach(ParameterInfo param in method.GetParameters()) {
						Console.Write("{0} {1}, ", param.ParameterType.FullName, param.Name);
					}
					Console.WriteLine(")");
				}
				*/
			}
			Console.WriteLine("--- Generate native code");
			Console.WriteLine(methodname);
			//Engine.GenerateNativeCode(methodname);
		}

	}

}
