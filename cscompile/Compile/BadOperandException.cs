using System;
using CooS.Formats;

namespace CooS.Compile {

	public class BadOperandException : BadCodeException {

		public BadOperandException() {
		}

		public BadOperandException(string msg) : base(msg) {
		}

	}

}
