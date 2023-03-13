using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class DelegateType : ClassType  {

		public DelegateType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly, entity) {
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
				type = new DelegateType(assembly,rowIndex);
			}
			return type;
		}
#endif

	}

}
