using System;
using System.Collections.Generic;
using CooS.Formats.CLI;

namespace CooS.Reflection {

	class Program {
	
		static void Main(string[] args) {
			DomainTest test = new DomainTest();
			test.SetUp();
			test.ResolveParameter();
			DomainImpl domain = new DomainImpl();
			AssemblyBase assembly = domain.LoadAssembly("CLI", Test.MscorlibPath);
			foreach(TypeBase type in assembly.EnumType()) {
				foreach(FieldBase field in type.EnumField()) {
					Console.WriteLine(field.FullName);
				}
				foreach(MethodBase method in type.EnumMethod()) {
					Console.WriteLine(method.FullName);
				}
			}
		}

	}

}
