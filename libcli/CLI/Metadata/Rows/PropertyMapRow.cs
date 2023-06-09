/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct PropertyMapRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.PropertyMap;

		public int Index;
		public TypeDefRowIndex Parent;
		public PropertyRowIndex PropertyList;

		public int RowIndex {
			get {
				return this.Index;
			}
			set {
				this.Index = value;
			}
		}

	}

	public struct PropertyMapRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal PropertyMapRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator PropertyMapRowIndex(RowIndex rowIndex) {
			return new PropertyMapRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(PropertyMapRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class PropertyMapRowFactory : RowFactory<PropertyMapRow> {

		public PropertyMapRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return PropertyMapRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ Heap.GetPhysicalSizeOfIndex(TypeDefRow.TableId)
				+ Heap.GetPhysicalSizeOfIndex(PropertyRow.TableId)
				;
		}

		public override PropertyMapRow Parse(byte[] buf, ref int position) {
			PropertyMapRow value;
			value.Index = 0;
			value.Parent = (TypeDefRowIndex)Heap.ReadRowIndex(TypeDefRow.TableId, buf, ref position);
			value.PropertyList = (PropertyRowIndex)Heap.ReadRowIndex(PropertyRow.TableId, buf, ref position);
			return value;
		}

	}

}
