using System;

namespace CooS.FileSystem.FAT {

	public struct FATTime {

		public ushort Value;

		public int Hour {
			get {
				return Value>>11;
			}
		}

		public int Minute {
			get {
				return (this.Value>>5)&0x3F;
			}
		}

		public int Second {
			get {
				return (this.Value&0x1F)*2;
			}
		}

		public TimeSpan GetTimeSpan() {
			return new TimeSpan(this.Hour, this.Minute, this.Second);
		}

	}

}
