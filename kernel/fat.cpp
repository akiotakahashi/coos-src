#include "kernel.h"
#include "memory.h"
#include "stdlib.h"
#include "fdd.h"
#include "console.h"


namespace FAT {

#include "enalign.h"
#include "enpack.h"

	// BIOS Parameter Block
	struct BPB {
		byte jmpBoot[3];
		char oemName[8];
		uint16	bytesPerSector;
		uint8	sectorsPerCluster;
		uint16	numberOfReservedSectors;
		uint8	numberOfFats;
		uint16	numberOfRootDirEntries;
		uint16	numberOfSectors16;
		uint8	media;
		uint16	sectorsPerFat16;
		uint16	sectorsPerTrack;
		uint16	numberOfHeads;
		uint32	numberOfHiddenSectors;
		uint32	numberOfSectors32;
		union {
			struct {
				// for FAT12 and FAT16
				uint8	driveNumber;
				uint8	reserved1;
				uint8	bootSignature;
				uint32	volumeId;
				char	volumeLabel[11];
				char	fileSystemType[8];
			} fat16;
			struct {
				// for FAT32
				uint32	sectorsPerFat32;
				uint16	extFlags;
				uint16	fsVersion;
				uint32	rootCluster;
				uint16	fsInfo;
				uint16	backupBootSector;
				byte	reserved[12];
				uint8	driveNumber;
				uint8	reserved1;
				uint8	bootSignature;
				uint32	volumeId;
				char	volumeLabel[11];
				char	fileSystemType[8];
			} fat32;
		};
		// for convinience of computation about FAT12 and FAT16
		uint32	sectorsPerFat;
		uint32	numberOfSectors;
	};

	struct DirectoryEntry {
		char	name[8];
		char	ext[3];
		byte	attributes;
		byte	NTReserved;
		byte	createTimeTenth;
		uint16	createTime;
		uint16	createDate;
		uint16	lastAccessDate;
		uint16	firstClusterH;
		uint16	writeTime;
		uint16	writeDate;
		uint16	firstClusterL;
		uint32	fileSize;
		uint32 getFirstCluster() const {
			return firstClusterL | (((uint32)firstClusterH)<<16);
		}
	};

	struct LongNameEntry {
		byte	seq;				// sequence number for slot 
		wchar_t	name0_4[5];			// first 5 characters in name 
		byte	attributes;			// attribute byte
		byte	NTReserved;			// always 0 
		byte	alias_checksum;		// checksum for 8.3 alias 
		wchar_t	name5_10[6];		// 6 more characters in name
		uint16	firstClusterL;		// starting cluster number
		wchar_t	name11_12[2];		// last 2 characters in name
	};

#include "unpack.h"
#include "unalign.h"


	class FileSystem;

	class FATRootDirectory : public IDirectory {
		FileSystem& fat;
		std::wstring name;
	public:
		FATRootDirectory(FileSystem& _fat) : fat(_fat) {
			name = L"";
		}
	public:
		virtual IDirectory* getParent() {
			return NULL;
		}
		virtual const std::wstring& getName() const {
			return name;
		}
		virtual void getDirectories(DirectoryList& dirlist) {
			//TODO: Implements
		}
		virtual void getFiles(FileList& filelist);
	};

	class FATFile : public IStream {
		FileSystem& fat;
		std::wstring name;
		DirectoryEntry entry;
		std::vector<uint> clusterchain;
		uint position;
	public:
		FATFile(FileSystem& _fat, wchar_t* _name, const DirectoryEntry& _entry);
	public:
		virtual IDirectory* getParent() {
			return NULL;
		}
		virtual const std::wstring& getName() const {
			return name;
		}
		virtual uint getPosition() const {
			return position;
		}
		virtual uint getLength() const {
			return entry.fileSize;
		}
		virtual uint getAttributes() const {
			return entry.attributes;
		}
		virtual void Seek(uint _position) {
			position = _position;
		}
		virtual uint Read(byte* buf, uint size);
	};

