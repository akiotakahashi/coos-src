#include "stdafx.h"
#include "csbridge.h"
#include "../cstdlib/cstdlib.h"
#include <math.h>

using namespace System;
using namespace System::Runtime;
using namespace System::Runtime::CompilerServices;


namespace System {

	public __gc class Bridge {
		static IntPtr address;
		static void setKernel(IntPtr address) {
			Bridge::address = address;
		}
		static IKernel* getKernel() {
			return (IKernel*)address.ToPointer();
		}
	public:
		[MethodImplAttribute(MethodImplOptions::InternalCall)]
		static Object* PointerToObject(void* p);
	public:
		static bool NotAsSystem() {
			IKernel* kernel = getKernel();
			return kernel->NotAsSystem();
		}
		static void SetDebugMode(bool enabled) {
			IKernel* kernel = getKernel();
			return kernel->SetDebugMode(enabled);
		}
		static void ReloadMethodCode(Type* type, int rowIndex, unsigned char code __gc[]) {
			IKernel* kernel = getKernel();
			RuntimeTypeHandle handle = type->get_TypeHandle();
			unsigned char __pin* p = &code[0];
			return kernel->ReloadMethodCode(handle.Value.ToPointer(), rowIndex, p);
		}
	public:
		static void* ReserveMemory(unsigned int size) {
			IKernel* kernel = getKernel();
			return kernel->alloc(size);
		}
		static void ReleaseMemory(void* p) {
			IKernel* kernel = getKernel();
			return kernel->free(p);
		}
		static String* ReadLine() {
			IKernel* kernel = getKernel();
			return static_cast<String*>(PointerToObject(kernel->ReadLine()));
		}
		static void Write(wchar_t ch) {
			IKernel* kernel = getKernel();
			kernel->Write(ch);
		}
		static Type* GetTypeFromHandle(IntPtr handle) {
			IKernel* kernel = getKernel();
			return static_cast<Type*>(PointerToObject(kernel->GetTypeFromHandle(handle.ToPointer())));
		}
		static Object* CreateInstance(Type* type) {
			IKernel* kernel = getKernel();
			RuntimeTypeHandle handle = type->get_TypeHandle();
			return PointerToObject(kernel->CreateInstance(handle.Value.ToPointer()));
		}
		static Object* CloneInstance(Object* obj) {
			IKernel* kernel = getKernel();
			Object __pin* p = obj;
			return PointerToObject(kernel->CloneInstance((void*)(int)p));
		}
		static String* CreateString(unsigned int length) {
			IKernel* kernel = getKernel();
			return static_cast<String*>(PointerToObject(kernel->CreateString(length)));
		}
		static Array* CreateArray(Type* elementType, unsigned int length) {
			IKernel* kernel = getKernel();
			RuntimeTypeHandle handle = elementType->get_TypeHandle();
			return static_cast<Array*>(PointerToObject(kernel->CreateArray(handle.Value.ToPointer(), length)));
		}
		static Object* BoxValue(Type* type, unsigned char buf __gc[], int index) {
			IKernel* kernel = getKernel();
			RuntimeTypeHandle handle = type->get_TypeHandle();
			void* obj = kernel->CreateInstance(handle.Value.ToPointer());
			unsigned char __pin* p = &buf[index];
			memcpy(obj, p, System::Runtime::InteropServices::Marshal::SizeOf(type));
			return PointerToObject(obj);
		}
		static Object* BoxValue(Type* type, void* container, int offset) {
			IKernel* kernel = getKernel();
			RuntimeTypeHandle handle = type->get_TypeHandle();
			void* obj = kernel->CreateInstance(handle.Value.ToPointer());
			memcpy(obj, (char*)container+offset, System::Runtime::InteropServices::Marshal::SizeOf(type));
			return PointerToObject(obj);
		}
	public:
		static void* NotifyLoadingType(String* asmname, int rowIndex, Object* obj) {
			IKernel* kernel = getKernel();
			wchar_t buf __gc[] = asmname->ToCharArray();
			wchar_t __pin* p = &buf[0];
			Object __pin* _obj = obj;
			return kernel->NotifyLoadingType(p, buf->Length, rowIndex, (void*)(int)_obj);
		}
		static const void* GenerateProxyCode(String* asmname, int methodrid) {
			IKernel* kernel = getKernel();
			wchar_t buf __gc[] = asmname->ToCharArray();
			wchar_t __pin* p = &buf[0];
			return kernel->GenerateProxyCode(p, buf->Length, methodrid);
		}
	public:
		static void LoadAssembly(Object* assembly, void* p, int size) {
			IKernel* kernel = getKernel();
			Object __pin* obj = assembly;
			kernel->LoadAssembly((void*)(int)obj, p, size);
		}
		static Object* GetExecutingAssembly(int depth) {
			IKernel* kernel = getKernel();
			return PointerToObject(kernel->GetExecutingAssembly(depth));
		}
	};

}

