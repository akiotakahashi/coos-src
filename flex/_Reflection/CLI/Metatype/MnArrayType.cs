using System;
using System.Reflection;
using CooS.Formats.CLI.Metadata;

namespace CooS.Reflection.CLI.Metatype {

	class MnArrayType : ArrayType  {
	
		int rank;
		
		public MnArrayType(AssemblyDef assembly, SuperType elemtype) : base(assembly,elemtype) {
		}
		
		private void SetRank(int rank) {
			this.rank = rank;
		}
		
		public int GetRank() {
			return rank;
		}
	
		public override string TypeSuffix {
			get {
				string name = GetElementType().Name;
				name += "[";
				for(int i=1; i<rank; ++i) {
					name += ",";
				}
				name += "]";
				return name;
			}
		}

		public override int OffsetToContents {
			get {
				// Keep this value sync with Ponytai.Wrap.System._Array
				return 8+4*rank;
			}
		}

	}

}
