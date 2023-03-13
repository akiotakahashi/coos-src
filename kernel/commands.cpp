#include "commands.h"
#include "threading.h"
#include "atapi.h"
#include "fdd.h"
#include "fat.h"
#include "iso9660.h"
#include "pe.h"
#include "metadata.h"
#include "reflection.h"
#include "ilengine.h"
#include "assemblymanager.h"


struct ShellState {
	IFileSystemSP fs;
	IDirectorySP root;
	static ShellState& getCurrent() {
		static ShellState* current = NULL;
		if(current==NULL) {
			current = new ShellState();
		}
		return *current;
	}
};

struct SlotData {
	std::wstring name;
	byte* data;
	uint size;
	SlotData() {
		name = L"#N/A#";
		data = NULL;
		size = 0;
	}
};

static std::vector<ICommandProcessor*>* cmdprocs;
static std::vector<SlotData>* slots = NULL;

static int current_media = 2;


class TestCommand : public ICommandProcessor {
	TestCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new TestCommand());
	}
public:
	virtual std::wstring getName() const { return L"test"; }
	virtual std::wstring getDescription() const { return L"Examine."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		long long tsc0 = getTimestampCounter();
		//*
		Commands::Execute(L"read 0 application.exe");
		Commands::Execute(L"load 0");
		Commands::Execute(L"execute application");
		//*/
		long long tsc1 = getTimestampCounter();
		console << "Complete in " << tsc1-tsc0 << " tsc." << endl;
		return 0;
	}
};

class LinkCommand : public ICommandProcessor {
	LinkCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new LinkCommand());
	}
public:
	virtual std::wstring getName() const { return L"link"; }
	virtual std::wstring getDescription() const { return L"Examine."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		return !AssemblyManager::LinkAssembly(L"cskorlib");
	}
};

class HelpCommand : public ICommandProcessor {
	HelpCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new HelpCommand());
	}
public:
	virtual std::wstring getName() const { return L"help"; }
	virtual std::wstring getDescription() const { return L"Shows commands and thier descriptions."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		for(uint i=0; i<cmdprocs->size(); ++i) {
			console << cmdprocs->at(i)->getName() << "\t" << cmdprocs->at(i)->getDescription() << endl;
		}
		return 0;
	}
};

class DumpCommand : public ICommandProcessor {
	DumpCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new DumpCommand());
	}
public:
	virtual std::wstring getName() const { return L"dump"; }
	virtual std::wstring getDescription() const { return L"Shows memory allocation map."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		getMemoryManager().Dump();
		console << "--- Assemblies ---" << endl;
		for(int i=0; i<AssemblyManager::GetAssemblyCount(); ++i) {
			Reflection::Assembly* assem = AssemblyManager::GetAssembly(i);
			console << "[" << (int)i << "] " << assem->getName() << endl;
		}
		console << "--- Memory Slots ---" << endl;
		for(uint i=0; i<slots->size(); ++i) {
			const SlotData& slot = slots->at(i);
			console << "[" << (int)i << "] " << slot.data << "-" << slot.size << " " << slot.name << endl;
		}
		return 0;
	}
};

#include "enpack.h"
#include "enalign.h"

struct ImageLocation {
	void* Start;
	int Size;
};

#include "unpack.h"
#include "unalign.h"

class ThreadCommand : public ICommandProcessor {
	static void threadmain(void* param) {
		getConsole() << "Thread was started with " << param;
		Console& console = *(Console*)param;
		int c = 0;
		while(true) {
			++c;
			long long tsc = getTimestampCounter();
			console << "\r" << tsc;
		}
	}
public:
	static void Register() {
		cmdprocs->push_back(new ThreadCommand());
	}
public:
	virtual std::wstring getName() const { return L"thread"; }
	virtual std::wstring getDescription() const { return L"Starts a new thread."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		static int count = 0;
		count++;
		Threading::CreateThread(threadmain, new Console(60,80,count,count+1,1), (void*)(0x2000000+0x10000*count));
		return 0;
	}
};

