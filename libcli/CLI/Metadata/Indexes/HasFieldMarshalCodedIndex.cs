using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct HasFieldMarshalCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.Field,
			TableId.Param,
		};

		public const int TagBits = 1;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal HasFieldMarshalCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.Field, TableId.Param);
		}

		public static explicit operator HasFieldMarshalCodedIndex(CodedIndex codedIndex) {
			return new HasFieldMarshalCodedIndex(codedIndex.Value);
		}

	}

}
