using System;
using CooS.Reflection;

namespace CooS.Reflection.CLI.Metatype {

	abstract class PointerType : DerivedType {

		public PointerType(AssemblyDef assembly, SuperType elemtype) : base(assembly,elemtype) {
		}

		protected override bool IsValueTypeImpl() {
			return true;
		}

		protected override bool IsArrayImpl() {
			return false;
		}

		public override int InstanceSize {
			get {
				return IntPtr.Size;
			}
		}

		public override int OffsetToContents {
			get {
				return 0;
			}
		}

	}

}
