#pragma once

#include "stdafx.h"
#include "MethodDefSig.h"


namespace Signature {

	struct MethodRefSig : MethodSig {
		ParamSigList ExtraParams;
	public:
		~MethodRefSig();
		bool Parse(SignatureStream& _ss);
		bool Match(const MethodRefSig& value) const;
	};

}
