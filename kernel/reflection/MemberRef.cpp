#include "MemberRef.h"
#include "TypeRef.h"
#include "TypeDef.h"


namespace Reflection {

	MemberRef::MemberRef(Assembly& parent, uint tableIndex, const Metadata::MemberRef& mr) : Metadata::MemberRef(mr), AssemblyItem(parent,tableIndex) {
		name = getRoot().getStringStream().getString(Name);
		signature = getRoot().getBlobStream()+Signature;
		fieldSig = NULL;
		methodSig = NULL;
		field = NULL;
		method = NULL;
	}

	MemberRef::~MemberRef() {
		delete fieldSig;
		delete methodSig;
	}
	
	const Signature::FieldSig& MemberRef::getFieldSig() const {
		if(fieldSig==NULL) {
			fieldSig = new Signature::FieldSig();
			if(!fieldSig->Parse(Signature::SignatureStream(signature))) {
				panic("MemberRef::getFieldSig can't parse signature.");
			}
		}
		return *fieldSig;
	}
    
	const Signature::MethodSig& MemberRef::getMethodSig() const {
		if(methodSig==NULL) {
			switch(Class.table) {
			case TABLE_TypeRef:
			case TABLE_ModuleRef:
				methodSig = new Signature::MethodRefSig();
				break;
			case TABLE_Method:
			case TABLE_TypeDef:
			case TABLE_TypeSpec:
				methodSig = new Signature::MethodDefSig();
				break;
			default:
				panic("MemberRef::getMethodSig");
			}
			if(!methodSig->Parse(Signature::SignatureStream(signature))) {
				panic("MemberRef::getMethodSig can't parse signature "+itos<char,16>(Signature));
			}
		}
		return *methodSig;
	}

	Field* MemberRef::ResolveField() const {
		if(field!=NULL) return field;
		const Metadata::StringStream& ss = getRoot().getStringStream();
		switch(Class.table) {
		case TABLE_TypeRef:
			{
				TypeRef& typeref = getAssembly().getTypeRef(Class.index);
				const TypeDef* typdef = typeref.Resolve();
				if(typdef==NULL) {
					panic("Assembly.ResolveTypeRef Failed");
				} else {
					return field = typdef->ResolveField(name,true);
				}
			}
		case TABLE_ModuleRef:
		case TABLE_Method:
		case TABLE_TypeSpec:
		case TABLE_TypeDef:
		default:
			panic("MemberRef::ResolveField");
		}
	}

	Method* MemberRef::ResolveMethod() const {
		if(method!=NULL) return method;
		const Metadata::StringStream& ss = getRoot().getStringStream();
		switch(Class.table) {
		case TABLE_TypeRef:
			{
				TypeRef& typeref = getAssembly().getTypeRef(Class.index);
				const TypeDef* typdef = typeref.Resolve();
				if(typdef==NULL) {
					panic("Assembly.ResolveTypeRef Failed");
				} else {
					Signature::MethodSig& sig = const_cast<Signature::MethodSig&>(getMethodSig());
					sig.Params.Link(getAssembly());
					return method = typdef->ResolveMethod(name, sig.Params);
				}
			}
		case TABLE_Method:
		case TABLE_ModuleRef:
		case TABLE_TypeSpec:
		case TABLE_TypeDef:
		default:
			panic("MemberRef::ResolveMethod");
		}
	}

}
