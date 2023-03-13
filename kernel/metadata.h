#pragma once

#include "stdtype.h"
#include "stdlib.h"
#include "win32.h"
#include "pe.h"
#include "stl.h"
#include "console.h"


#include "metadata/index.h"
#include "metadata/flags.h"
#include "metadata/table.h"
#include "metadata/index2.h"
#include "metadata/flags2.h"
#include "metadata/table2.h"

#include "enalign.h"
#include "enpack.h"

namespace Metadata {

	namespace Raw {

		//
		// CLI (COM+ 2.0) header structure
		//

		struct CliHeader {
			// Header versioning
			DWORD					cb;
			WORD					MajorRuntimeVersion;
			WORD					MinorRuntimeVersion;
			// Symbol table and startup information
			PE::Raw::DataDirectory	MetaData;
			DWORD					Flags;
			DWORD					EntryPointToken;
			// Binding information
			PE::Raw::DataDirectory	Resources;
			PE::Raw::DataDirectory	StrongNameSignature;
			// Regular fixup and binding information
			PE::Raw::DataDirectory	CodeManagerTable;
			PE::Raw::DataDirectory	VTableFixups;
			PE::Raw::DataDirectory	ExportAddressTableJumps;
			// Precompiled image info (internal use only - set to zero)
			PE::Raw::DataDirectory	ManagedNativeHeader;
		};

		#define CLI_FLAGS_ILONLY				0x01
		#define CLI_FLAGS_32BITREQUIRED			0x02
		#define CLI_FLAGS_STRONGNAMESIGNED		0x8
		#define CLI_FLAGS_TRACKDEBUGDATA		0x00010000

		//
		// Metadata root
		//

		struct MetadataRoot1 {
			DWORD	signature;
			WORD	majorVersion;
			WORD	minorVersion;
			DWORD	reserved;
			DWORD	length;
			char	version[256];	// is this safe?
		};

		struct MetadataRoot2 {
			WORD	flags;
			WORD	streams;
		};

		struct StreamHeader {
			DWORD	offset;
			DWORD	size;
			char	name[64];		//TODO: to be flexible size
		};

		struct MainStream_ {
			DWORD	reserved;
			BYTE	majorVersion;
			BYTE	minorVersion;
			BYTE	heapSizes;
			BYTE	reserved2;		// rid?
			uint64	valid;
			uint64	sorted;
		};

		//
		// VTable Fixup
		//

		struct VTableFixup {
			DWORD	VirtualAddress;
			WORD	Size;
			WORD	Type;
		};

		//extern void LoadImage(const CliHeader& clihdr);
		extern void LoadImage(const void* const image);

	}

	class MetadataRoot;

	class Stream : public Raw::StreamHeader {
	public:
		Stream(const Raw::StreamHeader& sh) : Raw::StreamHeader(sh) {
			//getConsole() << sh.name << ": " << sh.offset << " - " << sh.size << "\r\n";
		}
		Stream(const char* name) {
			strcpy(this->name, name);
			this->offset = 0xFFFFFFFF;
			this->size = 0;
		}
		virtual ~Stream() {
		}
		virtual void Parse(const MetadataRoot& metadata) {
		}
	};

	class MainStream : public Stream, public Raw::MainStream_ {
		uint rowcounts[TABLE_LAST];
		uint rowsizes[TABLE_LAST];
		uint idxsizes[INDEX_LAST];
		uint offsets[TABLE_LAST];
		const byte* imgbase;
		const byte* database;
	public:
		MainStream(const Raw::StreamHeader& sh, const void* base);
		~MainStream();
	public:
		uint getRowCount(TableId tid) const { return rowcounts[tid]; }
		Table* getRow(TableId tid, int index) const;
		Table* getRow(Index codedindex) const { return getRow(codedindex.table,codedindex.index); }
		const MemoryRegion getRowData(TableId tid, int index) const
		{ return MemoryRegion(database+offsets[tid]+rowsizes[tid]*index,rowsizes[tid]); }
	public:
		virtual void Parse(const MetadataRoot& metadata);
	};

	class BlobStream : public Stream {
	protected:
		const byte* base;
	public:
		BlobStream(const Raw::StreamHeader& sh, const void* base);
	protected:
		BlobStream(const char* name) : Stream(name) {
		}
	public:
		const byte* operator +(int offset) const { return base+offset; }
	public:
		const MemoryRegion getData(uint index) const;
		static uint ReadCompressedInteger(const byte*& p);
	};

	class GuidStream : public Stream {
		const byte* base;
	public:
		GuidStream(const Raw::StreamHeader& sh, const void* base) : Stream(sh) {
			this->base = (const byte*)base;
		}
	};

	class StringStream : public Stream {
		const char* base;
	public:
		StringStream(const Raw::StreamHeader& sh, const void* base);
	public:
		const char* operator +(int offset) const { return base+offset; }
	public:
		std::wstring getString(int index) const {
			return std::wstring(base+index,base+index+strlen(base+index));
		}
	};

	class UserStringStream : public BlobStream {
	public:
		UserStringStream(const Raw::StreamHeader& sh, const void* base) : BlobStream(sh,base) {
		}
		UserStringStream() : BlobStream("#US") {
		}
	public:
		std::wstring getString(int index) const {
			MemoryRegion mem = getData(index);
			return std::wstring((const wchar_t*)(mem+0), (const wchar_t*)(mem+mem.size()));
		}
	};

	class MetadataRoot : public Raw::MetadataRoot1, public Raw::MetadataRoot2 {
		const PE::Image& peimg;
		Raw::CliHeader clihdr;
		MainStream* stm_main;
		BlobStream* stm_blob;
		GuidStream* stm_guid;
		StringStream* stm_strings;
		UserStringStream* stm_userstring;
	public:
		static MetadataRoot* FromPEImage(const PE::Image& peimg);
	public:
		MetadataRoot(const PE::Image& peimg, const Raw::CliHeader& clihdr, const void* image);
		~MetadataRoot();
	public:
		int GetVTableFixupCount() const;
		const Raw::VTableFixup& GetVTableFixup(int index) const;
	public:
		const MainStream& getMainStream() const { return *stm_main; }
		const BlobStream& getBlobStream() const { return *stm_blob; }
		const Stream& getGuidStream() const { return *stm_guid; }
		const StringStream& getStringStream() const { return *stm_strings; }
		const UserStringStream& getUserStringStream() const { return *stm_userstring; }
	};

}

#include "unpack.h"
#include "unalign.h"
