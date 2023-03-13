using System;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	class RetType {

		CustomMod[] customMods;
		TypeSig type;

		public RetType(SignatureReader reader) {
			customMods = SignatureUtility.ReadCustomMods(reader);
			switch(reader.GetMark()) {
			default:
				type = new TypeSig(reader);
				return;
			case ElementType.TypedByRef:
				throw new NotImplementedException();
			}
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

