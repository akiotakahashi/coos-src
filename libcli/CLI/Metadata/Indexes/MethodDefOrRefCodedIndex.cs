using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct MethodDefOrRefCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.MethodDef,
			TableId.MemberRef,
		};

		public const int TagBits = 1;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal MethodDefOrRefCodedIndex(int value) {
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

		public static explicit operator MethodDefOrRefCodedIndex(CodedIndex codedIndex) {
			return new MethodDefOrRefCodedIndex(codedIndex.Value);
		}

	}

}
