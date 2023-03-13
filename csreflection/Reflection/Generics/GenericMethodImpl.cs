using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {

	public sealed class GenericMethodImpl : GenericMethodBase {

		private readonly SpecializedType type;

		internal GenericMethodImpl(SpecializedType type, MethodBase master) : base(master) {
			this.type = type;
		}

		public override TypeBase Type {
			get {
				return this.type;
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.type.ResolveGenericParameterType(base.ReturnType);
			}
		}

		public override TypeBase GetParameterType(int index) {
			return this.type.ResolveGenericParameterType(base.GetParameterType(index));
		}

	}

}
