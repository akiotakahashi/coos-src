#pragma once

#include "reflection.h"
#include "interop.h"
#include "stl.h"


#include "il/instructions.h"
#include "il/ilstack.h"

namespace IL {

	struct ILMachine;

	struct Frame {
		const Reflection::Method& method;
		const Reflection::Assembly& assembly;
		const byte* code;
		int pc;						// Program Counter
		byte* vbp;					// Stack Base Pointer
		byte* abp;					// Argument Base Pointer
		std::list<byte*>* lalloc;	// for localloc instruction
#if defined(WIN32)
		Frame* prev;				// Previous frame
#endif
	public:
		Frame(ILMachine& machine, const Reflection::Method& method);
		~Frame();
	public:
		void allocWorkspace(ILStack& ilstack);
	};
	
	typedef std::list<Frame> FrameList;

	struct ILMachine {
		ILStack		stack;
		FrameList	frames;
		ILMachine*	next_machine;
	private:
		void init();
	public:
		ILMachine(uint stacksize);
		ILMachine();
		~ILMachine();
	public:
		void dump();
		ILMachine* getPrevious();
		static ILMachine& getCurrent();
		__declspec(noreturn) void panic(const char* msg);
		__declspec(noreturn) void panic(const wchar_t* msg);
		__declspec(noreturn) void panic(const std::wstring& msg);
	};

	/*
	Stack Frame Layout

		+------------------
		|	arg[0]
		+------------------
		|	arg[1]
		+------------------
		|
	ABP	|	arg[.param-1]
		+------------------
		|	var[0]	|	var[1]
		+------------------
		|	var[2]
		+------------------
		|
	VBP	|	var[.local-1]
		+------------------
		|	value[0]
		+------------------
		|	value[1]
		+------------------
		|	value[.maxstack]
		+------------------
	*/

	enum METATYPE {
		METATYPE_METATYPE,
		METATYPE_TYPEBASE,
		METATYPE_CLASSTYPE,
		METATYPE_STRING,
		METATYPE_DELEGATE,
		METATYPE_VALUETYPE,
		METATYPE_PRIMITIVE,
		METATYPE_INTERFACE,
		METATYPE_SZARRAY,
		METATYPE_MNARRAY,
		METATYPE_BYREFPTR,
		METATYPE_BYVALPTR,
		METATYPE_END
	};

#pragma warning(disable: 4103)
#include "enpack.h"
#include "enalign.h"
#pragma warning(default: 4103)

	using namespace Reflection;

	struct BlankType {
		void* type;
		void* getObject() const {
			return (void*)(this+1);
		}
		static BlankType* FromObject(void* obj) {
			return ((BlankType*)obj-1);
		}
	};

	struct Array {
		void* type;
		unint elemsize;
		unint length;
		byte start_elem;
	};

	struct String {
		int length;
		wchar_t start_char;
	};

#include "unpack.h"
#include "unalign.h"

	extern bool Initialize(void* baseaddress, uint size);
	extern void Install(IKernel* kernel, void* (*reader)(const wchar_t*, int*), void (*befree)(void*));
	extern void Finalize();

	extern void SetMetaType(METATYPE metatype, const TypeDef& typdef);
	extern const TypeDef& GetMetaType(METATYPE metatype);
	extern BlankType* NewType(METATYPE metatype);
	extern BlankType* NewBlankType(uint size);
	extern void IntroduceNewType(METATYPE metatype, TypeDef& typdef);

	extern bool IsNewProtocol(METATYPE metatype);
	extern void SetNewProtocol(METATYPE metatype);

	extern RuntimeTypeHandle GetHandleOfType(void* p);
	extern RuntimeTypeHandle GetHandleOfObject(void* p);
	extern void* CreateInstance(const TypeDef& typdef);
	extern void* CloneInstance(void* p);
	extern String* NewString(uint length);
	extern String* NewString(const std::wstring& str);
	extern Array* NewArray(const TypeDef& typdef, uint length);

	extern void EnableDebug(int level);
	extern void EnableDebug();
	extern void DisableDebug();
	extern bool IsDebugEnabled();
	extern int GetMachineDepth();

	extern void Execute(ILMachine& machine, const Reflection::Method& method);
	extern void Execute(ILMachine& machine, const Reflection::Assembly& assembly, void(*method)(), const Signature::MethodSig& signature);

	extern void* LoadManagedType(TypeDef& typdef);
	extern void SupressTypeReplacing();
	extern uint FlushTypeReplacing();
	extern uint ReplaceExistingType(void* from, void* to);

	extern void Synchronize();

}

extern __declspec(noreturn) void clrpanic(const char* msg);

inline static Console& operator <<(Console& console, const IL::ILStack& stack) {
	return stack.Dump(console);
}
