#pragma once

#include "index.h"
#include "table.h"


namespace Metadata {

	class CustomAttributeTypeIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 5, rowcounts);
		}
	public:
		CustomAttributeTypeIndex() {
		}
		CustomAttributeTypeIndex(uint value) {
			parse(value, tables, 5);
		} 
	};

	class HasConstIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 3, rowcounts);
		}
	public:
		HasConstIndex() {
		}
		HasConstIndex(uint value) {
			parse(value, tables, 3);
		} 
	};

	class HasCustomAttributeIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 19, rowcounts);
		}
	public:
		HasCustomAttributeIndex() {
		}
		HasCustomAttributeIndex(uint value) {
			parse(value, tables, 19);
		} 
	};

	class HasDeclSecurityIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 3, rowcounts);
		}
	public:
		HasDeclSecurityIndex() {
		}
		HasDeclSecurityIndex(uint value) {
			parse(value, tables, 3);
		} 
	};

	class HasFieldMarshalIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 2, rowcounts);
		}
	public:
		HasFieldMarshalIndex() {
		}
		HasFieldMarshalIndex(uint value) {
			parse(value, tables, 2);
		} 
	};

	class HasSemanticsIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 2, rowcounts);
		}
	public:
		HasSemanticsIndex() {
		}
		HasSemanticsIndex(uint value) {
			parse(value, tables, 2);
		} 
	};

	class ImplementationIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 3, rowcounts);
		}
	public:
		ImplementationIndex() {
		}
		ImplementationIndex(uint value) {
			parse(value, tables, 3);
		} 
	};

	class MemberForwardedIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 2, rowcounts);
		}
	public:
		MemberForwardedIndex() {
		}
		MemberForwardedIndex(uint value) {
			parse(value, tables, 2);
		} 
	};

	class MemberRefParentIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 5, rowcounts);
		}
	public:
		MemberRefParentIndex() {
		}
		MemberRefParentIndex(uint value) {
			parse(value, tables, 5);
		} 
	};

	class MethodDefOrRefIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 2, rowcounts);
		}
	public:
		MethodDefOrRefIndex() {
		}
		MethodDefOrRefIndex(uint value) {
			parse(value, tables, 2);
		} 
	};

	class ResolutionScopeIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 4, rowcounts);
		}
	public:
		ResolutionScopeIndex() {
		}
		ResolutionScopeIndex(uint value) {
			parse(value, tables, 4);
		} 
	};

	class TypeDefOrRefIndex : public Index {
		static TableId tables[];
	public:
		static uint CalcSize(const uint rowcounts[]) {
			return getSize(tables, 3, rowcounts);
		}
	public:
		TypeDefOrRefIndex() {
		}
		TypeDefOrRefIndex(uint value) {
			parse(value, tables, 3);
		} 
	};

}
