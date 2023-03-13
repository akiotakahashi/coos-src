using System;

namespace CooS.Reflection {

	public class MethodNotFoundException : NotFoundException {

		public MethodNotFoundException() {
		}

		public MethodNotFoundException(string msg) : base(msg) {
		}

	}
}
