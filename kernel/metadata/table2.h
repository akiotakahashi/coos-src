#pragma once

#include "flags.h"
#include "index.h"
#include "index2.h"


namespace Metadata {

	static uint Fetch(const byte*& row, int size) {
		uint value;
		switch(size) {
		default:
			panic("Table::Fetch");
		case 1:
			value = *(byte*)row;
			break;
		case 2:
			value = *(ushort*)row;
			break;
		case 3:
			value = 0xFFFFFF & *(uint32*)row;
			break;
		case 4:
			value = *(uint32*)row;
			break;
		}
		row += size;
		return value;
	}

	struct Assembly : Table {
		AssemblyHashAlgorithm HashAlgId;
		uint16 MajorVersion;
		uint16 MinorVersion;
		uint16 BuildNumber;
		uint16 RevisionNumber;
		AssemblyFlags Flags;
		uint PublicKey;
		uint Name;
		uint Culture;
		Assembly(const byte* row, const uint idxsizes[]) {
			HashAlgId = (AssemblyHashAlgorithm) Fetch(row, 4 ) ;
			MajorVersion = (uint16) Fetch(row, sizeof(uint16) ) ;
			MinorVersion = (uint16) Fetch(row, sizeof(uint16) ) ;
			BuildNumber = (uint16) Fetch(row, sizeof(uint16) ) ;
			RevisionNumber = (uint16) Fetch(row, sizeof(uint16) ) ;
			Flags = (AssemblyFlags) Fetch(row, 4 ) ;
			PublicKey = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Culture = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
		} 
	}; 

	struct AssemblyOS : Table {
		uint32 OSPlatformID;
		uint32 OSMajorVersion;
		uint32 OSMinorVersion;
		AssemblyOS(const byte* row, const uint idxsizes[]) {
			OSPlatformID = (uint32) Fetch(row, sizeof(uint32) ) ;
			OSMajorVersion = (uint32) Fetch(row, sizeof(uint32) ) ;
			OSMinorVersion = (uint32) Fetch(row, sizeof(uint32) ) ;
		} 
	}; 

	struct AssemblyProcessor : Table {
		uint32 Processor;
		AssemblyProcessor(const byte* row, const uint idxsizes[]) {
			Processor = (uint32) Fetch(row, sizeof(uint32) ) ;
		} 
	}; 

