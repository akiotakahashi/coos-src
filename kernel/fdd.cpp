#include "io.h"
#include "pic.h"
#include "dma.h"
#include "fdd.h"
#include "stdtype.h"
#include "stdlib.h"
#include "memory.h"
#include "stl.h"
#include "kernel.h"
#include "console.h"


#define FD_BYTES_PER_SECTOR			512
#define FD_SECTORS_PER_HEAD			18
#define FD_HEADS_PER_CYLINDER		2
#define FD_CYLINDERS_PER_MEDIA		80
#define FD_SECTORS_PER_CYLINDER		(FD_SECTORS_PER_HEAD*FD_HEADS_PER_CYLINDER)

#define FDC_DOR_RESET_Y		0x00
#define FDC_DOR_RESET_N		0x04
#define FDC_DOR_DMA_ENABLE	0x08
#define FDC_DOR_MOTOR_ON	0x10

#define FDC_DSR_RESET_Y		0x00
#define FDC_DSR_RESET_N		0x80

#define FDC_DR_MFM_1M		0x03
#define FDC_DR_MFM_500K		0x02
#define FDC_DR_MFM_300K		0x01
#define FDC_DR_MFM_250K		0x00

#define FDC_MSR_D0B			0x01
#define FDC_MSR_D1B			0x02
#define FDC_MSR_D2B			0x04
#define FDC_MSR_D3B			0x08
#define FDC_MSR_CB			0x10
#define FDC_MSR_NDM			0x20
#define FDC_MSR_DATA_READ	0x40
#define FDC_MSR_RQM			0x80

#define FDC_COMMAND_SEEK				0x0f
#define FDC_COMMAND_SENSE_INTERRUPT		0x08
#define FDC_COMMAND_SPECIFY				0x03
#define FDC_COMMAND_READ				0xE6		// Multitrack, MFM, Skip DDAM


static volatile bool InterruptedFlag = false;

static __declspec(naked) void __stdcall FDCInterruptHandler() {
	InterruptHandler_Prologue;
	InterruptedFlag = true;
	PIC::NotifyEndOfInterrupt(6);
	InterruptHandler_Epilogue;
}

namespace FDD {

	class FloppyDiskController;

	class FloppyDiskDrive : public IMedia {
		friend class FloppyDiskController;
		FloppyDiskController& fdc;
		int drive_no;
		int current_cylinder;
		int current_head;
		int current_sector;
	private:
		FloppyDiskDrive(FloppyDiskController& _fdc, int drvno);
		bool Initialize();
	public:
		~FloppyDiskDrive();
	public:
		uint32 getBlockCount() const { return 0x0b40; }
		size_t getBlockSize() const { return 512; }
	public:
		void Seek(uint32 block);
		void Read(void* buf, uint count);
	};

	union ST0 {
		byte value;
		struct {
			byte us		: 2;
			byte hd		: 1;
			byte nr		: 1;
			byte ec		: 1;
			byte se		: 1;
			byte ic		: 2;
		};
	};

	struct ReadDataResult {
		ST0 st0;
		byte st1;
		byte st2;
		byte c;
		byte h;
		byte r;
		byte n;
	};

