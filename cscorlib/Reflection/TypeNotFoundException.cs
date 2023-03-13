using System;

namespace CooS.Reflection {

	public class TypeNotFoundException : NotFoundException {
	
		public TypeNotFoundException() : base("Type is missing") {
		}
	
		public TypeNotFoundException(string fullname) : base("Type is missing: "+fullname) {
		}

	}

}
