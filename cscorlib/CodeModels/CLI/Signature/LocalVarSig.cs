using System;
using System.IO;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	class LocalVarSig {

		const byte LOCAL_SIG = 0x07;

		public readonly LocalVar[] LocalVars;

		public static bool Predict(byte prologue) {
			return prologue==LOCAL_SIG;
		}

		public LocalVarSig(SignatureReader reader) {
			reader.ConfirmMark((ElementType)LOCAL_SIG);
			int count = reader.ReadInt32();
			LocalVars = new LocalVar[count];
			for(int i=0; i<count; ++i) {
				LocalVars[i] = new LocalVar(reader);
			}
		}

	}

}
