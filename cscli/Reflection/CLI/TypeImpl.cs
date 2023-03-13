using System;
using CooS.Formats;
using CooS.Formats.CLI;
using CooS.Reflection.Generics;

namespace CooS.Reflection.CLI {

	public abstract class TypeImpl : TypeBase {

		private AssemblyImpl assembly;

		protected internal TypeImpl(AssemblyImpl assembly) {
			this.assembly = assembly;
		}

		public override AssemblyBase Assembly {
			get {
				return this.assembly;
			}
		}

		public AssemblyImpl _Assembly {
			get {
				return this.assembly;
			}
		}

		internal TypeBase Realize(GenericParamInfo type) {
			if(type.SourceType!=GenericSources.Type) { throw new NotSupportedException(); }
			return this.GetGenericArgumentType(type.Number);
		}

		private SpecializedList<TypeBase> specialized;

		public sealed override TypeBase Specialize(TypeBase[] args) {
			if(!this.ContainsGenericParameters) {
				return this;
			} else {
				TypeBase value;
				if(specialized.TryGetValue(args, out value)) {
					return value;
				} else {
					return specialized[args] = new SpecializedType(this, args);
				}
			}
		}

		public sealed override TypeBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

	}

}
