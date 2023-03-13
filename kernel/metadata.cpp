#include "kernel.h"
#include "utility.h"
#include "metadata.h"
#include "stdtype.h"


static inline int roundup(int n, int b) {
	return n + (b-n%b);
}

namespace Metadata {
	
	MetadataRoot* MetadataRoot::FromPEImage(const PE::Image& peimg) {
		PE::Section section = peimg.getSection(".text");
		const PE::Raw::OptionalHeader& opthdr = peimg.getOptionalHeader();
		const Raw::CliHeader& clihdr = *reinterpret_cast<const Raw::CliHeader*>(
			peimg.getImageBase()+opthdr.dataDirectory[IMAGE_DDE_COMPLUS].rva);
		return new MetadataRoot(peimg, clihdr, peimg.getImageBase()+clihdr.MetaData.rva);
	}

	MetadataRoot::MetadataRoot(const PE::Image& _peimg, const Raw::CliHeader& clihdr, const void* image) : peimg(_peimg) {

		Console& console = getConsole();

		this->clihdr = clihdr;

		stm_main = NULL;
		stm_blob = NULL;
		stm_guid = NULL;
		stm_strings = NULL;
		stm_userstring = NULL;

		const Raw::MetadataRoot1& mr1 = *reinterpret_cast<const Raw::MetadataRoot1*>(image);
		*(Raw::MetadataRoot1*)this = mr1;
		//*
		console << "signature: " << signature << "\r\n";
		console << "version: " << majorVersion << "." << minorVersion << "\r\n";
		console << "length: " << length << "\r\n";
		console << "version: " << version << "\r\n";
		//*/

		image = reinterpret_cast<const Raw::MetadataRoot1*>(image)->version+length;
		*(Raw::MetadataRoot2*)this = *reinterpret_cast<const Raw::MetadataRoot2*>(image);
		/*
		console << "flags: " << flags << "\r\n";
		console << "streams: " << streams << "\r\n";
		//*/

		image = reinterpret_cast<const Raw::MetadataRoot2*>(image)+1;
		const Raw::StreamHeader* sh = reinterpret_cast<const Raw::StreamHeader*>(image);
		const byte* base = reinterpret_cast<const byte*>(&mr1);
		for(int i=0; i<streams; ++i) {
			//console << sh->name << " (" << sh->size << " bytes)" << endl;
			if(streql(sh->name,"#~")) {
				stm_main = new MainStream(*sh, base+sh->offset);
			} else if(streql(sh->name,"#Blob")) {
				stm_blob = new BlobStream(*sh, base+sh->offset);
			} else if(streql(sh->name,"#GUID")) {
				stm_guid = new GuidStream(*sh, base+sh->offset);
			} else if(streql(sh->name,"#Strings")) {
				stm_strings = new StringStream(*sh, base+sh->offset);
			} else if(streql(sh->name,"#US")) {
				stm_userstring = new UserStringStream(*sh, base+sh->offset);
			} else {
				panic("UNKNOWN STREAM IN METADATA");
			}
			sh = reinterpret_cast<const Raw::StreamHeader*>(sh->name+roundup(strlen(sh->name),4));
		}

		if(stm_userstring==NULL) {
			stm_userstring = new UserStringStream();
		}

		stm_main->Parse(*this);
		stm_blob->Parse(*this);
		stm_guid->Parse(*this);
		stm_strings->Parse(*this);
		if(stm_userstring!=NULL) stm_userstring->Parse(*this);

	}

	MetadataRoot::~MetadataRoot() {
		delete stm_main;
		delete stm_blob;
		delete stm_guid;
		delete stm_strings;
		delete stm_userstring;
		/*
		for(uint i=0; i<streamBodies.size(); ++i) {
			delete streamBodies[i];
		}
		*/
	}

	int MetadataRoot::GetVTableFixupCount() const {
		return clihdr.VTableFixups.size/sizeof(Raw::VTableFixup);
	}

	const Raw::VTableFixup& MetadataRoot::GetVTableFixup(int index) const {
		const byte* p = peimg.getImageBase()+clihdr.VTableFixups.rva;
		p += sizeof(Raw::VTableFixup)*index;
		return *(const Raw::VTableFixup*)p;
	}

