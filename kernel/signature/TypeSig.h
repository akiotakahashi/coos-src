#pragma once

#include "stdafx.h"
#include "CustomMod.h"
#include "TypeDefOrRefEncoded.h"


namespace Signature {

	struct MethodDefSig;
	struct MethodRefSig;
	struct CustomMod;
	struct ArrayShape;

	struct TypeSig {
		ELEMENT_TYPE ElementType;
		TypeDefOrRefEncoded* TypeDefOrRef;
		MethodDefSig* MethodDefSig;
		MethodRefSig* MethodRefSig;
		bool Void;
		TypeSig* Type;
		ArrayShape* ArrayShape;
		std::vector<CustomMod> CustomMods;
	public:
		TypeSig();
		~TypeSig();
		TypeSig(const TypeSig& t);
		bool Parse(SignatureStream& ss);
		std::wstring ToString() const;
		bool Match(const TypeSig& type) const;
		void Link(const Reflection::Assembly& assembly);
	};

}
