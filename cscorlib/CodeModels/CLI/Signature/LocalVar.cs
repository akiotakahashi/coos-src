using System;
using System.Collections;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	class LocalVar {

		ConstraintSig[] constraints;
		//bool byref = false;
		TypeSig type;

		public LocalVar(SignatureReader reader) {
			ArrayList clist = new ArrayList();
			while(ConstraintSig.Predict(reader)) {
				clist.Add(new ConstraintSig(reader));
			}
			this.constraints = (ConstraintSig[])clist.ToArray(typeof(ConstraintSig));
			/*
			 * This is regular code but TypeSig accepts ByRef so I delete them.
			 * 
			switch((ElementType)reader.GetMark()) {
			case ElementType.ByRef:
				byref = true;
				break;
			}
			*/
			type = new TypeSig(reader);
		}

		public ConstraintSig[] Constraints {
			get {
				return this.constraints;
			}
		}

		public TypeSig Type {
			get {
				return this.type;
			}
		}

	}

}
