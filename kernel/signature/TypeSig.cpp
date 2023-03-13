#include "TypeSig.h"
#include "TypeDefOrRefEncoded.h"
#include "MethodDefSig.h"
#include "MethodRefSig.h"
#include "ArrayShape.h"


namespace Signature {

	TypeSig::TypeSig() {
		TypeDefOrRef = NULL;
		MethodDefSig = NULL;
		MethodRefSig = NULL;
		Type = NULL;
		ArrayShape = NULL;
		Void = false;
	}

	TypeSig::~TypeSig() {
		delete TypeDefOrRef;
		delete MethodDefSig;
		delete MethodRefSig;
		delete Type;
		delete ArrayShape;
	}

	TypeSig::TypeSig(const TypeSig& t) : CustomMods(t.CustomMods) {
		ElementType = t.ElementType;
		Void = t.Void;
		TypeDefOrRef = t.TypeDefOrRef ? new TypeDefOrRefEncoded(*t.TypeDefOrRef) : NULL;
		MethodDefSig = t.MethodDefSig ? new Signature::MethodDefSig(*t.MethodDefSig) : NULL;
		MethodRefSig = t.MethodRefSig ? new Signature::MethodRefSig(*t.MethodRefSig) : NULL;
		Type = t.Type ? new TypeSig(*t.Type) : NULL;
		ArrayShape = t.ArrayShape ? new Signature::ArrayShape(*t.ArrayShape) : NULL;
	}

