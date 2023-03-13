using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct CustomAttributeTypeCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.INVALID,
			TableId.INVALID,
			TableId.MethodDef,
			TableId.MemberRef,
		};

		public const int TagBits = 2;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal CustomAttributeTypeCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.MethodDef, TableId.MemberRef);
		}

		public static explicit operator CustomAttributeTypeCodedIndex(CodedIndex codedIndex) {
			return new CustomAttributeTypeCodedIndex(codedIndex.Value);
		}

	}

}
