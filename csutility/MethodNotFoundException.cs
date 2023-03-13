using System;

namespace CooS {

	public class MethodNotFoundException : NotFoundException {

		public MethodNotFoundException() {
		}

		public MethodNotFoundException(string msg) : base(msg) {
		}

	}
}
