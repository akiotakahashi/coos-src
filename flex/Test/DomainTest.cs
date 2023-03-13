using System;
using NUnit.Framework;

namespace CooS.Reflection {

	[TestFixture]
	public class DomainTest {

		DomainImpl domain = new DomainImpl();

		[SetUp]
		public void SetUp() {
			domain.LoadAssembly("CLI", Test.MscorlibPath);
			domain.LoadAssembly("CLI", Test.Test1Path);
			domain.LoadAssembly("CLI", Test.Test2Path);
		}

		[Test]
		public void LoadAssembly() {
		}

		[Test]
		public void ResolveAssembly() {
			Assert.IsNotNull(domain.ResolveAssembly("mscorlib"));
			Assert.IsNotNull(domain.ResolveAssembly("test1"));
			Assert.IsNotNull(domain.ResolveAssembly("test2"));
			Assert.IsNull(domain.ResolveAssembly("Test2"));
		}

		[Test]
		public void ResolveType() {
			AssemblyBase mscorlib = domain.ResolveAssembly("mscorlib");
			AssemblyBase test1 = domain.ResolveAssembly("test1");
			Assert.IsNotNull(mscorlib.FindType("Int32", "System"));
			Assert.IsNotNull(mscorlib.FindType("Double", "System"));
			Assert.IsNotNull(test1.FindType("Class1`1", "test1"));
		}

		[Test]
		public void ResolveField() {
			foreach(TypeBase type in domain.ResolveAssembly("mscorlib").EnumType()) {
				foreach(FieldBase field in type.EnumField()) {
					Assert.AreSame(type, field.Type);
				}
			}
		}

		[Test]
		public void ResolveMethod() {
			foreach(TypeBase type in domain.ResolveAssembly("mscorlib").EnumType()) {
				foreach(MethodBase method in type.EnumMethod()) {
					Assert.AreSame(type, method.Type);
				}
			}
		}

		[Test]
		public void ResolveParameter() {
			foreach(TypeBase type in domain.ResolveAssembly("mscorlib").EnumType()) {
				foreach(MethodBase method in type.EnumMethod()) {
					foreach(ParamBase param in method.EnumParam()) {
						Assert.AreSame(method, param.Method);
					}
				}
			}
		}
	
	}

}
