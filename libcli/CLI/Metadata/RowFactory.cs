using System;

namespace CooS.Formats.CLI.Metadata {
	using Heaps;

	public interface IRowFactory {

		int PhysicalSize { get; }

	}

	public abstract class RowFactory<Row> : IRowFactory {

		public readonly TablesHeap Heap;

		protected RowFactory(TablesHeap heap) {
			this.Heap = heap;
		}

		#region IRowFactory ÉÅÉìÉo

		public int PhysicalSize {
			get {
				return this.GetPhysicalSize();
			}
		}

		#endregion

		public abstract TableId TableId { get; }
		public abstract int GetPhysicalSize();
		public abstract Row Parse(byte[] buf, ref int position);

	}

}
