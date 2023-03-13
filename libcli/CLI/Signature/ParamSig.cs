using System;

namespace CooS.Formats.CLI.Signature {

	public class ParamSig {

		public readonly CustomMod[] CustomMods;
		public readonly TypeSig Type;

		internal ParamSig(SignatureReader reader) {
			CustomMods = SignatureUtility.ReadCustomMods(reader);
			Type = new TypeSig(reader);
		}

	}

}
