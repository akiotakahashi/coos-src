using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Reflection.Derived {

	public sealed class ByValPointerType : PointerType {
		
		public ByValPointerType(AssemblyBase assembly, TypeBase elemtype) : base(assembly,elemtype) {
		}

		public override CompondKind Kind {
			get { return CompondKind.ByVal; }
		}
		
		public override string TypeSuffix {
			get {
				return "*";
			}
		}

		public override TypeBase BaseType {
			get {
				return this.Assembly.World.ByValPointer;
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
