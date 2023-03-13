/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct ParamRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.Param;

		public int Index;
		public ParamAttributes Flags;
		public UInt16 Sequence;
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

	public struct ParamRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal ParamRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator ParamRowIndex(RowIndex rowIndex) {
			return new ParamRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(ParamRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class ParamRowFactory : RowFactory<ParamRow> {

		public ParamRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return ParamRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ 2
				+ 2
				+ Heap.GetPhysicalSizeOfIndex(HeapIndexes.String)
				;
		}

		public override ParamRow Parse(byte[] buf, ref int position) {
			ParamRow value;
			value.Index = 0;
			value.Flags = (ParamAttributes)Heap.ReadEnum16(buf, ref position);
			value.Sequence = Heap.ReadUInt16(buf, ref position);
			value.Name = Heap.ReadStringHeapIndex(buf, ref position);
			return value;
		}

	}

}