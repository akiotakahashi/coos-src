using System;
using System.IO;
using System.Runtime.InteropServices;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct DirectoryEntry {

		public byte		name0;
		public byte		name1;
		public byte		name2;
		public byte		name3;
		public byte		name4;
		public byte		name5;
		public byte		name6;
		public byte		name7;
		public byte		ext0;
		public byte		ext1;
		public byte		ext2;
		public FATAttributes attributes;
		public byte		NTReserved;
		public byte		createTimeTenth;
		public FATTime	createTime;
		public FATDate	createDate;
		public FATDate	lastAccessDate;
		public ushort	firstClusterH;
		public FATTime	writeTime;
		public FATDate	writeDate;
		public ushort	firstClusterL;
		public uint		fileSize;

		public uint FirstCluster {
			get {
				return firstClusterL | (((uint)firstClusterH)<<16);
			}
		}

		public BookInfo ConvertToBookInfo(string name) {
			FileAttributes attr = 0;
			if(0!=(attributes&FATAttributes.Archive)) attr |= FileAttributes.Archive;
			if(0!=(attributes&FATAttributes.Directory)) attr |= FileAttributes.Directory;
			if(0!=(attributes&FATAttributes.Hidden)) attr |= FileAttributes.Hidden;
			if(0!=(attributes&FATAttributes.ReadOnly)) attr |= FileAttributes.ReadOnly;
			if(0!=(attributes&FATAttributes.System)) attr |= FileAttributes.System;
			BookInfo bi = new BookInfo(name,attr,this.fileSize,
				this.createDate.GetDateTime().Add(this.createTime.GetTimeSpan()).AddSeconds(createTimeTenth/10.0),
				this.writeDate.GetDateTime().Add(this.writeTime.GetTimeSpan()),
				this.lastAccessDate.GetDateTime());
			return bi;
		}

	}

}
