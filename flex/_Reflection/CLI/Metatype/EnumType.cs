using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class EnumType : StructType {

		public EnumType(AssemblyDef assembly, TypeDefInfo row) : base(assembly,row) {
		}

		public EnumType(AssemblyDef assembly, int rowIndex) : base(assembly,rowIndex) {
		}
			
		internal new static Type Setup(AssemblyDef assembly, int rowIndex, ConcreteType candidate, byte[] staticheap) {
			ConcreteType type;
			bool loaded = assembly.IsTypeDefLoaded(rowIndex);
			if(loaded) {
				type = (ConcreteType)assembly.GetTypeDef(rowIndex);
			} else if(candidate!=null) {
				assembly.AssignTypeDef(rowIndex, candidate);
				type = candidate;
			} else {
				type = new EnumType(assembly,rowIndex);
			}
			return type;
		}

	}

}
