#include "storage.h"
#include "console.h"


BufferedStream::BufferedStream(IStream* stream, uint blocksize) : BaseStream(*stream) {
	buffer = new byte[blocksize];
	bufsize = blocksize;
	bufpos = (uint)-1;
	position = 0;
}

BufferedStream::~BufferedStream() {
	delete [] buffer;
}

const std::wstring& BufferedStream::getName() const {
	return BaseStream.getName();
}

uint BufferedStream::getLength() const {
	return BaseStream.getLength();
}

uint BufferedStream::getPosition() const {
	return position;
}

uint BufferedStream::getAttributes() const {
	return BaseStream.getAttributes();
}

void BufferedStream::Seek(uint position) {
	return BaseStream.Seek(position);
}

uint BufferedStream::Read(byte* buf, uint size) {
	uint length = getLength();
	// �T�C�Y�����E�ɒ���
	if(position+size>length) size=length-position;
	if(size==0) return 0;
	uint rest = size;
	byte* pbuf = buf;
	// �p�����^����
	uint bpc = bufsize;
	uint ci0 = (position+bpc-1)/bpc;
	uint ci1 = (position+size-1)/bpc;	//�ǂݍ��ݗ̈��ci1���܂�
	/*
	getConsole() << "Reading " << (int)size << " bytes from " << (int)position << " byte" << endl;
	getConsole() << "   at [" << (int)ci0 << "," << (int)ci1 << "] of cluster-index" << endl;
	getConsole() << "   by " << (int)bpc << " [b/c]" << endl;
	//*/
	// �o�b�t�@������
	if(bufpos<=position) {
		if(position+rest<=bufpos+bufsize) {
			// �o�b�t�@�ɗ]������
			memcpy(pbuf, buffer+(position-bufpos), size);
			position += size;
			return size;
		} else if(position<bufpos+bufsize) {
			// �o�b�t�@�g���؂�
			uint dsize = bufpos+bufsize-position;
			memcpy(pbuf, buffer+(position-bufpos), dsize);
			position += dsize;
			pbuf += dsize;
			rest -= dsize;
		} else {
			// �o�b�t�@�͊֌W�Ȃ��̈�
			BaseStream.Seek(position/bufsize*bufsize);
		}
	}
	// ���Ԃɂ��銮�S�u���b�N�𒼐ړǂݍ���
	if(rest>bufsize) {
		int count = rest/bufsize;
		count *= bufsize;
		BaseStream.Read(pbuf,count);
		position += count;
		pbuf += count;
		rest -= count;
	}
	// �c��̃N���X�^����
	if(rest>0) {
		if(rest>=bufsize) panic("The size of rest is larger than buffer size.");
		// �o�b�t�@�����O���悤�I
		BaseStream.Read(buffer, bufsize);
		bufpos = position;
		memcpy(pbuf, buffer, rest);
		position += rest;
		pbuf += rest;
		rest -= rest;
	}
	return size;
}
