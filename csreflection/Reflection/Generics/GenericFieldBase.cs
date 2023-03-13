using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Reflection.Generics {
	
	public abstract class GenericFieldBase : FieldBase {

		protected readonly FieldBase master;

		protected GenericFieldBase(FieldBase field) {
			this.master = field;
		}

		public override TypeBase Type {
			get {
				return this.master.Type;
			}
		}

		public override string Name {
			get {
				return this.master.Name;
			}
		}

		public override TypeBase ReturnType {
			get {
				return this.master.ReturnType;
			}
		}

		public override bool IsStatic {
			get {
				return this.master.IsStatic;
			}
		}

		public override FieldBase Instantiate(IGenericParameterize resolver) {
			return new InstantiatedField(this, resolver);
		}

	}

}
