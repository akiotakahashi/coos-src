using System;

namespace CooS.Formats.CLI.Signature {

	public abstract class MemberSig : SignatureBase {

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal MemberSig(SignatureReader reader) : base(reader) {
		}

		protected class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				switch((byte)reader.PeekMark()) {
				case FieldSig.FIELD:
					return FieldSig.Factory.Parse(reader);
				default:
					return MethodSig.Factory.Parse(reader);
				}
			}

		}

	}

}
