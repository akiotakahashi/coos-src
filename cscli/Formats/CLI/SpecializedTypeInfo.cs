using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;

	class SpecializedTypeInfo : TypeDeclInfo {

		private readonly TypeDeclInfo master;
		private readonly TypeSig[] args;

		internal SpecializedTypeInfo(TypeDeclInfo master, TypeSig[] args) : base(master.Assembly) {
			this.master = master;
			this.args = args;
		}

		public override int RowIndex {
			get {
				throw new NotSupportedException();
			}
		}

		public override string Name {
			get {
				return this.master.Name;
			}
		}

		public override string Namespace {
			get {
				return this.master.Namespace;
			}
		}

		public override bool IsNested {
			get {
				return this.master.IsNested;
			}
		}

		public TypeDeclInfo GenericType {
			get {
				return this.master;
			}
		}

		public override TypeSig[] GenericArguments {
			get {
				return this.args;
			}
		}

	}

}
