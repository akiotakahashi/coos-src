using System;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	public class FATArchive : ArchiveBase {

		Partition partition;

		readonly FATTypes FATType;
		readonly BPB bpb;
		readonly BPB16 bpb16;
		readonly BPB32 bpb32;
		readonly byte[] fatbuf;
	
		uint sectorsPerFat;
		uint numberOfSectors;
		uint FirstRootDirSectorNum;	// The start of Root Directory Entry
		uint RootDirSectors;		// The count of sectors occupied by the root directory
		uint FirstDataSector;		// The start of the data region (i.e. the first sector of cluster 2)
		uint DataSectors;
		uint CountOfClusters;

		public unsafe FATArchive(Partition partition) {
			this.partition = partition;
			byte[] bpbbuf = new byte[partition.BlockSize];
			this.partition.Read(bpbbuf, 0, 0, 1);
			fixed(byte* p0 = &bpbbuf[0]) {
				bpb = *(BPB*)p0;
				byte* p = p0+sizeof(BPB);
				bpb16 = *(BPB16*)p;
				bpb32 = *(BPB32*)p;
				FATType = Initialize();
			}
			fatbuf = new byte[bpb.bytesPerSector*sectorsPerFat];
			this.partition.Read(fatbuf, 0, bpb.numberOfReservedSectors, (int)sectorsPerFat);
		}

		private FATTypes Initialize() {
			// Fill appendix fields
			if(bpb.sectorsPerFat16>0) {
				sectorsPerFat = bpb.sectorsPerFat16;
			} else {
				sectorsPerFat = bpb32.sectorsPerFat32;
			}
			if(bpb.numberOfSectors16>0) {
				numberOfSectors = bpb.numberOfSectors16;
			} else {
				numberOfSectors = bpb.numberOfSectors32;
			}
			// Parameters
			FirstRootDirSectorNum = bpb.numberOfReservedSectors + (bpb.numberOfFats*sectorsPerFat);
			RootDirSectors = (uint)((((uint)bpb.numberOfRootDirEntries*32)+(bpb.bytesPerSector-1))/bpb.bytesPerSector);
			FirstDataSector = bpb.numberOfReservedSectors + (bpb.numberOfFats*sectorsPerFat) + RootDirSectors;
			DataSectors = numberOfSectors-(bpb.numberOfReservedSectors+(bpb.numberOfFats*sectorsPerFat)+RootDirSectors);
			CountOfClusters = DataSectors/bpb.sectorsPerCluster;
			// FileSystem Type
			if(CountOfClusters<4085) {
				return FATTypes.FAT12;
			} else if(CountOfClusters<65525) {
				return FATTypes.FAT16;
			} else {
				return FATTypes.FAT32;
			}
		}
		
		public uint BytesPerSector {
			get {
				return bpb.bytesPerSector;
			}
		}

		public uint SectorsPerCluster {
			get {
				return bpb.sectorsPerCluster;
			}
		}
		
		public int FirstSectorOfRootDir {
			get {
				switch(FATType) {
				case FATTypes.FAT12:
				case FATTypes.FAT16:
					return (int)(FirstDataSector-RootDirSectors);
				case FATTypes.FAT32:
					return (int)GetFirstSectorOfCluster(bpb32.rootCluster);
				default:
					throw new UnexpectedException();
				}
			}
		}

		public long GetFirstSectorOfCluster(long N) {
			// FirstSectorofCluster: the sector number of the first sector of that cluster
			return ((N-2)*bpb.sectorsPerCluster)+FirstDataSector;
		}

		/// <summary>
		/// クラスタNのFileSystemデータの先頭からオフセットを返します。
		/// </summary>
		/// <param name="N">クラスタ番号</param>
		/// <returns></returns>
		public uint GetFATOffset(uint N) {
			switch(FATType) {
			case FATTypes.FAT12:
				return N+(N/2);
			case FATTypes.FAT16:
				return N*2;
			case FATTypes.FAT32:
				return N*4;
			default:
				throw new UnexpectedException();
			}
		}

		/// <summary>
		/// FileSystemの先頭からFATOffsetバイト離れた位置のエントリを含む（当然FileSystem内の）セクタ番号を返します。
		/// </summary>
		/// <param name="FATOffset"></param>
		/// <returns></returns>
		public uint GetThisFATSectorNumber(uint FATOffset) {
			// If you want the sector number in the second FileSystem, you add FATSz to ThisFATSectorNumber;
			// for the third FileSystem, you add 2*FATSz, and so on.
			return bpb.numberOfReservedSectors+(FATOffset/bpb.bytesPerSector);
		}

		public uint GetThisFATEntryOffset(uint N) {
			//if(ThisFATEntOffset == (BPB_BytsPerSec-1)) {
			/* This cluster access spans a sector boundary in the FileSystem */
			/* There are a number of strategies to handling this. The */
			/* easiest is to always load FileSystem sectors into memory */
			/* in pairs if the volume is FAT12 (if you want to load */
			/* FileSystem sector N, you also load FileSystem sector N+1 immediately */
			/* following it in memory unless sector N is the last FileSystem */
			/* sector). It is assumed that this is the strategy used here */
			/* which makes this if test for a sector boundary span */
			/* unnecessary. */
			//}
			return GetFATOffset(N) % bpb.bytesPerSector;
		}
		
		public uint GetFATClusterEntryValue(uint N, byte[] SecBuff, int ThisFATEntOffset) {
			switch(FATType) {
			case FATTypes.FAT12:
				ushort fat12value = BitConverter.ToUInt16(SecBuff, ThisFATEntOffset);
				if(0!=(N&0x0001)) {
					return (uint)(fat12value >> 4); /* Cluster number is ODD */
				} else {
					return (uint)(fat12value & 0x0FFF); /* Cluster number is EVEN */
				}
			case FATTypes.FAT16:
				return BitConverter.ToUInt16(SecBuff, ThisFATEntOffset);
			case FATTypes.FAT32:
				return BitConverter.ToUInt32(SecBuff, ThisFATEntOffset) & 0x0FFFFFFF;
			default:
				throw new UnexpectedException();
			}
		}
		
		/*
		public void SetFATClusterEntryValue(uint N, byte[] SecBuff, uint ThisFATEntOffset, uint value) {
			switch(FATType) {
			case FATTypes.FAT12:
				uint16& fat12value = *(uint16*)&SecBuff[ThisFATEntOffset];
				if(N & 0x0001) {
					value <<= 4;			// Cluster number is ODD
					fat12value &= 0x000F;
				} else {
					fat12value &= 0xF000;	// Cluster number is EVEN
				}
				fat12value |= value & 0x0FFF;
				return;
			case FATTypes.FAT16:
				*((uint16*)&SecBuff[ThisFATEntOffset]) = value;
				return;
			case FATTypes.FAT32:
				{
				uint32& data = *((uint32*)&SecBuff[ThisFATEntOffset]);
				data = (data&0xF0000000) | (value&0x0FFFFFFF);
				return;
				}
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
		*/
		
		// Is FATContent End Of Clusterchain?
		public bool IsEOC(uint FATContent) {
			switch(FATType) {
			case FATTypes.FAT12:
				return FATContent>=0x0FF8;
			case FATTypes.FAT16:
				return FATContent>=0xFFF8;
			case FATTypes.FAT32:
				return FATContent>=0x0FFFFFF8;
			default:
				throw new UnexpectedException();
			}
		}
		
		public uint GetFATEntry(uint N) {
			return GetFATClusterEntryValue(N, fatbuf, (int)GetFATOffset(N));
		}
		
		public int ReadRootDirectoryData(byte[] buf, uint size) {
			switch(FATType) {
			case FATTypes.FAT12:
			case FATTypes.FAT16:
				int totalsize = bpb.numberOfRootDirEntries*32/*sizeof(DirectoryEntry)*/;
				if(buf==null) return totalsize;
				if(size<totalsize) {
					throw new ArgumentOutOfRangeException();
				} else {
					int count = totalsize/this.partition.BlockSize;
					this.partition.Read(buf, 0, FirstRootDirSectorNum, count);
					return count*this.partition.BlockSize;
				}
			case FATTypes.FAT32:
				throw new NotImplementedException();
			default:
				throw new UnexpectedException();
			}
		}

		public int ReadCluster(byte[] buf, int index, long cluster) {
			this.partition.Read(buf, index, GetFirstSectorOfCluster(cluster), bpb.sectorsPerCluster);
			return bpb.bytesPerSector*bpb.sectorsPerCluster;
		}

		internal int ClusterSize {
			get {
				return bpb.bytesPerSector*bpb.sectorsPerCluster;
			}
		}

		protected override Book OpenRootBook() {
			return new FATRootDirectory(this);
		}

	}

}