	class FileSystem : public IFileSystem {
		IMedia* dev;
		int bufsize;
		byte* fatbuf;
		BPB bpb;
		uint FirstRootDirSectorNum;	// The start of Root Directory Entry
		uint RootDirSectors;		// The count of sectors occupied by the root directory
		uint FirstDataSector;		// The start of the data region (i.e. the first sector of cluster 2)
		uint DataSectors;
		uint CountOfClusters;
		enum {
			FATTYPE_FAT12,
			FATTYPE_FAT16,
			FATTYPE_FAT32,
		} FATType;
	private:
		void Initialize(const BPB& _bpb) {
			bpb = _bpb;
			// Fill appendix fields
			if(bpb.sectorsPerFat16>0) {
				bpb.sectorsPerFat = bpb.sectorsPerFat16;
			} else {
				bpb.sectorsPerFat = bpb.fat32.sectorsPerFat32;
			}
			if(bpb.numberOfSectors16>0) {
				bpb.numberOfSectors = bpb.numberOfSectors16;
			} else {
				bpb.numberOfSectors = bpb.numberOfSectors32;
			}
			// Parameters
			FirstRootDirSectorNum = bpb.numberOfReservedSectors + (bpb.numberOfFats*bpb.sectorsPerFat);
			RootDirSectors = (((uint)bpb.numberOfRootDirEntries*32)+(bpb.bytesPerSector-1))/bpb.bytesPerSector;
			FirstDataSector = bpb.numberOfReservedSectors + (bpb.numberOfFats*bpb.sectorsPerFat) + RootDirSectors;
			DataSectors = bpb.numberOfSectors-(bpb.numberOfReservedSectors+(bpb.numberOfFats*bpb.sectorsPerFat)+RootDirSectors);
			CountOfClusters = DataSectors/bpb.sectorsPerCluster;
			// FileSystem Type
			if(CountOfClusters<4085) {
				FATType = FATTYPE_FAT12;
			} else if(CountOfClusters<65525) {
				FATType = FATTYPE_FAT16;
			} else {
				FATType = FATTYPE_FAT32;
			}
			//
		}
	public:
		FileSystem(IMedia* _dev) : dev(_dev) {
			bufsize = dev->getBlockSize();
			fatbuf = new byte[bufsize];
			dev->Seek(0);
			dev->Read(fatbuf, 1);
			Initialize(*(BPB*)fatbuf);
			delete [] fatbuf;
			bufsize = bpb.bytesPerSector*bpb.sectorsPerFat;
			fatbuf = new byte[bufsize];
			dev->Seek(bpb.numberOfReservedSectors);
			dev->Read(fatbuf, bpb.sectorsPerFat);
		}
		~FileSystem() {
			delete [] fatbuf;
			getConsole() << "~FAT" << endl;
		}
	public:
		uint getBytesPerSector() const {
			return bpb.bytesPerSector;
		}
		uint getSectorsPerCluster() const {
			return bpb.sectorsPerCluster;
		}
	public:
		uint getFirstSectorOfRootDir() const {
			switch(FATType) {
			case FATTYPE_FAT12:
			case FATTYPE_FAT16:
				return FirstDataSector-RootDirSectors;
			case FATTYPE_FAT32:
				return getFirstSectorOfCluster(bpb.fat32.rootCluster);
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
		// FirstSectorofCluster: the sector number of the first sector of that cluster
		uint getFirstSectorOfCluster(uint N) const {
			return ((N-2)*bpb.sectorsPerCluster)+FirstDataSector;
		}
		/*
			getFATOffsetはクラスタNのFileSystemデータの先頭からオフセットを返します。
		*/
		uint getFATOffset(uint N) const {
			switch(FATType) {
			case FATTYPE_FAT12:
				return N+(N/2);
			case FATTYPE_FAT16:
				return N*2;
			case FATTYPE_FAT32:
				return N*4;
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
		/*
			getThisFATSecNumはFileSystemの先頭からFATOffsetバイト離れた位置のエントリを含む（当然FileSystem内の）セクタ番号を返します。
		*/
		uint getThisFATSectorNumber(uint FATOffset) const {
			// If you want the sector number in the second FileSystem, you add FATSz to ThisFATSectorNumber;
			// for the third FileSystem, you add 2*FATSz, and so on.
			return bpb.numberOfReservedSectors+(FATOffset/bpb.bytesPerSector);
		}
		uint getThisFATEntryOffset(uint N) const {
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
			return getFATOffset(N) % bpb.bytesPerSector;
		}
	public:
		uint32 getFATClusterEntryValue(uint N, const byte* SecBuff, uint ThisFATEntOffset) const {
			switch(FATType) {
			case FATTYPE_FAT12:
				uint16 fat12value;
				fat12value = *(uint16*)&SecBuff[ThisFATEntOffset];
				if(N & 0x0001) {
					return fat12value >> 4; /* Cluster number is ODD */
				} else {
					return fat12value & 0x0FFF; /* Cluster number is EVEN */
				}
			case FATTYPE_FAT16:
				return *((uint16*)&SecBuff[ThisFATEntOffset]);
			case FATTYPE_FAT32:
				return (*((uint32*)&SecBuff[ThisFATEntOffset])) & 0x0FFFFFFF;
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
		void setFATClusterEntryValue(uint N, byte* SecBuff, uint ThisFATEntOffset, uint value) {
			switch(FATType) {
			case FATTYPE_FAT12:
				{
				uint16& fat12value = *(uint16*)&SecBuff[ThisFATEntOffset];
				if(N & 0x0001) {
					/* Cluster number is ODD */
					value <<= 4;
					fat12value &= 0x000F;
				} else {
					/* Cluster number is EVEN */
					fat12value &= 0xF000;
				}
				fat12value |= value & 0x0FFF;
				}
				return;
			case FATTYPE_FAT16:
				*((uint16*)&SecBuff[ThisFATEntOffset]) = value;
				return;
			case FATTYPE_FAT32:
				{
				uint32& data = *((uint32*)&SecBuff[ThisFATEntOffset]);
				data = (data&0xF0000000) | (value&0x0FFFFFFF);
				return;
				}
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
		// Is FATContent End Of Clusterchain?
		bool IsEOC(uint32 FATContent) const {
			switch(FATType) {
			case FATTYPE_FAT12:
				return FATContent>=0x0FF8;
			case FATTYPE_FAT16:
				return FATContent>=0xFFF8;
			case FATTYPE_FAT32:
				return FATContent>=0x0FFFFFF8;
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
	public:
		uint32 getFATEntry(uint N) const {
			return getFATClusterEntryValue(N, fatbuf, getFATOffset(N));
		}
		uint ReadRootDirectoryData(void* buf, uint size) {
			switch(FATType) {
			case FATTYPE_FAT12:
			case FATTYPE_FAT16:
				uint totalsize;
				totalsize = bpb.numberOfRootDirEntries*sizeof(DirectoryEntry);
				if(buf==NULL) return totalsize;
				if(size<totalsize) return 0;
				dev->Seek(FirstRootDirSectorNum);
				dev->Read(buf, totalsize/dev->getBlockSize());
				return totalsize;
			case FATTYPE_FAT32:
				panic("ReadRootDirectoryData(FAT32) is not Implemented");
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
		uint ReadCluster(uint cluster, void* buf) {
			dev->Seek(getFirstSectorOfCluster(cluster));
			dev->Read(buf, bpb.sectorsPerCluster);
			return bpb.bytesPerSector*bpb.sectorsPerCluster;
		}
	public:
		IDirectorySP getRootDirectory() {
			switch(FATType) {
			case FATTYPE_FAT12:
			case FATTYPE_FAT16:
				return IDirectorySP(new FATRootDirectory(*this));
			case FATTYPE_FAT32:
				//return new FATDirectory(*this,bpb.fat32.rootCluster);
				panic("getRootDirectory");
			default:
				panic("ILLEGAL FileSystem TYPE");
			}
		}
	};

	FATFile::FATFile(FileSystem& _fat, wchar_t* _name, const DirectoryEntry& _entry) : fat(_fat), name(_name) {
		entry = _entry;
		position = 0;
		uint32 cluster = entry.getFirstCluster();
		while(!fat.IsEOC(cluster)) {
			clusterchain.push_back(cluster);
			cluster = fat.getFATEntry(cluster);
		}
	}

	uint FATFile::Read(byte* buf, uint _size) {
		if(position+_size>entry.fileSize) {
			// ファイルサイズより大きく読み込もうとする場合の処理
			_size = entry.fileSize-position;
		}
		if(_size==0) return 0;
		uint size = _size;
		uint bps = fat.getBytesPerSector();
		uint spc = fat.getSectorsPerCluster();
		uint bpc = bps*spc;
		uint ci0 = (position+bpc-1)/bpc;
		uint ci1 = (position+size-1)/bpc;	//読み込み領域はci1を含む
		getConsole() << "Reading " << (int)size << " bytes from " << (int)position << " byte" << endl;
		getConsole() << "   at [" << (int)ci0 << "," << (int)ci1 << "] of cluster-index" << endl;
		getConsole() << "   by " << (int)bpc << " [b/c]" << endl;
		// 端数処理
		uint d0 = position-ci0*bpc;
		uint d1 = position+size-ci1*bpc;		//読み込み領域はd1を含まない
		if(d0>0 || d1>0) {
			byte* tmpbuf = new byte[bpc];
			if(ci0==ci1) {
				getConsole() << "Process as completely internal reading" << endl;
				// 読み込み領域が１クラスタより内部
				fat.ReadCluster(clusterchain[ci0], tmpbuf);
				memcpy(buf, tmpbuf+d0, size);
				delete [] tmpbuf;
				return size;
			} else {
				// 開始クラスタまたは終端クラスタの領域が端数
				if(d0>0) {
					getConsole() << "Truncated beginning cluster (" << (int)ci0 << ") to border" << endl;
					// 開始クラスタの領域が端数
					fat.ReadCluster(clusterchain[ci0], tmpbuf);
					memcpy(buf, tmpbuf+d0, bpc-d0);
					++ci0;
					buf = (byte*)buf+bpc-d0;
					size -= bpc-d0;
				}
				if(d1>0) {
					getConsole() << "Truncated ending cluster (" << (int)ci1 << ") to border" << endl;
					// 終端クラスタの領域が端数
					fat.ReadCluster(clusterchain[ci1], tmpbuf);
					memcpy((byte*)buf+size-d1, tmpbuf, d1);
					--ci1;
					size -= d1;
				}
			}
			delete [] tmpbuf;
		}
		getConsole() << "Reading clusters ";
		for(uint ci=ci0; ci<=ci1; ++ci) {
			getConsole() << (int)clusterchain[ci] << "/" << ci1 << ", ";
			fat.ReadCluster(clusterchain[ci], (byte*)buf+bpc*(ci-ci0));
		}
		getConsole() << "fin." << endl;
		position += _size;
		return _size;
	}

	static int getStringLength(const char* _s, char ch, int len) {
		const char* s;
		for(s=_s; len>0 && *s && *s!=ch; --len, ++s) {
		}
		return s-_s;
	}

	void FATRootDirectory::getFiles(FileList& filelist) {
		Console& console = getConsole();
		uint rootsize = fat.ReadRootDirectoryData(NULL,0);
		byte* buf = new byte[rootsize];
		fat.ReadRootDirectoryData(buf, rootsize);
		wchar_t lnbuf[261];
		bool lastingln = false;
		for(uint i=0; i<512/32; ++i) {
			const DirectoryEntry& entry = ((DirectoryEntry*)buf)[i];
			if((entry.attributes&ATTR_LONG_NAME)==ATTR_LONG_NAME) {
				const LongNameEntry& longname = *(const LongNameEntry*)&entry;
				// LN Entry
				int index;
				if(!lastingln) {
					index = longname.seq-1;
					if(index & 0x40) {
						lastingln = true;
						index &= ~0x40;
						memclr(lnbuf, sizeof(lnbuf));
					}
				} else {
					index = longname.seq-1;
				}
				if(lastingln) {
					memcpy(lnbuf+13*index,		longname.name0_4,	sizeof(longname.name0_4));
					memcpy(lnbuf+13*index+5,	longname.name5_10,	sizeof(longname.name5_10));
					memcpy(lnbuf+13*index+11,	longname.name11_12,	sizeof(longname.name11_12));
				}
			} else {
				if(entry.name[0]==0x00) {
					break;
				} else if(entry.name[0]==0xE5) {
					// NOP: This is free but there may be following entries.
				} else if(entry.attributes & ATTR_VOLUME_ID) {
					// NOP: We should ignore this entry.
				} else {
					if(!lastingln) {
						// nl:length of name, el:length of extension
						int nl = getStringLength(entry.name,' ',sizeof(entry.name));
						int el = getStringLength(entry.ext,' ',sizeof(entry.ext));
						for(int i=0; i<nl; ++i) lnbuf[i]=entry.name[i];
						if(el==0) {
							lnbuf[nl] = '\0';
						} else {
							for(int i=0; i<el; ++i) lnbuf[nl+1+i]=entry.ext[i];
							lnbuf[nl] = '.';
							lnbuf[nl+1+el] = '\0';
						}
					}
					filelist.push_back(new FATFile(fat,lnbuf,entry));
				}
				lastingln = false;
			}
		}
		delete [] buf;
	}

	extern void Initialize() {
	}

	extern void Finalize() {
	}

	extern bool IsSuitableMedia(const byte* data) {
		return true;
	}

	extern IFileSystemSP AttachMedia(IMedia* media) {
		return IFileSystemSP(new FileSystem(media));
	}

}
