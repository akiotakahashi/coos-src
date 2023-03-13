using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.CDFS {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct ShortDateTime {

		public byte		year;
		public byte		month;
		public byte		day;
		public byte		hour;
		public byte		minute;
		public byte		second;
		public sbyte	offset;

		public ShortDateTime(BinaryReader reader) {
			this.year = reader.ReadByte();
			this.month = reader.ReadByte();
			this.day = reader.ReadByte();
			this.hour = reader.ReadByte();
			this.minute = reader.ReadByte();
			this.second = reader.ReadByte();
			this.offset = reader.ReadSByte();
		}

		public DateTime ToDateTime() {
			int year = this.year+1900;
			if(year<1980) year+=100;
			return new DateTime(year,month,day,hour,minute,second);
		}

	}

}
