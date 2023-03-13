#include "stdafx.h"

using namespace Reflection;
using namespace IL;


extern Method* execute_newobj(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	Method* m = frame.assembly.ResolveMethod(token);
	if(m==NULL) {
		machine.panic("newobj: can't resolve constructor");
	} else if(m->IsInternalCallMethod()) {
		m = m->getTypeDef().ResolveMethod(L"InternalAllocateInstance", m->getSignature().Params);
		if(m==NULL) panic("newobj: InternalAllocateInstance not found");
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
		getConsole() << "newobj.altctor" << endl;
ILDEBUG_END;
#endif
		return m;
	} else {
		uint argsize = m->getArgumentTotalSize()-sizeof(void*);
		byte* buf = new byte[argsize];
		stack.popmem(buf, argsize);
		const TypeDef& typdef = m->getTypeDef();
		void* p;
		if(typdef.IsValueType()) {
			p = stack.allocmem(typdef.getInstanceSize());
		} else {
			p = IL::CreateInstance(typdef);
			stack.pusho(p);
#if defined(ILDEBUG)
			if(IL::GetHandleOfObject(p)==NULL) {
				machine.panic((L"Object is not typed: "+typdef.getFullName()).c_str());
			}
#endif
		}
		stack.pushmem(buf, STACK_TYPE_V, argsize);
		stack.pusho(p);
		delete [] buf;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
		getConsole() << "newobj obj=" << p << "+" << (int)m->getTypeDef().getInstanceSize() << endl;
ILDEBUG_END;
#endif
	}
	return m;
}

extern Method* execute_box(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) panic("box: Invalid TypeToken");
	if(!typdef->IsValueType()) machine.panic(L"box is executed for non-value type: "+typdef->getFullName()+L" : "+typdef->getBaseType()->getFullName());
	void* p = IL::CreateInstance(*typdef);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "box from " << stack << " to " << p << endl;
ILDEBUG_END;
#endif
	stack.popmem(p, typdef->getInstanceSize());
	stack.pusho(p);
	return NULL;
}

extern Method* execute_unbox(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token = *(uint32*)operand;
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) machine.panic("box: Invalid TypeToken");
	if(!typdef->IsValueType()) machine.panic("box is executed for non-value type.");
	void* obj = stack.popo();
	if(obj==NULL) machine.panic("unbox: NullReferenceException");
	// Unlike box, which is required to make a copy of a value type
	// for use in the object, unbox is not required to copy the value type
	// from the object. Typically it simply computes the address of the
	// value type that is already present inside of the boxed object.
	stack.pushp(obj);
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "unbox " << stack << " from " << obj << ":" << (int)typdef->getInstanceSize() << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_initobj(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token(*(uint32*)operand);
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) machine.panic("initobj can't find TypeDef");
	void* obj = stack.popp();
	memclr(obj, typdef->getInstanceSize());
	return NULL;
}

extern Method* execute_ldobj(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token(*(uint32*)operand);
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) machine.panic("ldobj can't find TypeDef");
	void* obj = stack.popo();
	stack.pushmem(obj, STACK_TYPE_V, typdef->getInstanceSize());
	return NULL;
}

extern Method* execute_stobj(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token(*(uint32*)operand);
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) machine.panic("stobj can't find TypeDef");
	uint size = typdef->getInstanceSize();
	void* addr = (void*)stack[ILStack::getLengthOnStack(size)];
	stack.popmem(addr, size);
	stack.popo();
	return NULL;
}

extern Method* execute_cpobj(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token(*(uint32*)operand);
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) machine.panic("cpobj can't find TypeDef");
	uint size = typdef->getInstanceSize();
	void* src = (void*)stack.popn();
	void* dst = (void*)stack.popn();
	memcpy(dst, src, size);
	return NULL;
}

extern Method* execute_sizeof(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 6;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token(*(uint32*)operand);
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) machine.panic("sizeof can't find TypeDef");
	stack.pushi(typdef->getInstanceSize());
	return NULL;
}

extern Method* execute_castclass(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	Metadata::MetadataToken token(*(uint32*)operand);
	const TypeDef* typdef = frame.assembly.ResolveTypeDef(token);
	if(typdef==NULL) panic("ldobj can't find TypeDef");
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "castclass //TODO: " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}

extern Method* execute_isinst(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 5;
	ILStack& stack = machine.stack;
	void* obj = stack.popo();
	const TypeDef* itype = TypeDef::getTypeFromHandle(IL::GetHandleOfObject(obj));
	Metadata::MetadataToken token = *(uint32*)operand;
	switch(token.table) {
	case TABLE_TypeDef:
	case TABLE_TypeRef:
	case TABLE_TypeSpec:
		{
			const TypeDef* ctype = frame.assembly.ResolveTypeDef(token);
			if(ctype==NULL) machine.panic("isinst can't find TypeDef");
			if(ctype->IsInterface()) {
				if(itype->IsInterfaceImpl(token.table, token.index)) {
					stack.pusho(obj);
				} else {
					stack.pusho(NULL);
				}
			} else {
				while(itype!=NULL) {
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
					getConsole().MakeNewLine();
					getConsole() << "instance of " << itype->getFullName() << endl;
ILDEBUG_END;
#endif
					if(itype==ctype) break;
					itype = itype->getBaseType();
				}
				if(itype==NULL) {
					stack.pusho(NULL);
				} else {
					stack.pusho(obj);
				}
			}
		}
		break;
	default:
		machine.panic("isinst");
	}
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
	getConsole() << "isinst //TODO: " << stack << endl;
ILDEBUG_END;
#endif
	return NULL;
}
