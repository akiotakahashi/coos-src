using System;
using CooS.Reflection;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class StructType : ConcreteType {

		public StructType(AssemblyDef assembly, TypeDefInfo row) : base(assembly,row) {
		}

		protected StructType(AssemblyDef assembly, int rowIndex) : base(assembly,rowIndex) {
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
				type = new StructType(assembly,rowIndex);
			}
			return type;
		}

		protected override bool IsValueTypeImpl() {
			return true;
		}

		protected override bool IsPrimitiveImpl() {
			return false;
		}

		public override int VariableSize {
			get {
				return this.InstanceSize;
			}
		}

	}

}
