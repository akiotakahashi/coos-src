#pragma once

#include "stdafx.h"


namespace Reflection {

	class Param {
		const char* name;
		enum TreatmentFlags {
			None,
			In,
			Out,
			Optional,
		} fTreatment;
	public:
		Param(const Metadata::Param& p, const Metadata::StringStream& ss) {
			name = ss+p.Name;
			fTreatment = None;
		}
	public:
		const char* getName() const { return name; }
	};

}
