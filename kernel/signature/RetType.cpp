#include "RetType.h"


namespace Signature {

	RetType::RetType() {
		ByRef = false;
		Type = NULL;
		Void = false;
		TypedByRef = false;
	}

	RetType::~RetType() {
		delete Type;
	}
	
	RetType::RetType(const RetType& value) {
		ByRef = false;
		Void = false;
		TypedByRef = false;
		Type = value.Type ? new TypeSig(*value.Type) : NULL;
	}

	bool RetType::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		while(true) {
			CustomMod cm;
			if(!cm.Parse(ss)) {
				break;
			}
			CustomMods.push_back(cm);
		}
		switch(ss.peekByte()) {
		case ELEMENT_TYPE_BYREF:
			ss.readByte();
			ByRef = true;
			// fall down
		default:
			if(Type!=NULL) panic("RetType Non-NULL");
			Type = new TypeSig();
			if(!Type->Parse(ss)) {
				return false;
			} else {
				ss.commit();
				return true;
			}
		case ELEMENT_TYPE_TYPEDBYREF:
			ss.readByte();
			TypedByRef = true;
			ss.commit();
			return true;
		case ELEMENT_TYPE_VOID:
			ss.readByte();
			Void = true;
			ss.commit();
			return true;
		}
	}

	std::wstring RetType::ToString() const {
		if(Void) {
			return L"void";
		} else if(TypedByRef) {
			return L"[TypedByRef]";
		} else {
			std::wstring ret;
			if(ByRef) {
				ret += L"[ByRef] ";
			}
			ret += Type->ToString();
			return ret;
		}
	}

	bool RetType::Match(const RetType& value) const {
		if(Void && value.Void) return true;
		if(TypedByRef && value.TypedByRef) return true;
		if(Type && value.Type) {
			if(ByRef!=value.ByRef) return false;
			return Type->Match(*value.Type);
		} else {
            return false;
		}
	}

	void RetType::Link(const Reflection::Assembly& assembly) {
		if(Type!=NULL) Type->Link(assembly);
	}

}
