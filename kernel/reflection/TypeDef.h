#pragma once

#include "stdafx.h"
#include "Assembly.h"

namespace Reflection {

	class Field;
	class Method;

	struct RuntimeTypeHandle {
		void* TrackingField;
		RuntimeTypeHandle(void* object) {
			TrackingField = object;
		}
		RuntimeTypeHandle(nint intptr) {
			TrackingField = (void*)intptr;
		}
		operator void*() const {
			return TrackingField;
		}
	};

	class TypeDef : public Metadata::TypeDef, public AssemblyItem {
		friend class Assembly;
		mutable void* pManagedType;
		std::wstring name;
		std::wstring ns;
		uint fieldCount;
		uint methodCount;
		int staticSize;
		int instanceSize;
		byte* staticheap;
		bool initialized;
		mutable bool fixedManagedType;
		const TypeDef* elemTypeDef;
		mutable TypeDef* arrayTypeDef;
		mutable TypeDef* pointerTypeDef;
		mutable const TypeDef* baseTypeDef;
	private:
		void Init();
		TypeDef(const TypeDef& typdef, int dimension);	// for Arrays
		TypeDef(const TypeDef& typdef);					// for Pointers
	public:
		TypeDef(Assembly& parent, uint tableIndex, const Metadata::TypeDef& td, const Metadata::TypeDef* ptdnext);
		~TypeDef();
	public:
		static TypeDef* getTypeFromHandle(RuntimeTypeHandle handle);
		RuntimeTypeHandle getHandleFromType() const;
	public:
		const std::wstring& getName() const { return name; }
		const std::wstring& getNamespace() const { return ns; }
		const std::wstring getFullName() const { return ns+L"."+name; }
		uint getFieldCount() const { return fieldCount; }
		uint getMethodCount() const { return methodCount; }
		Field& getField(uint index) const;
		Method& getMethod(uint index) const;
		uint getStaticSize() const;
		uint getInstanceSize() const;
		uint getVariableSize() const;
		bool IsValueType() const;
		bool IsInterface() const;
		bool IsRefType() const;
		bool IsArray() const;
		bool IsPointer() const;
		const TypeDef* getBaseType() const;
		const TypeDef* getElementType() const;
		bool IsSubclassOf(const TypeDef* typdef) const;
		bool IsInterfaceImpl(TableId table, uint index) const;
	public:
		bool ReadyManagedType() const {return pManagedType!=NULL;}
		bool IsFixedManagedType() const { return fixedManagedType; }
		void FixManagedType() const;
		void setManagedType(void* obj);
		void* getManagedType() const;
		void ReloadManagedType(bool newload);
	public:
		void MakeClassInitialized();
		bool IsClassInitialized() const { return initialized; }
		void MarkClassInitialized(bool init) { initialized=init; }
	public:
		TypeDef* CreateArrayType() const;
		TypeDef* CreatePointerType() const;
	public:
		void NotifyLoadingType(void* obj);
		byte* getStaticHeap() const;
		void setStaticHeap(void* p);
		void DetermineInstanceFieldLayout();
		void DetermineStaticFieldLayout();
	public:
		Field* ResolveField(const std::wstring& name, bool nofail) const;
		Method* GetSingleMethod(const std::wstring& name, bool nofail) const;
		Method* GetCallableMethod(const std::wstring& name, bool nofail) const;
		Method* ResolveOverrideMethod(const Method& method, bool nofail) const;
		Method* ResolveMethod(const std::wstring& name, const Signature::ParamSigList& params) const;
	};

	class TypeDefItem : public AssemblyItem {
		friend class Assembly;
		mutable TypeDef* typedef_;
	protected:
		TypeDefItem(Assembly& parent, uint tableIndex) : AssemblyItem(parent,tableIndex) {
			typedef_ = NULL;
		}
		virtual TypeDef* ResolveTypeDef() const = 0;
	private:
		void SetTypeDef(TypeDef* ptypdef) {
			typedef_ = ptypdef;
		}
	public:
		TypeDef& getTypeDef() const {
			if(typedef_==NULL) {
				typedef_ = ResolveTypeDef();
				if(typedef_==NULL) panic("getTypeDef Failed");
			}
			return *typedef_;
		}
	};

}
