#pragma once

#include "stdafx.h"
#include "../metadata.h"


namespace Signature {

	class TypeDefOrRefEncoded : public Metadata::TypeDefOrRefIndex {
		const Reflection::TypeDef* typdef;
	public:
		TypeDefOrRefEncoded();
		~TypeDefOrRefEncoded();
		bool Parse(SignatureStream& _ss);
		bool Match(const TypeDefOrRefEncoded& value) const;
		void Link(const Reflection::Assembly& assembly);
	};

}
