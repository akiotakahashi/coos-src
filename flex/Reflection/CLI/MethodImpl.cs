using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {
	
	public sealed class MethodImpl : MethodBase {

		private readonly TypeImpl type;
		private readonly MethodDefInfo entity;
		private readonly ParamImpl[] parameters;

		internal MethodImpl(TypeImpl type, MethodDefInfo entity) {
			this.type = type;
			this.entity = entity;
			this.parameters = new ParamImpl[entity.ParameterCount];
		}

		public override TypeBase Type {
			get {
				return this.type;
			}
		}

		public override string Name {
			get {
				return this.entity.Name;
			}
		}

		private ParamBase Realize(ParameterDefInfo param) {
			if(parameters[param.Sequence-1]==null) {
				parameters[param.Sequence-1] = new ParamImpl(this, param);
			}
			return parameters[param.Sequence-1];
		}

		public override System.Collections.Generic.IEnumerable<ParamBase> EnumParam() {
			foreach(ParameterDefInfo param in this.entity.ParameterCollection) {
				if(param.Sequence>0) {
					// [CLI4th 22.33-4]
					// Sequence shall have a value >= 0 and <= number of parameters in owner method.
					// A Sequencevalue of 0 refers to the owner methodfs return type;
					// its parameters are then numbered from 1 onwards
					yield return Realize(param);
				}
			}
		}

	}

}
