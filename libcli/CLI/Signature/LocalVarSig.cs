using System;

namespace CooS.Formats.CLI.Signature {

	public class LocalVarSig {

		const byte LOCAL_SIG = 0x07;

		public readonly LocalVar[] LocalVars;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal static bool Predict(byte prologue) {
			return prologue==LOCAL_SIG;
		}

		internal LocalVarSig(SignatureReader reader) {
			reader.ConfirmMark((ElementType)LOCAL_SIG);
			int count = reader.ReadInt32();
			LocalVars = new LocalVar[count];
			for(int i=0; i<count; ++i) {
				LocalVars[i] = new LocalVar(reader);
			}
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new LocalVarSig(reader);
			}

		}

	}

}
