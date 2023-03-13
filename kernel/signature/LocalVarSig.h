#pragma once

#include "stdafx.h"
#include "Constraint.h"
#include "TypeSig.h"
#include "../LayoutInfo.h"


namespace Signature {

	struct LocalVar {
		std::vector<Constraint> Constraints;
		std::vector<CustomMod> CustomMods;
		bool ByRef;
		TypeSig Type;
		bool TypedByRef;
		uint Modifier;
	public:
		LocalVar();
		bool Parse(SignatureStream& ss);
	};

	struct LocalVarSig {
		std::vector<LocalVar> LocalVars;
	public:
		LocalVarSig();
		~LocalVarSig();
		bool Parse(SignatureStream& ss);
	public:
		const LayoutInfo BuildVarLayout(const Reflection::Assembly& assembly) const;
	};

}
