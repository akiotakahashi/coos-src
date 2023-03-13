using System;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	public class BadSignatureException : BadMetadataException {

		public BadSignatureException() {
		}

		public BadSignatureException(string msg) : base(msg) {
		}

	}

}
