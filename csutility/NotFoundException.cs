using System;

namespace CooS {

	public class NotFoundException : Exception {

		public NotFoundException() {
		}

		public NotFoundException(string msg) : base(msg) {
		}

		public NotFoundException(object container, object searchfor) : base("Not found for "+searchfor+" in "+container) {
		}

	}

}
