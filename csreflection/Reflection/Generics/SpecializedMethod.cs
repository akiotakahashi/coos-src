using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {

	public sealed class SpecializedMethod : GenericMethodBase {

		private readonly TypeBase[] args;

		public SpecializedMethod(MethodBase method, TypeBase[] args) : base(method) {
			this.args = args;
		}

		#region IGenericParameterize ÉÅÉìÉo

		public override int GenericParameterCount {
			get {
				return this.args.Length;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return false;
			}
		}

		public override TypeBase GetGenericArgumentType(int index) {
			return this.args[index];
		}

		#endregion

		public override TypeBase GetParameterType(int index) {
			TypeBase type = base.GetParameterType(index);
			if(type.IsGenericParam) {
				return this.args[type.GenericParamPosition];
			} else {
				return type;
			}
		}

		public override TypeBase GetVariableType(int index) {
			TypeBase type = base.GetVariableType(index);
			if(type.IsGenericParam) {
				return this.args[type.GenericParamPosition];
			} else {
				return type;
			}
		}

	}

}
