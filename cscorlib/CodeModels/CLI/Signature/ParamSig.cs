using System;

namespace CooS.CodeModels.CLI.Signature {

	class ParamSig {

		CustomMod[] customMods;
		TypeSig type;

		public ParamSig(SignatureReader reader) {
			customMods = SignatureUtility.ReadCustomMods(reader);
			type = new TypeSig(reader);
		}

		public CustomMod[] CustomMods {
			get {
				return this.customMods;
			}
		}

		public TypeSig Type {
			get {
				return this.type;
			}
		}

	}

}
