using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {
	
	public sealed class SpecializedType : GenericTypeBase {

		private TypeBase[] args;

		public SpecializedType(TypeBase generic, TypeBase[] args) : base(generic) {
			this.args = args;
		}

		public override int GenericParameterCount {
			get {
				return this.args.Length;
			}
		}

		public override TypeBase GetGenericArgumentType(int index) {
			return this.args[index];
		}

		public override TypeBase BaseType {
			get {
				if(this.master.BaseType.HasGenericParameters) {
					return this.master.BaseType.Instantiate(this);
				} else {
					return this.master.BaseType;
				}
			}
		}

		public override bool HasGenericParameters {
			get {
				return true;
			}
		}

		public override bool IsGenericParam {
			get {
				return false;
			}
		}

		private GenericFieldImpl[] fields;
		private GenericMethodImpl[] methods;

		public override IEnumerable<FieldBase> EnumFields() {
			if(this.fields==null) {
				List<GenericFieldImpl> fields = new List<GenericFieldImpl>();
				foreach(FieldBase field in this.master.EnumFields()) {
					fields.Add(new GenericFieldImpl(this, field));
				}
				this.fields = fields.ToArray();
			}
			return this.fields;
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			if(this.methods==null) {
				List<GenericMethodImpl> methods = new List<GenericMethodImpl>();
				foreach(MethodBase method in this.master.EnumMethods()) {
					methods.Add(new GenericMethodImpl(this, method));
				}
				this.methods = methods.ToArray();
			}
			return this.methods;
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			foreach(TypeBase type in this.master.EnumInterfaces()) {
				yield return type;
			}
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			foreach(TypeBase type in this.master.EnumNestedTypes()) {
				yield return type;
			}
		}

		public override TypeBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

	}

}
