#include "stdafx.h"
#include "../kernel.h"
#include "../stl.h"
#include "../AssemblyManager.h"

using namespace Reflection;


extern void clrpanic(const char* msg) {
	IL::ILMachine::getCurrent().panic(msg);
}


namespace IL {

	Frame::Frame(ILMachine& machine, const Method& m) : method(m), assembly(m.getAssembly()) {
		code = m.getCodeBase();
		pc = 0;
		abp = (byte*)&machine.stack.top();
		vbp = NULL;
		lalloc = NULL;
#if defined(WIN32)
		if(machine.frames.size()==0) {
			prev = NULL;
		} else {
			prev = &machine.frames.back();
		}
#endif
	}

	Frame::~Frame() {
		if(lalloc!=NULL) {
			std::list<byte*>::iterator it; 
			it = lalloc->begin();
			while(it!=lalloc->end()) {
				delete [] *it;
				++it;
			}
			delete lalloc;
			lalloc = NULL;
		}
	}

	void Frame::allocWorkspace(ILStack& ilstack) {
		if(vbp!=NULL) clrpanic("workspace is already allocated.");
		uint size = method.getLocalVarTotalSize();
		vbp = (byte*)ilstack.allocmem(size);
	}

	static int debugLevel = 0;

	extern bool IsDebugEnabled() {
		return debugLevel>0 || debugLevel>=GetMachineDepth();
	}

	extern void EnableDebug(int level) {
		debugLevel = level;
	}

	extern void EnableDebug() {
		EnableDebug(GetMachineDepth());
	}

	extern void DisableDebug() {
		debugLevel = 0;
	}

	extern Method* (*dispatch00[256])(ILMachine&, Frame&, const byte*);
	extern Method* (*dispatchFE[256])(ILMachine&, Frame&, const byte*);

	static int nest = 0;

	extern int GetMachineDepth() {
		return 1;
	}

