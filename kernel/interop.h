#pragma once


struct __declspec(novtable) IKernel {
	virtual bool __stdcall NotAsSystem() = 0;
	virtual void* __stdcall alloc(unsigned int size) = 0;
	virtual void __stdcall free(void* p) = 0;
	virtual void* __stdcall ReadLine() = 0;
	virtual void __stdcall Write(wchar_t ch) = 0;
	virtual void __stdcall SetDebugMode(bool enabled) = 0;
	virtual void __stdcall LoadAssembly(void* assembly, void* p, int size) = 0;
	virtual void* __stdcall GetExecutingAssembly(int depth) = 0;
	virtual void __stdcall ReloadMethodCode(void* handle, int rowIndex, const unsigned char* p) = 0;
	virtual void* __stdcall GetTypeFromHandle(void* handle) = 0;
	virtual void* __stdcall CreateInstance(void* handle) = 0;
	virtual void* __stdcall CloneInstance(void* obj) = 0;
	virtual void* __stdcall CreateString(unsigned int length) = 0;
	virtual void* __stdcall CreateArray(void* handle, unsigned int length) = 0;
	virtual void* __stdcall NotifyLoadingType(const wchar_t* asmname, int len, int typerid, void* obj) = 0;
	virtual const void* __stdcall GenerateProxyCode(const wchar_t* asmname, int len, int methodrid) = 0;
};