	bool TypeSig::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		ElementType = (ELEMENT_TYPE)ss.readByte();
		switch(ElementType) {
		default:
			return false;
		case ELEMENT_TYPE_BOOLEAN:
		case ELEMENT_TYPE_CHAR:
		case ELEMENT_TYPE_I1:
		case ELEMENT_TYPE_U1:
		case ELEMENT_TYPE_I2:
		case ELEMENT_TYPE_U2:
		case ELEMENT_TYPE_I4:
		case ELEMENT_TYPE_U4:
		case ELEMENT_TYPE_I8:
		case ELEMENT_TYPE_U8:
		case ELEMENT_TYPE_R4:
		case ELEMENT_TYPE_R8:
		case ELEMENT_TYPE_I:
		case ELEMENT_TYPE_U:
		case ELEMENT_TYPE_STRING:
		case ELEMENT_TYPE_OBJECT:
			ss.commit();
			return true;
		case ELEMENT_TYPE_VALUETYPE:
		case ELEMENT_TYPE_CLASS:
			//| VALUETYPE TypeDefOrRefEncoded
			//| CLASS TypeDefOrRefEncoded
			TypeDefOrRef = new TypeDefOrRefEncoded();
			if(!TypeDefOrRef->Parse(ss)) {
				return false;
			} else {
				ss.commit();
				return true;
			}
		case ELEMENT_TYPE_BYREF:	// Is this correct?
		case ELEMENT_TYPE_PTR:
			//| PTR CustomMod* VOID
			//| PTR CustomMod* TypeSig
			while(true) {
				CustomMod cm;
				if(!cm.Parse(ss)) {
					break;
				}
				CustomMods.push_back(cm);
			}
			if(ss.peekByte()==ELEMENT_TYPE_VOID) {
				Void = true;
				ss.readByte();
				ss.commit();
				return true;
			} else {
				Void = false;
				Type = new TypeSig();
				if(Type->Parse(ss)) {
					ss.commit();
					return true;
				} else {
					return false;
				}
			}
		case ELEMENT_TYPE_FNPTR:
			//| FNPTR MethodDefSig
			//| FNPTR MethodRefSig
			MethodDefSig = new Signature::MethodDefSig();
			MethodRefSig = new Signature::MethodRefSig();
			if(MethodDefSig->Parse(ss)) {
				delete MethodRefSig;
				MethodRefSig = NULL;
				ss.commit();
				return true;
			} else if(MethodRefSig->Parse(ss)) {
				delete MethodDefSig;
				MethodDefSig = NULL;
				ss.commit();
				return true;
			} else {
				delete MethodRefSig;
				delete MethodDefSig;
				MethodRefSig = NULL;
				MethodDefSig = NULL;
				return false;
			}
		case ELEMENT_TYPE_ARRAY:
			//| ARRAY TypeSig ArrayShape  (general array, see clause 22.2.13)
			panic(L"Not Implemented: ELEMENT_TYPE_ARRAY");
		case ELEMENT_TYPE_SZARRAY:
			//| SZARRAY CustomMod* TypeSig (single dimensional, zero-based array i.e. vector)
			while(true) {
				CustomMod cm;
				if(!cm.Parse(ss)) {
					break;
				}
				CustomMods.push_back(cm);
			}
			Type = new TypeSig();
			if(Type->Parse(ss)) {
				ss.commit();
				return true;
			} else {
				return false;
			}
		case ELEMENT_TYPE_VOID:
		case ELEMENT_TYPE_PINNED:
		case ELEMENT_TYPE_TYPEDBYREF:
		case ELEMENT_TYPE_CMOD_REQD:
		case ELEMENT_TYPE_CMOD_OPT:
		case ELEMENT_TYPE_INTERNAL:
		case ELEMENT_TYPE_MODIFIER:
		case ELEMENT_TYPE_SENTINEL:
			panic("TypeSig::Parse detects unsupported beginning");
		}
	}

	std::wstring TypeSig::ToString() const {
		switch(ElementType) {
		default:
			panic("Signature::TypeSig::ToString");
		case ELEMENT_TYPE_BOOLEAN:
			return L"bool";
		case ELEMENT_TYPE_CHAR:
			return L"char";
		case ELEMENT_TYPE_I1:
			return L"sbyte";
		case ELEMENT_TYPE_U1:
			return L"byte";
		case ELEMENT_TYPE_I2:
			return L"short";
		case ELEMENT_TYPE_U2:
			return L"ushort";
		case ELEMENT_TYPE_I4:
			return L"int";
		case ELEMENT_TYPE_U4:
			return L"uint";
		case ELEMENT_TYPE_I8:
			return L"long";
		case ELEMENT_TYPE_U8:
			return L"ulong";
		case ELEMENT_TYPE_R4:
			return L"float";
		case ELEMENT_TYPE_R8:
			return L"double";
		case ELEMENT_TYPE_I:
			return L"nint";
		case ELEMENT_TYPE_U:
			return L"nuint";
		case ELEMENT_TYPE_VALUETYPE:
		case ELEMENT_TYPE_CLASS:
			{
				std::wstring retval;
				switch(ElementType) {
				case ELEMENT_TYPE_VALUETYPE:
					retval = L"[ValueType]";
					break;
				case ELEMENT_TYPE_CLASS:
					retval = L"[Class]";
					break;
				}
				return retval+L" 0x"+itos<wchar_t,16>((int)TypeDefOrRef->table,2,'0')+L":"+itos<wchar_t,10>((int)TypeDefOrRef->index);
			}
		case ELEMENT_TYPE_STRING:
			//| STRING
			return L"[String]";
		case ELEMENT_TYPE_OBJECT:
			//| OBJECT
			return L"[Object]";
		case ELEMENT_TYPE_PTR:
			//| PTR CustomMod* VOID
			//| PTR CustomMod* TypeSig
			return L"[PTR]";
		case ELEMENT_TYPE_FNPTR:
			//| FNPTR MethodDefSig
			//| FNPTR MethodRefSig
			return L"[FNPTR]";
		case ELEMENT_TYPE_ARRAY:
			//| ARRAY TypeSig ArrayShape  (general array, see clause 22.2.13)
			return L"[ARRAY]";
		case ELEMENT_TYPE_SZARRAY:
			//| SZARRAY CustomMod* TypeSig (single dimensional, zero-based array i.e. vector)
			return L"[SZARRAY]";
		}
	}

	bool TypeSig::Match(const TypeSig& type) const {
		if(ElementType!=type.ElementType) return false;
		switch(ElementType) {
		default:
			panic("TypeSig::Equals detects illegal ElementType "+itos<char,16>((uint)ElementType));
		case ELEMENT_TYPE_BOOLEAN:
		case ELEMENT_TYPE_CHAR:
		case ELEMENT_TYPE_I1:
		case ELEMENT_TYPE_U1:
		case ELEMENT_TYPE_I2:
		case ELEMENT_TYPE_U2:
		case ELEMENT_TYPE_I4:
		case ELEMENT_TYPE_U4:
		case ELEMENT_TYPE_I8:
		case ELEMENT_TYPE_U8:
		case ELEMENT_TYPE_R4:
		case ELEMENT_TYPE_R8:
		case ELEMENT_TYPE_I:
		case ELEMENT_TYPE_U:
		case ELEMENT_TYPE_STRING:
		case ELEMENT_TYPE_OBJECT:
			return true;
		case ELEMENT_TYPE_VALUETYPE:
		case ELEMENT_TYPE_CLASS:
			//| VALUETYPE TypeDefOrRefEncoded
			//| CLASS TypeDefOrRefEncoded
			return TypeDefOrRef->Match(*type.TypeDefOrRef);
		case ELEMENT_TYPE_PTR:
			//| PTR CustomMod* VOID
			//| PTR CustomMod* TypeSig
			if(Void!=type.Void) {
				return false;
			}
			if(Type && type.Type) {
				return Type->Match(*type.Type);
			} else if(Void && type.Void) {
				return true;
			} else {
				return false;
			}
		case ELEMENT_TYPE_FNPTR:
			//| FNPTR MethodDefSig
			//| FNPTR MethodRefSig
			if(MethodDefSig && type.MethodDefSig) {
				return MethodDefSig->Match(*type.MethodDefSig);
			} else if(MethodRefSig && type.MethodRefSig) {
				return MethodRefSig->Match(*type.MethodRefSig);
			} else {
				return false;
			}
		case ELEMENT_TYPE_ARRAY:
			//| ARRAY TypeSig ArrayShape  (general array, see clause 22.2.13)
			panic(L"TypeSig::Equals not implemented: ELEMENT_TYPE_ARRAY");
		case ELEMENT_TYPE_SZARRAY:
			//| SZARRAY CustomMod* TypeSig (single dimensional, zero-based array i.e. vector)
			return Type->Match(*type.Type);
		}
	}

	void TypeSig::Link(const Reflection::Assembly& assembly) {
		if(TypeDefOrRef!=NULL) TypeDefOrRef->Link(assembly);
		if(Type!=NULL) Type->Link(assembly);
		if(MethodDefSig!=NULL) MethodDefSig->Link(assembly);
		if(MethodRefSig!=NULL) MethodRefSig->Link(assembly);
	}

}
