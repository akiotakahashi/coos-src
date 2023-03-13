#pragma once

#include "stdafx.h"
#include "TypeDefOrRefEncoded.h"


namespace Signature {

	struct CustomMod {
		byte cmod;
		TypeDefOrRefEncoded codedIndex;
		bool Parse(SignatureStream& ss) {
			switch(ss.peekByte()) {
			case ELEMENT_TYPE_CMOD_OPT:
			case ELEMENT_TYPE_CMOD_REQD:
				break;
			default:
				return false;
			}
			cmod = ss.readByte();
			return codedIndex.Parse(ss);
		}
	};

}
