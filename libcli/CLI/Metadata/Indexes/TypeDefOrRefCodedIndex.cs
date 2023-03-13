using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct TypeDefOrRefCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.TypeDef,
			TableId.TypeRef,
			TableId.TypeSpec,
		};

		public const int TagBits = 2;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal TypeDefOrRefCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.TypeDef, TableId.TypeRef, TableId.TypeSpec);
		}

		public static explicit operator TypeDefOrRefCodedIndex(CodedIndex codedIndex) {
			return new TypeDefOrRefCodedIndex(codedIndex.Value);
		}

	}

}
