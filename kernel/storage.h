#pragma once

#include "stl.h"


/*************************************************************************************************
	Physical Storages
*************************************************************************************************/


struct __declspec(novtable) IMedia {
	virtual ~IMedia() {}
	virtual uint32 getBlockCount() const = 0;
	virtual size_t getBlockSize() const = 0;
	virtual void Seek(uint32 block) = 0;
	virtual void Read(void* buf, uint count) = 0;
};

typedef boost::shared_ptr<IMedia> IMediaSP;


/*************************************************************************************************
	FileSystems
*************************************************************************************************/

enum ATTEIBUTES {
	ATTR_READ_ONLY	= 0x01,
	ATTR_HIDDEN		= 0x02,
	ATTR_SYSTEM		= 0x04,
	ATTR_VOLUME_ID	= 0x08,
	ATTR_DIRECTORY	= 0x10,
	ATTR_ARCHIVE	= 0x20,
	ATTR_LONG_NAME	= ATTR_READ_ONLY|ATTR_HIDDEN|ATTR_SYSTEM|ATTR_VOLUME_ID,
};

struct __declspec(novtable) IStream {
	virtual ~IStream() {}
	virtual const std::wstring& getName() const = 0;
	virtual uint getLength() const = 0;
	virtual uint getPosition() const = 0;
	virtual uint getAttributes() const = 0;
	virtual void Seek(uint position) = 0;
	virtual uint Read(byte* buf, uint size) = 0;
};

struct IDirectory;

typedef std::vector<IDirectory*> DirectoryList;
typedef std::vector<IStream*> FileList;

struct __declspec(novtable) IDirectory {
	virtual ~IDirectory() {}
	virtual const std::wstring& getName() const = 0;
	virtual void getDirectories(DirectoryList& dirlist) = 0;
	virtual void getFiles(FileList& filelist) = 0;
};

typedef boost::shared_ptr<IDirectory> IDirectorySP;

struct __declspec(novtable) IFileSystem {
	virtual ~IFileSystem() {}
	virtual IDirectorySP getRootDirectory() = 0;
};

typedef boost::shared_ptr<IFileSystem> IFileSystemSP;


class BufferedStream : public IStream {
	IStream& BaseStream;
	uint bufsize;
	byte* buffer;
	uint bufpos;
	uint position;
public:
	BufferedStream(IStream* stream, uint blocksize);
	~BufferedStream();
public:
	virtual const std::wstring& getName() const;
	virtual uint getLength() const;
	virtual uint getPosition() const;
	virtual uint getAttributes() const;
	virtual void Seek(uint position);
	virtual uint Read(byte* buf, uint size);
};
