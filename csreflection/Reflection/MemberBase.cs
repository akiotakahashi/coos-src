using System;
using System.Collections.Generic;
using MemberTypes=System.Reflection.MemberTypes;

namespace CooS.Reflection {

	public abstract class MemberBase {

		public abstract TypeBase Type { get; }
		public abstract string Name { get; }
		public abstract MemberTypes Kind { get; }

		public string FullName {
			get {
				return this.Type.FullName+":"+this.Name;
			}
		}

		public sealed override string ToString() {
			return this.Type.ToString()+":"+this.Name;
		}

	}

}
