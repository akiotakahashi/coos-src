/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct ModuleRefRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.ModuleRef;

		public int Index;
		public StringHeapIndex Name;

		public int RowIndex {
			get {
				return this.Index;
			}
			set {
				this.Index = value;
			}
		}

	}

	public struct ModuleRefRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal ModuleRefRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator ModuleRefRowIndex(RowIndex rowIndex) {
			return new ModuleRefRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(ModuleRefRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class ModuleRefRowFactory : RowFactory<ModuleRefRow> {

		public ModuleRefRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return ModuleRefRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ Heap.GetPhysicalSizeOfIndex(HeapIndexes.String)
				;
		}

		public override ModuleRefRow Parse(byte[] buf, ref int position) {
			ModuleRefRow value;
			value.Index = 0;
			value.Name = Heap.ReadStringHeapIndex(buf, ref position);
			return value;
		}

	}

}
