using System;
using CooS.Formats.Java;

namespace CooS.Reflection.Java.Metatype {

	class InterfaceType : ClassType  {
	
		public InterfaceType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly,entity) {
		}

#if VMIMPL
		internal static Type Setup(AssemblyDef assembly, int rowIndex, ConcreteType candidate, byte[] staticheap) {
			ConcreteType type;
			bool loaded = assembly.IsTypeDefLoaded(rowIndex);
			if(loaded) {
				type = (ConcreteType)assembly.GetTypeDef(rowIndex);
			} else if(candidate!=null) {
				assembly.AssignTypeDef(rowIndex, candidate);
				type = candidate;
			} else {
				type = new InterfaceType(assembly,rowIndex);
			}
			return type;
		}
#endif

		public override TypeBase BaseType {
			get {
				return this.Assembly.World.Resolve(PrimitiveTypes.Object);
			}
		}

	}

}
