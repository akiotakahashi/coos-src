using System;
using CooS.Reflection;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Metatype {

	class PrimitiveType : StructType {

		readonly int size;

		public PrimitiveType(AssemblyDef assembly, TypeDefRow row) : base(assembly,row) {
			size = GetSize(this.Name);
		}

		public PrimitiveType(AssemblyDef assembly, int rowIndex) : base(assembly,rowIndex) {
			size = GetSize(this.Name);
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
				type = new PrimitiveType(assembly,rowIndex);
			}
			return type;
		}
		
		static int GetSize(string name) {
			switch(name) {
			case "IntPtr":
			case "UIntPtr":
				return 4;
			case "Void":
				return 0;
			case "Boolean":
			case "Byte":
			case "SByte":
				return 1;
			case "Int16":
			case "UInt16":
			case "Char":
				return 2;
			case "Int32":
			case "UInt32":
			case "Single":
				return 4;
			case "Int64":
			case "UInt64":
			case "Double":
				return 8;
			default:
				throw new ArgumentException(name+" is not a primitive type.");
			}
		}

		protected override bool IsPrimitiveImpl() {
			return true;
		}

		public override int InstanceSize {
			get {
				return size;
			}
		}

		public override int VariableSize {
			get {
				return size;
			}
		}

		public bool Signed {
			get {
				switch(this.Name) {
				case "IntPtr":
				case "SByte":
				case "Int16":
				case "Int32":
				case "Int64":
					return true;
				default:
					return false;
				}
			}
		}

	}

}
