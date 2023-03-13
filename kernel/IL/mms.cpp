#include "stdafx.h"
#include "../ilengine.h"
#include "../assemblymanager.h"

using namespace Reflection;


namespace IL {

	static void* base;
	static void* limt;
	static byte* curr;

	struct Header {
		unint size;		// Whole size including this header.
		void* type;		// Type must be located at the end.
	};

	extern void Finalize() {
		getConsole() << "managed: " << (curr-(byte*)base) << " bytes in use" << endl;
	}

	extern void AssignMemory(void* address, uint size) {
		base = address;
		limt = (byte*)address+size-1;
		curr = (byte*)address;
		memclr(curr, sizeof(Header));
	}

	extern void SetLimitAddress(void* address) {
		limt = address;
	}

	extern void Synchronize() {
		IL::ILMachine& machine = IL::ILMachine::getCurrent();
		const nint* ptop = &machine.stack.top();
		machine.stack.pushn(base);
		machine.stack.pushn(limt);
		machine.stack.pushn(curr);
		AssemblyManager::Execute(L"cscorlib", L"CooS", L"Memory", L"BeSynchronized", machine);
		if(ptop!=&machine.stack.top()) panic("MACHINE STATCK DESTROYED");
	}

	static void* Allocate(void* managedType, uint size) {
		if(size>1024*1024*64) clrpanic("IL::Allocate> Too large!");
		size = (size+sizeof(void*)-1)&~(sizeof(void*)-1);
		Header* phdr;
		while((phdr = (Header*)curr)->size!=0) {
			if(phdr->size<0 || 64*1024*1024<=phdr->size) clrpanic("Too huge allocation block");
			if(curr+phdr->size>limt) clrpanic("Allocation block overflow");
			curr += phdr->size;
		}
		if(curr+size>limt) panic("Memory overflow");
		phdr->size = sizeof(*phdr)+size;
		phdr->type = managedType;
		curr += phdr->size;
		*(unint*)curr = 0;
		void* buf = phdr+1;
#if defined(ILDEBUG)
ILDEBUG_BEGIN;
		memset(buf,0xFF,size);
ILDEBUG_END;
#endif
		return buf;
	}

	static void* Allocate(const TypeDef& typdef, uint size) {
		return Allocate(typdef.getManagedType(), size);
	}

	static const TypeDef* typeTypeDef[METATYPE_END] = { };
	static bool metaProtocol[METATYPE_END] = { };

	extern bool IsNewProtocol(METATYPE metatype) {
		return metaProtocol[metatype];
	}

	extern void SetNewProtocol(METATYPE metatype) {
		metaProtocol[metatype] = true;
	}

	extern void SetMetaType(METATYPE metatype, const TypeDef& typdef) {
		typeTypeDef[metatype] = &typdef;
	}
	
	extern const TypeDef& GetMetaType(METATYPE metatype) {
		if(typeTypeDef[metatype]==NULL) {
			panic("IL::GetMetaType called but not set");
		}
		return *typeTypeDef[metatype];
	}
	
	extern bool ReadyMetaType(METATYPE metatype) {
		return typeTypeDef[metatype]!=NULL;
	}

	extern BlankType* NewBlankType(uint size) {
		BlankType* obj = (BlankType*)((byte*)Allocate(NULL, size)-sizeof(BlankType));
		memclr(obj->getObject(), size);
		obj->type = NULL;
		return obj;
	}
	
	extern BlankType* NewType(METATYPE metatype) {
		const TypeDef& typdef = GetMetaType(metatype);
		uint size = typdef.getInstanceSize();
		BlankType* obj = (BlankType*)((byte*)Allocate(NULL, size)-sizeof(BlankType));
		memclr(obj->getObject(), size);
		obj->type = typdef.getManagedType();
		return obj;
	}

	static int handle_offset = -1;

	extern RuntimeTypeHandle GetHandleOfType(void* p) {
		if(handle_offset==-1) {
			const Reflection::TypeDef& typebase = GetMetaType(METATYPE_TYPEBASE);
			const Reflection::Field* field = typebase.ResolveField(L"_handle",true);
			handle_offset = field->getOffset();
		}
		void* handle = *(void**)((byte*)p+handle_offset);
		if(handle==NULL) clrpanic("TypeHandle is null");
		return RuntimeTypeHandle(handle);
	}

