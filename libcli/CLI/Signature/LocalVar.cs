using System;
using System.Collections.Generic;

namespace CooS.Formats.CLI.Signature {

	public class LocalVar {

		public readonly ConstraintSig[] Constraints;
		public readonly TypeSig Type;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal LocalVar(SignatureReader reader) {
			List<ConstraintSig> clist = new List<ConstraintSig>();
			while(ConstraintSig.Predict(reader)) {
				clist.Add(new ConstraintSig(reader));
			}
			this.Constraints = clist.ToArray();
			this.Type = new TypeSig(reader);
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new LocalVar(reader);
			}

		}

	}

}
