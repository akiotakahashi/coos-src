#include "kernel.h"
#include "memory.h"
#include "io.h"
#include "interrupt.h"
#include "pic.h"
#include "dma.h"
#include "fdd.h"
#include "atapi.h"
#include "fat.h"
#include "iso9660.h"
#include "timer.h"
#include "keyboard.h"
#include "console.h"
#include "threading.h"
#include "commands.h"
#include "interop.h"
#include "ilengine.h"
#include "assemblymanager.h"


extern volatile int intref = 1;	// Interruption is disabled initially.


extern const char* getKernelName() {
	return "CooS";
}

extern const char* getKernelVersion() {
	return "0.3";
}


void SegmentData::dump(Console& console) const {
	console << " cs:" << "----------" << "  ds:" << ds << "  ss:" << ss << endl;
	console << " es:" << es << "  fs:" << fs << "  gs:" << gs << endl;
}

void RegisterData::dump(Console& console) const {
	console << "eax:" << eax << " ebx:" << ebx << " ecx:" << ecx << " edx:" << edx << endl;
	console << "edi:" << edi << " esi:" << esi << " ebp:" << ebp << " esp:" << esp << endl;
}

static byte* frame;
static int pixelsize;
static int scanline;
static int xresolution;
static int yresolution;

static void TimerHandler() {
	static int count = 0;
	if(frame) {
		if(0==(count&0x3)) {
			int i0 = count >> 2;
			for(int n=0; n<5; ++n) {
				int i = (i0-n+16)%16;
				int x = xresolution-3+(i-16)*16;
				for(int dy=0; dy<3; ++dy) {
					memset(frame+(3+dy)*scanline+x*pixelsize, 0xFF>>n, pixelsize*15);
				}
			}
		}
	} else {
		static char rotch[] = {'|','/','-','\\'};
		PutCharacter(rotch[(count>>0)&0x3], 0x0F, 79, 0);
		PutCharacter(rotch[(count>>3)&0x3], 0x0F, 78, 0);
		PutCharacter(rotch[(count>>6)&0x3], 0x0F, 77, 0);
	}
	++count;
}

typedef std::deque<KeyCode> KeyCodeList;
KeyCodeList* typedkeys = NULL;

extern void KeyboardHandler(KeyCode ch, KeyStatus ks) {
	Console& console = getConsole();
	if(ks==KS_DOWN) {
		Console& console = getConsole();
		switch(ch) {
		case KEY_PAGEUP:
			console.setWindowBaseline(console.getWindowBaseline()-console.getWindowHeight()/2-1);
			break;
		case KEY_PAGEDOWN:
			console.setWindowBaseline(console.getWindowBaseline()+console.getWindowHeight()/2+1);
			break;
		default:
			typedkeys->push_back(ch);
			break;
		}
	}
}


#include "kernelimpl.h"

struct Kernel : KernelImpl {
	virtual bool __stdcall NotAsSystem() {
		return false;
	}
	virtual void* __stdcall ReadLine() {
		return NULL;
	}
	virtual void __stdcall Write(wchar_t ch) {
		getConsole().Write((char)ch);
	}
};

static bool LoadAssemblyIntoManagedSpace(const std::wstring& assemname) {
	if(0==Commands::Execute(L"read 1 "+assemname+L".dll")) {
		Commands::IntroduceAssemblyIntoManaged(assemname, 1);
		return true;
	}
	getConsole() << "FAILED" << endl;
	return false;
}

static __declspec(naked) void __stdcall Div0ExceptionHandler() {
	InterruptHandler_Prologue;
	clrpanic("DIVIDE BY ZERO");
	InterruptHandler_Epilogue;
}

static __declspec(naked) void __stdcall StackExceptionHandler() {
	InterruptHandler_Prologue;
	clrpanic("STACK EXCEPTION");
	InterruptHandler_Epilogue;
}

static __declspec(naked) void __stdcall EmptyHandler() {
	InterruptHandler_Prologue;
	PIC::NotifyEndOfInterrupt(0x67);
	InterruptHandler_Epilogue;
}

extern uint64 AllocationCount;
extern uint64 TotalAllocationTime;

static void* Read(const wchar_t* filename, int* sz) {
	Commands::Execute(std::wstring(L"read 0 ")+filename);
	getConsole() << "READ " << Commands::GetSlotSize(0) << " BYTES" << endl;
	*sz = Commands::GetSlotSize(0);
	return Commands::GetSlotData(0);
}

static void Free(void* p) {
	// NOP
}

extern mspace g_mspace = NULL;
extern "C" void mspace_malloc_stats(mspace msp);
extern "C" mspace create_mspace_with_base(void* base, size_t capacity, int locked);

