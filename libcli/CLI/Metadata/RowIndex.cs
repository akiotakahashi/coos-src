using System;

namespace CooS.Formats.CLI.Metadata {

	public struct RowIndex {

		public readonly int Value;

		internal RowIndex(int value) {
			this.Value = value;
		}

		public bool IsInvalid {
			get {
				return this.Value<0;
			}
		}

		public bool IsNull {
			get {
				return this.Value==0;
			}
		}

		public bool IsNotNull {
			get {
				return this.Value>0;
			}
		}

		public static implicit operator int(RowIndex rowIndex) {
			return rowIndex.Value-1;
		}

		public static implicit operator RowIndex(int rawIndex) {
			return new RowIndex(rawIndex+1);
		}

	}

	public interface IRowIndex {

		int RawIndex { get; }

	}

	public interface IRow {

		int RowIndex { get; set; }

	}

}
