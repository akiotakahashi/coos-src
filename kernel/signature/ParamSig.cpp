#include "ParamSig.h"


namespace Signature {

	ParamSig::ParamSig() {
		TypedByRef = false;
		ByRef = false;
	}
	
	ParamSig::ParamSig(const ParamSig& p) : CustomMods(p.CustomMods), Type(p.Type) {
		TypedByRef = p.TypedByRef;
		ByRef = p.ByRef;
	}

	bool ParamSig::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		while(true) {
			CustomMod cmod;
			if(cmod.Parse(ss)) {
				CustomMods.push_back(cmod);
			} else {
				break;
			}
		}
		switch(ss.peekByte()) {
		case ELEMENT_TYPE_TYPEDBYREF:
			ss.readByte();
			TypedByRef = true;
			ss.commit();
			return true;
		case ELEMENT_TYPE_BYREF:
			ss.readByte();
			ByRef = true;
			// fall down
		default:
			if(Type.Parse(ss)) {
				ss.commit();
				return true;
			} else {
				return false;
			}
		}
	}

	bool ParamSig::Match(const ParamSig& value) const {
		return TypedByRef==value.TypedByRef
			&& ByRef==value.ByRef
			&& Type.Match(value.Type);
	}

	void ParamSig::Link(const Reflection::Assembly& assembly) {
		Type.Link(assembly);
	}

	bool ParamSigList::Match(const ParamSigList& list) const {
		if(size()!=list.size()) return false;
		for(uint i=0; i<size(); ++i) {
			const ParamSig& param1 = (*this)[i];
			const ParamSig& param2 = list[i];
			if(!param1.Match(param2)) {
				return false;
			}
		}
		return true;
	}

	void ParamSigList::Link(const Reflection::Assembly& assembly) {
		if(linked) return;
		for(uint i=0; i<size(); ++i) {
			(*this)[i].Link(assembly);
		}
		linked = true;
	}

}
