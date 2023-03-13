#pragma once


enum TableId {
	TABLE_Module = 0x00,
	TABLE_TypeRef = 0x01,
	TABLE_TypeDef = 0x02,
	TABLE_Field = 0x04,
	TABLE_Method = 0x06,
	TABLE_Param = 0x08,
	TABLE_InterfaceImpl = 0x09,
	TABLE_MemberRef = 0x0A,
	TABLE_MethodRef = 0x0A,
	TABLE_FieldRef = 0x0A,
	TABLE_Constant = 0x0B,
	TABLE_CustomAttribute = 0x0C,
	TABLE_FieldMarshal = 0x0D,
	TABLE_DeclSecurity = 0x0E,
	TABLE_ClassLayout = 0x0F,
	TABLE_FieldLayout = 0x10,
	TABLE_StandAloneSig = 0x11,
	TABLE_EventMap = 0x12,
	TABLE_Event = 0x14,
	TABLE_PropertyMap = 0x15,
	TABLE_Property = 0x17,
	TABLE_MethodSemantics = 0x18,
	TABLE_MethodImpl = 0x19,
	TABLE_ModuleRef = 0x1A,
	TABLE_TypeSpec = 0x1B,
	TABLE_ImplMap = 0x1C,
	TABLE_FieldRVA = 0x1D,
	TABLE_Assembly = 0x20,
	TABLE_AssemblyProcessor = 0x21,
	TABLE_AssemblyOS = 0x22,
	TABLE_AssemblyRef = 0x23,
	TABLE_AssemblyRefProcessor = 0x24,
	TABLE_AssemblyRefOS = 0x25,
	TABLE_File = 0x26,
	TABLE_ExportedType = 0x27,
	TABLE_ManifestResource = 0x28,
	TABLE_NestedClass = 0x29,
	//
	TABLE_NOT_USED,
	TABLE_LAST = 64,
};

namespace Metadata {

	struct __declspec(novtable) Table {
	};

}
