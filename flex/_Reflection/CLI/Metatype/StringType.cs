using System;
using CooS.Reflection;
using CooS.Formats.CLI.Metadata;

namespace CooS.Reflection.CLI.Metatype {

	class StringType : ClassType {
	
		public StringType(AssemblyDef assembly, int rowIndex) : base(assembly,rowIndex) {
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
				type = new StringType(assembly,rowIndex);
			}
			return type;
		}

	}

}
