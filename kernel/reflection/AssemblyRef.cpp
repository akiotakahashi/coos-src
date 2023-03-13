#include "AssemblyRef.h"
#include "../assemblymanager.h"


namespace Reflection {

	AssemblyRef::AssemblyRef(Assembly& parent, uint tableIndex, const Metadata::AssemblyRef& _assemblyref) : assemblyref(_assemblyref), AssemblyItem(parent,tableIndex) {
		const Metadata::MainStream& ms = getRoot().getMainStream();
		const Metadata::StringStream& ss = getRoot().getStringStream();
		name = ss.getString(assemblyref.Name);
	}

	MemoryRegion AssemblyRef::getPublicKeyOrToken() const {
		return getRoot().getBlobStream().getData(assemblyref.PublicKeyOrToken);
	}

	Assembly* AssemblyRef::Resolve() const {
		Assembly* assem = AssemblyManager::FindAssembly(name);
		if(assem==NULL) {
			panic(L"AssemblyRef::Resolve failed to resolve "+getName());
		} else {
			return assem;
		}
	}

}
