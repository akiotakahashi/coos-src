using System;
using System.Collections.Generic;
using AssemblyName = System.Reflection.AssemblyName;

namespace CooS {

	public class AssemblyNotFoundException : NotFoundException {

		public AssemblyNotFoundException(string name, string culture, Version version) : base(name+" (culture="+culture+", ver="+version+")") {
		}

		public AssemblyNotFoundException(string name) : base(name) {
		}

		public AssemblyNotFoundException(AssemblyName name)
			: base(name.ToString()) {
		}

	}

}
