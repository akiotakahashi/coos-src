
#include "TypeRef.h"
#include "TypeDef.h"
#include "Assembly.h"
#include "AssemblyRef.h"


namespace Reflection {

	TypeRef::TypeRef(Assembly& parent, uint tableIndex, const Metadata::TypeRef& typeref) : Metadata::TypeRef(typeref), AssemblyItem(parent,tableIndex) {
		const Metadata::StringStream& ss = getRoot().getStringStream();
		name = ss.getString(typeref.Name);
		ns = ss.getString(typeref.Namespace);
	}
	
	TypeDef* TypeRef::Resolve() {
		Metadata::Table* prow = getRoot().getMainStream().getRow(ResolutionScope);
		switch(ResolutionScope.table) {
		case TABLE_Module:
		case TABLE_ModuleRef:
		case TABLE_TypeRef:
		default:
			panic("TypeRef::Resolve");
		case TABLE_AssemblyRef:
			{
				AssemblyRef assemref(getAssembly(), ResolutionScope.index, *(const Metadata::AssemblyRef*)prow);
				Assembly* assem = assemref.Resolve();
				delete prow;
				if(assem==NULL) {
					panic(std::wstring(L"AssemblyRef:")+assemref.getName()+L" can't be resolved");
				} else {
					return assem->FindTypeDef(name,ns);
				}
			}
		}
	}

}
