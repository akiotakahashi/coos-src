#include "index2.h"


namespace Metadata {

TableId CustomAttributeTypeIndex::tables[] = {
TABLE_NOT_USED,
TABLE_NOT_USED,
TABLE_Method,
TABLE_MethodRef,
TABLE_NOT_USED,
};

TableId HasConstIndex::tables[] = {
TABLE_Field,
TABLE_Param,
TABLE_Property,
};

TableId HasCustomAttributeIndex::tables[] = {
TABLE_Method,
TABLE_Field,
TABLE_TypeRef,
TABLE_TypeDef,
TABLE_Param,
TABLE_InterfaceImpl,
TABLE_MemberRef,
TABLE_Module,
//TABLE_Permission,		// * UNDEFINED *
TABLE_DeclSecurity,		// Use instead of Permission table
TABLE_Property,
TABLE_Event,
//TABLE_Signature,		// * UNDEFINED *
TABLE_StandAloneSig,	// Use instead of Signature table
TABLE_ModuleRef,
TABLE_TypeSpec,
TABLE_Assembly,
TABLE_AssemblyRef,
TABLE_File,
TABLE_ExportedType,
TABLE_ManifestResource,
};

TableId HasDeclSecurityIndex::tables[] = {
TABLE_TypeDef,
TABLE_Method,
TABLE_Assembly,
};

TableId HasFieldMarshalIndex::tables[] = {
TABLE_Field,
TABLE_Param,
};

TableId HasSemanticsIndex::tables[] = {
TABLE_Event,
TABLE_Property,
};

TableId ImplementationIndex::tables[] = {
TABLE_File,
TABLE_AssemblyRef,
TABLE_ExportedType,
};

TableId MemberForwardedIndex::tables[] = {
TABLE_Field,
TABLE_Method,
};

TableId MemberRefParentIndex::tables[] = {
TABLE_NOT_USED,
TABLE_TypeRef,
TABLE_ModuleRef,
TABLE_Method,
TABLE_TypeSpec,
};

TableId MethodDefOrRefIndex::tables[] = {
TABLE_Method,
TABLE_MemberRef,
};

TableId ResolutionScopeIndex::tables[] = {
TABLE_Module,
TABLE_ModuleRef,
TABLE_AssemblyRef,
TABLE_TypeRef,
};

TableId TypeDefOrRefIndex::tables[] = {
TABLE_TypeDef,
TABLE_TypeRef,
TABLE_TypeSpec,
};

}
