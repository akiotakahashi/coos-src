#include "iso9660.h"
#include "kernel.h"
#include "memory.h"
#include "stdlib.h"
#include "console.h"


namespace Iso9660 {

	typedef uint16 u16l;
	typedef uint32 u32l;

#include "enalign.h"
#include "enpack.h"

	struct u16m {
		uint16 value;
	};

	struct u32m {
		uint32 value;
	};

	struct u16b {
		u16l	l;
		u16m	m;
		operator uint16() const {
			return l;
		}
	};

	struct u32b {
		u32l	l;
		u32m	m;
		operator uint32() const {
			return l;
		}
	};

	struct ShortDateTime {
		byte	year;
		byte	month;
		byte	day;
		byte	hour;
		byte	minute;
		byte	second;
		int8	offset;
	};

	struct LongDateTime {
		char year[4];
		char month[2];
		char day[2];
		char hour[2];
		char minute[2];
		char second[2];
		char tick[2];
		int8 offset;
	};

	struct DirectoryRecord {
		byte	LengthOfDirectoryRecord;
		byte	ExtendedAttributeRecordLength;
		u32b	LocationOfExtent;
		u32b	DataLength;
		ShortDateTime RecordingDateTime;
		byte	FileFlags;
		byte	FileUnitSize;
		byte	InterleaveGapSize;
		u16b	VolumeSequenceNumber;
		byte	LengthOfFileIdentifier;
		//byte	FileIdentifier[];		// 1 byte at least
	};

	struct VolumeDescriptorBase {
		byte	VolumeDerscriptorType;
		char	StandardIdentifier[5];
		byte	VolumeDescriptorVersion;
	};

	struct BootRecord : VolumeDescriptorBase {
		char	BootSystemIdentifier[32];
		char	BootIdentifier[32];
		byte	BootSystemUse[1977];
	};

	struct VolumeDescriptorSetTerminator : VolumeDescriptorBase {
		byte	Reserved[2041];
	};

	struct VolumeDescriptor : VolumeDescriptorBase {
		byte	VolumeFlags;				// Only SVD
		char	SystemIdentifier[32];
		char	VolumeIdentifier[32];
		byte	UnusedField[8];
		u32b	VolumeSpaceSize;
		byte	EscapeSequences[32];		// Only SVD
		u16b	VolumeSetSize;
		u16b	VolumeSequenceNumber;
		u16b	LogicalBlockSize;
		u32b	PathTableSize;
		u32l	LocationOfTypeLPathTable;
		u32l	OptionalLocationOfTypeLPathTable;
		u32m	LocationOfTypeMPathTable;
		u32m	OptionalLocationOfTypeMPathTable;
		DirectoryRecord RootDirectory;
		char	RootDirectoryFileIdentifier;
		char	VolumeSetIdentifier[128];
		char	PublisherIdentifier[128];
		char	DataPreparerIdentifier[128];
		char	ApplicationIdentifier[128];
		char	CopyrightFileIdentifier[37];
		char	AbstractFileIdentifier[37];
		char	BibliographicFileIdentifier[37];
		LongDateTime CreationDateTime;
		LongDateTime ModificationDateTime;
		LongDateTime ExpirationDateTime;
		LongDateTime EffectiveDateTime;
		byte	FileStructureVersion;
		byte	Reserved1;
		byte	ApplicationUse[512];
		byte	Reserved2[653];
	};

	struct VolumePartitionDescriptor : VolumeDescriptorBase {
		byte	UnusedField;				// Only SVD
		char	SystemIdentifier[32];
		char	VolumePartitionIdentifier[32];
		u32b	VolumePartitionLocation;
		u32b	VolumePartitionSize;
		byte	SystemUse[1960];
	};

	struct PathTableRecord {
		byte	LengthOfDirectoryIdentifier;
		byte	ExtendedAttributeRecordLength;
		union {
			u32l	l;
			u32m	m;
		} LocationOfExtent;
		union {
			u16l	l;
			u16m	m;
		} ParentDirectoryNumber;
		//char	DirectoryIdentifier[];		// 1 byte at least
	};

#include "unpack.h"
#include "unalign.h"

	class FileSystem;

	class Item {
	protected:
		std::wstring name;
		IMedia* media;
		FileSystem& fs;
		DirectoryRecord record;
	protected:
		Item(IMedia* _media, FileSystem& _fs, const DirectoryRecord* pdr) : media(_media), fs(_fs) {
			record = *pdr;
			const char* p = (const char*)(pdr+1);
			name.assign((const char*)p, (const char*)(p+record.LengthOfFileIdentifier+1));
			std::wstring::size_type i = name.rfind(';');
			if(i!=std::wstring::npos) name.resize(i);
		}
	protected:
		bool ReadExtent(int index, byte* buf, int count) {
			//getConsole() << "Try to read [" << index << "," << index+count-1 << "] sectors from CD-ROM..." << endl;
			media->Seek(record.LocationOfExtent+index);
			media->Read(buf, count);
			return true;
		}
	public:
		virtual const std::wstring& getName() const {
			return name;
		}
	};

