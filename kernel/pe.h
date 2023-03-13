#pragma once

#include "stl.h"
#include "stdtype.h"
#include "win32.h"


#include "enalign.h"
#include "enpack.h"

namespace PE {

	namespace Raw {

	#define IMAGE_FILE_RELOCS_STRIPPED			 0x0001  // Relocation info stripped from file.
	#define IMAGE_FILE_EXECUTABLE_IMAGE 		 0x0002  // File is executable	(i.e. no unresolved externel references).
	#define IMAGE_FILE_LINE_NUMS_STRIPPED		 0x0004  // Line nunbers stripped from file.
	#define IMAGE_FILE_LOCAL_SYMS_STRIPPED		 0x0008  // Local symbols stripped from file.
	#define IMAGE_FILE_AGGRESIVE_WS_TRIM		 0x0010  // Agressively trim working set
	#define IMAGE_FILE_LARGE_ADDRESS_AWARE		 0x0020  // App can handle >2gb addresses
	#define IMAGE_FILE_BYTES_REVERSED_LO		 0x0080  // Bytes of machine word are reversed.
	#define IMAGE_FILE_32BIT_MACHINE			 0x0100  // 32 bit word machine.
	#define IMAGE_FILE_DEBUG_STRIPPED			 0x0200  // Debugging info stripped from file in .DBG file
	#define IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP	 0x0400  // If Image is on removable media, copy and run from the swap file.
	#define IMAGE_FILE_NET_RUN_FROM_SWAP		 0x0800  // If Image is on Net, copy and run from the swap file.
	#define IMAGE_FILE_SYSTEM					 0x1000  // System File.
	#define IMAGE_FILE_DLL						 0x2000  // File is a DLL.
	#define IMAGE_FILE_UP_SYSTEM_ONLY			 0x4000  // File should only be run on a UP machine
	#define IMAGE_FILE_BYTES_REVERSED_HI		 0x8000  // Bytes of machine word are reversed.

	#define IMAGE_FILE_MACHINE_UNKNOWN			 0
	#define IMAGE_FILE_MACHINE_I386 			 0x014c  // Intel 386.
	#define IMAGE_FILE_MACHINE_R3000			 0x0162  // MIPS little-endian, 0x160 big-endian
	#define IMAGE_FILE_MACHINE_R4000			 0x0166  // MIPS little-endian
	#define IMAGE_FILE_MACHINE_R10000			 0x0168  // MIPS little-endian
	#define IMAGE_FILE_MACHINE_WCEMIPSV2		 0x0169  // MIPS little-endian WCE v2
	#define IMAGE_FILE_MACHINE_ALPHA			 0x0184  // Alpha_AXP
	#define IMAGE_FILE_MACHINE_SH3				 0x01a2  // SH3 little-endian
	#define IMAGE_FILE_MACHINE_SH3DSP			 0x01a3
	#define IMAGE_FILE_MACHINE_SH3E 			 0x01a4  // SH3E little-endian
	#define IMAGE_FILE_MACHINE_SH4				 0x01a6  // SH4 little-endian
	#define IMAGE_FILE_MACHINE_SH5				 0x01a8  // SH5
	#define IMAGE_FILE_MACHINE_ARM				 0x01c0  // ARM Little-Endian
	#define IMAGE_FILE_MACHINE_THUMB			 0x01c2
	#define IMAGE_FILE_MACHINE_AM33 			 0x01d3
	#define IMAGE_FILE_MACHINE_POWERPC			 0x01F0  // IBM PowerPC Little-Endian
	#define IMAGE_FILE_MACHINE_POWERPCFP		 0x01f1
	#define IMAGE_FILE_MACHINE_IA64 			 0x0200  // Intel 64
	#define IMAGE_FILE_MACHINE_MIPS16			 0x0266  // MIPS
	#define IMAGE_FILE_MACHINE_ALPHA64			 0x0284  // ALPHA64
	#define IMAGE_FILE_MACHINE_MIPSFPU			 0x0366  // MIPS
	#define IMAGE_FILE_MACHINE_MIPSFPU16		 0x0466  // MIPS
	#define IMAGE_FILE_MACHINE_AXP64			 IMAGE_FILE_MACHINE_ALPHA64
	#define IMAGE_FILE_MACHINE_TRICORE			 0x0520  // Infineon
	#define IMAGE_FILE_MACHINE_CEF				 0x0CEF
	#define IMAGE_FILE_MACHINE_EBC				 0x0EBC  // EFI Byte Code
	#define IMAGE_FILE_MACHINE_AMD64			 0x8664  // AMD64 (K8)
	#define IMAGE_FILE_MACHINE_M32R 			 0x9041  // M32R little-endian
	#define IMAGE_FILE_MACHINE_CEE				 0xC0EE

