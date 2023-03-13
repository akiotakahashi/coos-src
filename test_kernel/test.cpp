#include "../kernel/stl.h"
#include "stdafx.h"
#include "../kernel/kernel.h"
#include "../kernel/interop.h"
#include "../kernel/metadata.h"
#include "../kernel/reflection.h"
#include "../kernel/ilengine.h"
#include "../kernel/assemblymanager.h"

typedef unsigned char byte;


#include "../kernel/kernelimpl.h"

struct Kernel : KernelImpl {
	virtual bool __stdcall NotAsSystem() {
		return true;
	}
	virtual void* __stdcall ReadLine() {
		wchar_t buf[512] = {'\0'};
		_getws(buf);
		return IL::NewString(std::wstring(buf));
	}
	virtual void __stdcall Write(wchar_t ch) {
		fprintf(stderr, "%C", ch);
	}
};

extern "C" wchar_t* __stdcall GetCommandLineW();
extern "C" char __stdcall SetCurrentDirectoryW(const wchar_t* lpPathName);

static void* Read(const wchar_t* filename, int* sz) {
	FILE* file = _wfopen(filename,L"rb");
	byte* buf = new byte[1024*1024*2];
	*sz = (int)fread(buf, 1, 1024*1024*2, file);
	fclose(file);
	return buf;
}

static void Free(void* p) {
	delete [] p;
}

void LoadAssembly(const wchar_t* name, char* filename) {
	FILE* file = fopen(filename,"rb");
	if(file==NULL) perror("file not found");
	byte* buf = new byte[1024*1024*2];
	int size = fread(buf, 1, 1024*1024*2, file);
	fclose(file);
	AssemblyManager::LoadAssembly(buf);
	AssemblyManager::IntroduceAssemblyIntoManaged(name, buf, size);
	delete [] buf;
}

extern "C" void mspace_malloc_stats(mspace msp);

// Interruption is disabled initially.
extern volatile int intref = 1;

static long __stdcall MyInterlockedExchange(long volatile* Target, long Value) {
	if(&getConsole()!=NULL) getConsole() << "Target=" << (uint)*Target << ", Value=" << (uint)Value << endl;
	__asm {
		mov ecx, [Target];
		mov eax, Value;
		xchg [ecx], eax;
		mov Value, eax;
	}
	return Value;
}

void TestCalc2(unsigned long long op1, unsigned long long op2) {
	getConsole() << (uint64)op1 << " + " << (uint64)op2 << " = " << (uint64)(op1+op2) << endl;
	getConsole() << (uint64)op1 << " - " << (uint64)op2 << " = " << (uint64)(op1-op2) << endl;
	getConsole() << (uint64)op1 << " * " << (uint64)op2 << " = " << (uint64)(op1*op2) << endl;
	getConsole() << (uint64)op1 << " / " << (uint64)op2 << " = " << (uint64)(op1/op2) << endl;
	getConsole() << (uint64)op1 << " % " << (uint64)op2 << " = " << (uint64)(op1%op2) << endl;
}

void TestCalc(long long op1, long long op2) {
	getConsole() << (uint64)op1 << " + " << (uint64)op2 << " = " << (uint64)(op1+op2) << endl;
	getConsole() << (uint64)op1 << " - " << (uint64)op2 << " = " << (uint64)(op1-op2) << endl;
	getConsole() << (uint64)op1 << " * " << (uint64)op2 << " = " << (uint64)(op1*op2) << endl;
	getConsole() << (uint64)op1 << " / " << (uint64)op2 << " = " << (uint64)(op1/op2) << endl;
	getConsole() << (uint64)op1 << " % " << (uint64)op2 << " = " << (uint64)(op1%op2) << endl;
	TestCalc2((uint64)op1, (uint64)op2);
}

#include <math.h>

