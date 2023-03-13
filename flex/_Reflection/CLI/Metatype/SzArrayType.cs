using System;
using CooS.Reflection;
using CooS.Formats.CLI.Metadata;

namespace CooS.Reflection.CLI.Metatype {

	class SzArrayType : ArrayType  {

		public SzArrayType(AssemblyDef assembly, SuperType elemtype) : base(assembly,elemtype) {
		}
	
		/*
		public override Type BaseType {
			get {
				return this.AssemblyInfo.Manager.Array;
			}
		}
		*/

		public override string TypeSuffix {
			get {
				return "[]";
			}
		}

		public override int OffsetToContents {
			get {
				return this.AssemblyInfo.Manager.SzArray.InstanceSize;
			}
		}

		public override int GetArrayRank() {
			return 1;
		}

	}

}
