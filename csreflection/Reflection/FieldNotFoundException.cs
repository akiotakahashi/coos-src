using System;

namespace CooS.Reflection {

	public class FieldNotFoundException : NotFoundException {

		public FieldNotFoundException() {
		}

		public FieldNotFoundException(string msg) : base(msg) {
		}

	}
}
