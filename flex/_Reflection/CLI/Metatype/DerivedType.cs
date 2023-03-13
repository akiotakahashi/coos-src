using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Reflection.CLI.Metatype {

	abstract class DerivedType : SuperType {
	
		SuperType elementType;
		
		public DerivedType(AssemblyDef assembly, SuperType elemtype) : base(assembly) {
			this.elementType = elemtype;
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

		protected override bool IsPrimitiveImpl() {
			return false;
		}

		public override Type GetElementType() {
			return this.elementType;
		}

		public override Type DeclaringType {
			get {
				return this.elementType.DeclaringType;
			}
		}

		public override int VariableSize {
			get {
				return IntPtr.Size;
			}
		}

		public override int StaticSize {
			get {
				return 0;
			}
		}

		public override MethodInfoImpl[] ConstructSlots(out InterfaceBase[] ifbases) {
			return ((TypeImpl)this.BaseType).ConstructSlots(out ifbases);
		}

		public override TypeAttributes AttributeFlags {
			get {
				return TypeAttributes.Sealed|(TypeAttributes.VisibilityMask&this.GetElementType().Attributes);
			}
		}

		public override System.Collections.IEnumerable DeclaredConstructors {
			get {
				return ((TypeImpl)this.BaseType).DeclaredConstructors;
			}
		}

		public override System.Collections.IEnumerable DeclaredFields {
			get {
				return ((TypeImpl)this.BaseType).DeclaredFields;
			}
		}

		public override System.Collections.IEnumerable DeclaredMethods {
			get {
				return ((TypeImpl)this.BaseType).DeclaredMethods;
			}
		}

		public override System.Collections.IEnumerable DeclaredPeoperties {
			get {
				return ((TypeImpl)this.BaseType).DeclaredPeoperties;
			}
		}

		public override System.Collections.IEnumerable DeclaredEvents {
			get {
				return ((TypeImpl)this.BaseType).DeclaredEvents;
			}
		}

		public override System.Collections.IEnumerable DeclaredNestedTypes {
			get {
				return ((TypeImpl)this.BaseType).DeclaredNestedTypes;
			}
		}

		public override System.Collections.IEnumerable ImplementedInterfaces {
			get {
				return new TypeImpl[0];
			}
		}

		public abstract string TypeSuffix {get;}

	}

}
