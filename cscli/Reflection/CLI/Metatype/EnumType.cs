using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class EnumType : StructType {

		public EnumType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly,entity) {
		}

#if VMIMPL	
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
#endif

		public override bool IsEnum {
			get {
				return true;
			}
		}

	}

}
