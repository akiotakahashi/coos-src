using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Reflection.Derived {
	
	public abstract class ArrayType : DerivedType {

		public ArrayType(AssemblyBase assembly, TypeBase elemtype) : base(assembly,elemtype) {
		}
		
		public override TypeBase BaseType {
			get {
				return this.Assembly.World.Array;
			}
		}

		public override bool IsArray {
			get {
				return true;
			}
		}

		public override bool IsValueType {
			get {
				return false;
			}
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

		public override IntrinsicTypes IntrinsicType {
			get {
				return IntrinsicTypes.Objt;
			}
		}

	}

}
