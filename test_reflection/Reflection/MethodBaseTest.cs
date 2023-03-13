using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using CooS.Formats;

namespace CooS.Reflection.Test {

	[TestFixture]
	public class MethodBaseTest {

		private World world;

		public class Test<T> {
			public T methodReturnT() { return default(T); }
			public T[] methodReturnArrayOfT() { return null; }
			public void methodUseS<S>(S value) { }
			public void methodUseArrayOfS<S>(S[] value) { }
		}

		[SetUp]
		public void Setup() {
			WorldTest test = new WorldTest();
			this.world = test.LoadAssembly();
		}

		[Test]
		public void Type() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				foreach(MethodBase method in type.EnumMethods()) {
					Assert.AreSame(type, method.Type);
				}
			}
		}

		[Test]
		public void CheckArguments() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				foreach(MethodBase method in type.EnumMethods()) {
					if(method.IsStatic) {
						Assert.AreEqual(method.ParameterCount, method.ArgumentCount);
						for(int i=0; i<method.ParameterCount; ++i) {
							Assert.AreSame(method.GetParameterType(i), method.GetArgumentType(i));
						}
					} else {
						Assert.AreEqual(method.ParameterCount+1, method.ArgumentCount);
						for(int i=0; i<method.ParameterCount; ++i) {
							Assert.AreSame(method.GetParameterType(i), method.GetArgumentType(i+1));
						}
					}
				}
			}
		}

		[Test]
		public void ContainsGenericParameters() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				foreach(MethodBase method in type.EnumMethods()) {
					if(method.ContainsGenericParameters) {
						Assert.IsFalse(method.IsClosedGeneric);
					}
					if(method.IsGenericMethod) {
						Assert.Greater(method.GenericParameterCount, 0);
						Assert.IsTrue(method.ContainsGenericParameters);
						for(int i=0; i<method.GenericParameterCount; ++i) {
							TypeBase arg = method.GetGenericArgumentType(i);
							Assert.IsTrue(arg.IsGenericParam);
							Assert.AreEqual(i, arg.GenericParamPosition);
							Assert.AreEqual(GenericSources.Method, arg.GenericSource);
						}
					}
				}
			}
		}

	}

}
