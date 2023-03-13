#include "Method.h"
#include "Param.h"
#include "../ilengine.h"
#include "../AssemblyManager.h"


namespace Reflection {

#include "../enalign.h"
#include "../enpack.h"

	struct MethodFatHeader {
		uint16	Flags	: 12;
		uint16	Size	: 4;
		uint16	MaxStack;
		uint32	CodeSize;
		uint32	LocalVarSigTok;
	};

#include "../unpack.h"
#include "../unalign.h"

	Method::Method(Assembly& parent, uint tableIndex, const Metadata::Method& m, const Metadata::Method* pm2) : TypeDefItem(parent,tableIndex) {
		const Metadata::MainStream& ms = getRoot().getMainStream();
		const Metadata::StringStream& ss = getRoot().getStringStream();
		const Metadata::BlobStream& blob = getRoot().getBlobStream();
		name = ss.getString(m.Name);
		if(m.RVA==0) {
			code = NULL;
		} else {
			code = getAssembly().getImageBase()+m.RVA;
		}
		flags = (MethodAttributes::Type)m.Flags;
		implFlags = (MethodImplAttributes::Type)m.ImplFlags;
		// determine flags
		switch(flags & MethodAttributes::MemberAccessMask) {
		default:
			panic("Illegal MemberAccess");
		case MethodAttributes::CompilerControlled:
			fAccess = CompilerControlled;
			break;
		case MethodAttributes::Private:
			fAccess = Private;
			break;
		case MethodAttributes::FamANDAssem:
			fAccess = FamAndAssem;
			break;
		case MethodAttributes::Assem:
			fAccess = Assem;
			break;
		case MethodAttributes::Family:
			fAccess = Family;
			break;
		case MethodAttributes::FamORAssem:
			fAccess = FamOrAssem;
			break;
		case MethodAttributes::Public:
			fAccess = Public;
			break;
		}
		if(flags & MethodAttributes::Static) {
			fInstantiation = Static;
		} else {
			fInstantiation = Instance;
		}
		if(flags & MethodAttributes::Final) {
			fModification = Final;
		} else if(flags & MethodAttributes::Virtual) {
			fModification = Virtual;
		} else if(flags & MethodAttributes::Abstract) {
			fModification = Abstract;
		} else {
			fModification = Plain;
		}
		if(flags & MethodAttributes::HideBySig) {
			fHidingRule = HideBySig;
		} else {
			fHidingRule = HideByName;
		}
		if(flags & MethodAttributes::NewSlot) {
			fSlotUsage = NewSlot;
		} else {
			fSlotUsage = ReuseSlot;
		}
		switch(implFlags & MethodImplAttributes::ManagedMask) {
		default:
			panic("Illegal ManagedType");
		case MethodImplAttributes::Managed:
			fMethodType = Managed;
			break;
		case MethodImplAttributes::Unmanaged:
			fMethodType = Unmanaged;
			break;
		}
		fImplementationInfo = (ImplementationInfoFlags)0;
		if(implFlags & MethodImplAttributes::InternalCall) {
			fImplementationInfo = InternalCall;
		}
		if(code==NULL || !IsILMethod()) {
			methodFlags = (CorILMethodFlags)0;
			codeSize = 0;
			maxStack = 0;
		} else {
			// detect Method Header
			switch(*code & 0x3) {
			case 2:	// Tiny format
				methodFlags = (CorILMethodFlags)(*code&0x3);
				codeSize = *code >> 2;
				maxStack = 8;
				code += 1;
				break;
			case 3:	// Fat format
				MethodFatHeader fathdr;
				fathdr = *(MethodFatHeader*)code;
				methodFlags = (CorILMethodFlags)fathdr.Flags;
				maxStack = fathdr.MaxStack;
				codeSize = fathdr.CodeSize;
				if(fathdr.LocalVarSigTok==0) {
					// No local variables.
				} else {
					Metadata::MetadataToken token = fathdr.LocalVarSigTok;
					Metadata::Table* row = ms.getRow(token);
					if(token.table==TABLE_StandAloneSig) {
						Metadata::StandAloneSig& sas = *(Metadata::StandAloneSig*)row;
						if(!localVarSig.Parse(Signature::SignatureStream(blob+sas.Signature))) {
							getConsole() << MemoryRegion(blob+sas.Signature,64);
							panic(L"Failed to parse LocalVarSig of "+getFullName());
						}
					} else {
						panic(L"LocalVarSig indicates Non StandAloneSig table: "+itos<wchar_t,16>((int)token.table));
					}
					delete row;
				}
				code += 12;
				break;
			}
		}
		EHClauseData = NULL;
		EHClauseDataLength = 0;
		isEHClauseDataFat = true;
		if(methodFlags&CorILMethod_MoreSects) {
			const byte* extra = code+codeSize;
			CorILMethodSectFlags sectFlags;
			do {
				extra = (byte*)((uint)(extra+3)&~3);
				sectFlags = (CorILMethodSectFlags)*extra;
				if(sectFlags&~0xC3) panic("Invalid method section flags");
				if(0==(sectFlags&3)) panic("Invalid method section flags");
				if(3==(sectFlags&3)) panic("Invalid method section flags");
				uint length;
				if(sectFlags&CorILMethodSect_FatFormat) {
					length = *(uint32*)extra >> 8;
					length -= 4;
					extra += 4;
				} else {
					length = extra[1];
					length -= 2;
					extra += 2;
				}
				if(sectFlags & CorILMethodSect_EHTable) {
					if(sectFlags&CorILMethodSect_FatFormat) {
						isEHClauseDataFat = true;
					} else {
						isEHClauseDataFat = false;
						length -= 2;
						extra += 2;
					}
					EHClauseData = extra;
					EHClauseDataLength = length;
				} else if(sectFlags & CorILMethodSect_OptILTable) {
					panic("Method::Method detects CorILMethodSect_OptILTable");
				} else {
					panic("Method::Method detects a unknown extra data section: "+itos<char,16>((int)sectFlags));
				}
				extra += length;
			} while(sectFlags & CorILMethodSect_MoreSects);
		}
		// Signature
		if(!signature.Parse(Signature::SignatureStream(blob+m.Signature))) {
			getConsole() << MemoryRegion(blob+m.Signature,32);
			panic(L"Failed to parse a signature for "+getFullName());
		}
		if(IsNativeMethod() && signature.Default()) {
			//TODO: CLI does NOT specify __cdecl or __stdcall in MethodDefSig, but MethodRefSig.
			signature.Flags = (Signature::CallingConventionFlags)(signature.Flags|Signature::C_LANG);
		}
		// Parameters
		argcount = signature.Params.size();
		if(HasThis()) ++argcount;
		// Layout
		retsize = -1;
		ready_arginfo = false;
		ready_varinfo = false;
	}

