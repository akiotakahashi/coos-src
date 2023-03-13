using System;
using System.Text;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.FAT {

	// BIOS Parameter Block
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	struct BPB {
		public byte		jmpBoot0;
		public byte		jmpBoot1;
		public byte		jmpBoot2;
		public ulong	oemName;
		public ushort	bytesPerSector;
		public byte		sectorsPerCluster;
		public ushort	numberOfReservedSectors;
		public byte		numberOfFats;
		public ushort	numberOfRootDirEntries;
		public ushort	numberOfSectors16;
		public byte		media;
		public ushort	sectorsPerFat16;
		public ushort	sectorsPerTrack;
		public ushort	numberOfHeads;
		public uint		numberOfHiddenSectors;
		public uint		numberOfSectors32;
		public override string ToString() {
			StringBuilder buf = new StringBuilder();
			string format = "{0,-24}: {1}"+Environment.NewLine;
			buf.AppendFormat(format, "Bytes per Sector", this.bytesPerSector);
			buf.AppendFormat(format, "Sectors per Cluster", this.sectorsPerCluster);
			buf.AppendFormat(format, "Number of Reserved Sec", this.numberOfReservedSectors);
			buf.AppendFormat(format, "Number of FATs", this.numberOfFats);
			buf.AppendFormat(format, "Number of RootDirEnt", this.numberOfRootDirEntries);
			buf.AppendFormat(format, "Number of Sectors16", this.numberOfSectors16);
			buf.AppendFormat(format, "Sectors per FAT", this.sectorsPerFat16);
			buf.AppendFormat(format, "Sectors per Track", this.sectorsPerTrack);
			buf.AppendFormat(format, "Number of Heads", this.numberOfHeads);
			buf.AppendFormat(format, "Number of HiddenSec", this.numberOfHiddenSectors);
			buf.AppendFormat(format, "Number of Sectors32", this.numberOfSectors32);
			return buf.ToString();
		}

	}

}
