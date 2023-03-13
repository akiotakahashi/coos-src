using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.CDFS {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public class LongDateTime {
		
		public uint		year;
		public ushort	month;
		public ushort	day;
		public ushort	hour;
		public ushort	minute;
		public ushort	second;
		public ushort	tick;
		public sbyte	offset;

		public LongDateTime(BinaryReader reader) {
			this.year = reader.ReadUInt32();
			this.month = reader.ReadUInt16();
			this.day = reader.ReadUInt16();
			this.hour = reader.ReadUInt16();
			this.minute = reader.ReadUInt16();
			this.second = reader.ReadUInt16();
			this.tick = reader.ReadUInt16();
			this.offset = reader.ReadSByte();
		}

	}

}
