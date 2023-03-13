#include "Constraint.h"


namespace Signature {

	Constraint::Constraint() {
		Pinned = false;
	}

	Constraint::~Constraint() {
	}

	bool Constraint::Parse(SignatureStream& ss) {
		if(ss.peekByte()==ELEMENT_TYPE_PINNED) {
			Pinned = true;
			ss.readByte();
			return true;
		} else {
			return false;
		}
	}

}
