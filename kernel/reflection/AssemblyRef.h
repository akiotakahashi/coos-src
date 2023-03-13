#pragma once

#include "stdafx.h"
#include "assembly.h"


namespace Reflection {

	class AssemblyRef : public AssemblyItem {
		Metadata::AssemblyRef assemblyref;
		std::wstring name;
	public:
		AssemblyRef(Assembly& parent, uint tableIndex, const Metadata::AssemblyRef& assemblyref);
	public:
		const std::wstring& getName() const { return name; }
		MemoryRegion getPublicKeyOrToken() const;
	public:
		Assembly* Resolve() const;
	};

}