	Method::~Method() {
	}

	TypeDef* Method::ResolveTypeDef() const {
		return getAssembly().getTypeDefOfMethod(getTableIndex());
	}

	void Method::DetermineArgumentLayout() {
		arginfo = signature.BuildArgLayout(getAssembly());
		if(this->IsConstructor()) {
			for(uint i=0; i<arginfo.offsets.size(); ++i) {
				arginfo.offsets[i] += sizeof(void*);
			}
			arginfo.offsets[0] = 0;
		}
		ready_arginfo = true;
		/*
		Console& console = getConsole();
		for(uint i=0; i<argumentLayout.size(); ++i) {
			console.MakeNewLine();
			console << "Param[" << (int)i
				<< "]: " << argumentLayout[i].first
				<< ":" << (uint16)argumentLayout[i].second << endl;
		}
		//*/
	}

	void Method::DetermineLocalVarLayout() {
		varinfo = localVarSig.BuildVarLayout(getAssembly());
		ready_varinfo = true;
		/*
		Console& console = getConsole();
		for(uint i=0; i<localVarLayout.size(); ++i) {
			console.MakeNewLine();
			console << "LocalVar[" << (int)i
				<< "]: " << localVarLayout[i].first
				<< ":" << (uint16)localVarLayout[i].second << endl;
		}
		//*/
	}

	void Method::SetCode(const byte* code, MethodImplAttributes::Type flags) {
		this->code = code;
		this->implFlags = flags;
	}

	std::wstring Method::getFullName() const {
		return getTypeDef().getNamespace()+L"."+getTypeDef().getName()+L":"+name;
	}
	
	int Method::getReturnSize() const {
		if(retsize<0) {
			const Signature::RetType& rettype = getSignature().RetType;
			retsize = IL::ILStack::getSizeOnStack(getAssembly().CalcSizeOfType(rettype));
			if(retsize>1024) {
				Console& console = getConsole();
				console.MakeNewLine();
				console << "Method = " << getFullName() << endl;
				console << "Return Size = " << retsize << endl;
				clrpanic("Methrod return: Return size is too large");
			}
		}
		return retsize;
	}

