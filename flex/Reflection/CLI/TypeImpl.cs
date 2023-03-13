using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {

	public class TypeImpl : TypeBase {

		private AssemblyImpl assembly;
		private TypeDefInfo entity;
		private FieldImpl[] fields;
		private MethodImpl[] methods;

		public TypeImpl(AssemblyImpl assembly, TypeDefInfo type) {
			this.assembly = assembly;
			this.entity = type;
			this.fields = new FieldImpl[type.FieldCount];
			this.methods = new MethodImpl[type.MethodCount];
		}

		public override AssemblyBase Assembly {
			get {
				return this.assembly;
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

		public override bool IsNested {
			get {
				return this.entity.IsNested;
			}
		}

		public override TypeBase EnclosingType {
			get {
				TypeDefInfo enc = this.entity.EnclosingType;
				return assembly.Realize(enc);
			}
		}

		public override bool IsGenericParam {
			get {
				return this.entity.IsGenericParam;
			}
		}

		public override System.Collections.Generic.IEnumerable<FieldBase> EnumField() {
			foreach(FieldDefInfo field in entity.FieldCollection) {
				if(fields[field.Index]==null) {
					fields[field.Index] = new FieldImpl(this, field);
				}
				yield return fields[field.Index];
			}
		}

		public override System.Collections.Generic.IEnumerable<MethodBase> EnumMethod() {
			foreach(MethodDefInfo method in entity.MethodCollection) {
				if(methods[method.Index]==null) {
					methods[method.Index] = new MethodImpl(this, method);
				}
				yield return methods[method.Index];
			}
		}

	}

}
