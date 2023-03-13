using System;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	public class ConstraintSig {

		bool pinned = false;

		public static bool Predict(SignatureReader reader) {
			return reader.GetMark()==ElementType.Pinned;
		}

		public ConstraintSig(SignatureReader reader) {
			switch(reader.ReadMark()) {
			case ElementType.Pinned:
				pinned = true;
				break;
			default:
				throw new BadSignatureException();
			}
		}

		public bool Pinned {
			get {
				return pinned; 
			}
		}

	}

}
