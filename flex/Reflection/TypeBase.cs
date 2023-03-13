using System;
using System.Collections.Generic;

namespace CooS.Reflection {
	
	public abstract class TypeBase /*: Type*/ {

		public abstract AssemblyBase Assembly {
			get;
		}

		public abstract string Name {
			get;
		}

		public abstract string Namespace {
			get;
		}

		public abstract bool IsNested {
			get;
		}

		public abstract TypeBase EnclosingType {
			get;
		}

		public abstract bool IsGenericParam {
			get;
		}

		public string FullName {
			get {
				if(IsNested) {
					TypeBase enclosing = this.EnclosingType;
					return enclosing.FullName+"+"+this.Name;
				} else if(IsGenericParam) {
					return this.Namespace+"`"+this.Name;
				} else {
					string ns = this.Namespace;
					if(ns==null) {
						return this.Name;
					} else {
						return ns+"."+this.Name;
					}
				}
			}
		}

		public abstract IEnumerable<FieldBase> EnumField();
		public abstract IEnumerable<MethodBase> EnumMethod();

	}

}