static void* Execute(void* fp, void* stack, int _size) {
	if(_size%sizeof(void*)!=0) return NULL;
	int _sz4 = _size/4;
	void* retval;
	__asm {
		push esp;
		sub esp, _size;
		mov edi, esp;
		mov esi, stack;
		mov ecx, _sz4;
		cld;
		rep movsd;
		call fp;
		pop esp;
		mov retval, eax;
	}
	return retval;
}

namespace CooS {
namespace Wrap {

namespace _System {

	using ::System::Runtime::InteropServices::OutAttribute;

	public __gc class _Bridge {
	public:
		static void* PointerToObject(void* p) {
			return p;
		}
	};

	public __gc class _Engine {
	public:
		static bool get_Privileged() {
			return !Bridge::NotAsSystem();
		}
		static void SetDebugMode(bool enabled) {
			Bridge::SetDebugMode(enabled);
		}
		static void ReloadMethodCode(Type* type, int rowIndex, unsigned char code __gc[]) {
			Bridge::ReloadMethodCode(type, rowIndex, code);
		}
		static IntPtr GetMethodProxyCode(String* asmname, int methodrid) {
			return IntPtr((void*)Bridge::GenerateProxyCode(asmname, methodrid));
		}
		static Object* InternalInvoke(IntPtr fp, unsigned char stack __gc[]) {
			unsigned char __pin* p = &stack[0];
			//Console::WriteLine(S"Function: 0x{0:X8}", __box(fp.ToInt32()));
			return Bridge::PointerToObject(::Execute(fp.ToPointer(), p, stack->Length));
		}
		static RuntimeTypeHandle NotifyLoadingTypeImpl(String* asmname, int rowIndex, Object* obj) {
			RuntimeTypeHandle rth;
			IntPtr p = Bridge::NotifyLoadingType(asmname, rowIndex, obj);
			*(IntPtr*)IntPtr(&rth).ToPointer() = p;
			return rth;
		}
		static void RuntimeLoadAssembly(Object* assembly, unsigned char buf __gc[]) {
			unsigned char __pin* p = &buf[0];
			Bridge::LoadAssembly(assembly, p, buf->Length);
		}
		static Object* GetExecutingAssembly(int depth) {
			return Bridge::GetExecutingAssembly(depth);
		}
	};

	public __gc class _Object {
	public:
		Type* GetType() {
			Object __pin* p = static_cast<Object*>(this);
			IntPtr __nogc* obj = reinterpret_cast<IntPtr __nogc*>(p);
			return static_cast<Type*>(Bridge::PointerToObject(obj[-1].ToPointer()));
		}
		Object* MemberwiseClone() {
			return Bridge::CloneInstance(this);
		}
	};

	public __gc class _Type {
	private:
		static Type* internal_from_handle(IntPtr handle) {
			return Bridge::GetTypeFromHandle(handle);
		}
	};

