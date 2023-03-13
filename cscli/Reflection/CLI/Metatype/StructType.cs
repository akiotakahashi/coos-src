using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class StructType : ConcreteType {

		public StructType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly,entity) {
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
				type = new StructType(assembly,rowIndex);
			}
			return type;
		}
#endif

		public override bool IsSigned {
			get {
				// user-defined valuetype sould be treated as unsiged
				// because it's wrong to be sign-extend.
				return false;
			}
		}

		public override bool IsValueType {
			get {
				return true;
			}
		}

		public override bool IsEnum {
			get {
				return false;
			}
		}

		public override IntrinsicTypes IntrinsicType {
			get {
				return IntrinsicTypes.Any;
			}
		}

	}

}
