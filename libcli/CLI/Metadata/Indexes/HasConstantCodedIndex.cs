using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct HasConstantCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.Field,
			TableId.Param,
			TableId.Property,
		};

		public const int TagBits = 2;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal HasConstantCodedIndex(int value) {
			this.Value = value;
		}

		public TableId TableId {
			get {
				return Mapping[this.Value&TagMask];
			}
		}

		public RowIndex RowIndex {
			get {
				return new RowIndex(this.Value >> TagBits);
			}
		}

		public static int GetPhysicalSize(TablesHeap heap) {
			return heap.GetCodedIndexPhysicalSize(TableId.Field, TableId.Param, TableId.Property);
		}

		public static explicit operator HasConstantCodedIndex(CodedIndex codedIndex) {
			return new HasConstantCodedIndex(codedIndex.Value);
		}

	}

}
