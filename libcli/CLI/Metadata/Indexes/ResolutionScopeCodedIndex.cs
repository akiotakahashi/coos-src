using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct ResolutionScopeCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.Module,
			TableId.ModuleRef,
			TableId.AssemblyRef,
			TableId.TypeRef,
		};

		public const int TagBits = 2;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal ResolutionScopeCodedIndex(int value) {
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
			return heap.GetCodedIndexPhysicalSize(TableId.Module, TableId.ModuleRef, TableId.AssemblyRef, TableId.TypeRef);
		}

		public static explicit operator ResolutionScopeCodedIndex(CodedIndex codedIndex) {
			return new ResolutionScopeCodedIndex(codedIndex.Value);
		}

	}

}
