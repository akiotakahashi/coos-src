using System;

namespace CooS.Formats.CLI.Signature {

	public class BadSignatureException : BadImageException {

		public BadSignatureException() {
		}

		public BadSignatureException(string msg) : base(msg) {
		}

	}

}