	extern RuntimeTypeHandle GetHandleOfObject(void* p) {
		return GetHandleOfType(((void**)p)[-1]);
	}

	extern void* CreateInstance(const TypeDef& typdef) {
		uint size = typdef.getInstanceSize();
		void* p = Allocate(typdef,size);
		memclr(p, size);
		return p;
	}

	extern void* CloneInstance(void* obj) {
		TypeDef* typdef = TypeDef::getTypeFromHandle(GetHandleOfObject(obj));
		void* p = CreateInstance(*typdef);
		memcpy(p, obj, typdef->getInstanceSize());
		return p;
	}

	extern Array* NewArray(const TypeDef& typdef, uint length) {
		static const TypeDef* arrayTypeDef = NULL;
		if(arrayTypeDef==NULL) {
			Assembly* assembly = AssemblyManager::FindAssembly(L"mscorlib");
			if(assembly==NULL) panic("NewString can't find mscorlib Assembly");
			arrayTypeDef = assembly->FindTypeDef(L"Array", L"System");
			if(arrayTypeDef==NULL) panic("NewString can't find System.Array");
		}

		TypeDef* thisArrayType = typdef.CreateArrayType();
		uint size = length*typdef.getVariableSize();
		Array* arr = (Array*)Allocate(*thisArrayType, sizeof(Array)+size-1/*start_elem*/);
		arr->type = typdef.getManagedType();
		arr->length = length;
		arr->elemsize = typdef.getVariableSize();
		memclr(&arr->start_elem, size);
		return arr;
	}

	extern void* GetFirstElement(void* p) {
		return (byte*)p+sizeof(void*)+sizeof(void*)+sizeof(void*);
	}

	static const TypeDef& getStringTypeDef() {
		static const TypeDef* stringTypeDef = NULL;
		if(stringTypeDef==NULL) {
			Assembly* assembly = AssemblyManager::FindAssembly(L"mscorlib");
			if(assembly==NULL) panic("NewString can't find mscorlib Assembly");
			stringTypeDef = assembly->FindTypeDef(L"String", L"System");
			if(stringTypeDef==NULL) panic("NewString can't find System.String");
		}
		return *stringTypeDef;
	}

	extern String* NewString(uint length) {
		uint size = sizeof(uint32)+sizeof(wchar_t)*(length+1);	// Mono needs some following buffer.
		String* s = (String*)Allocate(getStringTypeDef(), size);
		s->length = length;
		(&s->start_char)[length] = '\0';	// set the last char null
		return s;
	}

	extern String* NewString(const std::wstring& str) {
		String* s = NewString(str.length());
		memcpy(&s->start_char, str.c_str(), sizeof(wchar_t)*str.length());
		return s;
	}

	typedef std::map<void*,void*> ReplaceQueue;
	static ReplaceQueue* pReplaceQueue = NULL;
	static bool supressReplacing = false;

	extern void SupressTypeReplacing() {
		supressReplacing = true;
	}

	extern uint FlushTypeReplacing() {
		supressReplacing = false;
		if(pReplaceQueue==NULL || pReplaceQueue->size()==0) {
			return 0;
		} else {
			int count = 0;
			byte* p = (byte*)base;
			while(p<curr) {
				Header* phdr = (Header*)p;
				if(phdr->size==0) {
					panic("ZERO-SIZED BLOCK IN MANAGED MEMORY: 0x"+itos<char,16>((byte*)p-(byte*)base));
				}
				ReplaceQueue::iterator it;
				it = pReplaceQueue->find(phdr->type);
				if(it!=pReplaceQueue->end()) {
					++count;
					phdr->type = it->second;
				}
				TypeDef& typdef = *TypeDef::getTypeFromHandle(IL::GetHandleOfType(phdr->type));
				if(typdef.IsArray()) {
					Array* arr = (Array*)(phdr+1);
					it = pReplaceQueue->find(arr->type);
					if(it!=pReplaceQueue->end()) {
						arr->type = it->second;
					}
				}
				p += phdr->size;
			}
			pReplaceQueue->clear();
			return count;
		}
	}

	extern uint ReplaceExistingType(void* from, void* to) {
		if(to==NULL) panic("Type is NULL");
		if(pReplaceQueue==NULL) {
			pReplaceQueue = new ReplaceQueue();
		}
		(*pReplaceQueue)[from] = to;
		if(supressReplacing) {
			return -1;
		} else {
			return FlushTypeReplacing();
		}
	}

}
