#include "TypeDefOrRefEncoded.h"
#include "../reflection.h"


namespace Signature {

	TypeDefOrRefEncoded::TypeDefOrRefEncoded() {
		typdef = NULL;
	}

	TypeDefOrRefEncoded::~TypeDefOrRefEncoded() {
	}

	bool TypeDefOrRefEncoded::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		*((Metadata::TypeDefOrRefIndex*)this) = Metadata::TypeDefOrRefIndex(ss.readInt());
		switch(table) {
		case TABLE_TypeRef:
		case TABLE_TypeDef:
		case TABLE_TypeSpec:
			ss.commit();
			return true;
		default:
			return false;
		}
	}

	bool TypeDefOrRefEncoded::Match(const TypeDefOrRefEncoded& value) const {
		if(typdef==NULL || value.typdef==NULL) {
			panic("TypeDefOrRefEncoded::Match detects NullReference");
		} else if(table==TABLE_TypeSpec || value.table==TABLE_TypeSpec) {
			panic("TypeDefOrRefEncoded::Match detects TABLE_TypeSpec");
		} else {
			return typdef==value.typdef;
		}
	}

	void TypeDefOrRefEncoded::Link(const Reflection::Assembly& assembly) {
		if(typdef==NULL) {
			switch(table) {
			case TABLE_TypeDef:
				typdef = &assembly.getTypeDef(index);
				break;
			case TABLE_TypeRef:
				typdef = assembly.ResolveTypeDef(*this);
				break;
			case TABLE_TypeSpec:
				panic("TypeDefOrRefEncoded::Link detects TABLE_TypeSpec");
				//typdef = NULL;
				break;
			default:
				panic("TypeDefOrRefEncoded::Link");
			}
		}
	}

}
