using System;
using System.Collections.Generic;

namespace CooS.Reflection {

	public struct MemberRefDesc {
		
		public string name;
		public TypeBase returntype;
		public TypeBase[] parameters;

		public MemberRefDesc(MethodBase method) {
			this.name = method.Name;
			this.returntype = method.ReturnType;
			this.parameters = new TypeBase[method.ParameterCount];
			for(int i=0; i<this.parameters.Length; ++i) {
				this.parameters[i] = method.GetParameterType(i);
			}
		}

		private bool matchName(string value) {
			return this.name==null || this.name==value;
		}

		private bool matchReturnType(TypeBase type) {
			return this.returntype==null || this.returntype==type;
		}

		private bool matchParameters(IEnumerable<TypeBase> list) {
			if(this.parameters==null) {
				return true;
			} else {
				int i = 0;
				foreach(TypeBase type in list) {
					if(i>=this.parameters.Length) {
						return false;
					} else if(type!=this.parameters[i]) {
						return false;
					}
					++i;
				}
				return i==this.parameters.Length;
			}
		}

		public bool IsMatch(FieldBase field) {
			if(parameters!=null) { return false; }
			if(!matchName(field.Name)) { return false; }
			if(!matchReturnType(field.ReturnType)) { return false; }
			return true;
		}

		public bool IsMatch(MethodBase method) {
			if(!matchName(method.Name)) { return false; }
			if(!matchReturnType(method.ReturnType)) { return false; }
			if(!matchParameters(method.EnumParameterType())) { return false; }
			return true;
		}

	}

}
