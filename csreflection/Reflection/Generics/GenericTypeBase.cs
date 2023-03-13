using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {
	
	public abstract class GenericTypeBase : TypeBase {

		protected readonly TypeBase master;

		protected GenericTypeBase(TypeBase generic) {
			this.master = generic;
		}

		public override string ToString() {
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append(base.ToString());
			if(this.IsClosedGeneric) {
				builder.Append("[");
			} else {
				builder.Append("<");
			}
			for(int i=0; i<this.GenericParameterCount; ++i) {
				builder.Append(this.GetGenericArgumentType(i).Name);
				builder.Append(",");
			}
			builder.Length -= 1;
			if(this.IsClosedGeneric) {
				builder.Append("]");
			} else {
				builder.Append(">");
			}
			return builder.ToString();
		}

		public override AssemblyBase Assembly {
			get { return this.master.Assembly; }
		}

		public override bool HasGenericParameters {
			get {
				return true;
			}
		}

		public override TypeBase GetGenericArgumentType(int position) {
			return this.master.GetGenericArgumentType(position);
		}

		public override int GenericParameterCount {
			get { return this.master.GenericParameterCount; }
		}

		public override bool ContainsGenericParameters {
			get {
				return true;
			}
		}

		public override int Id {
			get { return this.master.Id; }
		}

		public override string Name {
			get { return this.master.Name; }
		}

		public override string Namespace {
			get { return this.master.Namespace; }
		}

		public override TypeBase BaseType {
			get { return this.master.BaseType; }
		}

		public override bool IsAbstract {
			get { return this.master.IsAbstract; }
		}

		public override bool IsSealed {
			get { return this.master.IsSealed; }
		}

		public override bool IsNested {
			get { return this.master.IsNested; }
		}

		public override bool IsInterface {
			get { return this.master.IsInterface; }
		}

		public override bool IsValueType {
			get { return this.master.IsValueType; }
		}

		public override bool IsEnum {
			get { return this.master.IsEnum; }
		}

		public override bool IsByRefPointer {
			get { return this.master.IsByRefPointer; }
		}

		public override bool IsByValPointer {
			get { return this.master.IsByValPointer; }
		}

		public override TypeBase ElementType {
			get { return this.master.ElementType; }
		}

		public override TypeBase EnclosingType {
			get { return this.master.EnclosingType; }
		}

		public override int FieldCount {
			get { return this.master.FieldCount; }
		}

		public override int MethodCount {
			get { return this.master.MethodCount; }
		}

		public override int GetArrayRank() {
			return this.master.GetArrayRank();
		}

		public override IEnumerable<FieldBase> EnumFields() {
			return this.master.EnumFields();
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			return this.master.EnumMethods();
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			return this.master.EnumInterfaces();
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			return this.master.EnumNestedTypes();
		}

		public sealed override TypeBase Specialize(TypeBase[] args) {
			return this.master.Specialize(args);
		}

		public override TypeBase Instantiate(IGenericParameterize resolver) {
			if(!this.HasGenericParameters) {
				return this;
			} else {
				return new InstantiatedType(this, resolver);
			}
		}

	}

}
