using System;
using FieldAttributes=System.Reflection.FieldAttributes;

namespace CooS.Reflection.Generics {

	public sealed class GenericFieldImpl : GenericFieldBase {

		private readonly SpecializedType type;

		internal GenericFieldImpl(SpecializedType type, FieldBase master) : base(master) {
			this.type = type;
		}

		public override TypeBase Type {
			get {
				return this.type;
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.type.ResolveGenericParameterType(this.master.ReturnType);
			}
		}

	}

}
