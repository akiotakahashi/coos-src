/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct MemberRefRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.MemberRef;

		public int Index;
		public MemberRefParentCodedIndex Class;
		public StringHeapIndex Name;
		public BlobHeapIndex Signature;

		public int RowIndex {
			get {
				return this.Index;
			}
			set {
				this.Index = value;
			}
		}

	}

	public struct MemberRefRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal MemberRefRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator MemberRefRowIndex(RowIndex rowIndex) {
			return new MemberRefRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(MemberRefRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class MemberRefRowFactory : RowFactory<MemberRefRow> {

		public MemberRefRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return MemberRefRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ Heap.GetPhysicalSizeOfIndex(CodedIndexes.MemberRefParent)
				+ Heap.GetPhysicalSizeOfIndex(HeapIndexes.String)
				+ Heap.GetPhysicalSizeOfIndex(HeapIndexes.Blob)
				;
		}

		public override MemberRefRow Parse(byte[] buf, ref int position) {
			MemberRefRow value;
			value.Index = 0;
			value.Class = (MemberRefParentCodedIndex)Heap.ReadCodedIndex(CodedIndexes.MemberRefParent, buf, ref position);
			value.Name = Heap.ReadStringHeapIndex(buf, ref position);
			value.Signature = Heap.ReadBlobHeapIndex(buf, ref position);
			return value;
		}

	}

}