	struct AssemblyRef : Table {
		uint16 MajorVersion;
		uint16 MinorVersion;
		uint16 BuildNumber;
		uint16 RevisionNumber;
		AssemblyFlags Flags;
		uint PublicKeyOrToken;
		uint Name;
		uint Culture;
		uint HashValue;
		AssemblyRef(const byte* row, const uint idxsizes[]) {
			MajorVersion = (uint16) Fetch(row, sizeof(uint16) ) ;
			MinorVersion = (uint16) Fetch(row, sizeof(uint16) ) ;
			BuildNumber = (uint16) Fetch(row, sizeof(uint16) ) ;
			RevisionNumber = (uint16) Fetch(row, sizeof(uint16) ) ;
			Flags = (AssemblyFlags) Fetch(row, 4 ) ;
			PublicKeyOrToken = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Culture = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			HashValue = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct AssemblyRefOS : Table {
		uint32 OSPlatformId;
		uint32 OSMajorVersion;
		uint32 OSMinorVersion;
		uint AssemblyRef;
		AssemblyRefOS(const byte* row, const uint idxsizes[]) {
			OSPlatformId = (uint32) Fetch(row, sizeof(uint32) ) ;
			OSMajorVersion = (uint32) Fetch(row, sizeof(uint32) ) ;
			OSMinorVersion = (uint32) Fetch(row, sizeof(uint32) ) ;
			AssemblyRef = (uint) Fetch(row, idxsizes[INDEX_AssemblyRef] ) ;
		} 
	}; 

	struct AssemblyRefProcessor : Table {
		uint32 Processor;
		uint AssemblyRef;
		AssemblyRefProcessor(const byte* row, const uint idxsizes[]) {
			Processor = (uint32) Fetch(row, sizeof(uint32) ) ;
			AssemblyRef = (uint) Fetch(row, idxsizes[INDEX_AssemblyRef] ) ;
		} 
	}; 

	struct ClassLayout : Table {
		uint16 PackingSize;
		uint32 ClassSize;
		uint Parent;
		ClassLayout(const byte* row, const uint idxsizes[]) {
			PackingSize = (uint16) Fetch(row, sizeof(uint16) ) ;
			ClassSize = (uint32) Fetch(row, sizeof(uint32) ) ;
			Parent = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
		} 
	}; 

	struct Constant : Table {
		uint8 Type;
		uint8 Padding;
		HasConstIndex Parent;
		uint Value;
		Constant(const byte* row, const uint idxsizes[]) {
			Type = (uint8) Fetch(row, sizeof(uint8) ) ;
			Padding = (uint8) Fetch(row, sizeof(uint8) ) ;
			Parent = HasConstIndex( Fetch(row, idxsizes[INDEX_HasConst] ) ) ;
			Value = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct CustomAttribute : Table {
		HasCustomAttributeIndex Parent;
		CustomAttributeTypeIndex Type;
		uint Value;
		CustomAttribute(const byte* row, const uint idxsizes[]) {
			Parent = HasCustomAttributeIndex( Fetch(row, idxsizes[INDEX_HasCustomAttribute] ) ) ;
			Type = CustomAttributeTypeIndex( Fetch(row, idxsizes[INDEX_CustomAttributeType] ) ) ;
			Value = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct DeclSecurity : Table {
		uint16 Action;
		HasDeclSecurityIndex Parent;
		uint PermissionSet;
		DeclSecurity(const byte* row, const uint idxsizes[]) {
			Action = (uint16) Fetch(row, sizeof(uint16) ) ;
			Parent = HasDeclSecurityIndex( Fetch(row, idxsizes[INDEX_HasDeclSecurity] ) ) ;
			PermissionSet = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct Event : Table {
		EventAttributes EventFlags;
		uint Name;
		TypeDefOrRefIndex EventType;
		Event(const byte* row, const uint idxsizes[]) {
			EventFlags = (EventAttributes) Fetch(row, 2 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			EventType = TypeDefOrRefIndex( Fetch(row, idxsizes[INDEX_TypeDefOrRef] ) ) ;
		} 
	}; 

	struct EventMap : Table {
		uint Parent;
		uint EventList;
		EventMap(const byte* row, const uint idxsizes[]) {
			Parent = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
			EventList = (uint) Fetch(row, idxsizes[INDEX_Event] ) ;
		} 
	}; 

	struct ExportedType : Table {
		TypeAttributes Flags;
		uint32 TypeDefId;
		uint TypeName;
		uint TypeNamespace;
		ImplementationIndex Implementation;
		ExportedType(const byte* row, const uint idxsizes[]) {
			Flags = (TypeAttributes) Fetch(row, 4 ) ;
			TypeDefId = (uint32) Fetch(row, sizeof(uint32) ) ;
			TypeName = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			TypeNamespace = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Implementation = ImplementationIndex( Fetch(row, idxsizes[INDEX_Implementation] ) ) ;
		} 
	}; 

	struct Field : Table {
		FieldAttributes Flags;
		uint Name;
		uint Signature;
		Field(const byte* row, const uint idxsizes[]) {
			Flags = (FieldAttributes) Fetch(row, 2 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Signature = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct FieldLayout : Table {
		uint32 Offset;
		uint Field;
		FieldLayout(const byte* row, const uint idxsizes[]) {
			Offset = (uint32) Fetch(row, sizeof(uint32) ) ;
			Field = (uint) Fetch(row, idxsizes[INDEX_Field] ) ;
		} 
	}; 

	struct FieldMarshal : Table {
		HasFieldMarshalIndex Parent;
		uint NativeType;
		FieldMarshal(const byte* row, const uint idxsizes[]) {
			Parent = HasFieldMarshalIndex( Fetch(row, idxsizes[INDEX_HasFieldMarshal] ) ) ;
			NativeType = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct FieldRVA : Table {
		uint32 RVA;
		uint Field;
		FieldRVA(const byte* row, const uint idxsizes[]) {
			RVA = (uint32) Fetch(row, sizeof(uint32) ) ;
			Field = (uint) Fetch(row, idxsizes[INDEX_Field] ) ;
		} 
	}; 

	struct File : Table {
		FileAttributes Flags;
		uint Name;
		uint HashValue;
		File(const byte* row, const uint idxsizes[]) {
			Flags = (FileAttributes) Fetch(row, 4 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			HashValue = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct ImplMap : Table {
		PInvokeAttributes MappingFlags;
		MemberForwardedIndex MemberForwarded;
		uint ImportName;
		uint ImportScope;
		ImplMap(const byte* row, const uint idxsizes[]) {
			MappingFlags = (PInvokeAttributes) Fetch(row, 2 ) ;
			MemberForwarded = MemberForwardedIndex( Fetch(row, idxsizes[INDEX_MemberForwarded] ) ) ;
			ImportName = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			ImportScope = (uint) Fetch(row, idxsizes[INDEX_ModuleRef] ) ;
		} 
	}; 

	struct InterfaceImpl : Table {
		uint Class;
		TypeDefOrRefIndex Interface;
		InterfaceImpl(const byte* row, const uint idxsizes[]) {
			Class = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
			Interface = TypeDefOrRefIndex( Fetch(row, idxsizes[INDEX_TypeDefOrRef] ) ) ;
		} 
	}; 

	struct ManifestResource : Table {
		uint32 Offset;
		ManifestResourceAttributes Flags;
		uint Name;
		ImplementationIndex Implementation;
		ManifestResource(const byte* row, const uint idxsizes[]) {
			Offset = (uint32) Fetch(row, sizeof(uint32) ) ;
			Flags = (ManifestResourceAttributes) Fetch(row, 4 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Implementation = ImplementationIndex( Fetch(row, idxsizes[INDEX_Implementation] ) ) ;
		} 
	}; 

	struct MemberRef : Table {
		MemberRefParentIndex Class;
		uint Name;
		uint Signature;
		MemberRef(const byte* row, const uint idxsizes[]) {
			Class = MemberRefParentIndex( Fetch(row, idxsizes[INDEX_MemberRefParent] ) ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Signature = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct Method : Table {
		uint32 RVA;
		MethodImplAttributes ImplFlags;
		MethodAttributes Flags;
		uint Name;
		uint Signature;
		uint ParamList;
		Method(const byte* row, const uint idxsizes[]) {
			RVA = (uint32) Fetch(row, sizeof(uint32) ) ;
			ImplFlags = (MethodImplAttributes) Fetch(row, 2 ) ;
			Flags = (MethodAttributes) Fetch(row, 2 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Signature = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
			ParamList = (uint) Fetch(row, idxsizes[INDEX_Param] ) ;
		} 
	}; 

	struct MethodImpl : Table {
		uint Class;
		MethodDefOrRefIndex MethodBody;
		MethodDefOrRefIndex MethodDeclaration;
		MethodImpl(const byte* row, const uint idxsizes[]) {
			Class = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
			MethodBody = MethodDefOrRefIndex( Fetch(row, idxsizes[INDEX_MethodDefOrRef] ) ) ;
			MethodDeclaration = MethodDefOrRefIndex( Fetch(row, idxsizes[INDEX_MethodDefOrRef] ) ) ;
		} 
	}; 

	struct MethodSemantics : Table {
		MethodSemanticsAttributes Semantics;
		uint Method;
		HasSemanticsIndex Association;
		MethodSemantics(const byte* row, const uint idxsizes[]) {
			Semantics = (MethodSemanticsAttributes) Fetch(row, 2 ) ;
			Method = (uint) Fetch(row, idxsizes[INDEX_Method] ) ;
			Association = HasSemanticsIndex( Fetch(row, idxsizes[INDEX_HasSemantics] ) ) ;
		} 
	}; 

	struct Module : Table {
		uint16 Generation;
		uint Name;
		uint Mvid;
		uint EncId;
		uint EncBaseId;
		Module(const byte* row, const uint idxsizes[]) {
			Generation = (uint16) Fetch(row, sizeof(uint16) ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Mvid = (uint) Fetch(row, idxsizes[INDEX_Guid] ) ;
			EncId = (uint) Fetch(row, idxsizes[INDEX_Guid] ) ;
			EncBaseId = (uint) Fetch(row, idxsizes[INDEX_Guid] ) ;
		} 
	}; 

	struct ModuleRef : Table {
		uint Name;
		ModuleRef(const byte* row, const uint idxsizes[]) {
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
		} 
	}; 

	struct NestedClass : Table {
		uint nestedClass;
		uint EnclosingClass;
		NestedClass(const byte* row, const uint idxsizes[]) {
			nestedClass = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
			EnclosingClass = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
		} 
	}; 

	struct Param : Table {
		ParamAttributes Flags;
		uint16 Sequence;
		uint Name;
		Param(const byte* row, const uint idxsizes[]) {
			Flags = (ParamAttributes) Fetch(row, 2 ) ;
			Sequence = (uint16) Fetch(row, sizeof(uint16) ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
		} 
	}; 

	struct Property : Table {
		PropertyAttributes Flags;
		uint Name;
		uint Type;
		Property(const byte* row, const uint idxsizes[]) {
			Flags = (PropertyAttributes) Fetch(row, 2 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Type = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct PropertyMap : Table {
		uint Parent;
		uint PropertyList;
		PropertyMap(const byte* row, const uint idxsizes[]) {
			Parent = (uint) Fetch(row, idxsizes[INDEX_TypeDef] ) ;
			PropertyList = (uint) Fetch(row, idxsizes[INDEX_Property] ) ;
		} 
	}; 

	struct StandAloneSig : Table {
		uint Signature;
		StandAloneSig(const byte* row, const uint idxsizes[]) {
			Signature = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

	struct TypeDef : Table {
		TypeAttributes Flags;
		uint Name;
		uint Namespace;
		TypeDefOrRefIndex Extends;
		uint FieldList;
		uint MethodList;
		TypeDef(const byte* row, const uint idxsizes[]) {
			Flags = (TypeAttributes) Fetch(row, 4 ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Namespace = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Extends = TypeDefOrRefIndex( Fetch(row, idxsizes[INDEX_TypeDefOrRef] ) ) ;
			FieldList = (uint) Fetch(row, idxsizes[INDEX_Field] ) ;
			MethodList = (uint) Fetch(row, idxsizes[INDEX_Method] ) ;
		} 
	}; 

	struct TypeRef : Table {
		ResolutionScopeIndex ResolutionScope;
		uint Name;
		uint Namespace;
		TypeRef(const byte* row, const uint idxsizes[]) {
			ResolutionScope = ResolutionScopeIndex( Fetch(row, idxsizes[INDEX_ResolutionScope] ) ) ;
			Name = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
			Namespace = (uint) Fetch(row, idxsizes[INDEX_Strings] ) ;
		} 
	}; 

	struct TypeSpec : Table {
		uint Signature;
		TypeSpec(const byte* row, const uint idxsizes[]) {
			Signature = (uint) Fetch(row, idxsizes[INDEX_Blob] ) ;
		} 
	}; 

}
