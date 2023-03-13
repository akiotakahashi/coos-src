#include "LocalVarSig.h"
#include "../ilengine.h"
#include "../reflection.h"


namespace Signature {

	LocalVarSig::LocalVarSig() {
	}

	LocalVarSig::~LocalVarSig() {
	}

	bool LocalVarSig::Parse(SignatureStream& _ss) {
		const int LOCAL_SIG = 0x07;
		SignatureStream ss(_ss);
		if(ss.readByte()!=LOCAL_SIG) {
			return false;
		}
		int Count = ss.readInt();
		if(Count==0) return false;
		for(int i=0; i<Count; ++i) {
			LocalVar var;
			if(!var.Parse(ss)) {
				return false;
			}
			LocalVars.push_back(var);
		}
		ss.commit();
		return true;
	}

	const LayoutInfo LocalVarSig::BuildVarLayout(const Reflection::Assembly& assembly) const {
		LayoutInfo varinfo(LocalVars.size());
		int offset = 0;
		for(int i=LocalVars.size()-1; i>=0; --i) {
			const LocalVar& localVar = LocalVars[i];
			uint size = assembly.CalcSizeOfType(localVar);
			if(size>4) offset=(offset+7)&~7;
			else if(size>2) offset=(offset+3)&~3;
			else if(size>1) offset=(offset+1)&~1;
			varinfo.sizes.push_front(size);
			varinfo.offsets.push_front(offset);
			offset += size;
		}
		varinfo.totalsize = IL::ILStack::getSizeOnStack(offset);
		return varinfo;
	}


	LocalVar::LocalVar() {
		ByRef = false;
		TypedByRef = false;
	}

	bool LocalVar::Parse(SignatureStream& _ss) {
		SignatureStream ss(_ss);
		//----------------------------------------------------------------------------------------
		// This is not documented but it seems to be needed from real Assembly.
		while(true) {
			CustomMod cm;
			if(!cm.Parse(ss)) {
				break;
			}
			CustomMods.push_back(cm);
		}
		//----------------------------------------------------------------------------------------
		while(true) {
			Constraint constraint;
			if(!constraint.Parse(ss)) {
				break;
			}
			Constraints.push_back(constraint);
		}
		switch(ss.peekByte()) {
		case ELEMENT_TYPE_BYREF:
			ss.readByte();
			ByRef = true;
			break;
		}
		if(ss.peekByte()==ELEMENT_TYPE_TYPEDBYREF) {
			TypedByRef = true;
			ss.readByte();
			ss.commit();
			return true;
		} else if(Type.Parse(ss)) {
			ss.commit();
			return true;
		} else {
			return false;
		}
	}

}
