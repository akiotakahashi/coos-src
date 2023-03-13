using System;

namespace CooS.Formats.CLI.Signature {

	public class CustomMod {

		public readonly ElementType ElementType;
		public readonly TypeDefOrRefEncoded TypeDefOrRef;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal static bool Predict(SignatureReader reader) {
			switch(reader.PeekMark()) {
			case ElementType.CModReqd:
			case ElementType.CModOpt:
				return true;
			default:
				return false;
			}
		}

		internal CustomMod(SignatureReader reader) {
			this.ElementType = reader.ReadMark();
			this.TypeDefOrRef = new TypeDefOrRefEncoded(reader);
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new CustomMod(reader);
			}

		}

	}

}
