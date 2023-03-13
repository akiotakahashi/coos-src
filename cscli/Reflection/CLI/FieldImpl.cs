using System;
using CooS.Formats.CLI;
using CooS.Formats.CLI.Metadata;
using CooS.Reflection.Generics;

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

		public override TypeBase ReturnType {
			get {
				return this.type._Assembly.Realize(this.entity.FieldType);
			}
		}

		public override bool IsStatic {
			get {
				return 0!=(FieldAttributes.Static&this.entity.Attributes);
			}
		}

		private SpecializedList<InstantiatedField> instantiated;

		public override FieldBase Instantiate(IGenericParameterize resolver) {
			InstantiatedField value;
			if(instantiated.TryGetValue(resolver, out value)) {
				return value;
			} else {
				return instantiated[resolver] = new InstantiatedField(this, resolver);
			}
		}

	}

}
