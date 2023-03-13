#pragma once

#include "stdafx.h"
#include "TypeSig.h"
#include "CustomMod.h"


namespace Signature {

	struct ParamSig {
		std::vector<CustomMod> CustomMods;
		bool TypedByRef;
		bool ByRef;
		TypeSig Type;
	public:
		ParamSig();
		ParamSig(const ParamSig& p);
		bool Parse(SignatureStream& _ss);
		bool Match(const ParamSig& value) const;
		void Link(const Reflection::Assembly& assembly);
	};

	class ParamSigList : public std::vector<ParamSig> {
		bool linked;
	public:
		ParamSigList() {
			linked = false;
		}
		bool IsLinked() const { return linked; }
		bool Match(const ParamSigList& list) const;
		void Link(const Reflection::Assembly& assembly);
	};

}
