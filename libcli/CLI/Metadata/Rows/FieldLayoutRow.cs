/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct FieldLayoutRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.FieldLayout;

		public int Index;
		public UInt32 Offset;
		public FieldRowIndex Field;

		public int RowIndex {
			get {
				return this.Index;
			}
			set {
				this.Index = value;
			}
		}

	}

	public struct FieldLayoutRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal FieldLayoutRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator FieldLayoutRowIndex(RowIndex rowIndex) {
			return new FieldLayoutRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(FieldLayoutRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class FieldLayoutRowFactory : RowFactory<FieldLayoutRow> {

		public FieldLayoutRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return FieldLayoutRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ 4
				+ Heap.GetPhysicalSizeOfIndex(FieldRow.TableId)
				;
		}

		public override FieldLayoutRow Parse(byte[] buf, ref int position) {
			FieldLayoutRow value;
			value.Index = 0;
			value.Offset = Heap.ReadUInt32(buf, ref position);
			value.Field = (FieldRowIndex)Heap.ReadRowIndex(FieldRow.TableId, buf, ref position);
			return value;
		}

	}

}
