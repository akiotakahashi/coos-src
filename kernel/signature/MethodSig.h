#pragma once

#include "stdafx.h"
#include "RetType.h"
#include "ParamSig.h"
#include "../LayoutInfo.h"


namespace Signature {

	struct MethodSig {
		RetType RetType;
		ParamSigList Params;
		CallingConventionFlags Flags;
	public:
		MethodSig();
		virtual ~MethodSig();
		virtual bool Parse(SignatureStream& _ss) = 0;
		bool Match(const MethodSig& value) const;
		void Link(const Reflection::Assembly& assembly);
	public:
		CallingConventionFlags getCallingConv() const
		{ return (CallingConventionFlags)(Flags&0xF); }
		bool HasThis() const { return 0!=(Flags&HASTHIS); }
		bool Default() const { return 0==(Flags&0xF); }
	public:
		const LayoutInfo BuildArgLayout(const Reflection::Assembly& assembly) const;
	};

}
