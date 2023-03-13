using System;
using System.IO;
using CooS.Reflection;
using System.Globalization;
using System.Collections.Generic;

namespace CooS {
	
	public abstract class World {

		public abstract AssemblyBase LoadAssembly(string type, Stream stream);
		public abstract bool IsSystemAssembly(AssemblyBase assembly);

		public abstract AssemblyBase ResolveAssembly(string name, CultureInfo culture, Version version);
		public abstract TypeBase Resolve(IntrinsicTypes it);
		public abstract TypeBase Resolve(PrimitiveTypes pt);
		public abstract TypeBase ResolveType(string fullname);
		public abstract TypeBase ResolveType(string name, string ns);
		public abstract FieldBase ResolveField(string ns, string type, string name);
		public abstract MethodBase ResolveMethod(string ns, string type, string name);

		public abstract IEnumerable<TypeBase> EnumTypes();

		public AssemblyBase LoadAssembly(string type, string filepath) {
			Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
			return LoadAssembly(type, stream);
		}

		public TypeBase Void {
			get {
				return ResolveType("Void", "System");
			}
		}

		public TypeBase Boolean {
			get {
				return ResolveType("Boolean", "System");
			}
		}

		public TypeBase Char {
			get {
				return ResolveType("Char", "System");
			}
		}

		public TypeBase SByte {
			get {
				return ResolveType("SByte", "System");
			}
		}

		public TypeBase Byte {
			get {
				return ResolveType("Byte", "System");
			}
		}

		public TypeBase Int16 {
			get {
				return ResolveType("Int16", "System");
			}
		}

		public TypeBase UInt16 {
			get {
				return ResolveType("UInt16", "System");
			}
		}

		public TypeBase Int32 {
			get {
				return ResolveType("Int32", "System");
			}
		}

		public TypeBase UInt32 {
			get {
				return ResolveType("UInt32", "System");
			}
		}

		public TypeBase Int64 {
			get {
				return ResolveType("Int64", "System");
			}
		}

		public TypeBase UInt64 {
			get {
				return ResolveType("UInt64", "System");
			}
		}

		public TypeBase IntPtr {
			get {
				return ResolveType("IntPtr", "System");
			}
		}

		public TypeBase UIntPtr {
			get {
				return ResolveType("UIntPtr", "System");
			}
		}

		public TypeBase Single {
			get {
				return ResolveType("Single", "System");
			}
		}

		public TypeBase Double {
			get {
				return ResolveType("Double", "System");
			}
		}

		public TypeBase Object {
			get {
				return ResolveType("Object", "System");
			}
		}

		public TypeBase String {
			get {
				return ResolveType("String", "System");
			}
		}

		public TypeBase ValueType {
			get {
				return ResolveType("ValueType", "System");
			}
		}

		public TypeBase Enum {
			get {
				return ResolveType("Enum", "System");
			}
		}

		public TypeBase Delegate {
			get {
				return ResolveType("Delegate", "System");
			}
		}

		public TypeBase Array {
			get {
				return ResolveType("Array", "System");
			}
		}

		public TypeBase SzArray {
			get {
				return ResolveType("SzArray", "CooS.Runtime");
			}
		}

		public TypeBase MnArray {
			get {
				return ResolveType("MnArray", "CooS.Runtime");
			}
		}

		public TypeBase ByRefPointer {
			get {
				return ResolveType("ByRefPointer", "CooS.Runtime");
			}
		}

		public TypeBase ByValPointer {
			get {
				return ResolveType("ByValPointer", "CooS.Runtime");
			}
		}

	}

}
