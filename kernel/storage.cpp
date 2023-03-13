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
	// サイズを限界に調整
	if(position+size>length) size=length-position;
	if(size==0) return 0;
	uint rest = size;
	byte* pbuf = buf;
	// パラメタ同定
	uint bpc = bufsize;
	uint ci0 = (position+bpc-1)/bpc;
	uint ci1 = (position+size-1)/bpc;	//読み込み領域はci1を含む
	/*
	getConsole() << "Reading " << (int)size << " bytes from " << (int)position << " byte" << endl;
	getConsole() << "   at [" << (int)ci0 << "," << (int)ci1 << "] of cluster-index" << endl;
	getConsole() << "   by " << (int)bpc << " [b/c]" << endl;
	//*/
	// バッファを消費
	if(bufpos<=position) {
		if(position+rest<=bufpos+bufsize) {
			// バッファに余分あり
			memcpy(pbuf, buffer+(position-bufpos), size);
			position += size;
			return size;
		} else if(position<bufpos+bufsize) {
			// バッファ使い切り
			uint dsize = bufpos+bufsize-position;
			memcpy(pbuf, buffer+(position-bufpos), dsize);
			position += dsize;
			pbuf += dsize;
			rest -= dsize;
		} else {
			// バッファは関係ない領域
			BaseStream.Seek(position/bufsize*bufsize);
		}
	}
	// 中間にある完全ブロックを直接読み込む
	if(rest>bufsize) {
		int count = rest/bufsize;
		count *= bufsize;
		BaseStream.Read(pbuf,count);
		position += count;
		pbuf += count;
		rest -= count;
	}
	// 残りのクラスタ処理
	if(rest>0) {
		if(rest>=bufsize) panic("The size of rest is larger than buffer size.");
		// バッファリングしよう！
		BaseStream.Read(buffer, bufsize);
		bufpos = position;
		memcpy(pbuf, buffer, rest);
		position += rest;
		pbuf += rest;
		rest -= rest;
	}
	return size;
}
