using System;
using CooS.Formats.Java;
using FieldAttributes=System.Reflection.FieldAttributes;

namespace CooS.Reflection.Java {

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

		public override TypeBase ReturnType {
			get {
				return this.type.Assembly_.Realize(this.entity.FieldType);
			}
		}

		public override bool IsStatic {
			get {
				return this.entity.IsStatic;
			}
		}

		public override FieldBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

	}

}