	uint Method::getParamCount() const {
		return signature.Params.size();
	}

	uint Method::getArgumentCount() const {
		return argcount;
	}
	
	uint Method::getLocalVarCount() const {
		return localVarSig.LocalVars.size();
	}

	ELEMENT_TYPE Method::getArgumentElemType(uint index) const {
		if(HasThis()) {
			if(index==0) {
				return ELEMENT_TYPE_CLASS;
			}
			--index;
		}
		const Signature::ParamSig& paramsig = getSignature().Params[index];
		if(paramsig.TypedByRef) return ELEMENT_TYPE_TYPEDBYREF;
		if(paramsig.ByRef) return ELEMENT_TYPE_BYREF;
		return paramsig.Type.ElementType;
	}

	int Method::getArgumentOffset(uint index) const {
		if(!ready_arginfo) const_cast<Method*>(this)->DetermineArgumentLayout();
		return arginfo.offsets[index];
	}
	
	uint Method::getArgumentSize(uint index) const {
		if(!ready_arginfo) const_cast<Method*>(this)->DetermineArgumentLayout();
		return arginfo.sizes[index];
	}
	
	uint Method::getArgumentTotalSize() const {
		if(!ready_arginfo) const_cast<Method*>(this)->DetermineArgumentLayout();
		return arginfo.totalsize;
	}
	
	ELEMENT_TYPE Method::getLocalVarElemType(uint index) const {
		const Signature::LocalVar& var = localVarSig.LocalVars[index];
		if(var.TypedByRef) return ELEMENT_TYPE_TYPEDBYREF;
		if(var.ByRef) return ELEMENT_TYPE_BYREF;
		return var.Type.ElementType;
	}

	int Method::getLocalVarOffset(uint index) const {
		if(!ready_varinfo) const_cast<Method*>(this)->DetermineLocalVarLayout();
		return varinfo.offsets[index];
	}

	uint Method::getLocalVarSize(uint index) const {
		if(!ready_varinfo) const_cast<Method*>(this)->DetermineLocalVarLayout();
		return varinfo.sizes[index];
	}

	uint Method::getLocalVarTotalSize() const {
		if(!ready_varinfo) const_cast<Method*>(this)->DetermineLocalVarLayout();
		return varinfo.totalsize;
	}

	uint Method::getEHClauseCount() const {
		if(isEHClauseDataFat) {
			return EHClauseDataLength/sizeof(ExceptionHandlingClause);
		} else {
			return EHClauseDataLength/sizeof(ExceptionHandlingClauseSmall);
		}
	}

	ExceptionHandlingClause Method::getEHClause(uint index) const {
		if(isEHClauseDataFat) {
			return ((ExceptionHandlingClause*)EHClauseData)[index];
		} else {
			ExceptionHandlingClauseSmall& ehc = ((ExceptionHandlingClauseSmall*)EHClauseData)[index];
			ExceptionHandlingClause val;
			val.Flags			= ehc.Flags;
			val.TryOffset		= ehc.TryOffset;
			val.TryLength		= ehc.TryLength;
			val.HandlerOffset	= ehc.HandlerOffset;
			val.HandlerLength	= ehc.HandlerLength;
			val.ClassToken		= ehc.ClassToken;
			return val;
		}
	}

	bool Method::getProperEHClause(ExceptionHandlingClause* clause, uint ip) const {
		for(uint i=0; i<getEHClauseCount(); ++i) {
			*clause = getEHClause(i);
			if(clause->TryOffset<=ip && ip<clause->TryOffset+clause->TryLength) {
				return true;
			}
		}
		return false;
	}

	void Method::ReloadMethodCode(const byte* code) {
		if(code!=NULL) {
			getConsole() << "Reload code> " << this->getFullName() << endl;
			this->SetCode(code, (MethodImplAttributes::Type)(MethodImplAttributes::Native|MethodImplAttributes::Managed));
		}
	}

