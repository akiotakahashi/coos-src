#pragma once

#include "console.h"
#include "stl.h"


struct ICommandProcessor {
	virtual std::wstring getName() const = 0;
	virtual std::wstring getDescription() const = 0;
	virtual void ShowManual(Console& console) const = 0;
	virtual int Execute(const std::vector<std::wstring>& args) = 0;
};

namespace Commands {

	extern void Initialize();
	extern void* GetSlotData(int index);
	extern uint GetSlotSize(int index);
	extern int Execute(const std::wstring& cmdline);
	extern int Execute(const std::vector<std::wstring>& args);

	extern bool IntroduceAssemblyIntoManaged(const std::wstring& name, int slotidx);

}
