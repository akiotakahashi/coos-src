using System;
using CooS.Formats.Java;
using System.Collections.Generic;

namespace CooS.Reflection.Java.Metatype {

	abstract class ConcreteType : TypeImpl {

		private readonly TypeDefInfo entity;
		private readonly FieldImpl[] fields;
		private readonly MethodImpl[] methods;

		public ConcreteType(AssemblyImpl assembly, TypeDefInfo type) : base(assembly) {
			this.entity = type;
			this.fields = new FieldImpl[type.FieldCount];
			this.methods = new MethodImpl[type.MethodCount];
		}

		public override int Id {
			get {
				return this.entity.Index;
			}
		}

		public override string Name {
			get {
				return this.entity.Name;
			}
		}

		public override string Namespace {
			get {
				return this.entity.Namespace;
			}
		}

		public override bool HasGenericParam {
			get {
				return this.entity.IsGenericType;
			}
		}

		public override int GenericParameterCount {
			get {
				return 0;
			}
		}

		public override bool IsGenericParam {
			get {
				return this.entity.IsGenericParam;
			}
		}

		public override bool IsInterface {
			get {
				return this.entity.IsInterface;
			}
		}

		public override bool IsAbstract {
			get {
				return this.entity.IsAbstract;
			}
		}
		
		public override bool IsSealed {
			get {
				return this.entity.IsSealed;
			}
		}

		public override bool IsNested {
			get {
				return this.entity.IsNested;
			}
		}

		public override bool IsByRefPointer {
			get {
				return false;
			}
		}

		public override bool IsByValPointer {
			get {
				return false;
			}
		}

		public override TypeBase BaseType {
			get {
				// In case of System.Object, this is overridden specially.
				return this.Assembly_.Realize(this.entity.BaseType);
			}
		}

		public override TypeBase ElementType {
			get {
				throw new InvalidOperationException();
			}
		}

		public override TypeBase EnclosingType {
			get {
				TypeDefInfo enc = this.entity.EnclosingType;
				return this.Assembly_.Realize(enc);
			}
		}

		public override int GetArrayRank() {
			throw new InvalidOperationException();
		}

		public override int FieldCount {
			get {
				return this.fields.Length;
			}
		}

		public override int MethodCount {
			get {
				return this.methods.Length;
			}
		}

		public override IEnumerable<FieldBase> EnumFields() {
			foreach(FieldDefInfo field in entity.FieldCollection) {
				if(fields[field.Index]==null) {
					fields[field.Index] = new FieldImpl(this, field);
				}
				yield return fields[field.Index];
			}
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			foreach(MethodDefInfo method in entity.MethodCollection) {
				if(methods[method.Index]==null) {
					methods[method.Index] = new MethodImpl(this, method);
				}
				yield return methods[method.Index];
			}
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			throw new Exception("The method or operation is not implemented.");
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			foreach(TypeBase type in this.Assembly_.EnumNestedTypes(this)) {
				if(type.EnclosingType!=this)
					throw new UnexpectedException();
				yield return type;
			}
		}

	}
}