	#define IMAGE_NT_OPTIONAL_HDR32_MAGIC	   0x10b
	#define IMAGE_NT_OPTIONAL_HDR64_MAGIC	   0x20b
	#define IMAGE_ROM_OPTIONAL_HDR_MAGIC	   0x107

	struct Header {
		WORD machine;
		WORD numberOfSections;
		DWORD timeDateStamp;
		DWORD pointerToSymbolTable;
		DWORD numberOfSymbols;
		WORD sizeOfOptionalHeader;
		WORD characteristics;
	};

	struct DataDirectory {
		DWORD	rva;
		DWORD	size;
	};

	struct OptionalHeader {
		//
		// Standard fields
		//
		WORD	magic;
		BYTE	majorLinkerVersion;
		BYTE	minorLinkerVersion;
		DWORD	sizeOfCode;
		DWORD	sizeOfInitializedData;
		DWORD	sizeOfUninitializedData;
		DWORD	addressOfEntryPoint;
		DWORD	baseOfCode;
		DWORD	baseOfData;						// absent in PE32+
		//
		// Windows NT-Specific Fields
		//
		DWORD	imageBase;						// extends QWORD in PE32+
		DWORD	sectionAlignment;
		DWORD	fileAlignment;
		WORD	majorOperatingSystemVersion;
		WORD	minorOperatingSystemVersion;
		WORD	majorImageVersion;
		WORD	minorImageVersion;
		WORD	majorSubsystemVersion;
		WORD	minorSubsystemVersion;
		DWORD	win32VersionValue;
		DWORD	sizeOfImage;
		DWORD	sizeOfHeaders;
		DWORD	checkSum;
		WORD	subsystem;
		WORD	dllCharacteristics;
		DWORD	sizeOfStackReserve;				// extends QWORD in PE32+
		DWORD	sizeOfStackCommit;				// extends QWORD in PE32+
		DWORD	sizeOfHeapReserve;				// extends QWORD in PE32+
		DWORD	sizeOfHeapCommit;				// extends QWORD in PE32+
		DWORD	loaderFlags;
		DWORD	numberOfRvaAndSizes;
		// Data Directory Entries are fllowed but not of fixed numbers.
		DataDirectory dataDirectory[16];
	};

	// Subsystem Values

	#define IMAGE_SUBSYSTEM_UNKNOWN 			 0	 // Unknown subsystem.
	#define IMAGE_SUBSYSTEM_NATIVE				 1	 // Image doesn't require a subsystem.
	#define IMAGE_SUBSYSTEM_WINDOWS_GUI 		 2	 // Image runs in the Windows GUI subsystem.
	#define IMAGE_SUBSYSTEM_WINDOWS_CUI 		 3	 // Image runs in the Windows character subsystem.
	#define IMAGE_SUBSYSTEM_OS2_CUI 			 5	 // image runs in the OS/2 character subsystem.
	#define IMAGE_SUBSYSTEM_POSIX_CUI			 7	 // image runs in the Posix character subsystem.
	#define IMAGE_SUBSYSTEM_NATIVE_WINDOWS		 8	 // image is a native Win9x driver.
	#define IMAGE_SUBSYSTEM_WINDOWS_CE_GUI		 9	 // Image runs in the Windows CE subsystem.
	#define IMAGE_SUBSYSTEM_EFI_APPLICATION 	 10  //
	#define IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER  11   //
	#define IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER	 12  //
	#define IMAGE_SUBSYSTEM_EFI_ROM 			 13
	#define IMAGE_SUBSYSTEM_XBOX				 14

	// DllCharacteristics Entries

