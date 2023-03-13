#pragma once

#include "stdafx.h"
#include "assembly.h"


namespace Reflection {

	class MemberRef : public AssemblyItem, public Metadata::MemberRef {
		std::wstring name;
		const byte* signature;
		mutable Signature::FieldSig* fieldSig;
		mutable Signature::MethodSig* methodSig;
		mutable Field* field;
		mutable Method* method;
	public:
		MemberRef(Assembly& parent, uint tableIndex, const Metadata::MemberRef& memref);
		~MemberRef();
	public:
		const std::wstring& getName() const { return name; }
		const Signature::FieldSig& getFieldSig() const;
		const Signature::MethodSig& getMethodSig() const;
	public:
		Field* ResolveField() const;
		Method* ResolveMethod() const;
	};

}
