using System;

namespace CooS {

	public class TypeNotFoundException : NotFoundException {
	
		public TypeNotFoundException() : base("Type is missing") {
		}

		public TypeNotFoundException(string name, string ns)
			: this(ns+"."+name) {
		}

		public TypeNotFoundException(string fullname)
			: base("Type is missing: "+fullname) {
		}

	}

}
