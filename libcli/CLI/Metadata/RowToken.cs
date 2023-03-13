using System;

namespace CooS.Formats.CLI.Metadata {

	public struct RowToken {

		public readonly uint Value;

		public RowToken(uint value) {
			this.Value = value;
		}

		public RowToken(TableId tableId, RowIndex rowIndex) {
			this.Value = (((uint)tableId)<<24) | (uint)rowIndex.Value;
		}

		public bool IsNotNull {
			get {
				return Value!=0;
			}
		}

		public TableId TableId {
			get {
				return (TableId)(Value >> 24);
			}
		}

		public RowIndex RowIndex {
			get {
				return new RowIndex((int)(Value&0xffffff));
			}
		}

	}

}
