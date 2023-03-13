#include "stdafx.h"


namespace IL {

	ILStack::ILStack(uint size) {
		int count = size/sizeof(nint);
		mem = new nint[count];
		mem[count-1] = 0xABCDEF;
		sp = count-1;
	}

	ILStack::~ILStack() {
		delete [] mem;
	}

	STACK_TYPE ILStack::getStackType(ELEMENT_TYPE type) {
		switch(type) {
		case ELEMENT_TYPE_I:
		case ELEMENT_TYPE_U:
			return STACK_TYPE_N;
		case ELEMENT_TYPE_I1:
		case ELEMENT_TYPE_BOOLEAN:
		case ELEMENT_TYPE_U1:
		case ELEMENT_TYPE_I2:
		case ELEMENT_TYPE_CHAR:
		case ELEMENT_TYPE_U2:
		case ELEMENT_TYPE_I4:
		case ELEMENT_TYPE_U4:
			return STACK_TYPE_I4;
		case ELEMENT_TYPE_I8:
		case ELEMENT_TYPE_U8:
			return STACK_TYPE_I8;
		case ELEMENT_TYPE_R4:
			return STACK_TYPE_R4;
		case ELEMENT_TYPE_R8:
			return STACK_TYPE_R8;
		case ELEMENT_TYPE_CLASS:
		case ELEMENT_TYPE_OBJECT:
		case ELEMENT_TYPE_STRING:
		case ELEMENT_TYPE_ARRAY:
		case ELEMENT_TYPE_SZARRAY:
			return STACK_TYPE_O;
		case ELEMENT_TYPE_BYREF:
		case ELEMENT_TYPE_PTR:
			return STACK_TYPE_P;
		case ELEMENT_TYPE_VALUETYPE:
			return STACK_TYPE_V;
		default:
			panic("getStackType");
		}
	}

	uint ILStack::getSizeOnStack(uint size) {
		if(size>0xfffff) {
			panic("getSizeOnStack applied to larger size than 1MB: 0x"+itos<char,16>(size));
		}
		return (size+sizeof(nint)-1)&~(sizeof(nint)-1);
	}

	uint ILStack::getLengthOnStack(uint size) {
		if(size>0xfffff) {
			panic("getLengthOnStack applied to larger size than 1MB: 0x"+itos<char,16>(size));
		}
		return (size+sizeof(nint)-1)>>2;
	}

	STACK_TYPE ILStack::getDataType() const {
		return top0type;
	}
	
	void ILStack::setDataType(STACK_TYPE type) {
		top0type = type;
		//top1type = STACK_TYPE_Unk;
		//top2type = STACK_TYPE_Unk;
	}
	
	void ILStack::setDataType(ELEMENT_TYPE type) {
		setDataType(getStackType(type));
	}

	void* ILStack::alloc(uint count) {
		if(count>0x100000) panic("Too large memory allocation");
		if(sp<count) panic("StackOverflowException");
		return &mem[sp-=count];
	}

	void ILStack::pushtype(STACK_TYPE type) {
		top15type = top14type;
		top14type = top13type;
		top13type = top12type;
		top12type = top11type;
		top11type = top10type;
		top10type = top9type;
		top9type = top8type;
		top8type = top7type;
		top7type = top6type;
		top6type = top5type;
		top5type = top4type;
		top4type = top3type;
		top3type = top2type;
		top2type = top1type;
		top1type = top0type;
		top0type = type;
	}

	void ILStack::poptype() {
		top0type = top1type;
		top1type = top2type;
		top2type = top3type;
		top3type = top4type;
		top4type = top5type;
		top5type = top6type;
		top6type = top7type;
		top7type = top8type;
		top8type = top9type;
		top9type = top10type;
		top10type = top11type;
		top11type = top12type;
		top12type = top13type;
		top13type = top14type;
		top14type = top15type;
		top15type = STACK_TYPE_Unk;
	}

