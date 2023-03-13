using System;

namespace CooS.Reflection.Derived {

	public sealed class MnArrayType : ArrayType  {
	
		private int rank;
		
		public MnArrayType(AssemblyBase assembly, TypeBase elemtype) : base(assembly,elemtype) {
		}

		public override CompondKind Kind {
			get { return CompondKind.MnArray; }
		}
		
		public override TypeBase BaseType {
			get {
				return this.Assembly.World.MnArray;
			}
		}

		private void SetRank(int rank) {
			this.rank = rank;
		}

		public override int GetArrayRank() {
			return this.rank;
		}
	
		public override string TypeSuffix {
			get {
				string name = this.ElementType.Name;
				name += "[";
				for(int i=1; i<rank; ++i) {
					name += ",";
				}
				name += "]";
				return name;
			}
		}

	}

}
