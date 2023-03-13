using System;

namespace CooS.Formats.CLI.Signature {

	public class RetType {

		public readonly CustomMod[] CustomMods;
		public readonly TypeSig Type;
		public readonly bool TypedByRef;
		public readonly bool Void;

		internal RetType(SignatureReader reader) {
			CustomMods = SignatureUtility.ReadCustomMods(reader);
			switch(reader.PeekMark()) {
			default:
				Type = new TypeSig(reader);
				return;
			case ElementType.TypedByRef:
				TypedByRef = true;
				reader.ReadMark();
				break;
			case ElementType.Void:
				Void = true;
				reader.ReadMark();
				break;
			}
		}

	}

}

