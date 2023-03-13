using System;
using System.Reflection;
using System.Collections;
using NUnit.Framework;

namespace CooS.Reflection {

	[TestFixture]
	public class TypeImplTest : IComparable {

		AssemblyManager manager;
		TypeImpl mytype;

		public TypeImplTest() {
			this.manager = TestUtility.CreateManager();
			this.mytype = this.manager.ResolveType(this.GetType().FullName, true);
		}

		Type[] GetNormTypes() {
			return new Type[]{
				typeof(byte),
				typeof(bool),
				typeof(ushort),
				typeof(char),
				typeof(int),
				typeof(uint),
				typeof(float),
				typeof(double),
				typeof(IntPtr),
				typeof(UIntPtr),
				typeof(object),
				typeof(string),
				//typeof(Enum),
				typeof(TypeImplTest),
				typeof(IComparable),
			};
		}

		Type[] GetTestTypes() {
			return new Type[]{
				this.manager.Byte,
				this.manager.Boolean,
				this.manager.UInt16,
				this.manager.Char,
				this.manager.Int32,
				this.manager.UInt32,
				this.manager.Single,
				this.manager.Double,
				this.manager.IntPtr,
				this.manager.UIntPtr,
				this.manager.Object,
				this.manager.String,
				//this.manager.Enum,
				this.mytype,
				this.manager.ResolveType("System.IComparable",true),
			};
		}

		[SetUp]
		public void Setup() {
			Type[] norm = this.GetNormTypes();
			Type[] test = this.GetTestTypes();
			Assert.AreEqual(norm.Length, test.Length);
		}

		[Test]
		public void BaseType() {
			Assert.AreEqual("System.Exception", this.manager.ResolveType("System.SystemException",true).BaseType.FullName);
		}

		[Test]
		public void IsPrimitive() {
			Type[] norm = this.GetNormTypes();
			Type[] test = this.GetTestTypes();
			for(int i=0; i<norm.Length; ++i) {
				Assert.AreEqual(norm[i].IsPrimitive, test[i].IsPrimitive, test[i].FullName, norm[i].FullName);
			}
		}

		[Test]
		public void IsValueType() {
			Type[] norm = this.GetNormTypes();
			Type[] test = this.GetTestTypes();
			for(int i=0; i<norm.Length; ++i) {
				Assert.AreEqual(norm[i].IsValueType, test[i].IsValueType, test[i].FullName, norm[i].FullName);
			}
		}

		[Test]
		public void IsAssignableFrom() {
			Type[] norm = this.GetNormTypes();
			Type[] test = this.GetTestTypes();
			for(int i=0; i<norm.Length; ++i) {
				for(int j=0; j<norm.Length; ++j) {
					Console.WriteLine("{0} is assignable from {1}: {2}", norm[i].FullName, norm[j].FullName, norm[i].IsAssignableFrom(norm[j]));
					Assert.AreEqual(norm[i].IsAssignableFrom(norm[j]), test[i].IsAssignableFrom(test[j])
						, norm[i].FullName+" and "+norm[j].FullName);
				}
			}
		}

		[Test]
		public void IsSubclassOf() {
			Type[] norm = this.GetNormTypes();
			Type[] test = this.GetTestTypes();
			for(int i=0; i<norm.Length; ++i) {
				for(int j=0; j<norm.Length; ++j) {
					Assert.AreEqual(norm[i].IsSubclassOf(norm[j]), test[i].IsSubclassOf(test[j])
						, norm[i].FullName+" and "+norm[j].FullName);
				}
			}
		}

		public bool P {
			get {
				return false;
			}
		}

		[Test]
		public void GetMethod() {
			Assert.AreEqual(null!=typeof(TypeImplTest).GetMethod("get_P"), null!=this.mytype.GetMethod("get_P"));
		}

		[Test]
		public void GetMethod_Object_Equals() {
			MethodInfo method = this.manager.Object.GetMethod("Equals"
				, BindingFlags.Public|BindingFlags.Instance
				, null, new TypeImpl[]{this.manager.Object}, null);
			Assert.IsNotNull(method);
		}