	//		IMAGE_LIBRARY_PROCESS_INIT			 0x0001 	// Reserved.
	//		IMAGE_LIBRARY_PROCESS_TERM			 0x0002 	// Reserved.
	//		IMAGE_LIBRARY_THREAD_INIT			 0x0004 	// Reserved.
	//		IMAGE_LIBRARY_THREAD_TERM			 0x0008 	// Reserved.
	#define IMAGE_DLLCHARACTERISTICS_NO_SEH 	 0x0400 	// Image does not use SEH.	No SE handler may reside in this image
	#define IMAGE_DLLCHARACTERISTICS_NO_BIND	 0x0800 	// Do not bind this image.
	//											 0x1000 	// Reserved.
	#define IMAGE_DLLCHARACTERISTICS_WDM_DRIVER  0x2000 	// Driver uses WDM model
	//											 0x4000 	// Reserved.
	#define IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE	   0x8000

	//
	// Data Directory Entry
	//

	#define IMAGE_DDE_EXPORT					0
	#define IMAGE_DDE_IMPORT					1
	#define IMAGE_DDE_RESOURCE					2
	#define IMAGE_DDE_EXCEPTION					3
	#define IMAGE_DDE_CERTIFICATE				4
	#define IMAGE_DDE_BASERELOCATION			5
	#define IMAGE_DDE_DEBUG						6
	#define IMAGE_DDE_ARCHITECTURE				7
	#define IMAGE_DDE_GLOBALPOINTER				8
	#define IMAGE_DDE_TLS						9
	#define IMAGE_DDE_LOADCONFIG				10
	#define IMAGE_DDE_BOUNDIMPORT				11
	#define IMAGE_DDE_IAT						12
	#define IMAGE_DDE_DELAYIMPORT				13
	#define IMAGE_DDE_COMPLUS					14
	#define IMAGE_DDE_RESERVED					15

	//
	// Section header format
	//

	typedef struct SectionHeader {
		char	name[8];
		union {
			DWORD	physicalAddress;	// is this a alias ?
			DWORD	virtualSize;
		};
		DWORD	virtualAddress;
		DWORD	sizeOfRawData;
		DWORD	pointerToRawData;
		DWORD	pointerToRelocations;
		DWORD	pointerToLinenumbers;
		WORD	numberOfRelocations;
		WORD	numberOfLinenumbers;
		DWORD	characteristics;
	} IMAGE_SECTION_HEADER, *PIMAGE_SECTION_HEADER;

	//
	// Section characteristics
	//
	//      IMAGE_SCN_TYPE_REG                   0x00000000  // Reserved.
	//      IMAGE_SCN_TYPE_DSECT                 0x00000001  // Reserved.
	//      IMAGE_SCN_TYPE_NOLOAD                0x00000002  // Reserved.
	//      IMAGE_SCN_TYPE_GROUP                 0x00000004  // Reserved.
	#define IMAGE_SCN_TYPE_NO_PAD                0x00000008  // Reserved.
	//      IMAGE_SCN_TYPE_COPY                  0x00000010  // Reserved.

	#define IMAGE_SCN_CNT_CODE                   0x00000020  // Section contains code.
	#define IMAGE_SCN_CNT_INITIALIZED_DATA       0x00000040  // Section contains initialized data.
	#define IMAGE_SCN_CNT_UNINITIALIZED_DATA     0x00000080  // Section contains uninitialized data.

	#define IMAGE_SCN_LNK_OTHER                  0x00000100  // Reserved.
	#define IMAGE_SCN_LNK_INFO                   0x00000200  // Section contains comments or some other type of information.
	//      IMAGE_SCN_TYPE_OVER                  0x00000400  // Reserved.
	#define IMAGE_SCN_LNK_REMOVE                 0x00000800  // Section contents will not become part of image.
	#define IMAGE_SCN_LNK_COMDAT                 0x00001000  // Section contents comdat.
	//                                           0x00002000  // Reserved.
	//      IMAGE_SCN_MEM_PROTECTED - Obsolete   0x00004000
	#define IMAGE_SCN_NO_DEFER_SPEC_EXC          0x00004000  // Reset speculative exceptions handling bits in the TLB entries for this section.
	#define IMAGE_SCN_GPREL                      0x00008000  // Section content can be accessed relative to GP
	#define IMAGE_SCN_MEM_FARDATA                0x00008000
	//      IMAGE_SCN_MEM_SYSHEAP  - Obsolete    0x00010000
	#define IMAGE_SCN_MEM_PURGEABLE              0x00020000
	#define IMAGE_SCN_MEM_16BIT                  0x00020000
	#define IMAGE_SCN_MEM_LOCKED                 0x00040000
	#define IMAGE_SCN_MEM_PRELOAD                0x00080000

