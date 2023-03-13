using System;

namespace CooS.Drivers.ATAPI {

	public class ATAPITimeoutException : ATAPIException {

		public ATAPITimeoutException() {
		}

		public ATAPITimeoutException(StatusRegister status) : base(status) {
		}

	}

}
