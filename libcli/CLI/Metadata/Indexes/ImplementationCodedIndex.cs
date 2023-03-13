using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct ImplementationCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.File,
			TableId.AssemblyRef,
			TableId.ExportedType,
		};

		public const int TagBits = 2;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal ImplementationCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.File, TableId.AssemblyRef, TableId.ExportedType);
		}

		public static explicit operator ImplementationCodedIndex(CodedIndex codedIndex) {
			return new ImplementationCodedIndex(codedIndex.Value);
		}

	}

}
