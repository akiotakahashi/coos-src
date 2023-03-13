using System;

namespace CooS.CodeModels {

	public class BadOperandException : BadILException {

		public BadOperandException() {
		}

		public BadOperandException(string msg) : base(msg) {
		}

	}

}