	public __gc class _Array {
	public:
		Type* type;
		int elemsize;
		int length;
		int start_elem;
	private:
		Object* GetValueImpl(int pos) {
			_Array __pin* array = this;
			if(type->get_BaseType()!=__typeof(ValueType)) {
				return Bridge::PointerToObject(*(void**)((char*)&array->start_elem+elemsize*pos));
			} else {
				Object* obj = Bridge::CreateInstance(type);
				Object __pin* dst = obj;
				void* p = (char*)&array->start_elem+elemsize*pos;
				memcpy((void*)(int)dst, p, elemsize);
				return obj;
			}
		}
		void SetValueImpl(Object* value, int pos) {
			_Array __pin* array = this;
			void* dst = (char*)&array->start_elem+array->elemsize*pos;
			if(value==NULL) {
				memclr(dst, elemsize);
			} else if(type->get_BaseType()!=__typeof(ValueType)) {
				if(elemsize!=sizeof(void*)) {
					Console::WriteLine("elemsize: {0}",__box(elemsize));
					Console::WriteLine("typename: {0}",type->FullName);
					throw new InvalidProgramException();
				}
				Object __pin* src = value;
				*(void**)dst = *(void**)&src;
			} else {
				Object __pin* src = value;
				memcpy(dst, (void*)(int)src, elemsize);
			}
		}
		int GetRank() {
			return 1;
		}
		static Array* CreateInstance(Type* elementType, int lengths __gc[]) {
			if(elementType==NULL) throw new ArgumentNullException("elementType");
			if(lengths==NULL) throw new ArgumentNullException("lengths");
			return CreateInstanceImpl(elementType, lengths, NULL);
		}
		static Array* CreateInstanceImpl(Type* elementType, int lengths __gc[], int bounds __gc[]) {
			if(lengths->get_Length()>1) throw new NotSupportedException(S"Number of array dimension must be 1");
			if(bounds!=NULL) throw new NotSupportedException(S"Bounds of array must be null");
			return Bridge::CreateArray(elementType, lengths[0]);
		}
	public:
		int GetLength(int dimension) {
			if(dimension<0) throw new IndexOutOfRangeException();
			if(dimension>0) return 0;
			return length;
		}
		int GetLowerBound(int dimension) {
			if(dimension<0) throw new IndexOutOfRangeException();
			if(dimension>0) return 0;
			return 0;
		}
		int GetUpperBound(int dimension) {
			if(dimension<0) throw new IndexOutOfRangeException();
			if(dimension>0) return 0;
			return length-1;
		}
	};

