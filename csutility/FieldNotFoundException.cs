using System;

namespace CooS {

	public class FieldNotFoundException : NotFoundException {
	
		public FieldNotFoundException() {
		}

		public FieldNotFoundException(string fullname) : base(fullname) {
		}

	}

}
