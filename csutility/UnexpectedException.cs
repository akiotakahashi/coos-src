using System;

namespace CooS {

	public class UnexpectedException : SystemException {

		public UnexpectedException() {
		}

		public UnexpectedException(string msg) : base(msg) {
		}

	}

}