	void ILStack::pushi(int32 n)	{
		*(int32*)alloc(1)=n;
		pushtype(STACK_TYPE_I4);
	}

	void ILStack::pushl(int64 n)	{
		*(int64*)alloc(2)=n;
		pushtype(STACK_TYPE_I8);
	}

	void ILStack::pushn(nint n)	{
		*(nint*)alloc(1) = n;
		pushtype(STACK_TYPE_N);
	}

	void ILStack::pushn(const void* p)	{
		*(void**)alloc(1) = (void*)p;
		pushtype(STACK_TYPE_N);
	}

	void ILStack::pushr4(float n)	{
		*(float*)alloc(1) = n;
		pushtype(STACK_TYPE_R4);
	}

	void ILStack::pushr8(double n)	{
		*(double*)alloc(sizeof(double)/sizeof(nint)) = n;
		pushtype(STACK_TYPE_R8);
	}

	void ILStack::pusho(void* p)	{
		*(void**)alloc(1) = p;
		pushtype(STACK_TYPE_O);
	}

	void ILStack::pushp(void* p)	{
		*(void**)alloc(1) = p;
		pushtype(STACK_TYPE_P);
	}

	void ILStack::pushmem(const void* p, uint size) {
		if(size==0) return;
		uint len = getLengthOnStack(size);
		nint* m = (nint*)alloc(len);
		m[len-1] = 0;
		memcpy(m, p, size);
		pushtype(STACK_TYPE_V);
	}

	void ILStack::pushmem(const void* p, ELEMENT_TYPE type, uint size) {
		switch(type) {
		case ELEMENT_TYPE_I:
		case ELEMENT_TYPE_U:
			pushn(*(nint*)p);
			break;
		case ELEMENT_TYPE_I1:
			pushi(*(char*)p);
			break;
		case ELEMENT_TYPE_BOOLEAN:
		case ELEMENT_TYPE_U1:
			pushi(*(byte*)p);
			break;
		case ELEMENT_TYPE_I2:
			pushi(*(int16*)p);
			break;
		case ELEMENT_TYPE_CHAR:
		case ELEMENT_TYPE_U2:
			pushi(*(uint16*)p);
			break;
		case ELEMENT_TYPE_I4:
			pushi(*(int32*)p);
			break;
		case ELEMENT_TYPE_U4:
			pushi(*(uint32*)p);
			break;
		case ELEMENT_TYPE_I8:
		case ELEMENT_TYPE_U8:
			pushl(*(int64*)p);
			break;
		case ELEMENT_TYPE_R4:
			pushr4(*(float*)p);
			break;
		case ELEMENT_TYPE_R8:
			pushr8(*(double*)p);
			break;
		case ELEMENT_TYPE_CLASS:
		case ELEMENT_TYPE_OBJECT:
		case ELEMENT_TYPE_STRING:
		case ELEMENT_TYPE_ARRAY:
		case ELEMENT_TYPE_SZARRAY:
			pusho(*(void**)p);
			break;
		case ELEMENT_TYPE_BYREF:
		case ELEMENT_TYPE_PTR:
		case ELEMENT_TYPE_FNPTR:
			pushp(*(void**)p);
			break;
		case ELEMENT_TYPE_VALUETYPE:
			pushmem(p, size);
			break;
		default:
			panic("ILStack: Unknown ELEMENT_TYPE");
		}
	}

	void ILStack::pushmem(const void* p, STACK_TYPE type, uint size) {
		switch(type) {
		case STACK_TYPE_N:
			pushn(*(nint*)p);
			break;
		case STACK_TYPE_I4:
			pushi(*(int32*)p);
			break;
		case STACK_TYPE_I8:
			pushl(*(int64*)p);
			break;
		case STACK_TYPE_R4:
			pushr4(*(float*)p);
			break;
		case STACK_TYPE_R8:
			pushr8(*(double*)p);
			break;
		case STACK_TYPE_O:
			pusho(*(void**)p);
			break;
		case STACK_TYPE_P:
			pushp(*(void**)p);
			break;
		case STACK_TYPE_V:
			pushmem(p, size);
			break;
		default:
			panic("ILStack: Unknown STACK_TYPE");
		}
		pushtype(type);
	}

