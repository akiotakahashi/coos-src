using System;
using System.Collections.Generic;
using MemberTypes=System.Reflection.MemberTypes;

namespace CooS.Reflection {
	
	public abstract class FieldBase : MemberBase {

		public abstract TypeBase ReturnType {
			get;
		}

		public override MemberTypes Kind {
			get { return MemberTypes.Method; }
		}

		public abstract bool IsStatic {
			get;
		}

		public FieldBase Bind(CooS.Reflection.Generics.InstantiatedType instantiatedType) {
			throw new Exception("The method or operation is not implemented.");
		}

		public abstract FieldBase Instantiate(IGenericParameterize resolver);

	}

}
