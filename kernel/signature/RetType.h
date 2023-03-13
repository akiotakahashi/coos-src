#pragma once

#include "stdafx.h"
#include "TypeSig.h"
#include "CustomMod.h"


namespace Signature {

	struct RetType {
		std::vector<CustomMod> CustomMods;
		bool ByRef;
		TypeSig* Type;
		bool TypedByRef;
		bool Void;
	public:
		RetType();
		~RetType();
		RetType(const RetType& value);
		bool Parse(SignatureStream& ss);
		std::wstring ToString() const;
		bool Match(const RetType& value) const;
		void Link(const Reflection::Assembly& assembly);
	};

}
