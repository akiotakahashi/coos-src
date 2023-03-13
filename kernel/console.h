#pragma once

#include "stdtype.h"
#include "stl.h"


struct TEXT {
	char ch;
	byte at;
	TEXT() {
	}
	TEXT(char _ch, byte _at) : ch(_ch), at(_at) {
	}
};

struct TextColor {
	unsigned char fc;
	unsigned char bc;
	explicit TextColor(unsigned char f, unsigned char b) {
		fc = f;
		bc = b;
	}
	explicit TextColor(unsigned char f) {
		fc = f;
		bc = 0;
	}
	unsigned char Attribute() const {
		return fc | (bc << 4);
	}
};

class Console {
	friend class Caret;
	int x0, x1;
	int y0, y1;
	int Attribute;
	// buffer
	int linecount;		// �o�b�t�@�s��
	TEXT** scrbuf;
	// window
	int baseline;		// ��ʏ�̏�[�s
	// caret
	int cx;
	int cy;
public:
	static void Initialize();
public:
	Console(int X0, int X1, int Y0, int Y1, int bufscale);
	~Console();
private:
	TEXT* getLineBuffer(int y) { return scrbuf[y]; }
	void Put(int x, int y, unsigned char ch, unsigned char at);
	void Refresh();
public:
	int getWidth() const { return x1-x0; }
	int getWindowHeight() const { return y1-y0; }
	int getBufferHeight() const { return linecount; }
	bool IsVisible(int y) { return baseline<=y && y<baseline+getWindowHeight(); }
public:
	void Clear();
	void RollUp();
	void Back(int len=1);
	void Write(char ch);
	void MakeNewLine();
public:
	int getWindowBaseline() const { return baseline; }
	void setWindowBaseline(int y);
public:
	Console& operator <<(const TextColor& c);
	Console& operator <<(const char* s);
	Console& operator <<(const wchar_t* s);
	Console& operator <<(byte n);
	Console& operator <<(short n);
	Console& operator <<(ushort n);
	Console& operator <<(int32 n);
	Console& operator <<(uint32 n);
	Console& operator <<(int64 n);
	Console& operator <<(uint64 n);
	Console& operator <<(float r);
	Console& operator <<(double r);
	Console& operator <<(int n) { return operator <<((int32)n); }
	Console& operator <<(uint n) { return operator <<((uint32)n); }
	Console& operator <<(const std::string& s) { return operator <<(s.c_str()); }
	Console& operator <<(const std::wstring& s) { return operator <<(s.c_str()); }
	Console& operator <<(const void* n) { return operator <<(reinterpret_cast<uint>(n)); }
	Console& operator <<(const MemoryRegion& mem);
public:
	void Write(const char* sz) {operator <<(sz);}
};


extern const char* endl;		// end line
extern const char* clrs;		// clear screen
extern const char* newline;		// clear line

extern Console& getConsole();
extern void setConsole(Console& c);

extern void PutCharacter(char ch, byte at, int x, int y);
