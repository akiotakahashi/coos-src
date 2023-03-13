using System;

namespace CooS.Manipulation {

	public class BadOperandException : BadILException {

		public BadOperandException() {
		}

		public BadOperandException(string msg) : base(msg) {
		}

	}

}
