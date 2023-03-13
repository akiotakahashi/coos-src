using System;
using CooS.Formats.CLI;
using System.Collections.Generic;

namespace CooS.Reflection {

	public abstract class MethodBase {

		public abstract TypeBase Type {
			get;
		}

		public abstract string Name {
			get;
		}

		public string FullName {
			get {
				return this.Type.FullName+":"+this.Name;
			}
		}

		public abstract IEnumerable<ParamBase> EnumParam();

	}

}
