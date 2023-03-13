using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Reflection.CLI.Metatype {

	class ByRefPointerType : PointerType {

		public ByRefPointerType(AssemblyDef assembly, SuperType elemtype) : base(assembly,elemtype) {
		}

		public override Type BaseType {
			get {
				return this.AssemblyInfo.Manager.ByRefPointer;
			}
		}

		public override string TypeSuffix {
			get {
				return "&";
			}
		}

		public override bool IsByRefPointer {
			get {
				return true;
			}
		}

		public override bool IsByValPointer {
			get {
				return false;
			}
		}

	}

}
