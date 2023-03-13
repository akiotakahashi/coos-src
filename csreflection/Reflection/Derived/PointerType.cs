using System;

namespace CooS.Reflection.Derived {

	public abstract class PointerType : DerivedType {

		public PointerType(AssemblyBase assembly, TypeBase elemtype) : base(assembly,elemtype) {
		}

		public override bool IsValueType {
			get {
				return true;
			}
		}

		public override int GetArrayRank() {
			throw new InvalidOperationException();
		}

		public override IntrinsicTypes IntrinsicType {
			get {
				return IntrinsicTypes.Pter;
			}
		}

	}

}
