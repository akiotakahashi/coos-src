using System;
using CooS.Formats;
using CooS.Formats.CLI;
using CooS.Formats.CLI.IL;
using CooS.Formats.CLI.Metadata;
using System.Collections.Generic;

namespace CooS.Reflection.CLI {
	
	public sealed class RegularMethodImpl : MethodImpl {

		private readonly MethodDefInfo entity;

		internal RegularMethodImpl(TypeImpl type, MethodDefInfo entity) : base(type, entity.ParameterCount) {
			this.entity = entity;
		}

		public override string Name {
			get {
				return this.entity.Name;
			}
		}

		public override int Id {
			get {
				return this.entity.Index;
			}
		}

		public override bool IsStatic {
			get {
				return this.entity.IsStatic;
			}
		}

		[Obsolete]
		public override bool IsGenericMethod {
			get {
				return this.entity.IsGeneric;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return this.entity.IsGeneric;
			}
		}

		public override bool IsConstructor {
			get {
				return this.Name==".ctor";
			}
		}

		public override bool IsBlank {
			get {
				return this.entity.IsBlank;
			}
		}

		public override bool IsVirtual {
			get {
				return this.entity.IsVirtual;
			}
		}

		public override bool HasNewSlot {
			get {
				return this.entity.HasNewSlot;
			}
		}

		public override System.Reflection.MethodImplAttributes ImplFlags {
			get {
				return (System.Reflection.MethodImplAttributes)this.entity.GetMethodImplementationFlags();
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.Realize(this.entity.ReturnType);
			}
		}

		public override int GenericParameterCount {
			get {
				return this.entity.GenericParameterCount;
			}
		}

		public override int ParameterCount {
			get {
				return this.entity.ParameterCount;
			}
		}

		public override TypeBase GetParameterType(int index) {
			return this.Realize(this.entity.GetParameterType(index));
		}

		public override IEnumerable<ParamBase> EnumParameterInfo() {
			foreach(ParameterDefInfo param in this.entity.ParameterCollection) {
				if(param.Sequence>=1) {
					// [CLI4th 22.33-4]
					// Sequence shall have a value >= 0 and <= number of parameters in owner method.
					// A Sequencevalue of 0 refers to the owner methodfs return type;
					// its parameters are then numbered from 1 onwards
					yield return this.Realize(param);
				}
			}
		}

		public override int VariableCount {
			get {
				return this.entity.VariableCount;
			}
		}

		public override TypeBase GetVariableType(int index) {
			return this.Realize(this.entity.GetVariableType(index));
		}

		public override IEnumerable<object> EnumInstructions() {
			foreach(CooS.Formats.CLI.IL.Instruction inst in this.entity.EnumInstructions()) {
				yield return new Instruction(this, inst);
			}
		}

	}

}
