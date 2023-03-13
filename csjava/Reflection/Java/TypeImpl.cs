using System;
using CooS.Formats.Java;
using System.Collections.Generic;

namespace CooS.Reflection.Java {

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

		public AssemblyImpl Assembly_ {
			get {
				return this.assembly;
			}
		}

		public override TypeBase Specialize(TypeBase[] args) {
			throw new NotSupportedException();
		}

		public override TypeBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

	}

}
