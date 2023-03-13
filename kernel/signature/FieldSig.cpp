#include "FieldSig.h"


namespace Signature {

	FieldSig::FieldSig() {
	}

	FieldSig::~FieldSig() {
	}
	
	FieldSig::FieldSig(const FieldSig& value) : CustomMods(value.CustomMods), Type(value.Type) {
	}

	bool FieldSig::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		if(ss.readByte()!=0x6/*FIELD*/) {
			return false;
		}
		while(true) {
			CustomMod cm;
			if(!cm.Parse(ss)) {
				break;
			}
			CustomMods.push_back(cm);
		}
		if(!Type.Parse(ss)) {
			return false;
		} else {
			ss.commit();
			return true;
		}
	}

}
