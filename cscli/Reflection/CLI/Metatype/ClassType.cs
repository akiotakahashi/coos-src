using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class ClassType : ConcreteType {

		public ClassType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly, entity) {
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
				type = new ClassType(assembly,rowIndex);
			}
			return type;
		}
#endif

		public override bool IsSigned {
			get {
				// class-type sould be treated as unsigned since it's a pointer on stack.
				return false;
			}
		}

		public override bool IsValueType {
			get {
				return false;
			}
		}

		public override bool IsEnum {
			get {
				return false;
			}
		}

		public override IntrinsicTypes IntrinsicType {
			get {
				return IntrinsicTypes.Objt;
			}
		}

	}

}
