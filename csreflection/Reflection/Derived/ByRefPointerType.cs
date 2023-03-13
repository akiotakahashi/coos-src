using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Reflection.Derived {

	public sealed class ByRefPointerType : PointerType {

		public ByRefPointerType(AssemblyBase assembly, TypeBase elemtype) : base(assembly,elemtype) {
		}

		public override CompondKind Kind {
			get { return CompondKind.ByRef; }
		}

		public override string TypeSuffix {
			get {
				return "&";
			}
		}

		public override TypeBase BaseType {
			get {
				return this.Assembly.World.ByRefPointer;
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