	class File : public Item, public IStream {
		uint current_block;
		byte* buf;
	public:
		File(IMedia* media, FileSystem& fs, const DirectoryRecord* pdr) : Item(media,fs,pdr) {
			current_block = 0;
		}
	public:
		virtual const std::wstring& getName() const {
			return name;
		}
		virtual uint getLength() const {
			return record.DataLength;
		}
		virtual uint getPosition() const {
			return current_block*media->getBlockSize();
		}
		virtual uint getAttributes() const {
			return 0;
		}
		virtual void Seek(uint position) {
			current_block = position/media->getBlockSize();
		}
		virtual uint Read(byte* buf, uint size) {
			size /= media->getBlockSize();
			if(!ReadExtent(current_block, buf, size)) {
				getConsole() << "{CDFS:READ}";
				return -1;
			} else {
				current_block += size;
				return size*media->getBlockSize();
			}
		}
	};

	class Directory : public Item, public IDirectory {
		byte* buf;
		uint size;
		std::vector<DirectoryRecord*> records;
	public:
		Directory(IMedia* media, FileSystem& fs, const DirectoryRecord* pdr) : Item(media,fs,pdr) {
			size = (record.DataLength+media->getBlockSize()-1)/media->getBlockSize();
			buf = new byte[size*media->getBlockSize()];
			ReadExtent(0, buf, size);
			byte* p = buf;
			while((uint)(p-buf)<record.DataLength) {
				DirectoryRecord* pdr = (DirectoryRecord*)p;
				if(pdr->LengthOfDirectoryRecord==0) break;
				records.push_back(pdr);
				p += pdr->LengthOfDirectoryRecord;
			}
		}
		~Directory() {
			delete [] buf;
		}
	public:
		virtual const std::wstring& getName() const {
			return name;
		}
		virtual void getDirectories(DirectoryList& dirlist) {
		}
		virtual void getFiles(FileList& filelist) {
			for(uint i=0; i<records.size(); ++i) {
				if(records[i]->FileFlags&2) continue;
				filelist.push_back(new BufferedStream(new File(media,fs,records[i]),media->getBlockSize()));
			}
		}
	};

	class FileSystem : public IFileSystem {
		IMedia* media;
		byte* buf;
		uint size;
		VolumeDescriptor vd;
	public:
		FileSystem(IMedia* _media) : media(_media) {
			size = media->getBlockSize();
			buf = new byte[size];
			for(int ivd=0; ivd<32; ++ivd) {
				media->Seek(16+ivd);
				media->Read(buf,1);
				switch(((VolumeDescriptorBase*)buf)->VolumeDerscriptorType) {
				case 0:
					// Boot Record
					break;
				case 1:
				case 2:
					vd = *(VolumeDescriptor*)buf;
					if((vd.VolumeSpaceSize.l&0xFF) != (vd.VolumeSpaceSize.m.value>>24)) {
						panic("mismatch");
					}
					break;
				case 3:
					// Volume Partition Descriptor
					break;
				case 255:
					return;
				default:
					// future standardization
					break;
				}
			}
			// Timeout
			panic("Volume-Descriptor-Set-Terminator Not Found");
			return;
		}
		~FileSystem() {
			delete [] buf;
			getConsole() << "~Iso9660" << endl;
		}
	public:
		virtual IDirectorySP getRootDirectory() {
			return IDirectorySP(new Directory(media, *this, &vd.RootDirectory));
		}
	public:
	};

	extern bool Initialize() {
		if(sizeof(ShortDateTime)!=7) panic("ShortDateTime WrongSize");
		if(sizeof(LongDateTime)!=17) panic("LongDateTime WrongSize");
		if(sizeof(DirectoryRecord)!=33) panic("DirectoryRecord WrongSize");
		if(sizeof(VolumeDescriptorBase)!=7) panic("VolumeDescriptorBase WrongSize");
		if(sizeof(BootRecord)!=2048) panic("BootRecord WrongSize");
		if(sizeof(VolumeDescriptorSetTerminator)!=2048) panic("VolumeDescriptorSetTerminator WrongSize");
		if(sizeof(VolumeDescriptor)!=2048) panic("VolumeDescriptor WrongSize");
		if(sizeof(VolumePartitionDescriptor)!=2048) panic("VolumePartitionDescriptor WrongSize");
		if(sizeof(PathTableRecord)!=8) panic("PathTableRecord WrongSize");
		return true;
	}

	extern bool IsSuitableMedia(const byte* data) {
		return true;
	}

	extern IFileSystemSP AttachMedia(IMedia* media) {
		return IFileSystemSP(new FileSystem(media));
	}

}
