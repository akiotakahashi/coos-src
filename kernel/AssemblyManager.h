#pragma once

#include "reflection.h"
#include "ilengine.h"
#include "stl.h"


namespace AssemblyManager {

	extern Reflection::Assembly* LoadAssembly(const void* buf);

	extern void Finalize();

	extern int GetAssemblyCount();
	extern Reflection::Assembly* GetAssembly(int index);
	extern Reflection::Assembly* FindAssembly(const std::wstring& name);
	extern Reflection::TypeDef* FindTypeDef(const std::wstring& asmname, const std::wstring& ns, const std::wstring& name);

	extern bool LinkAssembly(const wchar_t* assemblyname);
	extern bool IntroduceAssemblyIntoManaged(const std::wstring& name, const void* data, int size);
	extern bool Execute(const std::wstring& assemblyName, const std::wstring& Namespace, const std::wstring& typeName, const std::wstring& methodName, IL::ILMachine& machine);

}