	class FloppyDiskController {
		uint16 p_dor;
		uint16 p_msr;
		uint16 p_ccr;
		uint16 p_dsr;
		uint16 p_data;
		byte dor;
		std::vector<FloppyDiskDrive> drives;
	public:
		FloppyDiskController() {
		}
	private:
		bool Initialize() {
			if(inp8(p_msr)==0xFF) return false; 
			/* 初期化できねー
			outp8(p_dor, FDC_DOR_RESET_Y|FDC_DOR_DMA_ENABLE);
			outp8(p_dsr, FDC_DSR_RESET_Y|FDC_DR_MFM_500K);	//BOCHS: bochs doesn't support
			waitForReady();
			*/
			dor = FDC_DOR_RESET_N|FDC_DOR_DMA_ENABLE;
			outp8(p_dor, dor);
			outp8(p_ccr, FDC_DR_MFM_500K);
			if(!waitForIdle()) {
				return false;
			} else {
				getConsole() << "Specify Device Parameters" << endl;
				byte specifyCommand[] = { 0x03/*FDC_COMMAND_SPECIFY*/
					, 0xC1 // SRT=4ms  HUT=16ms
					, 0x10 // HLT=16ms DMAEnabled
				};
				return sendCommand(specifyCommand, sizeof(specifyCommand));
			}
		}
	public:
		bool InitializeAsMaster() {
			p_dor = 0x3f2;
			p_msr = 0x3f4;
			p_dsr = p_msr;
			p_ccr = 0x3f7;
			p_data = 0x3f5;
			return Initialize();
		}
		bool InitializeAsSlave() {
			p_dor = 0x372;
			p_msr = 0x374;
			p_dsr = p_msr;
			p_ccr = 0x377;
			p_data = 0x375;
			return Initialize();
		}
		void DetectDrives() {
			for(int i=0; i<4; ++i) {
				FloppyDiskDrive fdd(*this,i);
				getConsole() << "FDD#" << i << ": ";
				if(fdd.Initialize()) {
					getConsole() << "Exists." << endl;
					drives.push_back(fdd);
				} else {
					getConsole() << "None." << endl;
					break;
				}
			}
		}
	private:
		bool selectDrive(byte drvno) {
			dor &= ~0x3;
			dor |= drvno;
			outp8(p_dor, dor);
			return true;
		}
		bool checkStatus(byte mask, byte expected) {
			return expected==(inp8(p_msr)&mask);
		}
		bool _waitStatus(byte mask, byte expected) {
			int limit = 10;
			while(!checkStatus(mask,expected)) {
				Delay(10);
			}
			return limit>0;
		}
		bool isRedayForWriting() {
			return checkStatus(FDC_MSR_RQM|FDC_MSR_DATA_READ,FDC_MSR_RQM);
		}
		bool isRequiredToRead() {
			//return checkStatus(FDC_MSR_RQM|FDC_MSR_DATA_READ,FDC_MSR_RQM|FDC_MSR_DATA_READ);
			return checkStatus(FDC_MSR_DATA_READ,FDC_MSR_DATA_READ);
		}
		void discardData() {
			while(isRequiredToRead()) {
				inp8(p_data);
				getConsole() << ".";
			}
		}
		bool waitForIdle() {
			discardData();
			return _waitStatus(FDC_MSR_RQM|FDC_MSR_DATA_READ|FDC_MSR_CB,FDC_MSR_RQM);
		}
		bool sendCommand(byte* command, int length) {
			for(int i=0; i<length; ) {
				if(checkStatus(FDC_MSR_RQM|FDC_MSR_DATA_READ,FDC_MSR_RQM)) {
					outp8(p_data, command[i]);
					++i;
				} else if(isRequiredToRead()) {
					return false;
				}
			}
			return true;
		}
		byte getResult() {
			if(!isRequiredToRead()) {
				//panic("FDC::getResult was timeout");
				getConsole() << "{FDC-REPLY-TIMEOUT}";
				return 0xFF;
			} else {
				return inp8(p_data);
			}
		}
	public:
		bool DriveMotor(byte drvno, bool on) {
			drvno = 0x10<<drvno;
			if(on) {
				dor |= drvno;
			} else {
				dor &= ~drvno;
			}
			outp8(p_dor, dor);
			return true;
		}
		bool Recalibrate(byte drvno) {
			Console& console = getConsole();
			console << "Recalibrate";
			selectDrive(drvno);
			byte command[] = { 0x07, drvno };
			if(!waitForIdle()) {
				return false;
			} else {
				console << " and wait for";
				if(!sendCommand(command,sizeof(command))) {
					discardData();
					return false;
				} else {
					console << " interrupt...";
					while(!InterruptedFlag);
					console << "complete: ";
					return SenseInterrupt();
				}
			}
		}
		bool Seek(int drvno, int track) {
			selectDrive(drvno);
			byte command[] = { 0x0F, 0, track };
			InterruptedFlag = false;
			if(!waitForIdle()) {
				return false;
			} else if(!sendCommand(command, sizeof(command))) {
				discardData();
				return false;
			} else {
				while(!InterruptedFlag);
				return SenseInterrupt();
			}
		}
		bool SenseInterrupt() {
			byte command[] = { 0x08 };
			if(!waitForIdle()) {
				return false;
			} else if(!sendCommand(command, sizeof(command))) {
				discardData();
				return false;
			} else {
				ST0 st0;
				byte pcn;
				st0.value = getResult();	/* ST0 */
				pcn = getResult();			/* PCN */
				return st0.ic==00;
			}
		}
		ReadDataResult ReadData(void* buf, byte drvno, byte cylinder, byte head, byte sector, uint count) {
			if(count==0) {
				ReadDataResult result;
				result.st0.ic = 0x0;
				result.st1 = 0;
				result.st2 = 0;
				result.c = cylinder;
				result.h = head;
				result.r = sector;
				result.n = 2;
				return result;
			} else {
				selectDrive(drvno);
				byte command[] = { 0xE6
					, head<<2 | drvno
					, cylinder			// C
					, head				// H
					, sector+1			// R
					, 0x02				// N
					, sector+1+count-1	// EOT
					, 0x1B				// GSL
					, 0xFF				// DTL
				};
				InterruptedFlag = false;
				if(!waitForIdle()) {
					ReadDataResult result;
					result.st0.us = drvno;
					result.st0.ic = 0x1;
					return result;
				} else {
					// transfer
					DMA::BeginTransfer(2, DMA::IOMODE_IO_TO_MEM, buf, count*512);
					if(!sendCommand(command, sizeof(command))) {
						DMA::EndTransfer(2);
						discardData();
						ReadDataResult result;
						result.st0.us = drvno;
						result.st0.ic = 0x1;
						return result;
					} else {
						while(!InterruptedFlag) {
							Delay(10);
						}
						DMA::EndTransfer(2);
						// receive result data
						ReadDataResult result;
						result.st0.value = getResult();
						result.st1 = getResult();
						result.st2 = getResult();
						result.c = getResult();
						result.h = getResult();
						result.r = getResult();
						result.n = getResult();
						return result;
					}
				}
			}
		}
	public:
		FloppyDiskDrive& getDrive(byte drvno) {
			return drives[drvno];
		}
	};


