using System;

namespace CooS.Reflection {

	public struct TypeRefDesc {

		public string name;
		public string ns;

		private bool matchName(string value) {
			return this.name==null || this.name==value;
		}

		private bool matchNamespace(string value) {
			return this.ns==null || this.ns==value;
		}

		public bool IsMatch(TypeBase type) {
			if(!matchName(type.Name)) { return false; }
			if(!matchNamespace(type.Namespace)) { return false; }
			return true;
		}

	}

}
