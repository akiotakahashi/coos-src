using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.CodeModels.CLI.Metatype {
	
	abstract class ArrayType : DerivedType {

		public ArrayType(AssemblyDef assembly, SuperType elemtype) : base(assembly,elemtype) {
		}
		
		public override Type BaseType {
			get {
				return typeof(System.Array);
			}
		}

		protected override bool IsValueTypeImpl() {
			return false;
		}

		protected override bool IsArrayImpl() {
			return true;
		}

		public override bool IsByRefPointer {
			get {
				return false;
			}
		}

		public override bool IsByValPointer {
			get {
				return false;
			}
		}

		public override int InstanceSize {
			get {
				throw new NotSupportedException();
			}
		}

	}

}
