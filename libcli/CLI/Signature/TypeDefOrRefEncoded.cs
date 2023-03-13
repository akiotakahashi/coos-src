using System;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Signature {

	public struct TypeDefOrRefEncoded {

		public TypeDefOrRefCodedIndex Token;	// TypeDefOrRefCodedIndex

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal TypeDefOrRefEncoded(SignatureReader reader) {
			Token = new TypeDefOrRefCodedIndex(reader.ReadInt32());
		}

		public override string ToString() {
			return Token.ToString();
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new TypeDefOrRefEncoded(reader);
			}

		}

	}

}
