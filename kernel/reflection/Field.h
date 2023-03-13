#pragma once

#include "stdafx.h"
#include "TypeDef.h"


namespace Reflection {

	class Field : public Metadata::Field, public TypeDefItem {
		//friend class TypeDef;
		std::wstring name;
		Signature::FieldSig Signature;
		mutable uint size;
		uint offset;
		void* staticValue;
	public:
		Field(Assembly& parent, uint fieldIndex, const Metadata::Field& field);
		~Field();
		void InitStaticValue();
	protected:
		virtual TypeDef* ResolveTypeDef() const;
	public:
		const std::wstring& getName() const { return name; }
		bool IsStatic() const { return 0!=(Flags&Metadata::FieldAttributes_Static); }
		bool IsInstance() const { return 0==(Flags&Metadata::FieldAttributes_Static); }
		bool HasFieldRVA() const { return 0!=(Flags&Metadata::FieldAttributes_HasFieldRVA); }
		const Signature::FieldSig& getSignature() const { return Signature; }
		uint getSize() const;
		uint getOffset() const;
		void setOffset(uint offset);
		void ResetFieldMemory();
		const void* getStaticValue();
		void setStaticValue(const void* p);
	};

}