	MainStream::MainStream(const Raw::StreamHeader& sh, const void* base) : Stream(sh) {
		Console& console = getConsole();
		imgbase = (const byte*)base;
		//
		*static_cast<Raw::MainStream_*>(this) = *reinterpret_cast<const Raw::MainStream_*>(base);
		/*
		console << "version: " << majorVersion << "." << minorVersion << "\r\n";
		console << "heapsize: " << heapSizes << "\r\n";
		console << "valid: " << valid << "\r\n";
		console << "sorted: " << sorted << "\r\n";
		//*/
		zero(rowcounts, sizeof(rowcounts));
		zero(offsets, sizeof(offsets));
		//console << "rowcounts: ";
		int n = 0;
		for(int i=0; i<TABLE_LAST; ++i) {
			//console << (valid & (static_cast<uint64>(1)<<i)) << "/";
			if(valid & (static_cast<uint64>(1)<<i)) {
				rowcounts[i] = reinterpret_cast<const DWORD*>((const Raw::MainStream_*)base+1)[n++];
				//console << (byte)i << "-" << (int)rowcounts[i] << ",";
			}
		}
		//console << endl;
		//
		database = (const byte*)base+sizeof(Raw::MainStream_)+sizeof(DWORD)*n;
	}

	MainStream::~MainStream() {
	}

	static int getIndexFieldSize(uint rowcount) {
		return rowcount>0xFFFF ? 4 : 2;
	}

