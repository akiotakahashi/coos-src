using System;
using FieldAttributes=System.Reflection.FieldAttributes;

namespace CooS.Reflection.Generics {

	public sealed class InstantiatedField : GenericFieldBase {

		private readonly IGenericParameterize resolver;

		public InstantiatedField(FieldBase entity, IGenericParameterize resolver) : base(entity) {
			this.resolver = resolver;
		}

		public override TypeBase Type {
			get {
				return this.master.Type.Instantiate(this.resolver);
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.resolver.ResolveGenericParameterType(this.master.ReturnType);
			}
		}

	}

}
