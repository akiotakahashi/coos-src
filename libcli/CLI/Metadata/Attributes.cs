using System;

namespace CooS.Formats.CLI.Metadata {

	[Flags]
	public enum AssemblyFlags : uint {
		PublicKey=0x0001,					// The assembly reference holds the full (unhashed) public key.
		SideBySideCompatible=0x0000,		// The assembly is side-by-side compatible
		Retargetable=0x0100,				// The implementation of this assembly used at runtime is not expected to match the version seen at compile time. (See the text following this table.)
		EnableJITcompileTracking=0x8000, 	// Reserved (a conforming implementation of the CLI can ignore this setting on read; some implementations might use this bit to indicate that a CIL-to-native-code compiler should generate CIL-to-native code map)
		DisableJITcompileOptimizer=0x4000,	// Reserved (a conforming implementation of the CLI can ignore this setting on read; some implementations might use this bit to indicate that a CIL-to-native-code compiler should not generate optimized code)
	}

	public enum ManifestResourceAttributes : uint {
		VisibilityMask=0x0007,	// These 3 bits contain one of the following values:
		Public=0x0001,			// The Resource is exported from the Assembly
		Private=0x0002,			// The Resource is private to the Assembly
	}

	public enum PInvokeAttributes : ushort {
		NoMangle=0x0001,			// PInvoke is to use the member name as specified
		/* Character set */
		CharSetMask=0x0006,			// This is a resource file or other non-metadata-containing file. These 2 bits contain one of the following values: */
		CharSetNotSpec=0x0000,
		CharSetAnsi=0x0002,
		CharSetUnicode=0x0004,
		CharSetAuto=0x0006,
		SupportsLastError=0x0040,	// Information about target function. Not relevant for fields
		/* Calling convention */
		CallConvMask=0x0700,		// These 3 bits contain one of the following values:
		CallConvWinapi=0x0100,
		CallConvCdecl=0x0200,
		CallConvStdcall=0x0300,
		CallConvThiscall=0x0400,
		CallConvFastcall=0x0500,
	}

	public enum TypeAttributes : uint {
		/* Visibility attributes */
		VisibilityMask=0x00000007,			// Use this mask to retrieve visibility information. These 3 bits contain one of the following values:
		NotPublic=0x00000000,				// Class has no public scope
		Public=0x00000001,					// Class has public scope
		NestedPublic=0x00000002,			// Class is nested with public visibility
		NestedPrivate=0x00000003,			// Class is nested with private visibility
		NestedFamily=0x00000004,			// Class is nested with family visibility
		NestedAssembly=0x00000005,			// Class is nested with assembly visibility
		NestedFamANDAssem=0x00000006,		// Class is nested with family and assembly visibility
		NestedFamORAssem=0x00000007,		// Class is nested with family or assembly visibility
		/* Class layout attributes */
		LayoutMask=0x00000018,				// Use this mask to retrieve class layout information. These 2 bits contain one of the following values:
		AutoLayout=0x00000000,				// Class fields are auto-laid out
		SequentialLayout=0x00000008,		// Class fields are laid out sequentially
		ExplicitLayout=0x00000010,			// Layout is supplied explicitly
		/* Class semantics attributes */
		ClassSemanticsMask=0x00000020,		// Use this mask to retrive class semantics information. This bit contains one of the following values:
		Class=0x00000000,					// Type is a class
		Interface=0x00000020,				// Type is an interface
		/* Special semantics in addition to class semantics */
		Abstract=0x00000080,				// Class is abstract
		Sealed=0x00000100,					// Class cannot be extended
		SpecialName=0x00000400,				// Class name is special
		/* Implementation Attributes */
		Import=0x00001000,					// Class/Interface is imported
		Serializable=0x00002000,			// Reserved (Class is serializable)
		/* String formatting Attributes */
		StringFormatMask=0x00030000,		// Use this mask to retrieve string information for native interop. These 2 bits contain one of the following values:
		AnsiClass=0x00000000,				// LPSTR is interpreted as ANSI
		UnicodeClass=0x00010000,			// LPSTR is interpreted as Unicode
		AutoClass=0x00020000,				// LPSTR is interpreted automatically
		CustomFormatClass=0x00030000,		// A non-standard encoding specified by CustomStringFormatMask
		CustomStringFormatMask=0x00C00000,	// Use this mask to retrieve non-standard encoding information for native interop. The meaning of the values of these 2 bits is unspecified.
		/* Class Initialization Attributes */
		BeforeFieldInit=0x00100000,			// Initialize the class before first static field access
		/* Additional Flags */
		RTSpecialName=0x00000800,			// CLI provides 'special' behavior, depending upon the name of the Type
		HasSecurity=0x00040000,				// Type has security associate with it
	}

	public enum ParamAttributes : ushort {
		In=0x0001,				// Param is [In]
		Out=0x0002,				// Param is [out]
		Optional=0x0010,		// Param is optional
		HasDefault=0x1000,		// Param has default value
		HasFieldMarshal=0x2000,	// Param has FieldMarshal
		Unused=0xcfe0,			// Reserved: shall be zero in a conforming implementation
	}

	public enum GenericParamAttributes : ushort {
		VarianceMask=0x0003,					// These 2 bits contain one of the following values:
		None=0x0000,							// The generic parameter is non-variant and has no special constraints
		Covariant=0x0001,						// The generic parameter is covariant
		Contravariant=0x0002,					// The generic parameter is contravariant
		SpecialConstraintMask=0x001C,			// These 3 bits contain one of the following values:
		ReferenceTypeConstraint=0x0004,			// The generic parameter has the class special constraint
		NotNullableValueTypeConstraint=0x0008,	// The generic parameter has the valuetype special constraint
		DefaultConstructorConstraint=0x0010,	// The generic parameter has the .ctor special constraint
	}

