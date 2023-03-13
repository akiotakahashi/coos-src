#include "Assembly.h"
#include "TypeDef.h"
#include "Field.h"
#include "Method.h"
#include "TypeRef.h"
#include "MemberRef.h"
#include "../AssemblyManager.h"


namespace Reflection {
	
	Assembly::Assembly(const char* _name, const Metadata::MetadataRoot& _root, const byte* _imgbase) : ReflectionItem(_root) {
		const Metadata::MainStream& ms = getRoot().getMainStream();
		const Metadata::StringStream& ss = getRoot().getStringStream();
		imgbase = _imgbase;
		name.assign(_name, _name+strlen(_name));
		cumulativeOffsetStatic = 0;
		// TypeDef
		typedefs.resize(ms.getRowCount(TABLE_TypeDef));
		fieldLists.resize(typedefs.size()+1);
		methodLists.resize(typedefs.size()+1);
		for(uint i=1; i<=ms.getRowCount(TABLE_TypeDef); ++i) {
			Metadata::TypeDef* typdef = (Metadata::TypeDef*)ms.getRow(TABLE_TypeDef, i);
			fieldLists[i-1] = typdef->FieldList;
			methodLists[i-1] = typdef->MethodList;
			delete typdef;
		}
		fieldLists.back() = ms.getRowCount(TABLE_Field)+1;
		methodLists.back() = ms.getRowCount(TABLE_Method)+1;
		//
		fields.resize(ms.getRowCount(TABLE_Field));
		methods.resize(ms.getRowCount(TABLE_Method));
		// TypeRef
		for(uint i=1; i<=ms.getRowCount(TABLE_TypeRef); ++i) {
			Metadata::TypeRef* typeref = (Metadata::TypeRef*)ms.getRow(TABLE_TypeRef, i);
			typerefs.push_back(new TypeRef(*this, i, *typeref));
			delete typeref;
		}
		// MemberRef
		for(uint i=1; i<=ms.getRowCount(TABLE_MemberRef); ++i) {
			Metadata::MemberRef* memref = (Metadata::MemberRef*)ms.getRow(TABLE_MemberRef, i);
			memrefs.push_back(new MemberRef(*this, i, *memref));
			delete memref;
		}
		//
		managedType = NULL;
	}

	Assembly::~Assembly() {
		Dispose();
	}

	void Assembly::Dispose() {
		for(uint i=0; i<memrefs.size(); ++i) {
			delete memrefs[i];
		}
		memrefs.clear();
		for(uint i=0; i<typerefs.size(); ++i) {
			delete typerefs[i];
		}
		typerefs.clear();
		std::map<int,Method*>::iterator it;
		it = patchedMethods.begin();
		while(it!=patchedMethods.end()) {
			methods[it->first] = it->second;
			++it;
		}
		patchedMethods.clear();
		for(uint i=0; i<methods.size(); ++i) {
			if(methods[i]) {
				//getConsole() << methods[i] << " [" << &methods[i]->getAssembly() << "] " << methods[i]->getFullName() << endl;
				delete methods[i];
			}
		}
		methods.clear();
		for(uint i=0; i<fields.size(); ++i) {
			if(fields[i]) delete fields[i];
		}
		fields.clear();
		for(uint i=0; i<typedefs.size(); ++i) {
			if(typedefs[i]) delete typedefs[i];
		}
		typedefs.clear();
	}

	TypeDef& Assembly::getTypeDef(uint index) const {
		if(index<1 || typedefs.size()<index) panic("Assembly::getTypeDef: out of index");
		TypeDefList::iterator it = typedefs.begin()+(index-1);
		if(*it==NULL) {
			const Metadata::MainStream& ms = getRoot().getMainStream();
			Metadata::TypeDef* td = (Metadata::TypeDef*)ms.getRow(TABLE_TypeDef, index);
			Metadata::TypeDef* td2 = NULL;
			if(index+1<=ms.getRowCount(TABLE_TypeDef)) {
				td2 = (Metadata::TypeDef*)ms.getRow(TABLE_TypeDef, index+1);
			}
			*it = new TypeDef(*const_cast<Assembly*>(this), index, *td, td2);
			delete td2;
			delete td;
		}
		return **it;
	}

