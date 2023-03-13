#pragma once

#include "../stdlib.h"
#include "../stl.h"
#include "../console.h"
#include "../metadata.h"


enum ELEMENT_TYPE {
	ELEMENT_TYPE_END		= 0x00,		// Marks end of a list
	ELEMENT_TYPE_VOID		= 0x01,
	ELEMENT_TYPE_BOOLEAN	= 0x02,
	ELEMENT_TYPE_CHAR		= 0x03,
	ELEMENT_TYPE_I1			= 0x04,
	ELEMENT_TYPE_U1			= 0x05,
	ELEMENT_TYPE_I2			= 0x06,
	ELEMENT_TYPE_U2			= 0x07,
	ELEMENT_TYPE_I4			= 0x08,
	ELEMENT_TYPE_U4			= 0x09,
	ELEMENT_TYPE_I8			= 0x0a,
	ELEMENT_TYPE_U8			= 0x0b,
	ELEMENT_TYPE_R4			= 0x0c,
	ELEMENT_TYPE_R8			= 0x0d,
	ELEMENT_TYPE_STRING		= 0x0e,
	ELEMENT_TYPE_PTR		= 0x0f,		// Followed by <type> token
	ELEMENT_TYPE_BYREF		= 0x10,		// Followed by <type> token
	ELEMENT_TYPE_VALUETYPE	= 0x11,		// Followed by <type> token
	ELEMENT_TYPE_CLASS		= 0x12,		// Followed by <type> token
	ELEMENT_TYPE_ARRAY		= 0x14,		// <type> <rank> <boundsCount> <bound1> Åc <loCount> <lo1> Åc
	ELEMENT_TYPE_TYPEDBYREF	= 0x16,
	ELEMENT_TYPE_I			= 0x18,		// System.IntPtr
	ELEMENT_TYPE_U			= 0x19,		// System.UIntPtr
	ELEMENT_TYPE_FNPTR		= 0x1b,		// Followed by full method signature
	ELEMENT_TYPE_OBJECT		= 0x1c,		// System.Object
	ELEMENT_TYPE_SZARRAY	= 0x1d,		// Single-dim array with 0 lower bound
	ELEMENT_TYPE_CMOD_REQD	= 0x1f,		// Required modifier : followed by a TypeDef or TypeRef token
	ELEMENT_TYPE_CMOD_OPT	= 0x20,		// Optional modifier : followed by a TypeDef or TypeRef token
	ELEMENT_TYPE_INTERNAL	= 0x21,		// Implemented within the CLI
	ELEMENT_TYPE_MODIFIER	= 0x40,		// OrÅfd with following element types
	ELEMENT_TYPE_SENTINEL	= 0x41,		// Sentinel for varargs method signature
	ELEMENT_TYPE_PINNED		= 0x45,		// Denotes a local variable that points at a pinned object
};

namespace Reflection {

	class Assembly;
	class TypeDef;

}

namespace Signature {

	enum CallingConventionFlags {
		HASTHIS			= 0x20,		// used to encode the keyword instance in the calling convention, see Section 14.3
		EXPLICITTHIS	= 0x40,		// used to encode the keyword explicit in the calling convention, see Section 14.3
		DEFAULT			= 0x00,		// used to encode the keyword default in the calling convention, see Section 14.3
		VARARG			= 0x05,		// used to encode the keyword vararg in the calling convention, see Section 14.3
		SENTINEL		= 0x41,
		C_LANG			= 0x01,
		STDCALL			= 0x02,
		THISCALL		= 0x03,
		FASTCALL		= 0x04,
	};

	class SignatureStream {
		SignatureStream& base;
		const byte* ptr0;
		const byte* ptr;
		uint size;
		uint value;
	public:
		SignatureStream(const byte* p) : base(base) {
			ptr0 = ptr = p;
			size = readInt();
			ptr0 = ptr;
			value = 0;
		}
		SignatureStream(SignatureStream& _base) : base(_base) {
			ptr0 = ptr = base.ptr;
			size = base.size-(ptr-base.ptr);
			value = base.value;
		}
	private:
		void setPointer(const byte* p) {
			ptr = p;
		}
	public:
		void Dump(Console& console) {
			console << MemoryRegion(ptr,size-(ptr-ptr0));
		}
	public:
		void commit() {
			ptr0 = ptr;
			if(&base) {
				base.setPointer(ptr0);
			}
		}
		void rollback() {
			ptr = ptr0;
		}
		byte peekByte() { return value = *ptr; }
		byte readByte() { return value = *(ptr++); }
		uint readInt() {
			return Metadata::BlobStream::ReadCompressedInteger(ptr);
		}
		uint getInt() const {
			return value;
		}
	};

}
