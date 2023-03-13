using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {
	
	abstract class ParameterDeclInfo {

		public abstract string Name {
			get;
		}

		public string FullName {
			get {
				return this.Method.FullName+"@"+Name;
			}
		}

		public abstract MethodDefInfo Method {
			get;
		}

	}

}