class DriveCommand : public ICommandProcessor {
public:
	static void Register() {
		cmdprocs->push_back(new DriveCommand());
	}
public:
	virtual std::wstring getName() const { return L"drive"; }
	virtual std::wstring getDescription() const { return L"Select current FD drive."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()==1) {
			console << "Current drive is #" << current_media << "." << endl;
		} else if(args.size()==3) {
			int drvno = stoi(args[2].c_str());
			if(drvno<0 || 3<drvno) {
				console << "Invalid drive number" << endl;
				return 1;
			} else {
				ShellState& ss = ShellState::getCurrent();
				if(args[1]==L"cd") {
					ss.root.reset();
//panic("break");
					ss.fs = Iso9660::AttachMedia(Atapi::getDevice(drvno));
				} else if(args[1]==L"fd") {
					ss.root.reset();
					ss.fs = FAT::AttachMedia(FDD::GetMedia(drvno));
				} else {
					console << "Invalid media type" << endl;
					return 1;
				}
			}
		} else {
			console << "Invalid arguments" << endl;
			return 1;
		}
		return 0;
	}
};

class ListCommand : public ICommandProcessor {
	ListCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new ListCommand());
	}
public:
	virtual std::wstring getName() const { return L"ls"; }
	virtual std::wstring getDescription() const { return L"Lists files in the root directory."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		IFileSystemSP fs = ShellState::getCurrent().fs;
		IDirectorySP root = fs->getRootDirectory();
		FileList files;
		root->getFiles(files);
		for(uint i=0; i<files.size(); ++i) {
			IStream* file = files[i];
			console << "[";
			if(file->getAttributes() & ATTR_DIRECTORY)	console << "D";		else console << "F";
			if(file->getAttributes() & ATTR_READ_ONLY)	console << "R";		else console << "-";
			if(file->getAttributes() & ATTR_HIDDEN)		console << "H";		else console << "-";
			if(file->getAttributes() & ATTR_SYSTEM)		console << "S";		else console << "-";
			if(file->getAttributes() & ATTR_ARCHIVE)	console << "A";		else console << "-";
			console << "] " << file->getName() << " (" << (int32)file->getLength() << ")" << endl;
			delete files[i];
		}
		return 0;
	}
};

inline static wchar_t getCaseInsensitiveCode(wchar_t ch) {
	if('A'<=ch && ch<='Z') return ch+('a'-'A'); 
	return ch;
}

static bool wcsieql(const wchar_t* s1, const wchar_t* s2) {
	wchar_t dist = 'a'-'A';
	while(*s1 && *s2 && (getCaseInsensitiveCode(*s1)==getCaseInsensitiveCode(*s2))) {
		++s1;
		++s2;
	}
	return *s1==*s2;
}

static bool wcsieql(const std::wstring& s1, const std::wstring& s2) {
	if(s1.length()!=s2.length()) return false;
	return wcsieql(s1.c_str(), s2.c_str());
}

class ReadCommand : public ICommandProcessor {
	ReadCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new ReadCommand());
	}
public:
	virtual std::wstring getName() const { return L"read"; }
	virtual std::wstring getDescription() const { return L"Reads contents of the specified file into a slot."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()!=3) {
			console << "Invalid arguments" << endl;
			return 1;
		}
		int slot = args[1][0]-'0';
		if(slot<0 || slot>9) {
			console << "Invaild slot number" << endl;
			return 1;
		}
		IDirectorySP root = ShellState::getCurrent().fs->getRootDirectory();
		FileList files;
		root->getFiles(files);
		IStream* file = NULL;
		for(uint i=0; i<files.size(); ++i) {
			if(wcsieql(files[i]->getName(), args[2])) {
				file = files[i];
			} else {
				delete files[i];
			}
		}
		if(file==NULL) {
			console << "File not found: " << args[2] << endl;
			return 1;
		} else {
			byte* buf;
			buf = new byte[file->getLength()];
			if(buf==NULL) {
				console << "Failed to allocate memory." << endl;
				return 1;
			} else {
				file->Read(buf, file->getLength());
				SlotData& s = slots->at(slot);
				delete [] s.data;
				s.name = file->getName();
				s.data = buf;
				s.size = file->getLength();
				/*
				console << "Successfully"
					<< " loaded " << (int)file->getLength() << " bytes"
					<< " into slot#" << slot
					<< " from " << file->getName() << endl;
				//*/
			}
			delete file;
		}
		return 0;
	}
};

class TypeCommand : public ICommandProcessor {
	TypeCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new TypeCommand());
	}