	// マネージからの間接呼び出し用
	static __int64 __stdcall ftn_proxy(Method* method) {
		IL::ILMachine& machine = IL::ILMachine::getCurrent();
		//IL::ILMachine machine;	// 割り込み関係で排他制御できない
		const nint* ptop = &machine.stack.top();
		switch(method->getSignature().getCallingConv()) {
		default:
			panic("Unsupported Calling Convention");
		case Signature::STDCALL:
		case Signature::C_LANG:
		case Signature::THISCALL:
		case Signature::FASTCALL:
			panic("Unsupported Calling Convention");
		case Signature::DEFAULT:
			{
				void* p = (byte*)&method + sizeof(void*)*2; // 'method' and EIP are skipped.
				machine.stack.buildFrame(*method, p);
			}
			break;
		}
		IL::Execute(machine, *method);
		if(!method->HasReturn()) {
			return 0;
		} else {
			int retsize = method->getReturnSize();
			if(retsize<=4) {
				return machine.stack.popi();
			} else if(retsize<=8) {
				return machine.stack.popl();
			} else {
				panic("ftn_proxy> unsupported returning value");
			}
		}
	}

	///ネイティブからの復帰用
	static __int64 __stdcall ftn_bridge_from_stdcall(Method* method) {
		IL::ILMachine& machine = IL::ILMachine::getCurrent();
		//IL::ILMachine machine;	// 割り込み関係で排他制御できない
		const nint* ptop = &machine.stack.top();
		switch(method->getSignature().Flags) {
		case Signature::STDCALL:
		case Signature::C_LANG:
			{
				void* p = (byte*)&method + sizeof(void*)*2; // EBP and EIP are skipped.
				machine.stack.buildFrame(*method, p);
			}
			break;
		case Signature::THISCALL:
		case Signature::FASTCALL:
			panic("Unsupported Calling Convention");
		case Signature::DEFAULT:
			// Cの呼び出し規約と.NETのは逆順。
			int offset;
			offset = 0;
			for(uint iarg=0; iarg<method->getArgumentCount(); ++iarg) {
				int size = method->getArgumentSize(iarg);
				void* p = (byte*)&method + sizeof(void*)*2 + offset;
				machine.stack.pushmem(p, method->getArgumentElemType(iarg), size);
				offset += IL::ILStack::getSizeOnStack(size);
			}
			break;
		}
		IL::Execute(machine, *method);
		if(!method->HasReturn()) {
			return 0;
		} else {
			int retsize = method->getReturnSize();
			if(retsize<=4) {
				return machine.stack.popi();
			} else if(retsize<=8) {
				return machine.stack.popl();
			} else {
				panic("ftn_bridge> unsupported returning value");
			}
		}
	}

	#include "../enpack.h"
	#include "../enalign.h"

	struct PROXYCODE {
		byte	push_opcode;
		void*	push_oprand;
		byte	mov_opcode;
		void*	mov_oprand;
		byte	call0;
		byte	call1;
		byte	ret0;
		short	ret1;
	};

	#include "../unpack.h"
	#include "../unalign.h"

	const void* Method::CreateProxyCode() const {
		if(!IsILMethod() && getCodeBase()!=NULL) {
			return getCodeBase();
		} else {
			// スタックの操作があるのでネイティブ関数も一度インタープリタを経由させる。
			if(this->IsConstructor()) panic("Don't create a proxy of constructor.");
			//TODO: Leak
			PROXYCODE* proxy = new PROXYCODE();
			proxy->push_opcode = 0x68;
			proxy->push_oprand = (void*)this;
			proxy->mov_opcode = 0xB8;
			proxy->mov_oprand = ftn_proxy;
			proxy->call0 = 0xFF;
			proxy->call1 = 0xD0;
			proxy->ret0 = 0xC2;
			proxy->ret1 = this->getArgumentTotalSize();
			//getConsole() << "PROXY " << proxy << endl;
			return (void*)proxy;
		}
	}

	const void* Method::CreateBridgeCode() const {
		// スタックの操作があるのでネイティブ関数も一度インタープリタを経由させる。
		//TODO: Leak
		PROXYCODE* proxy = new PROXYCODE();
		proxy->push_opcode = 0x68;
		proxy->push_oprand = (void*)this;
		proxy->mov_opcode = 0xB8;
		proxy->mov_oprand = ftn_bridge_from_stdcall;
		proxy->call0 = 0xFF;
		proxy->call1 = 0xD0;
		proxy->ret0 = 0xC2;
		proxy->ret1 = this->getArgumentTotalSize();
		//getConsole() << "BRIDGE " << proxy << endl;
		return (void*)proxy;
	}

}