	extern void Execute(ILMachine& machine, const Method& _method) {
		Console& console = getConsole();
		static int instcount = 0;

		nest++;

#if defined(ILDEBUG)
ILDEBUG_BEGIN;
#if defined(WIN32)
		console.MakeNewLine();
		console << "#region ";
#else
		console << "ENTER ";
#endif
		console << _method.getFullName() << endl;
ILDEBUG_END;
#endif

		ILStack& stack = machine.stack;
		FrameList& frames = machine.frames;
		uint initialsize = frames.size();
		frames.push_back(Frame(machine,_method));
		void** pinit = (void**)&stack.top();

		while(frames.size()-initialsize>0) {
			Frame& frame = frames.back();
			const Method& method = frame.method;
			method.getTypeDef().MakeClassInitialized();
			if(method.IsRuntimeMethod()) {
				//goto on_return;
				if(method.getName()==L".ctor") {
					// デリゲートだけ。
					byte* obj = (byte*)stack.popn();
					void* pfunc = (void*)stack.popn();
					void* target = (void*)stack.popn();
					memclr(obj, method.getTypeDef().getInstanceSize());
					*(void**)(obj+method.getTypeDef().ResolveField(L"m_target",true)->getOffset()) = target;
					*(void**)(obj+method.getTypeDef().ResolveField(L"method_ptr",true)->getOffset()) = pfunc;
					stack.pushn(obj);
					goto on_return;
					/*
					Method* calling = AssemblyManager::FindAssembly(L"cscorlib")->FindTypeDef(L"Assist",L"CooS")->GetSingleMethod(L"ConstructDelegate",true);
					frames.pop_back();
					frames.push_back(Frame(machine,*calling));
					goto on_call;
					*/
				} else if(method.getName()==L"Invoke") {
					//TODO: 多分デリゲート
					int offset = method.getArgumentTotalSize()-sizeof(void*);
					void** pobj = (void**)(&stack.top()+ILStack::getLengthOnStack(offset));
					void* obj = *pobj;
					machine.stack.pusho(obj);
					AssemblyManager::Execute(L"cscorlib", L"CooS",L"Assist",L"GetDelegateTarget",machine);
					void* target = machine.stack.popo();
					if(target!=NULL) {
						*pobj = target;
					} else {
						memmove((nint*)&machine.stack.top()+1, &machine.stack.top(),
							method.getArgumentTotalSize()-sizeof(void*));
						machine.stack.popn();
					}
					machine.stack.pusho(obj);
					AssemblyManager::Execute(L"cscorlib", L"CooS",L"Assist",L"GetFunctionPointer",machine);
					void (*fp)() = (void(*)())machine.stack.popn();
					IL::Execute(machine, frame.assembly, fp, method.getSignature());
					goto on_return;
				} else {
					machine.panic("Runtime Method is called");
				}
			} else if(frame.code==NULL) {
				getConsole().MakeNewLine();
				if(method.HasThis()) {
					int offset = method.getArgumentTotalSize()-sizeof(void*);
					void* obj = (void*)stack[ILStack::getLengthOnStack(offset)];
					if(obj==NULL) {
						machine.panic("Blank method: object is null");
					} else {
						RuntimeTypeHandle handle = IL::GetHandleOfObject(obj);
						if(handle.TrackingField==NULL) {
							getConsole() << "Object: " << obj << endl;
							machine.panic("Blank method: typehandle is null");
						}
						const TypeDef* realtype = TypeDef::getTypeFromHandle(handle);
						getConsole() << "Object: " << realtype->getFullName() << endl;
					}
				}
				machine.panic("Blank method is called");
			} else if(!method.IsILMethod() && frame.pc==0) {
				IL::Execute(machine, frame.assembly, (void(*)())method.getCodeBase(), method.getSignature());
				goto on_return;
			} else {
				int& pc = frame.pc;
				if(pc==0) frame.allocWorkspace(machine.stack);
				const byte* codebase = frame.code;
				while(true) {
					if(pc<0) panic("Illegal execution");
					//Delay3(10);
					//console.MakeNewLine();
					const byte* p = codebase+pc;
					Instructions inst = (Instructions)*(p++);
					++instcount;
					switch(instcount) {
					case -1:
						clrpanic("hungup");
						break;
					case 0x00c22347:
						{
							int y = 9;
						}
						break;
					}
#if defined(ILTRACE)
ILDEBUG_BEGIN;
					console << "\r";
					//console << itos<char,16>(instcount,5,'_') << " ";
					console << "[" << itos<char,16>(pc,3) << "] ";
					//console << (byte)inst << " ";
					console << "(" << itos<char,16>(pinit-(void**)&stack.top(),3) << ") ";
					for(int c=1; c<nest; ++c) {
						console << "-";
					}
					for(int c=(int)frames.size()-1; c>=0; --c) {
						console << ((c%1) ? " " : "|");
					}
					console << " ";
ILDEBUG_END;
#endif
					if(inst==IL_ret) {
						goto on_return;
					} else {
						if(dispatch00[inst]==NULL) {
							console << "No instruction routine of " << (byte)inst << endl;
							panic("Execution error");
						}
						Method* calling = dispatch00[inst](machine,frame,p);
						if(calling!=NULL) {
#if defined(ILTRACE)
ILDEBUG_BEGIN;
#if defined(WIN32)
							console.MakeNewLine();
							console << "#region ";
#else
							console << "call ";
#endif
							console << calling->getFullName()
								//<< "[" << (int)calling->getLocalVarCount() << "]"
								<< "(" << (int)calling->getParamCount() << ")" << endl;
ILDEBUG_END;
#endif
							frames.push_back(Frame(machine,*calling));
							goto on_call;
						}
					}
				}
			}
			panic("ILEngine: DON'T REACH HERE");
on_call:
			continue;
on_return:
			stack.releaseFrame(frame.abp, method);
			frames.pop_back();
#if defined(WIN32) && defined(ILDEBUG)
ILDEBUG_BEGIN;
			console.MakeNewLine();
			console << "#endregion ";
			if(frames.size()>0) {
				console << "back to " << frames.back().method.getFullName();
			} else {
				console << "<shutdown>";
			}
			console << endl;
ILDEBUG_END;
#endif
		}
		nest--;
	}

	extern void Execute(const Method& method) {
		Execute(ILMachine::getCurrent(), method);
	}

