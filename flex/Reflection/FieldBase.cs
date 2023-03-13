using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Reflection {
	
	public abstract class FieldBase {

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

	}

}
