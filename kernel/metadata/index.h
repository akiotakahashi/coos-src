#pragma once

#include "../stdtype.h"
#include "../stl.h"
#include "../console.h"
#include "table.h"


enum IndexId {
	INDEX_Blob,
	INDEX_Guid,
	INDEX_Strings,
	INDEX_UserString,
	//----------------------------
	INDEX_Assembly,
	INDEX_AssemblyOS,
	INDEX_AssemblyProcessor,
	INDEX_AssemblyRef,
	INDEX_AssemblyRefOS,
	INDEX_AssemblyRefProcessor,
	INDEX_ClassLayout,
	INDEX_Constant,
	INDEX_CustomAttribute,
	INDEX_CustomAttributeType,
	INDEX_DeclSecurity,
	INDEX_Event,
	INDEX_EventMap,
	INDEX_ExportedType,
	INDEX_Field,
	INDEX_FieldLayout,
	INDEX_FieldMarshal,
	INDEX_FieldRVA,
	INDEX_File,
	INDEX_HasConst,
	INDEX_HasCustomAttribute,
	INDEX_HasDeclSecurity,
	INDEX_HasFieldMarshal,
	INDEX_HasSemantics,
	INDEX_Implementation,
	INDEX_ImplMap,
	INDEX_InterfaceImpl,
	INDEX_ManifestResource,
	INDEX_MemberForwarded,
	INDEX_MemberRef,
	INDEX_MemberRefParent,
	INDEX_Method,
	INDEX_MethodDefOrRef,
	INDEX_MethodImpl,
	INDEX_MethodSemantics,
	INDEX_Module,
	INDEX_ModuleRef,
	INDEX_NestedClass,
	INDEX_Param,
	INDEX_Property,
	INDEX_PropertyMap,
	INDEX_ResolutionScope,
	INDEX_StandAloneSig,
	INDEX_TypeDef,
	INDEX_TypeDefOrRef,
	INDEX_TypeRef,
	INDEX_TypeSpec,
	//----------------------------
	INDEX_LAST,
};


namespace Metadata {

	class Index {
		static int getBitSize(int count) {
			if(count>128) {
				return 8;
			} else if(count>64) {
				return 7;
			} else if(count>32) {
				return 6;
			} else if(count>16) {
				return 5;
			} else if(count>8) {
				return 4;
			} else if(count>4) {
				return 3;
			} else if(count>2) {
				return 2;
			} else {
				return 1;
			}
		}
	protected:
		static uint getSize(TableId tables[], int count, const uint rowcounts[]) {
			uint max = 0;
			for(int i=0; i<count; ++i) {
				if(max<rowcounts[tables[i]]) {
					max = rowcounts[tables[i]];
				}
			}
			if(0==(max & (0xFFFFFFFF << (16-getBitSize(count))))) {
				return 2;
			} else {
				return 4;
			}
		}
	protected:
		Index() {
		}
		void parse(uint value, TableId tables[], int count) {
			int bits = getBitSize(count);
			table = tables[value & ((1<<bits)-1)];
			index = value >> bits;
		}
	public:
		TableId table;
		uint index;
	};

	struct MetadataToken : Index {
		MetadataToken(uint32 value) {
			table = (TableId)(value >> 24);
			index = value & 0xFFFFFF;
		}
	};

}

static Console& operator <<(Console& con, const Metadata::Index& idx) {
	con << (byte)idx.table << ":" << (uint)idx.index;
	return con;
}
