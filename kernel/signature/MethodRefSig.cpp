#include "MethodRefSig.h"
#include "ParamSig.h"


namespace Signature {

	MethodRefSig::~MethodRefSig() {
	}

	bool MethodRefSig::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		Flags = (CallingConventionFlags)ss.readByte();
		if(Flags & HASTHIS) {
			if(Flags & EXPLICITTHIS) {
			}
		}
		if(Flags & VARARG) {
		} else if(Flags & C_LANG) {
		} else if(Flags & STDCALL) {
		} else if(Flags & THISCALL) {
		} else if(Flags & FASTCALL) {
		} else {
			// DEFAULT
		}
		int paramCount = ss.readInt();
		if(!RetType.Parse(ss)) {
			return false;
		} else {
			std::vector<ParamSig>* params = &Params;
			for(int i=0; i<paramCount; ++i) {
				if(ss.peekByte()==SENTINEL) {
					ss.readByte();
					params = &ExtraParams;
				} else {
					ParamSig param;
					if(!param.Parse(ss)) {
						return false;
					}
					params->push_back(param);
				}
			}
			ss.commit();
			return true;
		}
	}

	bool MethodRefSig::Match(const MethodRefSig& value) const {
		return MethodSig::Match(value)
			&& ExtraParams.Match(value.ExtraParams);
	}

}
