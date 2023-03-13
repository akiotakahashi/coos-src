using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {

	public sealed class FieldImpl : FieldBase {

		private readonly TypeImpl type;
		private readonly FieldDefInfo entity;

		internal FieldImpl(TypeImpl type, FieldDefInfo entity) {
			this.type = type;
			this.entity = entity;
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

	}

}
