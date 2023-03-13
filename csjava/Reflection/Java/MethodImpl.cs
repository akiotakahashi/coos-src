using System;
using CooS.Formats.Java;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Java {
	
	public sealed class MethodImpl : MethodBase {

		private readonly TypeImpl type;
		private readonly MethodDefInfo entity;

		internal MethodImpl(TypeImpl type, MethodDefInfo entity) {
			this.type = type;
			this.entity = entity;
		}

		public override AssemblyBase Assembly {
			get {
				return type.Assembly;
			}
		}

		public AssemblyImpl Assembly_ {
			get {
				return type.Assembly_;
			}
		}

		public override TypeBase Type {
			get {
				return this.type;
			}
		}

		public TypeImpl Type_ {
			get {
				return this.type;
			}
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
				return false;
			}
		}

		public override int GenericParameterCount {
			get {
				return 0;
			}
		}

		public override bool IsConstructor {
			get {
				return this.Name==".ctor";
			}
		}

		public override bool IsBlank {
			get {
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override bool IsVirtual {
			get {
				return true;
			}
		}

		public override bool HasNewSlot {
			get {
				return false;
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.Assembly_.Realize(this.entity.ReturnType);
			}
		}

		public override int ParameterCount {
			get {
				return this.entity.ParameterCount;
			}
		}

		public override TypeBase GetParameterType(int index) {
			return this.Assembly_.Realize(this.entity.GetParameterType(index));
		}

		public override IEnumerable<ParamBase> EnumParameterInfo() {
			// NOP
			if(1==0) {
				yield return null;
			}
		}

		public override int VariableCount {
			get {
				throw new NotImplementedException();
			}
		}

		public override TypeBase GetVariableType(int index) {
			throw new NotImplementedException();
		}

		public override IEnumerable<object> EnumInstructions() {
			throw new NotImplementedException();
		}

		public override MethodBase Specialize(TypeBase[] args) {
			return this;
		}

		public override MethodBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

	}

}
