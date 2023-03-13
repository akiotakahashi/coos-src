#pragma once

#include "stdafx.h"


namespace Reflection {

	class TypeDef;
	class Field;
	class Method;
	class TypeRef;
	class MemberRef;

	class Assembly : public ReflectionItem {
		typedef std::vector<TypeDef*> TypeDefList;
		typedef std::vector<Field*> FieldList;
		typedef std::vector<Method*> MethodList;
		typedef std::vector<TypeRef*> TypeRefList;
		typedef std::vector<MemberRef*> MemberRefList;
		std::wstring name;
		const byte* imgbase;
		std::vector<uint> fieldLists;
		std::vector<uint> methodLists;
		void* managedType;
		mutable TypeDefList typedefs;
		mutable FieldList fields;
		mutable MethodList methods;
		mutable TypeRefList typerefs;
		mutable MemberRefList memrefs;
		mutable uint cumulativeOffsetStatic;
		std::map<int,Method*> patchedMethods;
	public:
		Assembly(const char* name, const Metadata::MetadataRoot& root, const byte* imgbase);
		~Assembly();
		void Dispose();
	public:
		const std::wstring& getName() const { return name; }
		const byte* getImageBase() const { return imgbase; }
		size_t getTypeDefCount() const { return typedefs.size(); }
		TypeDef& getTypeDef(uint index) const;
		TypeRef& getTypeRef(uint index) const;
		MemberRef& getMemberRef(uint index) const;
	public:
		Field& getField(uint index, TypeDef* ptypdef = NULL) const;
		Method& getMethod(uint index, TypeDef* ptypdef = NULL) const;
		TypeDef* getTypeDefOfField(uint index) const;
		TypeDef* getTypeDefOfMethod(uint index) const;
	public:
		void setManagedType(void* obj) {managedType=obj;}
		void* getManagedType() const {return managedType;}
	public:
		void LinkVTableFixups();
		Method* GetPatchedMethodBy(Method* method) const;
		void ReplaceMethodWith(uint index, Method* method);
	public:
		uint CalcSizeOfType(const Signature::TypeSig& type) const;
		uint CalcSizeOfType(const Signature::RetType& type) const;
		uint CalcSizeOfType(const Signature::ParamSig& param) const;
		uint CalcSizeOfType(const Signature::LocalVar& localvar) const;
		const TypeDef* ResolveTypeDef(Metadata::Index codedIndex) const;
		const TypeDef* ResolveTypeDef(const Signature::TypeSig& typeSig) const;
		Field* ResolveField(Metadata::Index codedIndex) const;
		Method* ResolveMethod(Metadata::Index codedIndex) const;
		TypeDef* FindTypeDef(const std::wstring& name, const std::wstring& Namespace);
		TypeRef* FindTypeRef(const std::wstring& name, const std::wstring& Namespace);
		TypeDef* ResolveType(const std::wstring& name, const std::wstring& Namespace);
	public:
		const TypeDef* FindTypeDef(const std::wstring& name, const std::wstring& Namespace) const
		{ return const_cast<Assembly*>(this)->FindTypeDef(name,Namespace); }
		const TypeRef* FindTypeRef(const std::wstring& name, const std::wstring& Namespace) const
		{ return const_cast<Assembly*>(this)->FindTypeRef(name,Namespace); }
		const TypeDef* ResolveType(const std::wstring& name, const std::wstring& Namespace) const
		{ return const_cast<Assembly*>(this)->ResolveType(name,Namespace); }
	};

	class AssemblyItem : public ReflectionItem {
		Assembly& assembly;
		uint tableIndex;
	protected:
		AssemblyItem(Assembly& parent, uint tableIndex) : ReflectionItem(parent.getRoot()), assembly(parent) {
			this->tableIndex = tableIndex;
		}
	public:
		Assembly& getAssembly() const { return assembly; }
		uint getTableIndex() const { return tableIndex; }
	};

}