	#define IMAGE_SCN_ALIGN_1BYTES               0x00100000  //
	#define IMAGE_SCN_ALIGN_2BYTES               0x00200000  //
	#define IMAGE_SCN_ALIGN_4BYTES               0x00300000  //
	#define IMAGE_SCN_ALIGN_8BYTES               0x00400000  //
	#define IMAGE_SCN_ALIGN_16BYTES              0x00500000  // Default alignment if no others are specified.
	#define IMAGE_SCN_ALIGN_32BYTES              0x00600000  //
	#define IMAGE_SCN_ALIGN_64BYTES              0x00700000  //
	#define IMAGE_SCN_ALIGN_128BYTES             0x00800000  //
	#define IMAGE_SCN_ALIGN_256BYTES             0x00900000  //
	#define IMAGE_SCN_ALIGN_512BYTES             0x00A00000  //
	#define IMAGE_SCN_ALIGN_1024BYTES            0x00B00000  //
	#define IMAGE_SCN_ALIGN_2048BYTES            0x00C00000  //
	#define IMAGE_SCN_ALIGN_4096BYTES            0x00D00000  //
	#define IMAGE_SCN_ALIGN_8192BYTES            0x00E00000  //
	// Unused                                    0x00F00000
	#define IMAGE_SCN_ALIGN_MASK                 0x00F00000

	#define IMAGE_SCN_LNK_NRELOC_OVFL            0x01000000  // Section contains extended relocations.
	#define IMAGE_SCN_MEM_DISCARDABLE            0x02000000  // Section can be discarded.
	#define IMAGE_SCN_MEM_NOT_CACHED             0x04000000  // Section is not cachable.
	#define IMAGE_SCN_MEM_NOT_PAGED              0x08000000  // Section is not pageable.
	#define IMAGE_SCN_MEM_SHARED                 0x10000000  // Section is shareable.
	#define IMAGE_SCN_MEM_EXECUTE                0x20000000  // Section is executable.
	#define IMAGE_SCN_MEM_READ                   0x40000000  // Section is readable.
	#define IMAGE_SCN_MEM_WRITE                  0x80000000  // Section is writeable.

	//
	// COFF Relocations
	//

	struct FixupBlock {
		DWORD	rva;
		DWORD	size;
	};

	struct BlockSize {
		WORD	offset	: 12;
		WORD	type	: 4;
	};

	/*
	struct Relocation {
		union {
			DWORD	virtualAddress;
			DWORD	relocCount; 			// ? win32: Set to the real count when IMAGE_SCN_LNK_NRELOC_OVFL is set
		};
		DWORD	symbolTableIndex;
		WORD	type;
	};
	*/

	//
	// I386 relocation types.
	//

	#define IMAGE_REL_I386_ABSOLUTE         0x0000  // Reference is absolute, no relocation is necessary
	#define IMAGE_REL_I386_DIR16            0x0001  // Direct 16-bit reference to the symbols virtual address
	#define IMAGE_REL_I386_REL16            0x0002  // PC-relative 16-bit reference to the symbols virtual address
	#define IMAGE_REL_I386_DIR32            0x0006  // Direct 32-bit reference to the symbols virtual address
	#define IMAGE_REL_I386_DIR32NB          0x0007  // Direct 32-bit reference to the symbols virtual address, base not included
	#define IMAGE_REL_I386_SEG12            0x0009  // Direct 16-bit reference to the segment-selector bits of a 32-bit virtual address
	#define IMAGE_REL_I386_SECTION          0x000A
	#define IMAGE_REL_I386_SECREL           0x000B
	#define IMAGE_REL_I386_TOKEN            0x000C  // clr token
	#define IMAGE_REL_I386_SECREL7          0x000D  // 7 bit offset from base of section containing target
	#define IMAGE_REL_I386_REL32            0x0014  // PC-relative 32-bit reference to the symbols virtual address

	//
	// Line number format.
	//

	struct LineNumber {
		union {
			DWORD	SymbolTableIndex;				// Symbol table index of function name if Linenumber is 0.
			DWORD	VirtualAddress; 				// Virtual address of line number.
		} type;
		WORD	lineNumber; 						// Line number.
	};

	//
	// Symbol format.
	//