	int32 ILStack::popi() {
		poptype();
		return mem[sp++];
	}

	int64 ILStack::popl() {
		int64 val = *(int64*)(mem+sp);
		sp += 2;
		poptype();
		return val;
	}

	nint ILStack::popn() {
		poptype();
		return mem[sp++];
	}

	float ILStack::popr4() {
		float val = *(float*)(mem+sp);
		sp += 1;
		poptype();
		return val;
	}

	double ILStack::popr8() {
		double val = *(double*)(mem+sp);
		sp += 2;
		poptype();
		return val;
	}

	void* ILStack::popo() {
		poptype();
		return (void*)mem[sp++];
	}

	void* ILStack::popp() {
		poptype();
		return (void*)mem[sp++];
	}

	void ILStack::popmem(void* p, uint size) {
		memcpy(p, mem+sp, size);
		sp += getLengthOnStack(size);
		poptype();
	}

	void* ILStack::allocmem(uint size) {
		pushtype(STACK_TYPE_Unk);
		return alloc(getLengthOnStack(size));
	}

	void ILStack::releasemem(uint size) {
		if((int)size<0) panic("Maybe minus size will be released from ILStack.");
		sp += getLengthOnStack(size);
		poptype();
	}

	const nint& ILStack::top() const {
		return mem[sp];
	}

	const nint& ILStack::operator [](uint index) const {
		return mem[sp+index];
	}

	Console& ILStack::Dump(Console& console) const {
		console << "value=";
		switch(getDataType()) {
		case STACK_TYPE_I8:
		case STACK_TYPE_R8:	
			console << *(const uint64*)&top();
			break;
		default:
			console << (unint)top();
			break;
		}
		console << "/" << (byte)getDataType() << ", adr=" << &top();
		return console;
	}

	void ILStack::buildFrame(const Reflection::Method& method, const void* p) {
		if(method.getArgumentCount()==0) return;
		pushmem(p, ELEMENT_TYPE_VALUETYPE, method.getArgumentTotalSize());
		poptype();
		for(uint iarg=0; iarg<method.getArgumentCount(); ++iarg) {
			pushtype(IL::ILStack::getStackType(method.getArgumentElemType(iarg)));
		}
	}

	void ILStack::releaseFrame(const void* ptoparg, int argsize, int retsize, const Signature::MethodSig& signature) {
		const byte* pretval = (byte*)&this->top();
		ptoparg = (const byte*)ptoparg+argsize;
		int discardsize = ((const byte*)ptoparg-pretval)-retsize;
		if(discardsize>0) {
			if(retsize>0) memmove((byte*)pretval+discardsize, pretval, retsize);
			this->releasemem(discardsize);
		}
		if(retsize>0) this->setDataType(signature.RetType.Type->ElementType);
#if defined(ILTRACE)
ILDEBUG_BEGIN;
		Console& console = getConsole();
		if(retsize>0) {
			console << "return " << *this << endl;
		} else {
			console << "return pop " << discardsize << " bytes" << endl;
		}
ILDEBUG_END;
#endif
	}

	void ILStack::releaseFrame(const void* ptoparg, const Signature::MethodSig& signature, const Reflection::Assembly& assembly) {
		int argsize = signature.BuildArgLayout(assembly).totalsize;
		int retsize = IL::ILStack::getSizeOnStack(assembly.CalcSizeOfType(signature.RetType));
		releaseFrame(ptoparg, argsize, retsize, signature);
	}

	void ILStack::releaseFrame(const void* ptoparg, const Reflection::Method& method) {
		releaseFrame(ptoparg, method.getArgumentTotalSize(), method.getReturnSize(), method.getSignature());
	}

}
