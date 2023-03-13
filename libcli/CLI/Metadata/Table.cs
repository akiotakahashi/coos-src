using System;
using System.Collections.Generic;

namespace CooS.Formats.CLI.Metadata {
	using Heaps;

	public interface ITable : System.Collections.IEnumerable {

		TableId TableId { get; }
		int RowCount { set; get; }
		IRowFactory RowFactory { get; }

		byte[] RowData { set; }
		long StartPosition { set; }

	}

	public class Table<Row,Index> : ITable, IEnumerable<Row> where Row : struct, IRow where Index : struct, IRowIndex {

		private readonly RowFactory<Row> factory;
		private readonly TablesHeap heap;
		private readonly int rowSize;

		private Row?[] rows;
		private int? rowCount;
		private byte[] rowData;
		private long? startPosition;

		public Table(RowFactory<Row> factory) {
			this.factory = factory;
			this.heap = factory.Heap;
			this.rowSize = this.factory.PhysicalSize;
		}

		private Row ReadRow(int index) {
			if(rows[index].HasValue) {
				return rows[index].Value;
			} else {
				int p0 = this.rowSize*(int)index;
				int pos = p0;
				Row row = factory.Parse(this.rowData, ref pos);
				row.RowIndex = index+1;
#if DEBUG
				if((pos-p0)!=this.rowSize) {
					throw new InvalidOperationException("read size wrong: "+(pos-p0-this.rowSize));
				}
#endif
				rows[index] = row; // needed for nullable
				return row;
			}
		}

		public Row this[Index rowIndex] {
			get {
				if(rowIndex.RawIndex==0) { throw new ArgumentNullException(); }
				return this.ReadRow(rowIndex.RawIndex);
			}
		}

		public Row this[RowIndex rowIndex] {
			get {
				if(rowIndex.Value==0) { throw new ArgumentNullException(); }
				return this.ReadRow(rowIndex.Value);
			}
		}

		#region ITable ÉÅÉìÉo

		public TableId TableId {
			get {
				return this.factory.TableId;
			}
		}

		public int RowCount {
			get {
				return this.rowCount.Value;
			}
			set {
				this.rowCount = value;
				this.rows = new Row?[value];
			}
		}

		public IRowFactory RowFactory {
			get {
				return this.factory;
			}
		}

		public byte[] RowData {
			set {
				this.rowData = value;
			}
		}

		public long StartPosition {
			set {
				this.startPosition = value;
			}
		}

		#endregion

		#region IEnumerable<Row> ÉÅÉìÉo

		public IEnumerator<Row> GetEnumerator() {
			for(int i=0; i<this.RowCount; ++i) {
				yield return this.ReadRow(i);
			}
		}

		#endregion

		#region IEnumerable ÉÅÉìÉo

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			for(int i=0; i<this.RowCount; ++i) {
				yield return this.ReadRow(i);
			}
		}

		#endregion

	}

}