	public enum EventAttributes : ushort {
		SpecialName=0x0200,		// Event is special.
		RTSpecialName=0x0400,	// CLI provides 'special' behavior, depending upon the name of the event
	}

	public enum MethodSemanticsAttributes : ushort {
		Setter=0x0001,		// Setter for property
		Getter=0x0002,		// Getter for property
		Other=0x0004,		// Other method for property or event
		AddOn=0x0008,		// AddOn method for event
		RemoveOn=0x0010,	// RemoveOn method for event
		Fire=0x0020,		// Fire method for event
	}

	public enum FileAttributes : uint {
		ContainsMetaData=0x0000,	// This is not a resource file
		ContainsNoMetaData=0x0001,	// This is a resource file or other non-metadata-containing file
	}


	public enum MethodImplAttributes : ushort {
		CodeTypeMask=0x0003,		// These 2 bits contain one of the following values:
		IL=0x0000,					// Method impl is CIL
		Native=0x0001,				// Method impl is native
		OPTIL=0x0002,				// Reserved: shall be zero in conforming implementations
		Runtime=0x0003,				// Method impl is provided by the runtime
		ManagedMask=0x0004,			// Flags specifying whether the code is managed or unmanaged. This bit contains one of the following values:
		Unmanaged=0x0004,			// Method impl is unmanaged, otherwise managed
		Managed=0x0000,				// Method impl is managed
		/* Implementation info and interop */
		ForwardRef=0x0010,			// Indicates method is defined; used primarily in merge scenarios
		PreserveSig=0x0080,			// Reserved: conforming implementations can ignore
		InternalCall=0x1000,		// Reserved: shall be zero in conforming implementations
		Synchronized=0x0020,		// Method is single threaded through the body
		NoInlining=0x0008,			// Method cannot be inlined
		MaxMethodImplVal=0xffff,	// Range check value
	}

	public enum PropertyAttributes : ushort {
		SpecialName=0x0200,		// Property is special
		RTSpecialName=0x0400,	// Runtime(metadata internal APIs) should check name encoding
		HasDefault=0x1000,		// Property has default
		Unused=0xe9ff,			// Reserved: shall be zero in a conforming implementation
	}

	public enum FieldAttributes : ushort {
		FieldAccessMask=0x0007,		// These 3 bits contain one of the following values:
		CompilerControlled=0x0000,	// Member not referenceable
		Private=0x0001,				// Accessible only by the parent type
		FamANDAssem=0x0002,			// Accessible by sub-types only in this Assembly
		Assembly=0x0003,			// Accessibly by anyone in the Assembly
		Family=0x0004,				// Accessible only by type and sub-types
		FamORAssem=0x0005,			// Accessibly by sub-types anywhere, plus anyone in assembly
		Public=0x0006,				// Accessibly by anyone who has visibility to this scope field contract attributes
		Static=0x0010,				// Defined on type, else per instance
		InitOnly=0x0020,			// Field can only be initialized, not written to after init
		Literal=0x0040,				// Value is compile time constant
		NotSerialized=0x0080,		// Reserved (to indicate this field should not be serialized when type is remoted)
		SpecialName=0x0200,			// Field is special
		/* Interop Attributes */
		PInvokeImpl=0x2000,			// Implementation is forwarded through PInvoke.
		/* Additional flags */
		RTSpecialName=0x0400,		// CLI provides 'special' behavior, depending upon the name of the field
		HasFieldMarshal=0x1000,		// Field has marshalling information
		HasDefault=0x8000,			// Field has default
		HasFieldRVA=0x0100,			// Field has RVA
	}

	public enum MethodAttributes : ushort {
		MemberAccessMask=0x0007,	// These 3 bits contain one of the following values:
		CompilerControlled=0x0000,	// Member not referenceable
		Private=0x0001,				// Accessible only by the parent type
		FamANDAssem=0x0002,			// Accessible by sub-types only in this Assembly
		Assem=0x0003,				// Accessibly by anyone in the Assembly
		Family=0x0004,				// Accessible only by type and sub-types
		FamORAssem=0x0005,			// Accessibly by sub-types anywhere, plus anyone in assembly
		Public=0x0006,				// Accessibly by anyone who has visibility to this scope
		Static=0x0010,				// Defined on type, else per instance
		Final=0x0020,				// Method cannot be overridden
		Virtual=0x0040,				// Method is virtual
		HideBySig=0x0080,			// Method hides by name+sig, else just by name
		VtableLayoutMask=0x0100,	// Use this mask to retrieve vtable attributes. This bit contains one of the following values:
		ReuseSlot=0x0000,			// Method reuses existing slot in vtable
		NewSlot=0x0100,				// Method always gets a new slot in the vtable
		Strict=0x0200,				// Method can only be overriden if also accessible
		Abstract=0x0400,			// Method does not provide an implementation
		SpecialName=0x0800,			// Method is special
		/* Interop attributes */
		PInvokeImpl=0x2000,			// Implementation is forwarded through PInvoke
		UnmanagedExport=0x0008,		// Reserved: shall be zero for conforming implementations
		/* Additional flags */
		RTSpecialName=0x1000,		// CLI provides 'special' behavior, depending upon the name of the method
		HasSecurity=0x4000,			// Method has security associate with it
		RequireSecObject=0x8000,	// Method calls another method containing security code.
	}

}
