using System;
using System.IO;
using CooS.CodeModels.CLI.Signature;

namespace CooS.CodeModels.CLI.Metadata {

	public class BlobHeap : Heap, Table {

		public BlobHeap(MetadataRoot mdroot, MDStream stream) : base(mdroot,stream) {
		}

		#region Table ÉÅÉìÉo

		public TablesHeap Heap {
			get {
				throw new NotSupportedException();
			}
		}

		public string Name {
			get {
				return "Blob";
			}
		}

		public CooS.CodeModels.CLI.Metadata.TableId Id {
			get {
				throw new NotSupportedException();
				//return TableId.Blob;
			}
		}

		public int RowCount {
			get {
				return 0;
			}
		}

		public int RowLogicalSize {
			get {
				throw new NotSupportedException();
			}
		}

		public int RowCodedSize {
			get {
				throw new NotSupportedException();
			}
		}

		public Row this[int index] {
			get {
				return new BlobRow(this,index);
			}
		}

		public void Dump(System.IO.TextWriter writer) {
			// TODO:  BlobHeap.Dump é¿ëïÇí«â¡ÇµÇ‹Ç∑ÅB
		}

		#endregion

		public SignatureReader OpenReader(int rowIndex) {
			return new SignatureReader(this.OpenData(rowIndex));
		}

	}

}