public:
	virtual std::wstring getName() const { return L"type"; }
	virtual std::wstring getDescription() const { return L"Outputs the specified slot."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()!=2) {
			console << "Invalid arguments" << endl;
			return 1;
		}
		int slot = args[1][0]-'0';
		if(slot<0 || slot>9) {
			console << "Invaild slot number" << endl;
			return 1;
		}
		const char* data = (const char*)slots->at(slot).data;
		int count = 0;
		for(uint i=0; i<slots->at(slot).size; ++i) {
			if(data[i]) {
				console.Write(data[i]);
				++count;
			}
		}
		int cnull = 0;
		int frag = 0;
		for(uint i=1; i<slots->at(slot).size; ++i) {
			if(data[i]=='\0') {
				++cnull;
				if(!data[i]!=!data[i-1]) {
					++frag;
				}
			}
		}
		console.Write(endl);
		console << count << " characters and " << cnull << " null terminators are found with " << frag << " changes." << endl;
		return 0;
	}
};

class CompareCommand : public ICommandProcessor {
	CompareCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new CompareCommand());
	}
public:
	virtual std::wstring getName() const { return L"compare"; }
	virtual std::wstring getDescription() const { return L"Compares two slots."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()!=3) {
			console << "Invalid arguments" << endl;
			return 1;
		}
		int slot1 = args[1][0]-'0';
		int slot2 = args[2][0]-'0';
		if(slot1<0 || slot1>9 || slot2<0 || slot2>9) {
			console << "Invaild slot number" << endl;
			return 1;
		}
		SlotData& s1 = slots->at(slot1);
		SlotData& s2 = slots->at(slot2);
		uint size = (s1.size<s2.size) ? s1.size : s2.size;
		for(uint i=0; i<size; ++i) {
			if(s1.data[i]!=s2.data[i]) {
				console << "Different at " << i << " byte." << endl;
				return 0;
			}
		}
		console << "Identical." << endl;
		return 0;
	}
};

class LoadCommand : public ICommandProcessor {
	LoadCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new LoadCommand());
	}
public:
	virtual std::wstring getName() const { return L"load"; }
	virtual std::wstring getDescription() const { return L"Loads an assembly from the specified slot."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()!=2) {
			console << "Invalid arguments" << endl;
			return 1;
		}
		int slot = stoi(args[1]);
		if(slot<0 || slot>9) {
			console << "Invaild slot number" << endl;
			return 1;
		} else if(slots->at(slot).data==NULL) {
			console << "Specified slot is empty" << endl;
			return 1;
		}
		const SlotData& s = slots->at(slot);
		Reflection::Assembly* assem = AssemblyManager::LoadAssembly(s.data);
		AssemblyManager::IntroduceAssemblyIntoManaged(assem->getName(), s.data, s.size);
		return 0;
	}
};

class ExecuteCommand : public ICommandProcessor {
	ExecuteCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new ExecuteCommand());
	}
public:
	virtual std::wstring getName() const { return L"execute"; }
	virtual std::wstring getDescription() const { return L"Executes the specified slot as Assembly."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()<2) {
			console << "Invalid arguments" << endl;
			return 1;
		}
		for(int i=0; i<AssemblyManager::GetAssemblyCount(); ++i) {
			Reflection::Assembly& assem = *AssemblyManager::GetAssembly(i);
			if(wcsieql(assem.getName(),args[1])) {
				for(uint i=1; i<=assem.getTypeDefCount(); ++i) {
					const Reflection::TypeDef& td = assem.getTypeDef(i);
					for(uint j=1; j<=td.getMethodCount(); ++j) {
						const Reflection::Method& m = td.getMethod(j);
						if(m.getName()==L"Main") {
							std::wstring argtext;
							for(uint i=2; i<args.size(); ++i) {
								if(i>2) argtext += L" ";
								argtext += args[i];
							}
							IL::ILMachine machine(1024*1024*1);
							machine.stack.pushn(argtext.c_str());
							machine.stack.pushi(0);
							AssemblyManager::Execute(L"cscorlib",L"CooS",L"Assist",L"BuildCommandArguments",machine);
							IL::Execute(machine, m);
							return 0;
						}
					}
				}
				console << "Entrypoint not found" << endl;
				return 1;
			}
		}
		if(Commands::Execute(L"read 0 "+args[1])) {
			return 1;
		} else {
			if(Commands::Execute(L"load 0")) {
				console << "Not an Assembly" << endl;
			} else {
				Reflection::Assembly& assembly = *AssemblyManager::GetAssembly(AssemblyManager::GetAssemblyCount()-1);
				/*
				if(AssemblyManager::IntroduceAssemblyIntoManaged(assembly.getName(), 0)) {
					console << "Failed to introduce" << endl;
				} else {
				*/
					std::vector<std::wstring> newargs;
					newargs.push_back(L"execute");
					newargs.push_back(assembly.getName());
					for(uint i=2; i<args.size(); ++i) {
						newargs.push_back(args[i]);
					}
					return Commands::Execute(newargs);
				//}
			}
		}
		return 0;
	}
};

