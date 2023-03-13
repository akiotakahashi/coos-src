using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct HasDeclSecurityCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.TypeDef,
			TableId.MethodDef,
			TableId.Assembly,
		};

		public const int TagBits = 2;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal HasDeclSecurityCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.TypeDef, TableId.MethodDef, TableId.Assembly);
		}

		public static explicit operator HasDeclSecurityCodedIndex(CodedIndex codedIndex) {
			return new HasDeclSecurityCodedIndex(codedIndex.Value);
		}

	}

}
