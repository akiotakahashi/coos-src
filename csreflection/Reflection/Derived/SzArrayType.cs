using System;

namespace CooS.Reflection.Derived {

	public class SzArrayType : ArrayType  {

		public SzArrayType(AssemblyBase assembly, TypeBase elemtype) : base(assembly,elemtype) {
		}

		public override TypeBase BaseType {
			get {
				//return this.Assembly.World.SzArray;
				return this.Assembly.World.Array;
			}
		}

		public override CompondKind Kind {
			get { return CompondKind.SzArray; }
		}
		
		public override string TypeSuffix {
			get {
				return "[]";
			}
		}

		public override int GetArrayRank() {
			return 1;
		}

	}

}
