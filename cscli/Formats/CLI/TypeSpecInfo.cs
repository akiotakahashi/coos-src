using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	sealed class TypeSpecInfo : TypeDeclInfo, IMemberRefParent {

		private TypeSpecRow row;
		private TypeSig signature;

		internal TypeSpecInfo(AssemblyDefInfo assembly, TypeSpecRow row) : base(assembly) {
			this.row = row;
			this.signature = (TypeSig)this.Assembly.LoadSignature(this.row.Signature, TypeSig.Factory);
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		public override string Name {
			get {
				if(this.IsSzArray) {
					return this.Assembly.LookupType(this.signature.Type).Name+"[]";
				} else if(this.IsMnArray) {
					return this.Assembly.LookupType(this.signature.Type).Name+"[~]";
				} else {
					return this.Assembly.LookupType(this.signature).Name;
				}
			}
		}

		public override string Namespace {
			get {
				return this.Assembly.LookupType(this.signature.Type).Namespace;
			}
		}

		public bool IsSzArray {
			get {
				return this.signature.ElementType==ElementType.SzArray;
			}
		}

		public bool IsMnArray {
			get {
				return this.signature.ElementType==ElementType.MnArray;
			}
		}

		public bool IsArray {
			get {
				return this.IsSzArray || this.IsMnArray;
			}
		}

		public TypeSig TypeSig {
			get {
				return this.signature;
			}
		}

	}

}
