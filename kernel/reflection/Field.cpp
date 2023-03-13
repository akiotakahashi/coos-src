#include "Field.h"


namespace Reflection {

	Field::Field(Assembly& parent, uint tableIndex, const Metadata::Field& field) : Metadata::Field(field), TypeDefItem(parent,tableIndex) {
		const Metadata::StringStream& ss = getRoot().getStringStream();
		const Metadata::BlobStream& blob = getRoot().getBlobStream();
		name = ss.getString(field.Name);
		if(!Signature.Parse(Signature::SignatureStream(blob+field.Signature))) {
			panic("Field::Field failed to parse Type of signature for field: "+itos<char,16>(field.Signature));
		}
		size = -1;
		offset = -1;
		staticValue = NULL;
	}

	Field::~Field() {
	}

	TypeDef* Field::ResolveTypeDef() const {
		return getAssembly().getTypeDefOfField(getTableIndex());
	}

	uint Field::getSize() const {
		if(size==-2) {
			panic("Field::getSize: Infinite recursive call");
		}
		if(size==-1) {
			size = -2;
			size = getAssembly().CalcSizeOfType(Signature.Type);
		}
		return size;
	}

	uint Field::getOffset() const {
		if(offset==-1) {
			TypeDef& typdef = getTypeDef();
			if(this->IsStatic()) {
				typdef.DetermineStaticFieldLayout();
			} else {
				typdef.DetermineInstanceFieldLayout();
			}
		}
		return offset;
	}

	void Field::setOffset(uint offset) {
		this->offset = offset;
		/*
		getConsole().MakeNewLine();
		getConsole() << "Layout: " << offset << " of " << getTypeDef().getFullName() << ":" << getName() << endl;
		*/
	}
	
	void Field::InitStaticValue() {
		staticValue = this->getTypeDef().getStaticHeap()+getOffset();
		if(HasFieldRVA()) {
			const Metadata::MainStream& ms = getTypeDef().getRoot().getMainStream();
			for(uint i=1; i<=ms.getRowCount(TABLE_FieldRVA); ++i) {
				Metadata::FieldRVA* table = (Metadata::FieldRVA*)ms.getRow(TABLE_FieldRVA,i);
				if(table->Field==getTableIndex()) {
					const byte* p = getAssembly().getImageBase()+table->RVA;
					/*
					memcpy(staticValue, p, this->getSize());
					///*/
					//TODO: fix me to copy literal data into local memory.
					staticValue = (void*)p;
					//*/
					delete table;
					goto exit;
				}
				delete table;
			}
			panic("Field with RVA doesn't has own FieldRVA.");
		}
exit:
		getTypeDef().MakeClassInitialized();
	}

	void Field::ResetFieldMemory() {
		InitStaticValue();
	}

	const void* Field::getStaticValue() {
		if(staticValue==NULL)
			InitStaticValue();
		return staticValue;
	}

	void Field::setStaticValue(const void* p) {
		if(staticValue==NULL)
			InitStaticValue();
		memcpy(staticValue, p, getSize());
	}

}