struct ArgbHeader {
	union{
		// ピクセルの並び方を強く主張
		byte	abyIdentifier[4]; // "BGRA"
		uint32	dwIdentifier; // 0x41524742
	};
	uint32 PixelFormat; // 0x08080808(32bitColor) or 0x10101010(64bitColor)
	uint32 Width;
	uint32 Height;
};

extern void kernelmain() {

	frame = *(byte**)(0x800+0x28);
	scanline = *(ushort*)(0x800+0x10);
	xresolution = *(ushort*)(0x800+0x12);
	yresolution = *(ushort*)(0x800+0x14);
	pixelsize = (*(byte*)(0x800+0x19)+7)/8;
	if(frame==NULL) {
		byte (*vram)[160] = reinterpret_cast<byte(*)[160]>(0xB8000);
		vram[0][4] = '<';
		vram[0][5] = 0xF;
		memclr(vram, 2*25*80);
	} else {
		memclr(frame, yresolution*scanline);
	}

	uint32 cr0old, cr0new;
	__asm {
		finit;
	}
	__asm {
		mov eax, cr0;
		mov cr0old, eax;
		// FPU
		or  eax,  0x02;			// MP
		and eax, ~0x04;			// EM
		and eax, ~0x20;			// NE
		// Cache
		and eax, ~0x60000000;	// CD & NW
		//
		mov cr0, eax;
		mov cr0new, eax;
	}

	uint memkbsz = *(uint*)0x06FC;
	uint avlmem = memkbsz;
	avlmem *= 1024;

	const uint knlmemsz = 1024*1024*32;
	g_mspace = create_mspace_with_base((void*)(avlmem-knlmemsz), knlmemsz, 0);
	/*
	MemoryManager mm((void*)(avlmem-knlmemsz), knlmemsz);
	setMemoryManager(mm);
	*/

	Console::Initialize();
	Console caption(0, 80, 0, 2, 1);
	caption.Clear();

	caption << TextColor(15,0);
	caption << TextColor(10) << getKernelName();
	caption << TextColor( 7) << " version ";
	caption << TextColor(12) << getKernelVersion();
	caption << TextColor( 7) << " built on " ;
	caption << TextColor( 9) << __DATE__ " " __TIME__;
	caption << TextColor( 7) << "  Mem: " << (int)memkbsz << "KB";

	Console workarea(0, 80,  2, 24, 10);
	Console infoarea(0, 80, 24, 25, 1);
	infoarea << TextColor(15,9);
	setConsole(workarea);
	infoarea << clrs << "Press CTRL+ALT+DEL to reset";

	//----------

	workarea << "Physical Memory " << (int)(memkbsz/1024) << " MB (" << (int)memkbsz << " KB)" << endl;

	//----------

	workarea << "Initialize Interrupt Descriptor Table" << endl;
	Interrupt::Initialize();

	workarea << "Initialize Programmable Interrupt Controllers" << endl;
	PIC::Initialize();

	//----------

	workarea << "Initialize Threading" << endl;
	Threading::Initialize();

	//----------

	workarea << "Enable Interruption" << endl;
	EnableInterrupt();

	//----------

	KeyCodeList _typedkeys;
	typedkeys = &_typedkeys;
	Keyboard::setKeyboardHandler(KeyboardHandler);

	workarea << "Initialize DMA Controllers" << endl;
	DMA::Initialize();
	
	workarea << "Initialize Keyboard Controller" << endl;
	Keyboard::Initialize();
	
	workarea << "Initialize Floppy Disk Controllers" << endl;
	FDD::Initialize();

	workarea << "Initialize Programmable Interval Timer" << endl;
	Timer::Initialize(8*8);

	//----------

	// QEMU occurs Int#0 and IRQ#7 unexpected exception. 
	Interrupt::RegisterInterruptGate(0x00, Div0ExceptionHandler);
	Interrupt::RegisterInterruptGate(0x0C, StackExceptionHandler);
	PIC::RegisterInterruptHandler(7, EmptyHandler);

	//----------

	Atapi::Initialize();

	//----------

	workarea << "Initialize FileSystems" << endl;
	FAT::Initialize();
	Iso9660::Initialize();

	//----------

	Commands::Initialize();
	IL::Initialize((void*)0x400000, avlmem-knlmemsz-0x400000);

	//----------

	bool loaded = false;
	for(int i=0; i<4; ++i) {
		if(Atapi::getMediaType(i)==Atapi::DEVICE_CDROM) {
			Commands::Execute(std::wstring(L"drive cd ")+itos<wchar_t,10>(i));
			loaded = true;
			break;
		}
	}
	if(!loaded) {
		panic("Connot find System CD-ROM media");
	}
	workarea << "Activated CD-ROM" << endl;

	//----------

	if(frame==NULL) {
		workarea << "+------------------------+" << endl;
		workarea << "|   NON-GRAPHICAL MODE   |" << endl;
		workarea << "+------------------------+" << endl;
	} else {
		Commands::Execute(L"read 0 coos.arg");
		ArgbHeader* hdr = (ArgbHeader*)Commands::GetSlotData(0);
		for(uint y=0; y<hdr->Height; ++y) {
			uint* src = (uint*)(hdr+1)+y*hdr->Width;
			byte* dst = frame+scanline*y;
			for(uint x=0; x<hdr->Width; ++x) {
				switch(pixelsize) {
				case 4:
					*(uint*)dst = *src;
					break;
				case 3:
					*(uint*)dst &= 0xFF000000;
					*(uint*)dst |= *src&0xFFFFFF;
					break;
				default:
					memset(dst, ((byte)*src+(byte)(*src>>8)+(byte)(*src>>16))/3, pixelsize);
					break;
				}
				++src;
				dst += pixelsize;
			}
		}
	}

	//----------

	Timer::SetTimerHandler(TimerHandler);

	//----------

	workarea << "_________________________________________________" << endl;
	workarea << "||                                             ||" << endl;
	workarea << "||            INITIAL BOOTING START            ||" << endl;
	workarea << "||_____________________________________________||" << endl;
	workarea << endl;
	IL::Install(new Kernel, Read, Free);
	uint64 allocn = AllocationCount;
	uint64 alloct = TotalAllocationTime;
	workarea << "Total Count of Memory Allocation: " << (int64)allocn << " [times]" << endl;
	workarea << "Total Time of Memory Allocation : " << (int64)alloct << " [clocks]" << endl;

	//----------

	IL::ILMachine machine(2*1024*1024);

	//----------

	mspace_malloc_stats(g_mspace);
	//Timer::SetTimerHandler(NULL);

	//----------

	workarea << "--- Load Extra Assemblies ---" << endl;
	Commands::Execute(L"read 0 csgraphics.dll");
	Commands::Execute(L"load 0");
	Commands::Execute(L"read 0 System.dll");
	Commands::Execute(L"load 0");
	//TODO: ISO9660 level 3 converts unsafe characters to safe ones,
	//so filename can't include '.' in file title.
	//Commands::Execute(L"read 0 System.Drawing.dll");
	Commands::Execute(L"read 0 System_Drawing.dll");
	Commands::Execute(L"load 0");
	Commands::Execute(L"read 0 freetype2.dll");
	Commands::Execute(L"load 0");
	Commands::Execute(L"read 0 ia32assembler.dll");
	Commands::Execute(L"load 0");

	//----------

	workarea << ">>--- CSCORLIB RECEIVES COMPLETE CONTROL" << endl;
	AssemblyManager::Execute(L"cscorlib", L"CooS", L"Initializer", L"Startup", machine);

	//

	workarea << ">>--- SHELL STARTS" << endl;
	AssemblyManager::Execute(L"csgraphics", L"CooS.Shell", L"CommandShell", L"Startup", machine);

	//----------

	workarea << endl;
	//workarea << "CR0: " << cr0old << " -> " << cr0new << endl;
	workarea << "Type 'help' to show available commands." << endl;
	workarea << ">";

	Console looparea(70,79,0,1,1);
	int count = 0;
	std::wstring cmdline(L"");
	for(;;) {
		__asm cli;
		KeyCodeList buf(*typedkeys);
		typedkeys->clear();
		__asm sti;
		while(buf.size()>0) {
			KeyCode keycode = buf.front();
			buf.pop_front();
			switch(keycode) {
			case KEY_RETURN:
				workarea << endl;
				Commands::Execute(cmdline);
				cmdline.clear();
				workarea << ">";
				break;
			case KEY_ESC:
				workarea.Clear();
				workarea << ">";
				break;
			case KEY_BACKSPACE:
				if(cmdline.length()>0) {
					workarea.Back();
					workarea.Write(' ');
					workarea.Back();
					cmdline.resize(cmdline.size()-1);
				}
				break;
			default:
				workarea << TextColor(15);
				workarea.Write(Keyboard::KeyCodeToChar(keycode));
				cmdline = cmdline+(wchar_t)Keyboard::KeyCodeToChar(keycode);
				break;
			}
		}
		//looparea << "\rc:" << count;
		//Threading::Reschedule();
		__asm hlt;
		++count;
	}
}

extern int main() {
	panic("main was called.");
}

extern void Delay(int nanoseconds) {
	while(nanoseconds>0) {
		__asm {
			nop;
			nop;
			nop;
			nop;
			nop;
			nop;
			//nop;
			//nop;
			//nop;
			//nop;
		}
		nanoseconds -= 3;
	}
}
