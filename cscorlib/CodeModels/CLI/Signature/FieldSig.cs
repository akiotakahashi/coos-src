using System;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	class FieldSig {

		const byte FIELD = 0x06;

		CustomMod[] customMods;
		TypeSig type;

		public static bool Predict(byte prologue) {
			return prologue==FIELD;
		}

		public FieldSig(SignatureReader reader) {
			reader.ConfirmMark((ElementType)FIELD);
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