	extern void Execute(ILMachine& machine, const Reflection::Assembly& assembly, void(*method)(), const Signature::MethodSig& signature) {
		Console& console = getConsole();
		ILStack& stack = machine.stack;
		if(method==NULL) panic("NULL pointer is executed as a function");

		bool retVoid = false;
		bool retEax = false;
		bool retEdx = false;
		bool retF4 = false;
		bool retF8 = false;
		if(signature.RetType.Void) {
			retVoid = true;
		} else if(signature.RetType.ByRef || signature.RetType.TypedByRef) {
			retEax = true;
		} else {
			switch(signature.RetType.Type->ElementType) {
			case ELEMENT_TYPE_VALUETYPE:
				/*
				const TypeDef* typdef;
				typdef = assembly.ResolveTypeDef(*signature.RetType.Type);
				*/
				switch(assembly.CalcSizeOfType(*signature.RetType.Type)) {
				case 1:
				case 2:
				case 4:
					retEax = true;
					break;
				case 8:
					retEdx = true;
					break;
				default:
					panic("Illigal size of return");
				}
				break;
			case ELEMENT_TYPE_I8:
			case ELEMENT_TYPE_U8:
				retEdx = true;
				break;
			case ELEMENT_TYPE_R4:
				retF4 = true;
				break;
			case ELEMENT_TYPE_R8:
				retF8 = true;
				break;
			default:
				retEax = true;
				break;
			}
		}

#if defined(ILTRACE)
ILDEBUG_BEGIN;
		getConsole() << "Executes native method " << (void*)method << endl;
ILDEBUG_END;
#endif

		byte* buf = NULL;
		bool alloced = false;
		LayoutInfo arginfo = signature.BuildArgLayout(assembly);
		int totalsize = arginfo.totalsize;

		switch(signature.Flags&0xF) {
		default:
			panic("Unsupported Calling Convention");
		case Signature::C_LANG:
		case Signature::STDCALL:
			{
				/*
				__asm {
					sub esp, totalsize;
					mov buf, esp;
				}
				alloced = false;
				/*///
				buf = new byte[totalsize];
				alloced = true;
				//*/
				for(int i=arginfo.sizes.size()-1; i>=0; --i) {
					int size = ILStack::getSizeOnStack(arginfo.sizes[i]);
					int offset = arginfo.offsets[i];
					const void* psrc = (byte*)&stack.top()+offset;
					void* pdst = buf+totalsize-offset-size;
					memcpy(pdst, psrc, size);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
					console.MakeNewLine();
					console << "Param: " << MemoryRegion(pdst, size);
ILDEBUG_END;
#endif
				}
			}
			break;
		case Signature::DEFAULT:
			buf = (byte*)&stack.top();
			alloced = false;
			break;
		}

#if defined(ILTRACE)
ILDEBUG_BEGIN;
		console.MakeNewLine();
		console << MemoryRegion(buf, totalsize);
ILDEBUG_END;
#endif

		// offset means the number of double.
		int cnt = totalsize/sizeof(void*);

#define BUILDSTACK \
	void* _save_sp;					\
	__asm { mov _save_sp, esp }		\
	__asm { sub esp, totalsize }	\
	__asm { mov ecx, cnt }			\
	__asm { mov esi, buf }			\
	__asm { mov edi, esp }			\
	__asm { cld }					\
	__asm { rep movsd }
#define RESTORESTACK \
	__asm { mov esp, _save_sp }

		if(retEax) {
			BUILDSTACK;
			int ret = ((int(*)())method)();
			RESTORESTACK;
			stack.pushn(ret);
		} else if(retVoid) {
			BUILDSTACK;
			((void(*)())method)();
			RESTORESTACK;
		} else if(retF8) {
			BUILDSTACK;
			double ret = ((double(*)())method)();
			RESTORESTACK;
			stack.pushr8(ret);
		} else if(retEdx) {
			BUILDSTACK;
			__int64 ret = ((__int64(*)())method)();
			RESTORESTACK;
			stack.pushl(ret);
		} else if(retF4) {
			BUILDSTACK;
			float ret = ((float(*)())method)();
			RESTORESTACK;
			stack.pushr4(ret);
		} else {
			panic("Can't determine the treatment of returned value");
		}

		if(alloced) delete [] buf;

	}

	extern void Execute(const Reflection::Assembly& assembly, void(*method)(), const Signature::MethodSig& signature) {
		Execute(ILMachine::getCurrent(), assembly, method, signature);
	}

}


using namespace IL;

extern int TopValueSize = 0;
extern ELEMENT_TYPE TopValueType = ELEMENT_TYPE_END;

extern Method* execute_default(ILMachine& machine, Frame& frame, const byte* operand) {
	Console& console = getConsole();
	console << "unsupport instruction: " << operand[-1] << endl;
	console << "    at " << frame.pc << " byte offset (" << (frame.method.getCodeBase()+frame.pc) << ")" << endl;
	console << "    in " << frame.method.getFullName() << endl;
	panic("IL EXECUTION FATAL ERROR");
}