	Field& Assembly::getField(uint index, TypeDef* ptypdef) const {
		if(index<1 || fields.size()<index) panic("Assembly::getField: out of index");
		FieldList::iterator it = fields.begin()+(index-1);
		if(*it==NULL) {
			const Metadata::MainStream& ms = getRoot().getMainStream();
			Metadata::Field* row = (Metadata::Field*)ms.getRow(TABLE_Field, index);
			*it = new Field(*const_cast<Assembly*>(this), index, *row);
			(*it)->SetTypeDef(ptypdef);
			delete row;
		}
		return **it;
	}

	Method& Assembly::getMethod(uint index, TypeDef* ptypdef) const {
		if(index<1 || methods.size()<index) panic("Assembly::getMethod: out of index");
		MethodList::iterator it = methods.begin()+(index-1);
		if(*it==NULL) {
			const Metadata::MainStream& ms = getRoot().getMainStream();
			Metadata::Method* row = (Metadata::Method*)ms.getRow(TABLE_Method, index);
			Metadata::Method* row2 = NULL;
			if(index+1<=ms.getRowCount(TABLE_Method)) {
				row2 = (Metadata::Method*)ms.getRow(TABLE_Method, index+1);
			}
			*it = new Method(*const_cast<Assembly*>(this), index, *row, row2);
			(*it)->SetTypeDef(ptypdef);
			//getConsole() << "Alloc [" << index << "]: " << (*it)->getFullName() << endl;
			delete row2;
			delete row;
		}
		return **it;
	}

	TypeRef& Assembly::getTypeRef(uint index) const {
		return *typerefs[index-1];
	}

	MemberRef& Assembly::getMemberRef(uint index) const {
		return *memrefs[index-1];
	}

	TypeDef* Assembly::getTypeDefOfField(uint index) const {
		for(uint i=0; i<typedefs.size(); ++i) {
			if(fieldLists[i]<=index && index<fieldLists[i+1]) {
				return &getTypeDef(i+1);
			}
		}
		return NULL;
	}

	TypeDef* Assembly::getTypeDefOfMethod(uint index) const {
		for(uint i=0; i<typedefs.size(); ++i) {
			if(methodLists[i]<=index && index<methodLists[i+1]) {
				return &getTypeDef(i+1);
			}
		}
		return NULL;
	}

	void Assembly::LinkVTableFixups() {
		for(int i=0; i<this->getRoot().GetVTableFixupCount(); ++i) {
			Metadata::Raw::VTableFixup vt = this->getRoot().GetVTableFixup(i);
			//getConsole() << "VA=" << vt.VirtualAddress << ", Size=" << vt.Size << ", Type=" << vt.Type << endl;
			uint* ptoken = (uint*)(this->getImageBase()+vt.VirtualAddress);
			Reflection::Method* method = this->ResolveMethod(Metadata::MetadataToken(*ptoken));
			if(method!=NULL) {
				*ptoken = (uint)method->CreateBridgeCode();
			}
		}
	}

	Method* Assembly::GetPatchedMethodBy(Method* method) const {
		std::map<int,Method*>::const_iterator it;
		it = patchedMethods.begin();
		while(it!=patchedMethods.end()) {
			if(methods[it->first]==method) {
				return it->second;
			}
			++it;
		}
		return NULL;
	}

	void Assembly::ReplaceMethodWith(uint index, Method* method) {
		--index;
		//getConsole() << "[" << index << "] " << methods[index]->getFullName() << " -> " << method->getFullName() << endl;
		if(patchedMethods[index]==NULL) {
			patchedMethods[index] = methods[index];
		}
		methods[index] = method;
	}