class ShutdownCommand : public ICommandProcessor {
	ShutdownCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new ShutdownCommand());
	}
public:
	virtual std::wstring getName() const { return L"shutdown"; }
	virtual std::wstring getDescription() const { return L"Halt operating system."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		panic("SHUTDOWN");
		return 0;
	}
};

class InitCommand : public ICommandProcessor {
	InitCommand() {}
public:
	static void Register() {
		cmdprocs->push_back(new InitCommand());
	}
public:
	virtual std::wstring getName() const { return L"init"; }
	virtual std::wstring getDescription() const { return L"Initialize Runtime Library."; }
	virtual void ShowManual(Console& console) const {
	}
	virtual int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		IFileSystemSP fs = ShellState::getCurrent().fs;
		IDirectorySP root = fs->getRootDirectory();
		FileList files;
		root->getFiles(files);
		for(uint i=0; i<files.size(); ++i) {
			IStream* file = files[i];
			std::wstring filename = file->getName();
			if(filename.length()>=5
				&& (wcsieql(L".dll", filename.substr(filename.length()-4))
				||  wcsieql(L".exe", filename.substr(filename.length()-4))))
			{
				console << filename << "...";
				Commands::Execute(L"read 0 "+filename);
				Commands::Execute(L"load 0");
				console << "OK" << endl;
			}
			delete file;
		}
		return 0;
	}
};


namespace Commands {

	extern void Initialize() {
		cmdprocs = new std::vector<ICommandProcessor*>();
		slots = new std::vector<SlotData>();
		slots->resize(10);
		TestCommand::Register();
		LinkCommand::Register();
		HelpCommand::Register();
		DumpCommand::Register();
		DriveCommand::Register();
		ListCommand::Register();
		ReadCommand::Register();
		TypeCommand::Register();
		LoadCommand::Register();
		ThreadCommand::Register();
		CompareCommand::Register();
		ExecuteCommand::Register();
		ShutdownCommand::Register();
		InitCommand::Register();
	}

	extern void* GetSlotData(int index) {
		return slots->at(index).data;
	}

	extern uint GetSlotSize(int index) {
		return slots->at(index).size;
	}

	extern int Execute(const std::wstring& cmdline) {
		std::vector<std::wstring> args;
		int offset = 0;
		while(true) {
			int next = cmdline.find(' ',offset);
			if(next==std::wstring::npos) {
				args.push_back(cmdline.substr(offset));
				break;
			} else {
				if(next-offset>0) {
					args.push_back(cmdline.substr(offset,next-offset));
				}
				offset = next+1;
			}
		}
		return Execute(args);
	}

	extern int Execute(const std::vector<std::wstring>& args) {
		Console& console = getConsole();
		if(args.size()==0 || args[0].length()==0) {
			//NOP
			return 0;
		} else if(args[0]==L"version") {
			console << getKernelName() << " version " << getKernelVersion() << endl;
			return 0;
		} else {
			for(uint i=0; i<cmdprocs->size(); ++i) {
				if(cmdprocs->at(i)->getName()==args[0]) {
					return cmdprocs->at(i)->Execute(args);
				}
			}
			if(args[0]==L"execute") {
				console << "Invalid command: " << args[0] << endl;
			} else {
				std::vector<std::wstring> args2;
				args2.push_back(L"execute");
				args2.insert(args2.end(), args.begin(), args.end());
				return Execute(args2);
			}
			return 1;
		}
	}

	extern bool IntroduceAssemblyIntoManaged(const std::wstring& name, int slotidx) {
		return AssemblyManager::IntroduceAssemblyIntoManaged(name, Commands::GetSlotData(slotidx), Commands::GetSlotSize(slotidx));
	}

}