extern Method* execute_nop(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "nop" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_break(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "break" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_throw(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	void* obj = stack.popo();
	RuntimeTypeHandle handle = IL::GetHandleOfObject(obj);
	const TypeDef* type = TypeDef::getTypeFromHandle(handle);
	getConsole().MakeNewLine();
	machine.stack.pusho(obj);
	AssemblyManager::Execute(L"cscorlib",L"CooS",L"Assist",L"DumpException",machine);
	getConsole() << "Stack Frames:" << endl;
	machine.panic("THROW");
}

extern Method* execute_prefix1(ILMachine& machine, Frame& frame, const byte* operand) {
	Instructions inst = (Instructions)*operand;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	if(dispatchFE[inst]==NULL) {
		getConsole() << "No instruction routine of 0xFE-" << (byte)inst << endl;
		panic("Execution error");
	}
ILDEBUG_END;
#endif
	return IL::dispatchFE[inst](machine, frame, operand+1);
}

extern Method* execute_localloc(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ILStack& stack = machine.stack;
	unint size = stack.popn();
	byte* buf = new byte[size];
	if(frame.lalloc==NULL) {
		frame.lalloc = new std::list<byte*>();
	}
	frame.lalloc->push_back(buf);
	stack.pushn(buf);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "localloc " << stack << ":" << (int)size << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_pop(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "pop " << stack << endl;
ILDEBUG_END;
#endif
	switch(stack.getDataType()) {
	case STACK_TYPE_V:
#if defined(ILWARN)
		getConsole() << "{pop:VALUETYPE}";
#endif
	case STACK_TYPE_N:	stack.releasemem(sizeof(nint));		break;
	case STACK_TYPE_I4:	stack.releasemem(sizeof(int32));	break;
	case STACK_TYPE_I8:	stack.releasemem(sizeof(int64));	break;
	case STACK_TYPE_R4:	stack.releasemem(sizeof(float));	break;
	case STACK_TYPE_R8:	stack.releasemem(sizeof(double));	break;
	case STACK_TYPE_O:	stack.releasemem(sizeof(void*));	break;
	case STACK_TYPE_P:	stack.releasemem(sizeof(void*));	break;
	default:
#if defined(ILWARN)
		getConsole() << "{pop:UNK}";
#endif
		stack.releasemem(sizeof(nint));
	}
	return NULL;
}

extern Method* execute_dup(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 1;
	ILStack& stack = machine.stack;
	switch(stack.getDataType()) {
	case STACK_TYPE_V:
#if defined(ILWARN)
		getConsole() << "{dup:VALUETYPE}";
#endif
	case STACK_TYPE_N:	stack.pushn(*(nint*)&stack.top());		break;
	case STACK_TYPE_I4:	stack.pushi(*(int32*)&stack.top());		break;
	case STACK_TYPE_I8:	stack.pushl(*(int64*)&stack.top());		break;
	case STACK_TYPE_R4:	stack.pushr4(*(float*)&stack.top());	break;
	case STACK_TYPE_R8:	stack.pushr8(*(double*)&stack.top());	break;
	case STACK_TYPE_O:	stack.pusho(*(void**)&stack.top());		break;
	case STACK_TYPE_P:	stack.pushp(*(void**)&stack.top());		break;
	default:
#if defined(ILWARN)
		getConsole() << "{dup:UNK}";
#endif
		stack.pushn(*(nint*)&stack.top());
	}
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "dup" << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_initblk(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ILStack& stack = machine.stack;
	uint32 size = (uint32)stack.popi();
	uint8 value = (uint8)stack.popi();
	void* address = (void*)stack.popn();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
getConsole() << "initblk " << address << ":" << (int)size << " of " << value << endl;
ILDEBUG_END;
#endif
	memset(address, value, size);
	return NULL;
}

extern Method* execute_cpblk(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	ILStack& stack = machine.stack;
	uint32 size = (uint32)stack.popi();
	void* srcadr = (void*)stack.popn();
	void* dstadr = (void*)stack.popn();
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "cpblk from " << srcadr << " to " << dstadr << " of " << (int)size << " bytes" << endl;
ILDEBUG_END;
#endif
	memcpy(dstadr, srcadr, size);
	return NULL;
}
