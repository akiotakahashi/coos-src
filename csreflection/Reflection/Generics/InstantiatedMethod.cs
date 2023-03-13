using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {

	public sealed class InstantiatedMethod : GenericMethodBase {

		private readonly IGenericParameterize resolver;

		public InstantiatedMethod(MethodBase master, IGenericParameterize resolver) : base(master) {
			this.resolver = resolver;
		}

		public override TypeBase Type {
			get {
				return this.master.Type.Instantiate(this.resolver);
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.resolver.ResolveGenericParameterType(this.BaseMethod.ReturnType);
			}
		}

		[Obsolete]
		public override bool IsGenericMethod {
			get {
				return false;
			}
		}

		public override bool IsClosedGeneric {
			get {
				return this.Type.IsClosedGeneric;
			}
		}

		public override TypeBase GetParameterType(int index) {
			return this.resolver.ResolveGenericParameterType(this.BaseMethod.GetParameterType(index));
		}

		public override TypeBase GetVariableType(int index) {
			return this.resolver.ResolveGenericParameterType(this.BaseMethod.GetVariableType(index));
		}

		public override IEnumerable<object> EnumInstructions() {
			foreach(object inst in this.master.EnumInstructions()) {
				yield return inst;
			}
		}

	}

}
