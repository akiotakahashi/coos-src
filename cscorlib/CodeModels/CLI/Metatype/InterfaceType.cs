using System;
using CooS.Reflection;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Metatype {

	class InterfaceType : ConcreteType  {
	
		public InterfaceType(AssemblyDef assembly, TypeDefRow row) : base(assembly,row) {
		}

		protected InterfaceType(AssemblyDef assembly, int rowIndex) : base(assembly,rowIndex) {
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
				type = new InterfaceType(assembly,rowIndex);
			}
			return type;
		}

		public override int VariableSize {
			get {
				return IntPtr.Size;
			}
		}

		public override Type BaseType {
			get {
				return this.MyAssembly.Manager.Object;
			}
		}

		protected override bool IsValueTypeImpl() {
			return false;
		}

		protected override bool IsPrimitiveImpl() {
			return false;
		}

		public override bool IsAssignableFrom(Type c) {
			if(c==null) return false;
			if(c==this) return true;
			foreach(Type t in c.GetInterfaces()) {
				if(t==this) return true;
			}
			return false;
		}

	}

}
