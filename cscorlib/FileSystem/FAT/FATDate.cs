using System;

namespace CooS.FileSystem.FAT {

	public struct FATDate {

		public ushort Value;

		public int Year {
			get {
				return 1980+(this.Value>>9);
			}
		}

		public int Month {
			get {
				return (this.Value>>5)&0xF;
			}
		}

		public int Day {
			get {
				return this.Value & 0x1F;
			}
		}

		public DateTime GetDateTime() {
			return new DateTime(this.Year, this.Month, this.Day);
		}

	}

}