	uint Assembly::CalcSizeOfType(const Signature::TypeSig& type) const {
		switch(type.ElementType) {
		case ELEMENT_TYPE_VOID:
			return 0;
		case ELEMENT_TYPE_I:
		case ELEMENT_TYPE_U:
			return sizeof(void*);
		case ELEMENT_TYPE_BOOLEAN:
		case ELEMENT_TYPE_I1:
		case ELEMENT_TYPE_U1:
			return 1;
		case ELEMENT_TYPE_CHAR:
		case ELEMENT_TYPE_I2:
		case ELEMENT_TYPE_U2:
			return 2;
		case ELEMENT_TYPE_I4:
		case ELEMENT_TYPE_U4:
		case ELEMENT_TYPE_R4:
			return 4;
		case ELEMENT_TYPE_I8:
		case ELEMENT_TYPE_U8:
		case ELEMENT_TYPE_R8:
			return 8;
		case ELEMENT_TYPE_VALUETYPE:
			{
				const TypeDef* typdef = ResolveTypeDef(*type.TypeDefOrRef);
				if(typdef==NULL) panic("Assembly::CalcSizeOfElementType cann't find TypeDef of ValueType.");
				return typdef->getInstanceSize();
			}
		default:
			return sizeof(void*);
		}
	}

	uint Assembly::CalcSizeOfType(const Signature::RetType& type) const {
		if(type.Void) return 0;
		if(type.ByRef) return sizeof(void*);
		return CalcSizeOfType(*type.Type);
	}

	uint Assembly::CalcSizeOfType(const Signature::ParamSig& param) const {
		if(param.ByRef) return sizeof(void*);
		if(param.TypedByRef) return sizeof(void*);
		return CalcSizeOfType(param.Type);
	}

	uint Assembly::CalcSizeOfType(const Signature::LocalVar& localvar) const {
		if(localvar.ByRef) return sizeof(void*);
		return CalcSizeOfType(localvar.Type);
	}

	const TypeDef* Assembly::ResolveTypeDef(Metadata::Index codedIndex) const {
		switch(codedIndex.table) {
		case TABLE_TypeDef:
			return &getTypeDef(codedIndex.index);
		case TABLE_TypeRef:
			{
				TypeRef& tr = getTypeRef(codedIndex.index);
				return tr.Resolve();
			}
		case TABLE_TypeSpec:
			{
				const Metadata::MainStream& ms = getRoot().getMainStream();
				Metadata::TypeSpec* table = (Metadata::TypeSpec*)ms.getRow(TABLE_TypeSpec, codedIndex.index);
				const Metadata::BlobStream& blob = getRoot().getBlobStream();
				Signature::SignatureStream ss(blob+table->Signature);
				delete table;
				table = NULL;
				Signature::TypeSig typesig;
				if(!typesig.Parse(ss)) panic(L"Failed to parse TypeSpec");
				return ResolveTypeDef(typesig);
			}
		default:
			panic(L"Assembly::ResolveTypeDef detects unknown table"+itos<wchar_t,16>((int)codedIndex.table));
		}
		return NULL;
	}

