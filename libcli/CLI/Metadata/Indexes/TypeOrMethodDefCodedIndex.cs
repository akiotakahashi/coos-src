using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct TypeOrMethodDefCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.TypeDef,
			TableId.MethodDef,
		};

		public const int TagBits = 1;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal TypeOrMethodDefCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.TypeDef, TableId.MethodDef);
		}

		public static explicit operator TypeOrMethodDefCodedIndex(CodedIndex codedIndex) {
			return new TypeOrMethodDefCodedIndex(codedIndex.Value);
		}

	}

}
