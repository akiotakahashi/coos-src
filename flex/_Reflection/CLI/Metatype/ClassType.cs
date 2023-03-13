using System;
using System.Reflection;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class ClassType : ConcreteType {

		public ClassType(AssemblyDef assembly, TypeDefInfo row) : base(assembly,row) {
		}

		protected ClassType(AssemblyDef assembly, int rowIndex) : base(assembly,rowIndex) {
		}

		public ClassType(AssemblyDef assembly, Type metatype) : base(assembly, ((TypeImpl)metatype).RowIndex) {
		}
		
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

		public override int VariableSize {
			get {
				return IntPtr.Size;
			}
		}

		protected override bool IsValueTypeImpl() {
			return false;
		}

		protected override bool IsPrimitiveImpl() {
			return false;
		}

	}

}
