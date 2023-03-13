using System;

namespace CooS.Formats.CLI.Signature {

	public abstract class SignatureBase {

		public readonly int RowIndex;

		internal SignatureBase(SignatureReader reader) {
			this.RowIndex = (int)reader.BaseStream.Position;
		}

	}

}
