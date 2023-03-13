using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.CodeModels.CLI.Metatype {

	class ByValPointerType : PointerType {
		
		public ByValPointerType(AssemblyDef assembly, SuperType elemtype) : base(assembly,elemtype) {
		}

		public override Type BaseType {
			get {
				return this.AssemblyInfo.Manager.ByValPointer;
			}
		}

		public override string TypeSuffix {
			get {
				return "*";
			}
		}

		public override bool IsByRefPointer {
			get {
				return false;
			}
		}

		public override bool IsByValPointer {
			get {
				return true;
			}
		}

	}

}
