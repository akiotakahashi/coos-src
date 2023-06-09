/* This file is automatically generated by cliutil.exe */
using System;
using CooS.Formats.CLI.Metadata.Heaps;
using CooS.Formats.CLI.Metadata.Indexes;

namespace CooS.Formats.CLI.Metadata.Rows {

	public struct AssemblyOSRow : IRow {

		public const TableId TableId = CooS.Formats.CLI.Metadata.TableId.AssemblyOS;

		public int Index;
		public UInt32 OSPlatformID;
		public UInt32 OSMajorVersion;
		public UInt32 OSMinorVersion;

		public int RowIndex {
			get {
				return this.Index;
			}
			set {
				this.Index = value;
			}
		}

	}

	public struct AssemblyOSRowIndex : IRowIndex {

		public readonly int Value;

		int IRowIndex.RawIndex {
			get {
				return this.Value;
			}
		}

		internal AssemblyOSRowIndex(int value) {
			this.Value = value;
		}

		public static explicit operator AssemblyOSRowIndex(RowIndex rowIndex) {
			return new AssemblyOSRowIndex(rowIndex.Value);
		}

		public static implicit operator RowIndex(AssemblyOSRowIndex rowIndex) {
			return new RowIndex(rowIndex.Value);
		}

	}

	internal class AssemblyOSRowFactory : RowFactory<AssemblyOSRow> {

		public AssemblyOSRowFactory(TablesHeap heap) : base(heap) {
		}

		public override TableId TableId { get { return AssemblyOSRow.TableId; } }

		public override int GetPhysicalSize() {
			return 0
				+ 4
				+ 4
				+ 4
				;
		}

		public override AssemblyOSRow Parse(byte[] buf, ref int position) {
			AssemblyOSRow value;
			value.Index = 0;
			value.OSPlatformID = Heap.ReadUInt32(buf, ref position);
			value.OSMajorVersion = Heap.ReadUInt32(buf, ref position);
			value.OSMinorVersion = Heap.ReadUInt32(buf, ref position);
			return value;
		}

	}

}
