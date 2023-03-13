#pragma once

#include "stdafx.h"
#include "MethodSig.h"


namespace Signature {

	struct MethodDefSig : MethodSig {
		MethodDefSig();
		~MethodDefSig();
		bool Parse(SignatureStream& _ss);
	};

}
