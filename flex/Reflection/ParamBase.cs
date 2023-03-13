using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Reflection {

	public abstract class ParamBase {

		public abstract MethodBase Method {
			get;
		}

		public abstract string Name {
			get;
		}

		public string FullName {
			get {
				return this.Method.FullName+"+"+this.Name;
			}
		}

	}

}
