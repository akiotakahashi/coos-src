#pragma once

#include "stdafx.h"


namespace Signature {

	struct Constraint {
		bool Pinned;
	public:
		Constraint();
		~Constraint();
		bool Parse(SignatureStream& ss);
	};

}
