using System;
using System.Collections.Generic;

namespace CooS.Reflection.Derived {

	public abstract class DerivedType : TypeBase {
	
		private readonly AssemblyBase assembly;
		private readonly TypeBase elementType;

		public DerivedType(AssemblyBase assembly, TypeBase elemtype) {
			this.assembly = assembly;
			this.elementType = elemtype;
		}

		public abstract CompondKind Kind { get; }
		public abstract string TypeSuffix { get; }

		public override AssemblyBase Assembly {
			get {
				return this.elementType.Assembly;
			}
		}

		public override int Id {
			get {
				throw new NotSupportedException();
			}
		}

		public override string Name {
			get {
				return this.elementType.Name+this.TypeSuffix;
			}
		}

		public override string Namespace {
			get {
				return this.elementType.Namespace;
			}
		}

		public override CooS.Formats.GenericSources GenericSource {
			get {
				return CooS.Formats.GenericSources.None;
			}
		}

		public override bool  HasGenericParameters {
			get {
				return false;
			}
		}

		public override bool IsGenericParam {
			get {
				return false;
			}
		}

		public override int GenericParameterCount {
			get {
				return 0;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return this.elementType.ContainsGenericParameters;
			}
		}

		public override TypeBase EnclosingType {
			get {
				return this.elementType.EnclosingType;
			}
		}

		public override bool IsNested {
			get {
				return false;
			}
		}

		public override bool IsSealed {
			get {
				return true;
			}
		}

		public override bool IsPrimitive {
			get {
				return false;
			}
		}

		public override bool IsInterface {
			get {
				return false;
			}
		}

		public override bool IsAbstract {
			get {
				return false;
			}
		}
		
		public override bool IsEnum {
			get {
				return false;
			}
		}

		public override TypeBase ElementType {
			get {
				return this.elementType;
			}
		}

		public override int FieldCount {
			get {
				return 0;
			}
		}

		public override int MethodCount {
			get {
				return 0;
			}
		}

		public override IEnumerable<FieldBase> EnumFields() {
			return new FieldBase[0];
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			return new MethodBase[0];
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			return new TypeBase[0];
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			return new TypeBase[0];
		}

		public override TypeBase Specialize(TypeBase[] args) {
			throw new NotSupportedException();
		}

		public override TypeBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

	}

}
