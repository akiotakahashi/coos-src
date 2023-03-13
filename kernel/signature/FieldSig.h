#pragma once

#include "stdafx.h"
#include "TypeSig.h"
#include "CustomMod.h"


namespace Signature {

	struct FieldSig {
		std::vector<CustomMod> CustomMods;
		TypeSig Type;
	public:
		FieldSig();
		~FieldSig();
		FieldSig(const FieldSig& value);
		bool Parse(SignatureStream& ss);
	};

}