static void engage() {

	Console screen(0,80,0,25,1);
	setConsole(screen);

#if 0
	TestCalc(5, 2);
	TestCalc(-5, 2);
	TestCalc(5, -2);
	TestCalc(-5, -2);
	TestCalc(0x1FFFFFFFF, 0xFFFFFFFF);
	TestCalc(0x8000000000000000, -1);
	getConsole() << (uint64)pow(2.0,32) << endl;
	getConsole() << (uint64)pow(2.0,36) << endl;
	getConsole() << (uint64)pow(2.0,40) << endl;
	getConsole() << (uint64)pow(2.0,44) << endl;
	getConsole() << (uint64)pow(2.0,48) << endl;
	getConsole() << (uint64)pow(2.0,52) << endl;
	getConsole() << (uint64)pow(2.0,56) << endl;
	getConsole() << (uint64)pow(2.0,60) << endl;
	getConsole() << (uint64)pow(2.0,61) << endl;
	getConsole() << (uint64)pow(2.0,62) << endl;
	getConsole() << (uint64)pow(2.0,63) << endl;
	getConsole() << (uint64)pow(2.0,64.0) << endl;
#endif

	uint size = 1024*1024*128;
	IL::Initialize(::malloc(size), size);
	IL::Install(new Kernel(), Read, Free);

	IL::ILMachine machine(1*1024*1024);

	//*/
	LoadAssembly(L"csgraphics", "csgraphics.dll");
	LoadAssembly(L"freetype2", "freetype2.dll");
	LoadAssembly(L"ia32assembler", "ia32assembler.dll");
	LoadAssembly(L"System", "System.dll");
	LoadAssembly(L"System.Drawing", "System.Drawing.dll");
	LoadAssembly(L"application", "application.exe");
	LoadAssembly(L"sum", "app\\sum.exe");
	//

	std::wstring cmdline = ::GetCommandLineW();
#if 0
	FILE* file = fopen("ipagui.ttf","rb");
	byte* buf = new byte[4*1024*1024];
	size_t sz = fread(buf, 1, 4*1024*1024, file);
	fclose(file);
	cmdline = L" ";
	cmdline += itos<wchar_t,10>((int)buf);
	cmdline += L" ";
	cmdline += itos<wchar_t,10>((int)sz);
#else
	byte* buf = new byte[0];
#endif

	/*
	Reflection::TypeDef* typdef = AssemblyManager::FindTypeDef(L"cscorlib",L"CooS.CodeModels.CLI.Metadata",L"TypeDefTable");
	for(int i=1; i<=typdef->getFieldCount(); ++i) {
		Reflection::Field& field = typdef->getField(i);
		getConsole() << "[" << field.getOffset() << "-" << (field.getOffset()+field.getSize()) << "] " << field.getName() << endl;
	}
	*/

	getConsole() << cmdline << endl;
	machine.stack.pushn(cmdline.c_str());
	machine.stack.pushi(1);
	AssemblyManager::Execute(L"cscorlib",L"CooS",L"Assist",L"BuildCommandArguments",machine);
	AssemblyManager::Execute(L"application",L"",L"MyClass",L"Main",machine);
	//AssemblyManager::Execute(L"sum",L"",L"sum",L"Main",machine);

	delete [] buf;

	IL::DisableDebug();
	IL::Finalize();
	AssemblyManager::Finalize();
}

int _tmain(int argc, wchar_t* argv[]) {
	::SetCurrentDirectoryW(L"D:\\Repository\\clios\\cdimage\\Release\\image");
	//::SetPriorityClass(::GetCurrentProcess(), IDLE_PRIORITY_CLASS);
	// Memory Initalization is proceed at init.cpp.
	mspace_malloc_stats(g_mspace);
	engage();
	mspace_malloc_stats(g_mspace);
	return 0;
}

static int sx, sy;
static std::string buf;

static void __stdcall dump_registers(bool unicode, SegmentData sd, RegisterData rd, const void* retip, const void* msg) {
	Console& console = getConsole();
	console.MakeNewLine();
	console << "KERNEL PANIC at " << retip << endl;
	rd.dump(console);
	sd.dump(console);
	console << "--- Callstack ---";
	void** p = (void**)rd.ebp;
	for(int i=0; i<24 && p; ++i) {
		if(i%6==0) console << endl;
		console << *(p+1) << " ";
		p = (void**)*p;
	}
	console << endl;
	console << "--- Stack Dump ---" << endl;
	uint32* sp_ = (uint32*)rd.esp+2;	// eip & msg
	for(int l=0; l<3; ++l) {
		console << sp_ << ":";
		for(int i=0; i<4; ++i) {
			console << " " << *sp_;
			++sp_;
		}
		console << endl;
	}
	if(msg!=0) {
		if(unicode) {
			console << "*** " << (const wchar_t*)msg << " ***";
		} else {
			console << "*** " << (const char*)msg << " ***";
		}
	}
	printf("%s\n", buf.c_str());
	fflush(stdout);
	fprintf(stderr,"panic> %S\n", msg);
}

void SegmentData::dump(Console& console) const {
	console << " cs:" << "----------" << "  ds:" << ds << "  ss:" << ss << endl;
	console << " es:" << es << "  fs:" << fs << "  gs:" << gs << endl;
}

void RegisterData::dump(Console& console) const {
	console << "eax:" << eax << " ebx:" << ebx << " ecx:" << ecx << " edx:" << edx << endl;
	console << "edi:" << edi << " esi:" << esi << " ebp:" << ebp << " esp:" << esp << endl;
}

__declspec(naked) extern void panic(const char* msg) {
	pushreg;
	pushseg;
	__asm push 0;
	__asm call dump_registers;
	abort();
}

__declspec(naked) extern void panic(const wchar_t* msg) {
	pushreg;
	pushseg;
	__asm push 1;
	__asm call dump_registers;
	abort();
}

/*
extern void panic(const char* msg) {
	printf("%s\n", buf.c_str());
	fflush(stdout);
	fprintf(stderr,"panic> %S\n", msg);
	getConsole().MakeNewLine();
	getConsole() << "panic> " << msg;
	abort();
}

extern void panic(const wchar_t* msg) {
	printf("%s\n", buf.c_str());
	fflush(stdout);
	fprintf(stderr,"panic> %S\n", msg);
	getConsole().MakeNewLine();
	getConsole() << "panic> " << msg;
	abort();
}
*/

void Console::Put(int x, int y, unsigned char ch, unsigned char at) {
	if(sx!=x) {
		sx = x;
		if(sy!=y) {
			sy = y;
			printf("%s\n", buf.c_str());
			fflush(stdout);
		}
		buf = "";
	}
	++sx;
	buf += ch;
}

void Console::Refresh() {
	printf("%s", buf.c_str());
	fflush(stdout);
	buf = "";
}

void Console::RollUp() {
	sy--;
}
