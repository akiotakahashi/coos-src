using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class PrimitiveType : StructType {

		public PrimitiveType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly,entity) {
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
				type = new PrimitiveType(assembly,rowIndex);
			}
			return type;
		}
#endif

		public override bool IsPrimitive {
			get {
				return true;
			}
		}

		public override bool IsSigned {
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

		public override IntrinsicTypes IntrinsicType {
			get {
				switch(this.Name) {
				case "IntPtr":
				case "UIntPtr":
					return IntrinsicTypes.NInt;
				case "Void":
					return IntrinsicTypes.Any;
				case "Boolean":
				case "Byte":
				case "SByte":
				case "Int16":
				case "UInt16":
				case "Char":
				case "Int32":
				case "UInt32":
					return IntrinsicTypes.Int4;
				case "Int64":
				case "UInt64":
					return IntrinsicTypes.Int8;
				case "Single":
					return IntrinsicTypes.Fp32;
				case "Double":
					return IntrinsicTypes.Fp64;
				default:
					throw new NotSupportedException(this.FullName);
				}
			}
		}

		public override int IntrinsicSize {
			get {
				switch(this.Name) {
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
					throw new NotSupportedException(this.FullName);
				}
			}
		}

	}

}
