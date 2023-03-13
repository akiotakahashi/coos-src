#include "dma.h"
#include "io.h"
#include "console.h"


namespace DMA {

	class DMAController {
		uint16 p_cur_address[4];
		uint16 p_cur_counter[4];
		uint16 p_status;
		uint16 p_command;
		uint16 p_request;
		uint16 p_single_mask;
		uint16 p_mode;
		uint16 p_clear_byte_ptr_flipflop;
		uint16 p_temporary;
		uint16 p_master_clear;
		uint16 p_clear_mask;
		uint16 p_all_mask;
	private:
		void Initialize(byte mode, byte mask) {
			outp8(p_master_clear, 0x00);	// master clear (reset)
			outp8(p_command, 0x00);			// cmd. reg.
			outp8(p_mode, mode);			// mode reg.
			outp8(p_all_mask, mask);		// all mask reg.
		}
	public:
		void InitializeAsMaster() {
			p_cur_address[0] = 0x00C0;
			p_cur_address[1] = 0x00C4;
			p_cur_address[2] = 0x00C8;
			p_cur_address[3] = 0x00CC;
			p_cur_counter[0] = 0x00C2;
			p_cur_counter[1] = 0x00C6;
			p_cur_counter[2] = 0x00CA;
			p_cur_counter[3] = 0x00CE;
			p_status		= 0x00D0;
			p_command		= 0x00D0;
			p_request		= 0x00D2;
			p_single_mask	= 0x00D4;
			p_mode			= 0x00D6;
			p_clear_byte_ptr_flipflop = 0x00D8;
			p_temporary		= 0x00DA;
			p_master_clear	= 0x00DA;
			p_clear_mask	= 0x00DC;
			p_all_mask		= 0x00DE;
			Initialize(0xC0, 0x0E);		// (cascade, verify, ch0), (all mask except for Slave)
		}
		void InitializeAsSlave() {
			p_cur_address[0] = 0x0000;
			p_cur_address[1] = 0x0002;
			p_cur_address[2] = 0x0004;
			p_cur_address[3] = 0x0006;
			p_cur_counter[0] = 0x0001;
			p_cur_counter[1] = 0x0003;
			p_cur_counter[2] = 0x0005;
			p_cur_counter[3] = 0x0007;
			p_status		= 0x0008;
			p_command		= 0x0008;
			p_request		= 0x0009;
			p_single_mask	= 0x000A;
			p_mode			= 0x000B;
			p_clear_byte_ptr_flipflop = 0x000C;
			p_temporary		= 0x000D;
			p_master_clear	= 0x000D;
			p_clear_mask	= 0x000E;
			p_all_mask		= 0x000F;
			Initialize(0x40, 0x0F);		// (single, verify, ch0), (all mask)
		}
	public:
		void Dump() {
			getConsole() << "DMAC Status: " << inp8(p_status) << endl;
			for(int i=0; i<4; ++i) {
				getConsole() << "Channel#" << i
					<< " Address: " << GetCurrentAddress(i)
					<< " Counter: " << GetCurrentCounter(i)
					<< endl;
			}
		}
		void EnableChannel(int chno) {
			outp8(p_single_mask, chno);
		}
		void DisableChannel(int chno) {
			outp8(p_single_mask, chno|0x4);
		}
		void SetDirection(int chno, IOMODES iomode) {
			outp8(p_mode, 0x40 | chno | iomode);
		}
		void ClearBytePointerFlopflop() {
			outp8(p_clear_byte_ptr_flipflop, 0x00);
		}
		void SetCurrentAddress(int chno, uint16 addr) {
			outp8(p_cur_address[chno], (addr&0x00FF)>>0);
			outp8(p_cur_address[chno], (addr&0xFF00)>>8);
		}
		void SetCurrentCounter(int chno, uint16 size) {
			outp8(p_cur_counter[chno], (size&0x00FF)>>0);
			outp8(p_cur_counter[chno], (size&0xFF00)>>8);
		}
		uint16 GetCurrentAddress(int chno) {
			return inp8(p_cur_address[chno]) | ((uint16)inp8(p_cur_address[chno])<<8);
		}
		uint16 GetCurrentCounter(int chno) {
			return inp8(p_cur_counter[chno]) | ((uint16)inp8(p_cur_counter[chno])<<8);
		}
	};

	static DMAController mas;
	static DMAController sla;
	static uint16 p_highaddr[8];
	static uint16 p_highaddr_ram_refresh;

	extern void Initialize() {
		p_highaddr[0] = 0x0087;
		p_highaddr[1] = 0x0083;
		p_highaddr[2] = 0x0081;
		p_highaddr[3] = 0x0082;
		p_highaddr[4] = 0xFFFF;
		p_highaddr[5] = 0x008B;
		p_highaddr[6] = 0x0089;
		p_highaddr[7] = 0x008A;
		p_highaddr_ram_refresh = 0x008F;
		mas.InitializeAsMaster();
		sla.InitializeAsSlave();
	}

	extern void Dump() {
		sla.Dump();
		mas.Dump();
	}

	static void EnableChannel(int chno) {
		if(chno<4) {
			sla.EnableChannel(chno);
		} else {
			mas.EnableChannel(chno-4);
		}
	}

	static void DisableChannel(int chno) {
		if(chno<4) {
			sla.DisableChannel(chno);
		} else {
			mas.DisableChannel(chno-4);
		}
	}

	static void* buffers[8];
	static uint16 sizes[8];

	static void* getDMABufferAddress(int chno) {
		return (void*)(0x00030000+0x00010000*chno);
	}

	extern void BeginTransfer(int chno, IOMODES mode, void* buf, uint16 size) {
		DMAController* dmac = NULL;
		int lchno;
		if(chno<4) {
			dmac = &sla;
			lchno = chno;
		} else {
			dmac = &mas;
			lchno = chno-4;
		}
		DisableChannel(chno);
		buffers[chno] = buf;
		sizes[chno] = size;
		buf = getDMABufferAddress(chno);
		dmac->SetDirection(lchno, mode);
		dmac->ClearBytePointerFlopflop();
		outp8(p_highaddr[chno], (byte)(((uint32)buf)>>16));
		dmac->SetCurrentAddress(lchno, (uint16)buf);
		dmac->SetCurrentCounter(lchno, size-1);
		EnableChannel(chno);
	}

	extern void EndTransfer(int chno) {
		DisableChannel(chno);
		memcpy(buffers[chno], getDMABufferAddress(chno), sizes[chno]);
		buffers[chno] = NULL;
		sizes[chno] = 0;
	}

	extern uint GetCurrentAddress(int chno) {
		DMAController* dmac = NULL;
		int lchno;
		if(chno<4) {
			dmac = &sla;
			lchno = chno;
		} else {
			dmac = &mas;
			lchno = chno-4;
		}
		return ((uint32)p_highaddr[chno]<<16) | dmac->GetCurrentAddress(lchno);
	}

	extern uint GetCurrentCounter(int chno) {
		DMAController* dmac = NULL;
		int lchno;
		if(chno<4) {
			dmac = &sla;
			lchno = chno;
		} else {
			dmac = &mas;
			lchno = chno-4;
		}
		return dmac->GetCurrentCounter(lchno);
	}

}
