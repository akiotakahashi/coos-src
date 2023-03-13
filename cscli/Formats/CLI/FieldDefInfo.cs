using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	sealed class FieldDefInfo : FieldDeclInfo {

		readonly FieldRow row;
		readonly FieldSig sig;
		readonly TypeDeclInfo fieldtype;
		string name;

		internal FieldDefInfo(AssemblyDefInfo assembly, FieldRow row) : base(assembly) {
			this.row = row;
			this.sig = (FieldSig)this.Assembly.LoadSignature(this.row.Signature, FieldSig.Factory);
			this.fieldtype = this.Assembly.LookupType(this.sig.Type);
		}

		public override int RowIndex {
			get {
				return row.Index;
			}
		}

		public int Index {
			get {
				return this.Assembly.SearchTypeOfField(this.RowIndex).GetFieldIndex(this.RowIndex);
			}
		}

		public override string Name {
			get {
				if(name==null) {
					name = this.Assembly.LoadBlobString(row.Name);
				}
				return name;
			}
		}

		public TypeDefInfo TypeDef {
			get {
				return this.Assembly.SearchTypeOfField(this.RowIndex);
			}
		}

		public override TypeDeclInfo Type {
			get {
				return this.Assembly.SearchTypeOfField(this.RowIndex);
			}
		}

		public FieldAttributes Attributes {
			get {
				return row.Flags;
			}
		}

		public TypeDeclInfo FieldType {
			get {
				return this.fieldtype;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return this.FieldType.IsGenericType;
			}
		}

	}

}
