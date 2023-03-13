using System;

namespace CooS.FileSystem.FAT {

	public enum FATAttributes : byte {
		ReadOnly	= 0x01,
		Hidden		= 0x02,
		System		= 0x04,
		VolumeId	= 0x08,
		Directory	= 0x10,
		Archive		= 0x20,
		LongName	= ReadOnly|Hidden|System|VolumeId,
	}

}
