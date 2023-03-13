using System;

namespace CooS.Formats.CLI.Signature {

	public class ConstraintSig {

		public readonly bool Pinned = false;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal static bool Predict(SignatureReader reader) {
			return reader.PeekMark()==ElementType.Pinned;
		}

		internal ConstraintSig(SignatureReader reader) {
			switch(reader.ReadMark()) {
			case ElementType.Pinned:
				this.Pinned = true;
				break;
			default:
				throw new BadSignatureException();
			}
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new ConstraintSig(reader);
			}

		}

	}

}
