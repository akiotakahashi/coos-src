#pragma once

#include "stdafx.h"


namespace Signature {

	struct ArrayShape {
		int rank;
		std::vector<int> sizes;
		std::vector<int> lbounds;
	public:
		bool parse(SignatureStream& _ss);
	};

}
