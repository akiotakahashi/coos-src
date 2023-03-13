using System;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	public class CustomMod {

		ElementType elementType;
		TypeDefOrRefEncoded typeDefOrRef;

		public static bool Predict(SignatureReader reader) {
			switch(reader.GetMark()) {
			case ElementType.CModReqd:
			case ElementType.CModOpt:
				return true;
			default:
				return false;
			}
		}

		public CustomMod(SignatureReader reader) {
			this.elementType = reader.ReadMark();
			this.typeDefOrRef = new TypeDefOrRefEncoded(reader);
		}

	}

}