		[Test]
		public void GetProperty() {
			Assert.AreEqual(null!=typeof(TypeImplTest).GetProperty("P"), null!=this.mytype.GetProperty("P"));
		}

		#region IComparable ƒƒ“ƒo

		public int CompareTo(object obj) {
			// TODO:  TypeImplTest.CompareTo ŽÀ‘•‚ð’Ç‰Á‚µ‚Ü‚·B
			return 0;
		}

		#endregion

		[Test]
		public void GetInterfaces() {
			Type[] interfaces = this.mytype.GetInterfaces();
			foreach(Type i in interfaces) {
				Console.WriteLine(i.FullName);
			}
			Assert.AreEqual(1, interfaces.Length);
			Assert.AreEqual("IComparable", interfaces[0].Name);
		}

		[Test]
		public void GetInterfaces2() {
			TypeImpl type = this.manager.ResolveType("System.Collections.Hashtable",true);
			Type[] types = type.GetInterfaces();
			Assert.AreEqual(6, types.Length);
		}

		private void DumpSlotInfo(TypeImpl type) {
			Console.WriteLine("Slot Information: {0}", type.FullName);
			for(int i=0; i<type.TotalSlotCount; ++i) {
				Console.WriteLine("#{0} {1} ({2})", i, type.GetSlotMethod(i), type.GetSlotMethod(i).SlotIndex);
			}
			Console.WriteLine("Interface Mapping: {0}", type.FullName);
			foreach(TypeImpl iftype in type.GetInterfaces()) {
				InterfaceMapping ifmap = type.GetInterfaceMap(iftype);
				Assert.AreSame(type, ifmap.TargetType);
				Assert.AreEqual(ifmap.InterfaceMethods.Length, ifmap.TargetMethods.Length);
				Console.WriteLine("Entry: {0}", ifmap.InterfaceType.FullName);
				for(int i=0; i<ifmap.InterfaceMethods.Length; ++i) {
					Console.WriteLine("   #{2} {0} < {1}", ifmap.InterfaceMethods[i].Name, ifmap.TargetMethods[i].Name, i);
				}
			}
		}

		[Test]
		public void ConstructSlots() {
			TypeImpl asmdef = this.manager.ResolveType("CooS.CodeModels.CLI.AssemblyDef",true);
			foreach(MethodInfoImpl method in asmdef.DeclaredMethods) {
				if(!method.IsVirtual) continue;
				bool found = false;
				for(int i=0; i<asmdef.TotalSlotCount; ++i) {
					if(asmdef.GetSlotMethod(i)==method) {
						found = true;
						break;
					}
				}
				Assert.IsTrue(found, "Slot is not constructed correctly.");
			}
		}

		[Test]
		public void ConstructSlots2() {
			TypeImpl asmdef = this.manager.ResolveType("System.Collections.Hashtable",true);
			DumpSlotInfo(asmdef);
			foreach(MethodInfoImpl method in asmdef.DeclaredMethods) {
				if(!method.IsVirtual) continue;
				bool found = false;
				for(int i=0; i<asmdef.TotalSlotCount; ++i) {
					if(asmdef.GetSlotMethod(i)==method) {
						found = true;
						break;
					}
				}
				Assert.IsTrue(found, "Slot is not constructed correctly.");
			}
		}

		[Test]
		public void LayoutFields() {
			TypeImpl type = this.manager.ResolveType("CooS.CodeModels.CLI.Metadata.TypeDefTable",true);
			int i = 0;
			foreach(FieldInfoImpl field in type.GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)) {
				Console.WriteLine("Field #{0} [{1}-{2}] {3}", i++
					, field.GetFieldOffset()
					, field.GetFieldOffset()+((TypeImpl)field.FieldType).VariableSize
					, field.FieldType.FullName+" "+field.Name);
			}
		}

	}

}
