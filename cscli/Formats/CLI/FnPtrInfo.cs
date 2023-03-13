using System;
using CooS.Formats.CLI.Signature;

namespace CooS.Formats.CLI {

	sealed class FnPtrInfo : TypeDeclInfo {

		public readonly MethodSig Signature;

		internal FnPtrInfo(AssemblyDefInfo assembly, MethodSig methodsig) : base(assembly) {
			this.Signature = methodsig;
		}

		private string Suffix {
			get {
				return "*";
			}
		}

		public override int RowIndex {
			get {
				return this.Signature.RowIndex;
			}
		}

		public override string Name {
			get {
				System.Text.StringBuilder buf = new System.Text.StringBuilder("(");
				foreach(ParamSig p in this.Signature.Params) {
					buf.Append(this.Assembly.LookupType(p.Type).Name);
					buf.Append(",");
				}
				if(buf.Length>1) { buf.Length--; }
				buf.Append(")");
				return this.Assembly.LookupType(this.Signature.RetType).Name+"(*)"+buf;
			}
		}

		public override string Namespace {
			get {
				return null;
			}
		}

		public override bool IsNested {
			get {
				return false;
			}
		}

	}

}
