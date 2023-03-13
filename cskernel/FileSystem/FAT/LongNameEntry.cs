using System;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.FAT {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	struct LongNameEntry {
		public byte		seq;				// sequence number for slot 
		public char		name0;				// first 5 characters in name 
		public char		name1;				// first 5 characters in name 
		public char		name2;				// first 5 characters in name 
		public char		name3;				// first 5 characters in name 
		public char		name4;				// first 5 characters in name 
		public byte		attributes;			// attribute byte
		public byte		NTReserved;			// always 0 
		public byte		alias_checksum;		// checksum for 8.3 alias 
		public char		name5;				// 6 more characters in name
		public char		name6;				// 6 more characters in name
		public char		name7;				// 6 more characters in name
		public char		name8;				// 6 more characters in name
		public char		name9;				// 6 more characters in name
		public char		name10;				// 6 more characters in name
		public ushort	firstClusterL;		// starting cluster number
		public char		name11;				// last 2 characters in name
		public char		name12;				// last 2 characters in name
	}

}
