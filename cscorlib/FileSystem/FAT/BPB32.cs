using System;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.FAT {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	struct BPB32 {
		// for FAT32
		public uint		sectorsPerFat32;
		public ushort	extFlags;
		public ushort	fsVersion;
		public uint		rootCluster;
		public ushort	fsInfo;
		public ushort	backupBootSector;
		public int		reserved0;
		public int		reserved1;
		public int		reserved2;
		public byte		driveNumber;
		public byte		reserved3;
		public byte		bootSignature;
		public uint		volumeId;
		public byte		volumeLabel0;
		public byte		volumeLabel1;
		public byte		volumeLabel2;
		public byte		volumeLabel3;
		public byte		volumeLabel4;
		public byte		volumeLabel5;
		public byte		volumeLabel6;
		public byte		volumeLabel7;
		public byte		volumeLabel8;
		public byte		volumeLabel9;
		public byte		volumeLabel10;
		public byte		fileSystemType0;
		public byte		fileSystemType1;
		public byte		fileSystemType2;
		public byte		fileSystemType3;
		public byte		fileSystemType4;
		public byte		fileSystemType5;
		public byte		fileSystemType6;
		public byte		fileSystemType7;
	}

}