	void MainStream::Parse(const MetadataRoot& metadata) {
		Console& console = getConsole();
		// Determine Index Sizes
		memclr(idxsizes, sizeof(idxsizes));
		idxsizes[INDEX_Blob] = getIndexFieldSize(metadata.getBlobStream().size);
		idxsizes[INDEX_Guid] = getIndexFieldSize(metadata.getGuidStream().size);
		idxsizes[INDEX_Strings] = getIndexFieldSize(metadata.getStringStream().size);
		idxsizes[INDEX_UserString] = getIndexFieldSize(metadata.getUserStringStream().size);
		//
		idxsizes[INDEX_Assembly] = getIndexFieldSize(getRowCount(TABLE_Assembly));
		idxsizes[INDEX_AssemblyOS] = getIndexFieldSize(getRowCount(TABLE_AssemblyOS));
		idxsizes[INDEX_AssemblyProcessor] = getIndexFieldSize(getRowCount(TABLE_AssemblyProcessor));
		idxsizes[INDEX_AssemblyRef] = getIndexFieldSize(getRowCount(TABLE_AssemblyRef));
		idxsizes[INDEX_AssemblyRefOS] = getIndexFieldSize(getRowCount(TABLE_AssemblyRefOS));
		idxsizes[INDEX_AssemblyRefProcessor] = getIndexFieldSize(getRowCount(TABLE_AssemblyRefProcessor));
		idxsizes[INDEX_ClassLayout] = getIndexFieldSize(getRowCount(TABLE_ClassLayout));
		idxsizes[INDEX_Constant] = getIndexFieldSize(getRowCount(TABLE_Constant));
		idxsizes[INDEX_CustomAttribute] = getIndexFieldSize(getRowCount(TABLE_CustomAttribute));
		idxsizes[INDEX_DeclSecurity] = getIndexFieldSize(getRowCount(TABLE_DeclSecurity));
		idxsizes[INDEX_Event] = getIndexFieldSize(getRowCount(TABLE_Event));
		idxsizes[INDEX_EventMap] = getIndexFieldSize(getRowCount(TABLE_EventMap));
		idxsizes[INDEX_ExportedType] = getIndexFieldSize(getRowCount(TABLE_ExportedType));
		idxsizes[INDEX_Field] = getIndexFieldSize(getRowCount(TABLE_Field));
		idxsizes[INDEX_FieldLayout] = getIndexFieldSize(getRowCount(TABLE_FieldLayout));
		idxsizes[INDEX_FieldMarshal] = getIndexFieldSize(getRowCount(TABLE_FieldMarshal));
		idxsizes[INDEX_FieldRVA] = getIndexFieldSize(getRowCount(TABLE_FieldRVA));
		idxsizes[INDEX_File] = getIndexFieldSize(getRowCount(TABLE_File));
		idxsizes[INDEX_ImplMap] = getIndexFieldSize(getRowCount(TABLE_ImplMap));
		idxsizes[INDEX_InterfaceImpl] = getIndexFieldSize(getRowCount(TABLE_InterfaceImpl));
		idxsizes[INDEX_ManifestResource] = getIndexFieldSize(getRowCount(TABLE_ManifestResource));
		idxsizes[INDEX_MemberRef] = getIndexFieldSize(getRowCount(TABLE_MemberRef));
		idxsizes[INDEX_Method] = getIndexFieldSize(getRowCount(TABLE_Method));
		idxsizes[INDEX_MethodImpl] = getIndexFieldSize(getRowCount(TABLE_MethodImpl));
		idxsizes[INDEX_MethodSemantics] = getIndexFieldSize(getRowCount(TABLE_MethodSemantics));
		idxsizes[INDEX_Module] = getIndexFieldSize(getRowCount(TABLE_Module));
		idxsizes[INDEX_ModuleRef] = getIndexFieldSize(getRowCount(TABLE_ModuleRef));
		idxsizes[INDEX_NestedClass] = getIndexFieldSize(getRowCount(TABLE_NestedClass));
		idxsizes[INDEX_Param] = getIndexFieldSize(getRowCount(TABLE_Param));
		idxsizes[INDEX_Property] = getIndexFieldSize(getRowCount(TABLE_Property));
		idxsizes[INDEX_PropertyMap] = getIndexFieldSize(getRowCount(TABLE_PropertyMap));
		idxsizes[INDEX_StandAloneSig] = getIndexFieldSize(getRowCount(TABLE_StandAloneSig));
		idxsizes[INDEX_TypeDef] = getIndexFieldSize(getRowCount(TABLE_TypeDef));
		idxsizes[INDEX_TypeRef] = getIndexFieldSize(getRowCount(TABLE_TypeRef));
		idxsizes[INDEX_TypeSpec] = getIndexFieldSize(getRowCount(TABLE_TypeSpec));
		//
		idxsizes[INDEX_CustomAttributeType] = CustomAttributeTypeIndex::CalcSize(rowcounts);
		idxsizes[INDEX_HasConst] = HasConstIndex::CalcSize(rowcounts);
		idxsizes[INDEX_HasCustomAttribute] = HasCustomAttributeIndex::CalcSize(rowcounts);
		idxsizes[INDEX_HasDeclSecurity] = HasDeclSecurityIndex::CalcSize(rowcounts);
		idxsizes[INDEX_HasFieldMarshal] = HasFieldMarshalIndex::CalcSize(rowcounts);
		idxsizes[INDEX_HasSemantics] = HasSemanticsIndex::CalcSize(rowcounts);
		idxsizes[INDEX_Implementation] = ImplementationIndex::CalcSize(rowcounts);
		idxsizes[INDEX_MemberForwarded] = MemberForwardedIndex::CalcSize(rowcounts);
		idxsizes[INDEX_MemberRefParent] = MemberRefParentIndex::CalcSize(rowcounts);
		idxsizes[INDEX_MethodDefOrRef] = MethodDefOrRefIndex::CalcSize(rowcounts);
		idxsizes[INDEX_ResolutionScope] = ResolutionScopeIndex::CalcSize(rowcounts);
		idxsizes[INDEX_TypeDefOrRef] = TypeDefOrRefIndex::CalcSize(rowcounts);
		//
		/*
		console << "idx:";
		for(int i=0; i<INDEX_LAST; ++i) {
			if(idxsizes[i]) {
				console << i << "-" << (int)idxsizes[i] << ",";
			}
		}
		console << endl;
		//*/
		memclr(rowsizes, sizeof(rowsizes));
		rowsizes[TABLE_Assembly] = + 4 /* AssemblyHashAlgorithm */ + sizeof(uint16) + sizeof(uint16) + sizeof(uint16) + sizeof(uint16) + 4 /* AssemblyFlags */ + idxsizes[INDEX_Blob] + idxsizes[INDEX_Strings] + idxsizes[INDEX_Strings] ;
		rowsizes[TABLE_AssemblyOS] = + sizeof(uint32) + sizeof(uint32) + sizeof(uint32) ;
		rowsizes[TABLE_AssemblyProcessor] = + sizeof(uint32) ;
		rowsizes[TABLE_AssemblyRef] = + sizeof(uint16) + sizeof(uint16) + sizeof(uint16) + sizeof(uint16) + 4 /* AssemblyFlags */ + idxsizes[INDEX_Blob] + idxsizes[INDEX_Strings] + idxsizes[INDEX_Strings] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_AssemblyRefOS] = + sizeof(uint32) + sizeof(uint32) + sizeof(uint32) + idxsizes[INDEX_AssemblyRef] ;
		rowsizes[TABLE_AssemblyRefProcessor] = + sizeof(uint32) + idxsizes[INDEX_AssemblyRef] ;
		rowsizes[TABLE_ClassLayout] = + sizeof(uint16) + sizeof(uint32) + idxsizes[INDEX_TypeDef] ;
		rowsizes[TABLE_Constant] = + sizeof(uint8) + sizeof(uint8) + idxsizes[INDEX_HasConst] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_CustomAttribute] = + idxsizes[INDEX_HasCustomAttribute] + idxsizes[INDEX_CustomAttributeType] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_DeclSecurity] = + sizeof(uint16) + idxsizes[INDEX_HasDeclSecurity] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_Event] = + 2 /* EventAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_TypeDefOrRef] ;
		rowsizes[TABLE_EventMap] = + idxsizes[INDEX_TypeDef] + idxsizes[INDEX_Event] ;
		rowsizes[TABLE_ExportedType] = + 4 /* TypeAttributes */ + sizeof(uint32) + idxsizes[INDEX_Strings] + idxsizes[INDEX_Strings] + idxsizes[INDEX_Implementation] ;
		rowsizes[TABLE_Field] = + 2 /* FieldAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_FieldLayout] = + sizeof(uint32) + idxsizes[INDEX_Field] ;
		rowsizes[TABLE_FieldMarshal] = + idxsizes[INDEX_HasFieldMarshal] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_FieldRVA] = + sizeof(uint32) + idxsizes[INDEX_Field] ;
		rowsizes[TABLE_File] = + 4 /* FileAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_ImplMap] = + 2 /* PInvokeAttributes */ + idxsizes[INDEX_MemberForwarded] + idxsizes[INDEX_Strings] + idxsizes[INDEX_ModuleRef] ;
		rowsizes[TABLE_InterfaceImpl] = + idxsizes[INDEX_TypeDef] + idxsizes[INDEX_TypeDefOrRef] ;
		rowsizes[TABLE_ManifestResource] = + sizeof(uint32) + 4 /* ManifestResourceAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_Implementation] ;
		rowsizes[TABLE_MemberRef] = + idxsizes[INDEX_MemberRefParent] + idxsizes[INDEX_Strings] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_Method] = + sizeof(uint32) + 2 /* MethodImplAttributes */ + 2 /* MethodAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_Blob] + idxsizes[INDEX_Param] ;
		rowsizes[TABLE_MethodImpl] = + idxsizes[INDEX_TypeDef] + idxsizes[INDEX_MethodDefOrRef] + idxsizes[INDEX_MethodDefOrRef] ;
		rowsizes[TABLE_MethodSemantics] = + 2 /* MethodSemanticsAttributes */ + idxsizes[INDEX_Method] + idxsizes[INDEX_HasSemantics] ;
		rowsizes[TABLE_Module] = + sizeof(uint16) + idxsizes[INDEX_Strings] + idxsizes[INDEX_Guid] + idxsizes[INDEX_Guid] + idxsizes[INDEX_Guid] ;
		rowsizes[TABLE_ModuleRef] = + idxsizes[INDEX_Strings] ;
		rowsizes[TABLE_NestedClass] = + idxsizes[INDEX_TypeDef] + idxsizes[INDEX_TypeDef] ;
		rowsizes[TABLE_Param] = + 2 /* ParamAttributes */ + sizeof(uint16) + idxsizes[INDEX_Strings] ;
		rowsizes[TABLE_Property] = + 2 /* PropertyAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_PropertyMap] = + idxsizes[INDEX_TypeDef] + idxsizes[INDEX_Property] ;
		rowsizes[TABLE_StandAloneSig] = + idxsizes[INDEX_Blob] ;
		rowsizes[TABLE_TypeDef] = + 4 /* TypeAttributes */ + idxsizes[INDEX_Strings] + idxsizes[INDEX_Strings] + idxsizes[INDEX_TypeDefOrRef] + idxsizes[INDEX_Field] + idxsizes[INDEX_Method] ;
		rowsizes[TABLE_TypeRef] = + idxsizes[INDEX_ResolutionScope] + idxsizes[INDEX_Strings] + idxsizes[INDEX_Strings] ;
		rowsizes[TABLE_TypeSpec] = + idxsizes[INDEX_Blob] ;
		/*
		console << "row:";
		for(int i=0; i<TABLE_LAST; ++i) {
			if(rowcounts[i]>0) {
				console << (byte)i << "-" << (int)rowsizes[i] << ",";
			}
		}
		console << endl;
		//*/
		uint offset = 0;
		const byte* p = database;
		for(int tid=0; tid<TABLE_LAST; ++tid) {
			//console << "Table Offset " << (byte)tid << ": " << offset << endl;
			uint rowsize = rowsizes[tid];
			// 1 ベースでアクセスされるので rowsize だけ戻しておく。
			offsets[tid] = offset-rowsize;
			if(rowcounts[tid]>0) {
				offset += rowsize*rowcounts[tid];
				//console << (byte)tid << "-" << (int)(offset+96-12) << endl;
			}
		}
		//
		//console << "MainStream Last Offset is " << offset << " (may equal " << size-(database-imgbase) << ")." << endl;
	}

	Table* MainStream::getRow(TableId tid, int index) const {
		if(index==0) return NULL;
		const byte* row = database+offsets[tid]+rowsizes[tid]*index;
		switch(tid) {
		default:
			panic("MainStream::getRow");
		case TABLE_Assembly:
			return new Assembly(row, idxsizes);
		case TABLE_AssemblyOS:
			return new AssemblyOS(row, idxsizes);
		case TABLE_AssemblyProcessor:
			return new AssemblyProcessor(row, idxsizes);
		case TABLE_AssemblyRef:
			return new AssemblyRef(row, idxsizes);
		case TABLE_AssemblyRefOS:
			return new AssemblyRefOS(row, idxsizes);
		case TABLE_AssemblyRefProcessor:
			return new AssemblyRefProcessor(row, idxsizes);
		case TABLE_ClassLayout:
			return new ClassLayout(row, idxsizes);
		case TABLE_Constant:
			return new Constant(row, idxsizes);
		case TABLE_CustomAttribute:
			return new CustomAttribute(row, idxsizes);
		case TABLE_DeclSecurity:
			return new DeclSecurity(row, idxsizes);
		case TABLE_Event:
			return new Event(row, idxsizes);
		case TABLE_EventMap:
			return new EventMap(row, idxsizes);
		case TABLE_ExportedType:
			return new ExportedType(row, idxsizes);
		case TABLE_Field:
			return new Field(row, idxsizes);
		case TABLE_FieldLayout:
			return new FieldLayout(row, idxsizes);
		case TABLE_FieldMarshal:
			return new FieldMarshal(row, idxsizes);
		case TABLE_FieldRVA:
			return new FieldRVA(row, idxsizes);
		case TABLE_File:
			return new File(row, idxsizes);
		case TABLE_ImplMap:
			return new ImplMap(row, idxsizes);
		case TABLE_InterfaceImpl:
			return new InterfaceImpl(row, idxsizes);
		case TABLE_ManifestResource:
			return new ManifestResource(row, idxsizes);
		case TABLE_MemberRef:
			return new MemberRef(row, idxsizes);
		case TABLE_Method:
			return new Method(row, idxsizes);
		case TABLE_MethodImpl:
			return new MethodImpl(row, idxsizes);
		case TABLE_MethodSemantics:
			return new MethodSemantics(row, idxsizes);
		case TABLE_Module:
			return new Module(row, idxsizes);
		case TABLE_ModuleRef:
			return new ModuleRef(row, idxsizes);
		case TABLE_NestedClass:
			return new NestedClass(row, idxsizes);
		case TABLE_Param:
			return new Param(row, idxsizes);
		case TABLE_Property:
			return new Property(row, idxsizes);
		case TABLE_PropertyMap:
			return new PropertyMap(row, idxsizes);
		case TABLE_StandAloneSig:
			return new StandAloneSig(row, idxsizes);
		case TABLE_TypeDef:
			return new TypeDef(row, idxsizes);
		case TABLE_TypeRef:
			return new TypeRef(row, idxsizes);
		case TABLE_TypeSpec:
			return new TypeSpec(row, idxsizes);
		}
	}

	StringStream::StringStream(const Raw::StreamHeader& sh, const void* _base) : Stream(sh), base((const char*)_base) {
	}

	BlobStream::BlobStream(const Raw::StreamHeader& sh, const void* _base) : Stream(sh), base((const byte*)_base) {
	}

	const MemoryRegion BlobStream::getData(uint index) const {
		const byte* p = base+index;
		uint value = ReadCompressedInteger(p);
		return MemoryRegion(p, value);
	}

	uint BlobStream::ReadCompressedInteger(const byte*& p) {
		uint value;
		if((p[0] & 0x80)==0) {
			value = p[0];
			p += 1;
			return value;
		} else if((p[0] & 0x40)==0) {
			value = p[1] | (p[0]&0x3F) << 8;
			p += 2;
			return value;
		} else if((p[0] & 0x20)==0) {
			value = p[3] | (p[2]<<8) | (p[1]<<16) | (p[0]&0x1F) << 24;
			p += 4;
			return value;
		} else {
			panic("Illegal Blob Szie Header");
		}
	}

}
