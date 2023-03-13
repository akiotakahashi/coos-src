using System;

namespace CooS.Formats.CLI {

	class DerivedTypeInfo : TypeDeclInfo {

		public readonly ElementType Kind;
		public readonly TypeDeclInfo BaseType;

		internal DerivedTypeInfo(AssemblyDefInfo assembly, TypeDeclInfo type, ElementType kind)
			: base(assembly) {
			this.Kind = kind;
			this.BaseType = type;
		}

		private string Suffix {
			get {
				switch(this.Kind) {
				case ElementType.ByVal:
					return "*";
				case ElementType.ByRef:
					return "&";
				case ElementType.SzArray:
					return "[]";
				case ElementType.MnArray:
					throw new NotImplementedException();
				default:
					throw new InvalidOperationException();
				}
			}
		}

		public override int RowIndex {
			get {
				throw new NotSupportedException();
			}
		}

		public override string Name {
			get {
				return this.BaseType.Name+Suffix;
			}
		}

		public override string Namespace {
			get {
				return this.BaseType.Namespace;
			}
		}

		public override bool IsNested {
			get {
				return false;
			}
		}

	}

}
