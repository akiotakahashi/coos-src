using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	sealed class MethodSigInfo : MethodDeclInfo {

		private int rowIndex;
		private BlobHeapIndex signature;

		internal MethodSigInfo(AssemblyDefInfo assembly, StandAloneSigRow row) : base(assembly) {
			this.rowIndex = row.Index;
			this.signature = row.Signature;
		}

		internal MethodSigInfo(AssemblyDefInfo assembly, BlobHeapIndex signature) : base(assembly) {
			this.signature = signature;
		}

		public override int RowIndex {
			get {
				return rowIndex;
			}
		}

		public override string Name {
			get {
				throw new NotSupportedException("MethodSpec doesn't have Name.");
			}
		}

		public override TypeDeclInfo Type {
			get {
				return null;
			}
		}

		private MethodSig _signature;

		internal override MethodSig Signature {
			get {
				if(_signature==null) {
					_signature = (MethodSig)this.Assembly.LoadSignature(this.signature, MethodSig.Factory);
				}
				return _signature;
			}
		}

	}

}
