#pragma once

#include "interop.h"
#include "kernel.h"
#include "memory.h"
#include "ilengine.h"
#include "reflection.h"


struct KernelImpl : IKernel {
	virtual void* __stdcall alloc(unsigned int size) {
		return new byte[size];
	}
	virtual void __stdcall free(void* p) {
		delete [] (byte*)p;
	}
	virtual void __stdcall SetDebugMode(bool enabled) {
		if(enabled) IL::EnableDebug(); else IL::DisableDebug();
	}
	virtual void __stdcall LoadAssembly(void* assembly, void* p, int size) {
		Reflection::Assembly* pasm = AssemblyManager::LoadAssembly(p);
		pasm->setManagedType(assembly);
		pasm->LinkVTableFixups();
	}
	virtual void* __stdcall GetExecutingAssembly(int depth) {
		const IL::ILMachine& machine = IL::ILMachine::getCurrent();
		if(machine.frames.size()<4+(uint)depth) return NULL;
		IL::FrameList::const_iterator it = machine.frames.end();
		for(int i=0; i<4+depth; ++i) {
			--it;
		}
		return it->assembly.getManagedType();
	}
	virtual void __stdcall ReloadMethodCode(void* handle, int rowIndex, const unsigned char* p) {
		Reflection::TypeDef* typdef = Reflection::TypeDef::getTypeFromHandle(Reflection::RuntimeTypeHandle(handle));
		Reflection::Method& method = typdef->getAssembly().getMethod(rowIndex);
		method.ReloadMethodCode(p);
	}
	virtual void* __stdcall GetTypeFromHandle(void* handle) {
		return Reflection::TypeDef::getTypeFromHandle(Reflection::RuntimeTypeHandle(handle))->getManagedType();
	}
	virtual void* __stdcall CreateInstance(void* handle) {
		return IL::CreateInstance(*Reflection::TypeDef::getTypeFromHandle(IL::RuntimeTypeHandle(handle)));
	}
	virtual void* __stdcall CloneInstance(void* obj) {
		return IL::CloneInstance(obj);
	}
	virtual void* __stdcall CreateString(unsigned int length) {
		return IL::NewString(length);
	}
	virtual void* __stdcall CreateArray(void* elementHandle, unsigned int length) {
		return IL::NewArray(*Reflection::TypeDef::getTypeFromHandle(elementHandle), length);
	}
	virtual void* __stdcall NotifyLoadingType(const wchar_t* asmname, int len, int typerid, void* obj) {
		std::wstring name;
		name.assign(asmname, len);
		Reflection::Assembly* assem = AssemblyManager::FindAssembly(name);
		if(assem==NULL) return 0;
		Reflection::TypeDef& typdef = assem->getTypeDef(typerid);
		typdef.NotifyLoadingType(obj);
		return typdef.getHandleFromType();
	}
	virtual const void* __stdcall GenerateProxyCode(const wchar_t* asmname, int len, int methodrid) {
		std::wstring name;
		name.assign(asmname, len);
		Reflection::Assembly* assem = AssemblyManager::FindAssembly(name);
		if(assem==NULL) return NULL;
		Reflection::Method& method = assem->getMethod(methodrid);
		return method.CreateProxyCode();
	}
};
