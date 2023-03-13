#pragma once

#include "stdtype.h"


namespace DMA {
    
	enum IOMODES {
		IOMODE_VERIFY	= 0x00,
		IOMODE_IO_TO_MEM	= 0x04,
		IOMODE_MEM_TO_IO	= 0x08,
		IOMODE_FORBIDDEN	= 0x0C,
	};

	extern void Initialize();
	extern void Dump();
	extern void BeginTransfer(int chno, IOMODES mode, void* buf, uint16 size);
	extern void EndTransfer(int chno);
	extern uint GetCurrentAddress(int chno);
	extern uint GetCurrentCounter(int chno);

}