	FloppyDiskDrive::FloppyDiskDrive(FloppyDiskController& _fdc, int drvno) : fdc(_fdc) {
		drive_no = drvno;
		current_cylinder = 0;
		current_head = 0;
		current_sector = 0;
	}

	FloppyDiskDrive::~FloppyDiskDrive() {
		//getConsole() << "~FloppyDiskDrive" << endl;
	}

	bool FloppyDiskDrive::Initialize() {
		fdc.DriveMotor(drive_no, true);
		return fdc.Recalibrate(drive_no);
	}

	void FloppyDiskDrive::Seek(uint32 block) {
		uint32 cylinder = block/FD_SECTORS_PER_CYLINDER;
		if(true || cylinder!=current_cylinder) {
			fdc.Seek(drive_no, cylinder);
			current_cylinder = cylinder;
		}
		int trackseq = block % FD_SECTORS_PER_CYLINDER;
		current_head	= trackseq/FD_SECTORS_PER_HEAD;
		current_sector	= trackseq%FD_SECTORS_PER_HEAD;
	}

	void FloppyDiskDrive::Read(void* buf, uint count) {
		ReadDataResult result = fdc.ReadData(buf, drive_no, current_cylinder, current_head, current_sector, count);
		current_cylinder	= result.c;
		current_head		= result.h;
		current_sector		= result.r;
	}


	static FloppyDiskController* fdc0 = NULL;
	static FloppyDiskController* fdc1 = NULL;

	extern bool Initialize() {
		Console& console = getConsole();
		PIC::RegisterInterruptHandler(6, FDCInterruptHandler);
		PIC::EnableInterrupt(6);
		fdc0 = new FloppyDiskController();
		if(!fdc0->InitializeAsMaster()) {
			console << "Failed to initialize FDC#0" << endl;
		} else {
			console << "Detecte FD Drives" << endl;
			fdc0->DetectDrives();
		}
		/*
		fdc1 = new FloppyDiskController();
		if(!fdc1->InitializeAsSlave()) {
			getConsole() << "Failed to initialize FDC#1" << endl;
		} else {
			fdc1.DetectDrives();
		}
		*/
		return true;
	}

	extern IMedia* GetMedia(int index) {
		return &fdc0->getDrive(index);
	}

}
