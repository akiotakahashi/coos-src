using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using CooS.Formats;

namespace CooS.Reflection.Test {

	[TestFixture]
	public class TypeBaseTest {

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
		public void ResolveTest() {
			Assert.IsNotNull(this.world.Resolve(IntrinsicTypes.Int4));
			Assert.IsNotNull(this.world.Resolve(IntrinsicTypes.Int8));
			Assert.IsNotNull(this.world.Resolve(IntrinsicTypes.Fp32));
			Assert.IsNotNull(this.world.Resolve(IntrinsicTypes.Fp64));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.I));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.I1));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.I2));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.I4));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.I8));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.U));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.U1));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.U2));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.U4));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.U8));
			Assert.IsNotNull(this.world.Resolve(PrimitiveTypes.String));
		}

		[Test]
		public void IsCompound() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				if(type.IsCompound) {
					Assert.IsTrue(type is CooS.Reflection.Derived.DerivedType);
					Assert.AreEqual(0, type.GenericParameterCount);
				} else {
					Assert.IsFalse(type is CooS.Reflection.Derived.DerivedType);
				}
			}
		}

		[Test]
		public void IsGenericParam() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				Assert.IsFalse(type.IsGenericParam);
				for(int i=0; i<type.GenericParameterCount; ++i) {
					TypeBase arg = type.GetGenericArgumentType(i);
					Assert.IsTrue(arg.IsGenericParam);
					Assert.AreEqual(GenericSources.Type, arg.GenericSource);
					Assert.Less(arg.GenericParamPosition, type.GenericParameterCount);
				}
			}
		}

		[Test]
		public void IsGenericParam2() {
			TypeBase type = this.world.ResolveType("CooS.Reflection.Test.TypeBaseTest+Test`1");
			Assert.IsNotNull(type);
			TypeBase gt = type.GetGenericArgumentType(0);
			Assert.IsTrue(gt.IsGenericParam);
			MethodBase method = type.FindMethod("methodReturnT");
			Assert.IsTrue(method.ReturnType.IsGenericParam);
			method = type.FindMethod("methodReturnArrayOfT");
			Assert.IsFalse(method.ReturnType.IsGenericParam);
		}

		[Test]
		public void GenericParameterCount() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				if(type.ContainsGenericParameters) {
					Assert.Greater(type.GenericParameterCount, 0);
				} else {
					Assert.AreEqual(0, type.GenericParameterCount);
				}
			}
		}

		[Test]
		public void ContainsGenericParameters() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				if(type.GenericParameterCount>0) {
					Assert.IsTrue(type.ContainsGenericParameters);
				} else {
					Assert.IsFalse(type.ContainsGenericParameters);
				}
			}
		}

		[Test]
		public void ContainsGenericParameters2() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				if(type.HasGenericParameters) {
					Assert.IsTrue(type.ContainsGenericParameters);
				}
			}
		}

		[Test]
		public void ContainsGenericParameters3() {
			TypeBase type = this.world.ResolveType("CooS.Reflection.Test.TypeBaseTest+Test`1");
			Assert.IsNotNull(type);
			Assert.AreEqual(1, type.GenericParameterCount);
			TypeBase gt = type.GetGenericArgumentType(0);
			MethodBase method = type.FindMethod("methodReturnT");
			Assert.IsTrue(method.ReturnType.ContainsGenericParameters);
			method = type.FindMethod("methodReturnArrayOfT");
			Assert.IsTrue(method.ReturnType.ContainsGenericParameters);
		}

		[Test]
		public void HasGenericParameters() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				if(type.ContainsGenericParameters) {
					if(type.EnclosingType==null || !type.EnclosingType.ContainsGenericParameters) {
						Assert.IsTrue(type.HasGenericParameters);
					}
				}
			}
		}

		[Test]
		public void CheckGenericRelatedConstraints() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				// every types from world must not be a kind of generic type.
				Assert.IsFalse(type.IsGenericParam);
				// does the type contain generic parameters?
				if(type.HasGenericParameters) {
					// if the type has generic parameters:
					Assert.IsTrue(type.ContainsGenericParameters);	// also means the type contains generic parameters.
					Assert.Greater(type.GenericParameterCount, 0);	// the type must have one or more generic parameters.
					for(int i=0; i<type.GenericParameterCount; ++i) {	// for each generic parameters:
						TypeBase arg = type.GetGenericArgumentType(i);
						Assert.IsTrue(arg.IsGenericParam);
						Assert.AreEqual(arg.GenericSource, GenericSources.Type);
						Assert.AreEqual(i, arg.GenericParamPosition);
					}
				}
				if(type.ContainsGenericParameters) {
					Assert.Greater(type.GenericParameterCount, 0);	// the type must have one or more generic parameters.
				} else {
					// the type who does not contain generic parameters must not do.
					Assert.IsFalse(type.HasGenericParameters);
					Assert.AreEqual(0, type.GenericParameterCount);
				}
				if(type.ContainsGenericParameters && !type.HasGenericParameters) {
					if(type.GenericParameterCount==0) {
						// maybe a derived type
						TypeBase et = type.ElementType;
						Assert.IsNotNull(et);
						Assert.IsTrue(et.ContainsGenericParameters);
					} else {
						// maybe a nested class
						Assert.IsTrue(type.IsNested);
						Assert.IsNotNull(type.EnclosingType);
						Assert.IsTrue(type.EnclosingType.ContainsGenericParameters);
					}
				}
			}
		}

		[Test]
		public void CheckConsistency() {
			foreach(TypeBase type in this.world.EnumTypes()) {
				try {
					Assert.AreEqual(!type.IsClosedGeneric, type.ContainsGenericParameters);
				} catch {
					Console.WriteLine(type);
					throw;
				}
			}
		}

	}

}
