#pragma once

#include "stdafx.h"
#include "TypeDef.h"


namespace Reflection {

	namespace MethodAttributes {
		enum Type {
			MemberAccessMask		= 0x0007,
			CompilerControlled		= 0x0000,	// Member not referenceable
			Private					= 0x0001,	// Accessible only by the parent type
			FamANDAssem				= 0x0002,	// Accessible by sub-types only in this Assembly
			Assem					= 0x0003,	// Accessibly by anyone in the Assembly
			Family					= 0x0004,	// Accessible only by type and sub-types
			FamORAssem				= 0x0005,	// Accessibly by sub-types anywhere, plus anyone in assembly
			Public					= 0x0006,	// Accessibly by anyone who has visibility to this scope

            Static					= 0x0010,	// Defined on type, else per instance
			Final					= 0x0020,	// Method may not be overridden
			Virtual					= 0x0040,	// Method is virtual
			HideBySig				= 0x0080,	// Method hides by name+sig, else just by name

			VtableLayoutMask		= 0x0100,	// Use this mask to retrieve vtable attributes
			ReuseSlot				= 0x0000,	// Method reuses existing slot in vtable
			NewSlot					= 0x0100,	// Method always gets a new slot in the vtable

			Abstract				= 0x0400,	// Method does not provide an implementation
			SpecialName				= 0x0800,	// Method is special
 
			// Interop attributes
            PInvokeImpl				= 0x2000,	// Implementation is forwarded through PInvoke
			UnmanagedExport			= 0x0008,	// Reserved: shall be zero for conforming implementations

			// Additional flags
			RTSpecialName			= 0x1000,	// CLI provides 'special' behavior, depending upon the name of the method
			HasSecurity				= 0x4000,	// Method has security associate with it
			RequireSecObject		= 0x8000,	// Method calls another method containing security code.
		};
	}

	namespace MethodImplAttributes {
		enum Type {
			// CodeType
			CodeTypeMask		= 0x0003,
            IL					= 0x0000,	// Method impl is CIL
			Native				= 0x0001,	// Method impl is native
			OPTIL				= 0x0002,	// Reserved: shall be zero in conforming implementations
			Runtime				= 0x0003,	// Method impl is provided by the runtime
			// ManagedType
			ManagedMask			= 0x0004,	// Flags specifying whether the code is managed or unmanaged.
			Unmanaged			= 0x0004,	// Method impl is unmanaged, otherwise managed
			Managed				= 0x0000,	// Method impl is managed
			// Implementation info and interop
			ForwardRef			= 0x0010,	// Indicates method is defined; used primarily in merge scenarios
			PreserveSig			= 0x0080,	// Reserved: conforming implementations may ignore
			InternalCall		= 0x1000,	// Reserved: shall be zero in conforming implementations
			Synchronized		= 0x0020,	// Method is single threaded through the body
			NoInlining			= 0x0008,	// Method may not be inlined
			//---
			MaxMethodImplVal	= 0xffff,	// Range check value    
		};
	}

	enum CorILMethodFlags {
		CorILMethod_Fat			= 0x3,		// Method header is fat.
		CorILMethod_TinyFormat	= 0x2,		// Method header is tiny.
		CorILMethod_MoreSects	= 0x8,		// More sections follow after this header (see Section 24.4.5).
		CorILMethod_InitLocals	= 0x10,		// Call default constructor on all local variables
	};

	enum CorILMethodSectFlags {
		CorILMethodSect_EHTable		= 0x1,	// Exception handling data.
		CorILMethodSect_OptILTable	= 0x2,	// Reserved, shall be 0.
		CorILMethodSect_FatFormat	= 0x40,	// Data format is of the fat variety, meaning there is a 3 byte length.
											// If not set, the header is small with a  1 byte length
		CorILMethodSect_MoreSects	= 0x80,	// Another data section occurs after this current section
	};

	enum ILExceptionClauseFlags {
		COR_ILEXCEPTION_CLAUSE_EXCEPTION	= 0x0000,	// A typed exception clause
		COR_ILEXCEPTION_CLAUSE_FILTER		= 0x0001,	// An exception filter and handler clause
		COR_ILEXCEPTION_CLAUSE_FINALLY		= 0x0002,	// A finally clause
		COR_ILEXCEPTION_CLAUSE_FAULT		= 0x0004,	// Fault clause (finally that is called on exception only)
 	};

#pragma warning(disable: 4103)
#include "../enpack.h"
#include "../enalign.h"
#pragma warning(default: 4103)

	struct ExceptionHandlingClause {
		uint32	Flags;						// Flags, see below.
		uint32	TryOffset;					// Offset in bytes of  try block from start of the header.
		uint32	TryLength;					// Length in bytes of the try block
		uint32	HandlerOffset;				// Location of the handler for this try block
		uint32	HandlerLength;				// Size of the handler code in bytes
		union {
			uint32	ClassToken;				// Meta data token for a type-based exception handler
			uint32	FilterOffset;			// Offset in method body for filter-based exception handler
		};
 	};