	const TypeDef* Assembly::ResolveTypeDef(const Signature::TypeSig& type) const {
		switch(type.ElementType) {
		case ELEMENT_TYPE_VOID:
		case ELEMENT_TYPE_BOOLEAN:
		case ELEMENT_TYPE_CHAR:
		case ELEMENT_TYPE_I1:
		case ELEMENT_TYPE_U1:
		case ELEMENT_TYPE_I2:
		case ELEMENT_TYPE_U2:
		case ELEMENT_TYPE_I4:
		case ELEMENT_TYPE_U4:
		case ELEMENT_TYPE_R4:
		case ELEMENT_TYPE_I8:
		case ELEMENT_TYPE_U8:
		case ELEMENT_TYPE_R8:
		case ELEMENT_TYPE_I:
		case ELEMENT_TYPE_U:
		case ELEMENT_TYPE_OBJECT:
		case ELEMENT_TYPE_STRING:
			const wchar_t* classname;
			switch(type.ElementType) {
			case ELEMENT_TYPE_VOID:
				classname = L"Void";
				break;
			case ELEMENT_TYPE_BOOLEAN:
				classname = L"Boolean";
				break;
			case ELEMENT_TYPE_CHAR:
				classname = L"Char";
				break;
			case ELEMENT_TYPE_I1:
				classname = L"SByte";
				break;
			case ELEMENT_TYPE_U1:
				classname = L"Byte";
				break;
			case ELEMENT_TYPE_I2:
				classname = L"Int16";
				break;
			case ELEMENT_TYPE_U2:
				classname = L"UInt16";
				break;
			case ELEMENT_TYPE_I4:
				classname = L"Int32";
				break;
			case ELEMENT_TYPE_U4:
				classname = L"UInt32";
				break;
			case ELEMENT_TYPE_R4:
				classname = L"Single";
				break;
			case ELEMENT_TYPE_I8:
				classname = L"Int64";
				break;
			case ELEMENT_TYPE_U8:
				classname = L"UInt64";
				break;
			case ELEMENT_TYPE_R8:
				classname = L"Double";
				break;
			case ELEMENT_TYPE_I:
				classname = L"IntPtr";
				break;
			case ELEMENT_TYPE_U:
				classname = L"UIntPtr";
				break;
			case ELEMENT_TYPE_OBJECT:
				classname = L"Object";
				break;
			case ELEMENT_TYPE_STRING:
				classname = L"String";
				break;
			default:
				panic("Unknown element-type");
			}
			return AssemblyManager::FindTypeDef(L"mscorlib", L"System", classname);
		case ELEMENT_TYPE_VALUETYPE:
		case ELEMENT_TYPE_CLASS:
			return ResolveTypeDef(*type.TypeDefOrRef);
		case ELEMENT_TYPE_ARRAY:
			panic("MnArray is not supported");
		case ELEMENT_TYPE_SZARRAY:
			return ResolveTypeDef(*type.Type)->CreateArrayType();
		case ELEMENT_TYPE_PTR:
			if(type.Void) {
				return AssemblyManager::FindTypeDef(L"mscorlib",L"System",L"Void")->CreatePointerType();
			} else {
				return ResolveTypeDef(*type.Type)->CreatePointerType();
			}
		default:
			panic("Assembly::ResolveTypeDef: "+itos<char,16>((int)type.ElementType));
		}
	}

	Field* Assembly::ResolveField(Metadata::Index codedIndex) const {
		switch(codedIndex.table) {
		case TABLE_Field:
			return &getField(codedIndex.index);
		case TABLE_FieldRef:
			{
				MemberRef& fr = getMemberRef(codedIndex.index);
				return fr.ResolveField();
			}
			break;
		}
		return NULL;
	}

	Method* Assembly::ResolveMethod(Metadata::Index codedIndex) const {
		switch(codedIndex.table) {
		case TABLE_Method:
			return &getMethod(codedIndex.index);
		case TABLE_MethodRef:
			{
				MemberRef& fr = getMemberRef(codedIndex.index);
				return fr.ResolveMethod();
			}
			break;
		}
		return NULL;
	}

	TypeDef* Assembly::FindTypeDef(const std::wstring& name, const std::wstring& Namespace) {
		for(uint i=1; i<=typedefs.size(); ++i) {
			TypeDef& td = getTypeDef(i);
			if(td.getName()==name && td.getNamespace()==Namespace) {
				return &td;
			}
		}
		return NULL;
	}

	TypeRef* Assembly::FindTypeRef(const std::wstring& name, const std::wstring& Namespace) {
		for(uint i=1; i<=typerefs.size(); ++i) {
			TypeRef& tr = getTypeRef(i);
			if(tr.getName()==name && tr.getNamespace()==Namespace) {
				return &tr;
			}
		}
		return NULL;
	}

	TypeDef* Assembly::ResolveType(const std::wstring& name, const std::wstring& Namespace) {
		TypeDef* typdef = FindTypeDef(name,Namespace);
		if(typdef!=NULL) return typdef;
		TypeRef* typref = FindTypeRef(name,Namespace);
		if(typref==NULL) return NULL;
		return typref->Resolve();
	}

}
