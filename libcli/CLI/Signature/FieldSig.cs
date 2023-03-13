using System;

namespace CooS.Formats.CLI.Signature {

	public class FieldSig : MemberSig {

		public const byte FIELD = 0x06;

		public readonly CustomMod[] CustomMods;
		public readonly TypeSig Type;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal static bool Predict(byte prologue) {
			return prologue==FIELD;
		}

		internal FieldSig(SignatureReader reader) : base(reader) {
			reader.ConfirmMark((ElementType)FIELD);
			CustomMods = SignatureUtility.ReadCustomMods(reader);
			Type = new TypeSig(reader);
		}

	}
}
