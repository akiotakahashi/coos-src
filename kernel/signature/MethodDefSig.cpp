#include "MethodDefSig.h"


namespace Signature {

	MethodDefSig::MethodDefSig() {
	}

	MethodDefSig::~MethodDefSig() {
	}

	bool MethodDefSig::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		Flags = (CallingConventionFlags)ss.readByte();
		if(Flags & ~(HASTHIS|EXPLICITTHIS|VARARG)) {
			return false;
		}
		if(Flags & HASTHIS) {
			if(Flags & EXPLICITTHIS) {
			}
		}
		if(Flags & VARARG) {
		} else {
			// DEFAULT
		}
		int paramCount = ss.readInt();
		if(!RetType.Parse(ss)) {
			return false;
		} else {
			for(int i=0; i<paramCount; ++i) {
				ParamSig param;
				if(!param.Parse(ss)) {
					return false;
				}
				Params.push_back(param);
			}
			ss.commit();
			return true;
		}
	}

}
