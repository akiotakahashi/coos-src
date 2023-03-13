using System;

namespace CooS.Drivers.ATAPI {

	public class ATAPIException : SystemException {

		public ATAPIException() {
		}

		public ATAPIException(string message) : base(message) {
		}

		public ATAPIException(StatusRegister status) : base(status.ToString()) {
		}

	}

}