	public __gc class _String {
		__int32 length;
		wchar_t start_char;
	public:
		[MethodImpl(MethodImplOptions::InternalCall)]
		_String(wchar_t val __gc[]) {}
		[MethodImpl(MethodImplOptions::InternalCall), CLSCompliant(false)]
		_String(wchar_t* value) {}
		[MethodImpl(MethodImplOptions::InternalCall), CLSCompliant(false)]
		_String(char* value) {}
		[MethodImpl(MethodImplOptions::InternalCall)]
		_String(wchar_t c, int count) {}
		[MethodImpl(MethodImplOptions::InternalCall)]
		_String(wchar_t val __gc[], int startIndex, int length) {}
		[MethodImpl(MethodImplOptions::InternalCall), CLSCompliant(false)]
		_String(wchar_t* value, int startIndex, int length) {}
		[MethodImpl(MethodImplOptions::InternalCall), CLSCompliant(false)]
		_String(char* value, int startIndex, int length) {}
		[MethodImpl(MethodImplOptions::InternalCall), CLSCompliant(false)]
		_String(char* value, int startIndex, int length, ::System::Text::Encoding* enc) {}
	private:
		static void InternalStrcpy(String* dest, int destPos, wchar_t chars __gc[], int sPos, int count) {
			if(count>0) {
				_String* d = static_cast<_String*>(static_cast<Object*>(dest));
				wchar_t __pin* pd = &d->start_char;
				wchar_t __pin* ps = &chars[0];
				memcpy(pd+destPos, ps+sPos, count*sizeof(wchar_t));
			}
		}
	private:
		static String* InternalAllocateInstance(wchar_t val __gc[]) {
			String* s = System::Bridge::CreateString(val->Length);
			if(val->Length>0) InternalStrcpy(s, 0, val, 0, val->Length);
			return s;
		}
		static String* InternalAllocateInstance(wchar_t* value) {
			size_t length = wcslen(value);
			_String* str = InternalAllocateStr(length);
			wchar_t __pin* p = &str->start_char;
			memcpy(p, value, length*sizeof(wchar_t));
			return static_cast<String*>(static_cast<Object*>(str));
		}
		static String* InternalAllocateInstance(char* value) {
			size_t length = strlen(value);
			_String* str = InternalAllocateStr(length);
			wchar_t __pin* p = &str->start_char;
			for(unsigned int i=0; i<length; ++i) {
				p[i] = (unsigned char)value[i];
			}
			return static_cast<String*>(static_cast<Object*>(str));
		}
		static String* InternalAllocateInstance(wchar_t c, int count) {
			_String* str = InternalAllocateStr(count);
			if(count>0) {
				wchar_t __pin* p = &str->start_char;
				for(int i=0; i<count; ++i) {
					p[i] = c;
				}
			}
			return static_cast<String*>(static_cast<Object*>(str));
		}
		static String* InternalAllocateInstance(wchar_t val __gc[], int startIndex, int length) {
			String* s = System::Bridge::CreateString(length);
			if(length>0) InternalStrcpy(s, 0, val, startIndex, length);
			return s;
		}
		static String* InternalAllocateInstance(wchar_t* value, int startIndex, int length) {
			_String* str = InternalAllocateStr(length);
			if(length>0) {
				wchar_t __pin* p = &str->start_char;
				memcpy(p, value+startIndex, length*sizeof(wchar_t));
			}
			return static_cast<String*>(static_cast<Object*>(str));
		}
		static String* InternalAllocateInstance(char* value, int startIndex, int length) {
			_String* str = InternalAllocateStr(length);
			if(length>0) {
				wchar_t __pin* p = &str->start_char;
				value += startIndex;
				for(int i=0; i<length; ++i) {
					p[i] = (unsigned char)value[i];
				}
			}
			return static_cast<String*>(static_cast<Object*>(str));
		}
		static _String* InternalAllocateStr(int length) {
			return static_cast<_String*>(static_cast<Object*>(System::Bridge::CreateString(length)));
		}
	};

	public __gc class _Enum {
	public:
		static bool IsDefined(Type* enumType, Object* value) {
			return true;
		}
	};

	public __gc class _Environment {
	public:
		static String* GetEnvironmentVariable(String* name) {
			return NULL;
		}
	};

	public __gc class _GC {
	public:
		static void SuppressFinalize(Object* obj) {
			// NOP
		}
	};

	namespace IO {

		public __gc class _MonoIO {
			static IntPtr get_ConsoleError() {
				return IntPtr::Zero;
			}
		};

	}

	namespace Text {

		public __gc class _Encoding {
			static String* InternalCodePage(Int32* code_page) {
				*code_page = 20127;
				return S"ascii";
			}
		};

	}

	namespace Threading {

		public __gc class _Monitor {
			static bool Monitor_try_enter(Object* obj, Int32 ms) {
				return true;
			}
		};

		public __gc class _Interlocked {
		public:
			static int Increment(Int32* location) {
				return ++*location;
			}
		};

	}

}	// _System

namespace _CooS {
	
	public __gc class _Initializer {
	public:
		static void WriteImpl(wchar_t ch) {
			System::Bridge::Write(ch);
		}
		static String* ReadLineImpl() {
			return System::Bridge::ReadLine();
		}
	};

}

}	// Wrap
}	// CooS