	struct Symbol {
		union {
			BYTE	shortName[8];
			/*
			struct {
				DWORD	Short;	   // if 0, use LongName
				DWORD	Long;	   // offset into string table
			} Name;
			DWORD	LongName[2];	// PBYTE [2]
			*/
			struct {
				DWORD	zeros;
				DWORD	offset;
			};
		};
		DWORD	value;
		SHORT	sectionNumber;
		WORD	type;
		BYTE	storageClass;
		BYTE	numberOfAuxSymbols;
	};

	//
	// Section values.
	//
	// Symbols have a section number of the section in which they are
	// defined. Otherwise, section numbers have the following meanings:
	//

	#define IMAGE_SYM_UNDEFINED           (SHORT)0          // Symbol is undefined or is common.
	#define IMAGE_SYM_ABSOLUTE            (SHORT)-1         // Symbol is an absolute value.
	#define IMAGE_SYM_DEBUG               (SHORT)-2         // Symbol is a special debug item.
	#define IMAGE_SYM_SECTION_MAX         0xFEFF            // Values 0xFF00-0xFFFF are special

	//
	// Type (fundamental) values.
	//

	#define IMAGE_SYM_TYPE_NULL                 0x0000  // no type.
	#define IMAGE_SYM_TYPE_VOID                 0x0001  //
	#define IMAGE_SYM_TYPE_CHAR                 0x0002  // type character.
	#define IMAGE_SYM_TYPE_SHORT                0x0003  // type short integer.
	#define IMAGE_SYM_TYPE_INT                  0x0004  //
	#define IMAGE_SYM_TYPE_LONG                 0x0005  //
	#define IMAGE_SYM_TYPE_FLOAT                0x0006  //
	#define IMAGE_SYM_TYPE_DOUBLE               0x0007  //
	#define IMAGE_SYM_TYPE_STRUCT               0x0008  //
	#define IMAGE_SYM_TYPE_UNION                0x0009  //
	#define IMAGE_SYM_TYPE_ENUM                 0x000A  // enumeration.
	#define IMAGE_SYM_TYPE_MOE                  0x000B  // member of enumeration.
	#define IMAGE_SYM_TYPE_BYTE                 0x000C  //
	#define IMAGE_SYM_TYPE_WORD                 0x000D  //
	#define IMAGE_SYM_TYPE_UINT                 0x000E  //
	#define IMAGE_SYM_TYPE_DWORD                0x000F  //
	#define IMAGE_SYM_TYPE_PCODE                0x8000  //

	//
	// Type (derived) values.
	//

	#define IMAGE_SYM_DTYPE_NULL                0       // no derived type.
	#define IMAGE_SYM_DTYPE_POINTER             1       // pointer.
	#define IMAGE_SYM_DTYPE_FUNCTION            2       // function.
	#define IMAGE_SYM_DTYPE_ARRAY               3       // array.

	//
	// Storage classes.
	//

	#define IMAGE_SYM_CLASS_END_OF_FUNCTION     (BYTE )-1
	#define IMAGE_SYM_CLASS_NULL                0x0000
	#define IMAGE_SYM_CLASS_AUTOMATIC           0x0001
	#define IMAGE_SYM_CLASS_EXTERNAL            0x0002
	#define IMAGE_SYM_CLASS_STATIC              0x0003
	#define IMAGE_SYM_CLASS_REGISTER            0x0004
	#define IMAGE_SYM_CLASS_EXTERNAL_DEF        0x0005
	#define IMAGE_SYM_CLASS_LABEL               0x0006
	#define IMAGE_SYM_CLASS_UNDEFINED_LABEL     0x0007
	#define IMAGE_SYM_CLASS_MEMBER_OF_STRUCT    0x0008
	#define IMAGE_SYM_CLASS_ARGUMENT            0x0009
	#define IMAGE_SYM_CLASS_STRUCT_TAG          0x000A
	#define IMAGE_SYM_CLASS_MEMBER_OF_UNION     0x000B
	#define IMAGE_SYM_CLASS_UNION_TAG           0x000C
	#define IMAGE_SYM_CLASS_TYPE_DEFINITION     0x000D
	#define IMAGE_SYM_CLASS_UNDEFINED_STATIC    0x000E
	#define IMAGE_SYM_CLASS_ENUM_TAG            0x000F
	#define IMAGE_SYM_CLASS_MEMBER_OF_ENUM      0x0010
	#define IMAGE_SYM_CLASS_REGISTER_PARAM      0x0011
	#define IMAGE_SYM_CLASS_BIT_FIELD           0x0012

