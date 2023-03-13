#include "MethodSig.h"
#include "../ilengine.h"
#include "../reflection.h"


namespace Signature {

	MethodSig::MethodSig() {
	}

	MethodSig::~MethodSig() {
	}

	bool MethodSig::Match(const MethodSig& value) const {
		return RetType.Match(value.RetType)
			&& Params.Match(value.Params)
			&& Flags==Flags;
	}

	void MethodSig::Link(const Reflection::Assembly& assembly) {
		RetType.Link(assembly);
		Params.Link(assembly);
	}

	const LayoutInfo MethodSig::BuildArgLayout(const Reflection::Assembly& assembly) const {
		int ac = Params.size();
		if(HasThis()) ++ac;
		LayoutInfo ai(ac);
		int offset = 0;
		for(int i=Params.size()-1; i>=0; --i) {
			const ParamSig& param = Params[i];
			uint size = assembly.CalcSizeOfType(param);
			if(size==0) panic("arg size is zero");
			ai.sizes.push_front(size);
			ai.offsets.push_front(offset);
			offset += IL::ILStack::getSizeOnStack(size);
		}
		if(HasThis()) {
			ai.sizes.push_front(sizeof(void*));
			ai.offsets.push_front(offset);
			offset += sizeof(void*);
		}
		ai.totalsize = offset;
		return ai;
	}

}