	struct ExceptionHandlingClauseSmall {
		uint16	Flags;						// Flags, see below.
		uint16	TryOffset;					// Offset in bytes of  try block from start of the header.
		uint8	TryLength;					// Length in bytes of the try block
		uint16	HandlerOffset;				// Location of the handler for this try block
		uint8	HandlerLength;				// Size of the handler code in bytes
		union {
			uint32	ClassToken;				// Meta data token for a type-based exception handler
			uint32	FilterOffset;			// Offset in method body for filter-based exception handler
		};
 	};

	class Param;

#include "../unpack.h"
#include "../unalign.h"

	class Method : public TypeDefItem {
		friend class TypeDef;
		enum AccessFlags {
			CompilerControlled,
			Private,
			FamAndAssem,
			Assem,
			Family,
			FamOrAssem,
			Public,
		} fAccess;
		enum InstantiateFlags {
			Static,
			Instance,
		} fInstantiation;
		enum ModificationFlags {
			Plain,
			Final,
			Virtual,
			Abstract,
		} fModification;
		enum HidingRules {
			HideBySig,
			HideByName,
		} fHidingRule;
		enum SlotUsages {
			ReuseSlot,
			NewSlot,
		} fSlotUsage;
		/*
		enum ImplementationFlags {
			IL,
			OPTIL,
			Native,
			Runtime,
		} fImplementation;
		*/
		enum ImplementationInfoFlags {
			ForwardRef			= 0x0010,	// Indicates method is defined; used primarily in merge scenarios
			PreserveSig			= 0x0080,	// Reserved: conforming implementations may ignore
			InternalCall		= 0x1000,	// Reserved: shall be zero in conforming implementations
			Synchronized		= 0x0020,	// Method is single threaded through the body
			NoInlining			= 0x0008,	// Method may not be inlined
		} fImplementationInfo;
		enum MethodTypeFlags {
			Unmanaged,
			Managed,
		} fMethodType;
		const byte* code;
		std::wstring name;
		Signature::MethodDefSig signature;
		//uint rva;
		MethodAttributes::Type flags;
		CorILMethodFlags methodFlags;
		const byte* EHClauseData;
		uint EHClauseDataLength;
		bool isEHClauseDataFat;
		MethodImplAttributes::Type implFlags;
		// Method Header Fields
		ushort maxStack;
		ulong codeSize;
		uint argcount;
		Signature::LocalVarSig localVarSig;
		mutable int retsize;
		bool ready_arginfo;
		bool ready_varinfo;
		LayoutInfo arginfo;
		LayoutInfo varinfo;
	public:
		Method(Assembly& parent, uint tableIndex, const Metadata::Method& m, const Metadata::Method* pm2);
		~Method();
	protected:
		virtual TypeDef* ResolveTypeDef() const;
	private:
		void DetermineArgumentLayout();
		void DetermineLocalVarLayout();
	public:
		void SetCode(const byte* code, MethodImplAttributes::Type flags);
	public:
		const std::wstring& getName() const { return name; }
		std::wstring getFullName() const;
		const byte* getCodeBase() const { return code; }
		ushort getMaxStackSize() const { return maxStack; }
	public:
		bool HasThis() const { return signature.HasThis(); }
		bool HasReturn() const { return !signature.RetType.Void; }
		bool IsConstructor() const { return name==L".ctor"; }
		bool IsManagedMathod() const { return fMethodType==Managed; }
		bool IsILMethod() const { return 0==(implFlags&MethodImplAttributes::CodeTypeMask); }
		bool IsNativeMethod() const { return MethodImplAttributes::Native==(implFlags&MethodImplAttributes::CodeTypeMask); }
		bool IsRuntimeMethod() const { return MethodImplAttributes::Runtime==(implFlags&MethodImplAttributes::CodeTypeMask); }
		bool IsInternalCallMethod() const { return fImplementationInfo==InternalCall; }
		Signature::MethodDefSig& getSignature() { return signature; }
		const Signature::MethodDefSig& getSignature() const { return signature; }
	public:
		int getReturnSize() const;
		uint getParamCount() const;
		uint getArgumentCount() const;
		uint getLocalVarCount() const;
	public:
		ELEMENT_TYPE getArgumentElemType(uint index) const;
		int getArgumentOffset(uint index) const;
		uint getArgumentSize(uint index) const;
		uint getArgumentTotalSize() const;
		ELEMENT_TYPE getLocalVarElemType(uint index) const;
		int getLocalVarOffset(uint index) const;
		uint getLocalVarSize(uint index) const;
		uint getLocalVarTotalSize() const;
	public:
		uint getEHClauseCount() const;
		ExceptionHandlingClause getEHClause(uint index) const;
		bool getProperEHClause(ExceptionHandlingClause* ehc, uint ip) const;
	public:
		void ReloadMethodCode(const byte* code);
		const void* CreateProxyCode() const;
		const void* CreateBridgeCode() const;
	};

}