	#define IMAGE_SYM_CLASS_FAR_EXTERNAL        0x0044  //

	#define IMAGE_SYM_CLASS_BLOCK               0x0064
	#define IMAGE_SYM_CLASS_FUNCTION            0x0065
	#define IMAGE_SYM_CLASS_END_OF_STRUCT       0x0066
	#define IMAGE_SYM_CLASS_FILE                0x0067
	// new
	#define IMAGE_SYM_CLASS_SECTION             0x0068
	#define IMAGE_SYM_CLASS_WEAK_EXTERNAL       0x0069

	#define IMAGE_SYM_CLASS_CLR_TOKEN           0x006B

	//
	// Auxiliary entry format.
	//

	union AuxSymbol {
		struct {
			DWORD	 tagIndex;						// struct, union, or enum tag index
			union {
				struct {
					WORD	lineNumber; 			// declaration line number
					WORD	size;					// size of struct, union, or enum
				};
				union {
					DWORD	totalSize;
					DWORD	characteristics;
				};
			};
			union {
				struct {							// if ISFCN, tag, or .bb
					DWORD	 pointerToLinenumber;
					DWORD	 pointerToNextFunction;
				} function;
				struct {							// if ISARY, up to 4 dimen.
					WORD	 dimension[4];
				} array;
			};
			WORD	TvIndex;						// tv index
		} symbol;	// function fefinitions / .bf and .ef symbols / weak externals
		struct {
			BYTE	Name[18];
		} file;		// files
		struct {
			DWORD	length; 						// section length
			WORD	numberOfRelocations;			// number of relocation entries
			WORD	numberOfLinenumbers;			// number of line numbers
			DWORD	checkSum;						// checksum for communal
			SHORT	number; 						// section number to associate with
			BYTE	selection;						// communal selection type
		} section;	// section definitions
	};

	//
	// Communal selection types.
	//

	#define IMAGE_COMDAT_SELECT_NODUPLICATES    1
	#define IMAGE_COMDAT_SELECT_ANY             2
	#define IMAGE_COMDAT_SELECT_SAME_SIZE       3
	#define IMAGE_COMDAT_SELECT_EXACT_MATCH     4
	#define IMAGE_COMDAT_SELECT_ASSOCIATIVE     5
	#define IMAGE_COMDAT_SELECT_LARGEST         6
	#define IMAGE_COMDAT_SELECT_NEWEST          7

	#define IMAGE_WEAK_EXTERN_SEARCH_NOLIBRARY  1
	#define IMAGE_WEAK_EXTERN_SEARCH_LIBRARY    2
	#define IMAGE_WEAK_EXTERN_SEARCH_ALIAS      3

	//
	// Export Format
	//

	struct ExportDirectory {
		union {
			DWORD	Characteristics;
			DWORD	exportFlags;
		};
		DWORD	TimeDateStamp;
		WORD	MajorVersion;
		WORD	MinorVersion;
		DWORD	Name;
		DWORD	Base;
		DWORD	NumberOfFunctions;
		DWORD	NumberOfNames;
		DWORD	AddressOfFunctions; 	// RVA from base of image
		DWORD	AddressOfNames; 		// RVA from base of image
		DWORD	AddressOfNameOrdinals;	// RVA from base of image
	};

	}

	//
	// Image classes
	//

	class Image;

	class Section : public Raw::SectionHeader {
		friend class Image;
		Image& image;
		Section(Image& img, const Raw::SectionHeader& sh) : image(img) {
			 *(Raw::SectionHeader*)this = sh;
		}
	public:
		const byte* getData() const;
	};

	class Image {
		byte* image;
		const Raw::Header* header;
		const Raw::OptionalHeader* optionalHeader;
		std::vector<Section> sections;
	public:
		Image(const void* p);
		~Image();
	public:
        const byte* getImageBase() const { return image; }
		PROPERTY(const Raw::Header&, Header) const { return *header; }
		PROPERTY(const Raw::OptionalHeader&, OptionalHeader) const { return *optionalHeader; }
	public:
		const Section& getSection(const char* name) const;
	};

}

#include "unpack.h"
#include "unalign.h"
