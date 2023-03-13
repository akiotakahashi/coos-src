#pragma once


namespace Metadata {

	enum AssemblyFlags {
	}; 

	enum AssemblyHashAlgorithm {
	}; 

	enum EventAttributes {
		EventAttributes_SpecialName = 0x0200, // Event is special.
		EventAttributes_RTSpecialName = 0x0400, 
	}; 

	enum FieldAttributes {
		FieldAttributes_FieldAccessMask		= 0x0007,
		FieldAttributes_Compilercontrolled	= 0x0000,	// Member not referenceable
		FieldAttributes_Private				= 0x0001,	// Accessible only by the parent type
		FieldAttributes_FamANDAssem			= 0x0002,	// Accessible by sub-types only in this Assembly
		FieldAttributes_Assembly			= 0x0003,	// Accessibly by anyone in the Assembly
		FieldAttributes_Family				= 0x0004,	// Accessible only by type and sub-types
		FieldAttributes_FamORAssem			= 0x0005,	// Accessibly by sub-types anywhere, plus anyone in assembly
		FieldAttributes_Public				= 0x0006,	// Accessibly by anyone who has visibility to this scope field contract attributes
		FieldAttributes_Static				= 0x0010,	// Defined on type, else per instance
		FieldAttributes_InitOnly			= 0x0020,	// Field may only be initialized, not written to after init
		FieldAttributes_Literal				= 0x0040,	// Value is compile time constant
		FieldAttributes_NotSerialized		= 0x0080,	// Field does not have to be serialized when type is remoted
		FieldAttributes_SpecialName			= 0x0200,	// Field is special
		// Interop Attributes
		FieldAttributes_PInvokeImpl			= 0x2000,	// Implementation is forwarded through PInvoke.
		// Additional flags
		FieldAttributes_RTSpecialName		= 0x0400,	// CLI provides 'special' behavior, depending upon the name of the field
		FieldAttributes_HasFieldMarshal		= 0x1000,	// Field has marshalling information
		FieldAttributes_HasDefault			= 0x8000,	// Field has default
		FieldAttributes_HasFieldRVA			= 0x0100,	// Field has RVA
	}; 

	enum FileAttributes {
	}; 

	enum ManifestResourceAttributes {
	}; 

	enum MethodAttributes {
	}; 

	enum MethodImplAttributes {
	}; 

	enum MethodSemanticsAttributes {
	}; 

	enum ParamAttributes {
	}; 

	enum PInvokeAttributes {
	}; 

	enum PropertyAttributes {
	}; 

	enum TypeAttributes {
		// Visibility attributes
		TypeAttribute_VisibilityMask		= 0x00000007,	// Use this mask to retrieve visibility information
		TypeAttribute_NotPublic				= 0x00000000,	// Class has no public scope
		TypeAttribute_Public				= 0x00000001,	// Class has public scope
		TypeAttribute_NestedPublic			= 0x00000002,	// Class is nested with public visibility
		TypeAttribute_NestedPrivate			= 0x00000003,	// Class is nested with private visibility
		TypeAttribute_NestedFamily			= 0x00000004,	// Class is nested with family visibility
		TypeAttribute_NestedAssembly		= 0x00000005,	// Class is nested with assembly visibility
		TypeAttribute_NestedFamANDAssem		= 0x00000006,	// Class is nested with family and assembly visibility
		TypeAttribute_NestedFamORAssem		= 0x00000007,	// Class is nested with family or assembly visibility
		// Class layout attributes
		TypeAttribute_LayoutMask			= 0x00000018,	// Use this mask to retrieve class layout information
		TypeAttribute_AutoLayout			= 0x00000000,	// Class fields are auto-laid out
		TypeAttribute_SequentialLayout		= 0x00000008,	// Class fields are laid out sequentially
		TypeAttribute_ExplicitLayout		= 0x00000010,	// Layout is supplied explicitly
		// Class semantics attributes
		TypeAttribute_ClassSemanticsMask	= 0x00000020,	// Use this mask to retrive class semantics information
		TypeAttribute_Class					= 0x00000000,	// Type is a class
		TypeAttribute_Interface				= 0x00000020,	// Type is an interface
		// Special semantics in addition to class semantics
		TypeAttribute_Abstract				= 0x00000080,	// Class is abstract
		TypeAttribute_Sealed				= 0x00000100,	// Class cannot be extended
		TypeAttribute_SpecialName			= 0x00000400,	// Class name is special
		// Implementation Attributes
		TypeAttribute_Import				= 0x00001000,	// Class/Interface is imported
		TypeAttribute_Serializable			= 0x00002000,	// Class is serializable
		// String formatting Attributes
		TypeAttribute_StringFormatMask		= 0x00030000,	// Use this mask to retrieve string information for native interop
		TypeAttribute_AnsiClass				= 0x00000000,	// LPSTR is interpreted as ANSI
		TypeAttribute_UnicodeClass			= 0x00010000,	// LPSTR is interpreted as Unicode
		TypeAttribute_AutoClass				= 0x00020000,	// LPSTR is interpreted automatically
		// Class Initialization Attributes
		TypeAttribute_BeforeFieldInit		= 0x00100000,	// Initialize the class before first static field access
		// Additional Flags
		TypeAttribute_RTSpecialName			= 0x00000800,	// CLI provides 'special' behavior, depending upon the name of the Type
		TypeAttribute_HasSecurity			= 0x00040000,	// Type has security associate with it
	}; 

}
