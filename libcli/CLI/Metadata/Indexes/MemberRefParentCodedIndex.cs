using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct MemberRefParentCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.TypeDef,
			TableId.TypeRef,
			TableId.ModuleRef,
			TableId.MethodDef,
			TableId.TypeSpec,
		};

		public const int TagBits = 3;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal MemberRefParentCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.TypeDef, TableId.TypeRef, TableId.ModuleRef, TableId.MethodDef, TableId.TypeSpec);
		}

		public static explicit operator MemberRefParentCodedIndex(CodedIndex codedIndex) {
			return new MemberRefParentCodedIndex(codedIndex.Value);
		}

	}

}
