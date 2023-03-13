#include "signature.h"


namespace Signature {

	Signature::Signature(Reflection::Assembly& parent, uint tableIndex) : AssemblyItem(parent,tableIndex) {
		const Metadata::BlobStream& blob = getRoot().getBlobStream();
	}

}
