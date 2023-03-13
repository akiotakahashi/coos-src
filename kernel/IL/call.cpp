#include "stdafx.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_call(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	Metadata::MetadataToken token = *(uint32*)operand;
	Method* method = frame.assembly.ResolveMethod(token);
	if(method==NULL) machine.panic("call> can't find target method");
	if(method->IsConstructor()) {
		// コンストラクタだけは
		//		arg1, arg2, ..., arg0
		// なので、呼び出す前に並び順を変える。
		ILStack& stack = machine.stack;
		uint argsize = method->getArgumentTotalSize()-sizeof(void*);
		void* target = (void*)stack[ILStack::getLengthOnStack(argsize)];
		memmove((void*)&stack[1], (const void*)&stack[0], argsize);
		*(void**)&stack[0] = target;
	}
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
		getConsole() << "call ";
ILDEBUG_END;
#endif
	return method;
}

extern Method* execute_callvirt(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	Method* method = frame.assembly.ResolveMethod(token);
	if(method==NULL) machine.panic("callvirt> can't find target method");
	int offset = method->getArgumentTotalSize()-sizeof(void*);
	void* obj = (void*)stack[ILStack::getLengthOnStack(offset)];
	if(obj==NULL) {
		getConsole().MakeNewLine();
		getConsole() << "Method: " << method->getFullName() << endl;
		machine.panic("callvirt: object is null");
	}
	RuntimeTypeHandle handle = IL::GetHandleOfObject(obj);
	const TypeDef* realtype = TypeDef::getTypeFromHandle(handle);
	Method* realmethod = realtype->ResolveOverrideMethod(*method, false);
	if(realmethod==NULL) {
		machine.panic("callvirt can't find method maching signature");
	}
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "callvirt ";
ILDEBUG_END;
#endif
	return realmethod;
}

extern Method* execute_calli(ILMachine& machine, Frame& frame, const byte* operand) {
	typedef std::map<std::pair<Assembly*,uint32>,Signature::StandAloneMethodSig*> CacheMap;
	static CacheMap* cache = NULL;
	if(cache==NULL) cache = new CacheMap();
	frame.pc += 5;
	ILStack& stack = machine.stack;
	std::pair<Assembly*,uint32> key((Assembly*)&frame.assembly,*(uint32*)operand);
	Signature::StandAloneMethodSig* sams;

	CacheMap::iterator it = cache->find(key);
	if(it==cache->end()) {
		Metadata::MetadataToken token = *(uint32*)operand;
		Metadata::StandAloneSig* table = (Metadata::StandAloneSig*)frame.assembly.getRoot().getMainStream().getRow(token);
		Signature::SignatureStream ss(frame.assembly.getRoot().getBlobStream()+table->Signature);
		delete table;
		table = NULL;
		sams = new Signature::StandAloneMethodSig();
		if(!sams->Parse(ss)) {
			panic("calli can't parse StandAloneMethodSig");
		} else if(sams->ExtraParams.size()>0) {
			panic("VARARG is not supported");
		}
		(*cache)[key] = sams;
	} else {
		sams = it->second;
	}

#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "calli ";
ILDEBUG_END;
#endif

	void (*fp)() = (void (*)())stack.popp();
	const void* parg = &stack.top();
	IL::Execute(machine, frame.assembly, fp, *sams);
	stack.releaseFrame(parg, *sams, frame.assembly);
	return NULL;
}
