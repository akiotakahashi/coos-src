#pragma once

#include "stdafx.h"
#include "Assembly.h"


namespace Reflection {

	class TypeRef : public Metadata::TypeRef, public AssemblyItem {
		std::wstring name;
		std::wstring ns;
	public:
		TypeRef(Assembly& parent, uint tableIndex, const Metadata::TypeRef& typeref);
	public:
		const std::wstring& getName() const { return name; }
		const std::wstring& getNamespace() const { return ns; }
	public:
		TypeDef* Resolve();
		const TypeDef* Resolve() const
		{ return const_cast<TypeRef*>(this)->Resolve(); }
	};

}
