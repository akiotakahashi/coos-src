/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct ClassLayoutRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.ClassLayout;

		public int Index;
		public UInt16 PackingSize;
		public UInt32 ClassSize;
		public TypeDefRowIndex Parent;

		public int RowIndex {
			get {
				return this.Index;
			}
			set {
				this.Index = value;
			}
		}

	}

	public struct ClassLayoutRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal ClassLayoutRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator ClassLayoutRowIndex(RowIndex rowIndex) {
			return new ClassLayoutRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(ClassLayoutRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class ClassLayoutRowFactory : RowFactory<ClassLayoutRow> {

		public ClassLayoutRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return ClassLayoutRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ 2
				+ 4
				+ Heap.GetPhysicalSizeOfIndex(TypeDefRow.TableId)
				;
		}

		public override ClassLayoutRow Parse(byte[] buf, ref int position) {
			ClassLayoutRow value;
			value.Index = 0;
			value.PackingSize = Heap.ReadUInt16(buf, ref position);
			value.ClassSize = Heap.ReadUInt32(buf, ref position);
			value.Parent = (TypeDefRowIndex)Heap.ReadRowIndex(TypeDefRow.TableId, buf, ref position);
			return value;
		}

	}

}
