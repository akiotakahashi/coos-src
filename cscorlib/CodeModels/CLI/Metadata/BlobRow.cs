using System;
using System.IO;
using CooS.IO;

namespace CooS.CodeModels.CLI.Metadata {

	public class BlobRow : Row {

		BlobHeap heap;
		int rowIndex;

		public BlobRow(BlobHeap heap, int rowIndex) {
			this.heap = heap;
			this.rowIndex = rowIndex;
		}

		#region Row ƒƒ“ƒo

		public Table Table {
			get {
				return heap;
			}
		}

		public int Size {
			get {
				throw new NotSupportedException();
			}
		}

		public int Index {
			get {
				return this.rowIndex;
			}
		}

		public void Dump(System.IO.TextWriter writer) {
			Stream stream = this.OpenStream();
			int data;
			while((data=stream.ReadByte())>=0) {
				writer.Write("{0:X2} ", data);
			}
			stream.Close();
		}

		#endregion

		Stream OpenStream() {
			return heap.OpenData(this.rowIndex);
		}

	}

}
