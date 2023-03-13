#include "atapi.h"
#include "stl.h"
#include "io.h"
#include "pic.h"
#include "kernel.h"
#include "memory.h"
#include "console.h"


/* デバイス選択 */
#define DEVICE_MAS		0	/* マスタ */
#define DEVICE_SLA		1	/* スレーブ */

enum MediaAccessModes {
	/* リードアクセス/ライトアクセス */
	MEDIA_ReadAccess	= 0,
	MEDIA_WriteAccess	= 1,
};

enum MediaControlModes {
	/* メディアロック/アンロック */
	MEDIA_Lock			= 1,
	MEDIA_UnLock		= 0,
};

/* CDプレーヤ制御用 */
struct STRUCT_TOC_POINT {
	byte dmy0; 		/* Reserved or Setion number */
	byte adr_ctl;		/* Sub-Q Address and Control bit */
	byte trk;			/* トラック番号 */
	byte dmy1; 		/* Reserved */
	byte dmy2; 		/* LBA[31:24] or Reserved */
	byte mm;			/* LBA[23:16] or MM */
	byte ss;			/* LBA[15:8]  or SS */
	byte ff;			/* LBA[7:0]   or FF */
};

struct STRUCT_TOC {
	uint16 length; 		/* TOCの長さ */
	byte first_trk;	/* 最初のトラック */
	byte last_trk;		/* 最終トラック */
	struct STRUCT_TOC_POINT point[99];
};

namespace Atapi {

	static Controller* controllers[] = {NULL,NULL};

	template < int index, int irq >
	static __declspec(naked) void __stdcall raw_atapi_packet_interrupt() {
		InterruptHandler_Prologue;
		controllers[index]->IDE_atapi_packet_interrupt();
		PIC::NotifyEndOfInterrupt(irq);
		InterruptHandler_Epilogue;
	}

	extern bool Initialize() {

		Console& console = getConsole();

		controllers[0] = new Controller(14,0x1F0,0x3F6);
		controllers[1] = new Controller(15,0x170,0x376);

		console << "Initialize ATAPI Controller#0" << endl;
		Controller& ata0 = *controllers[0];
		ata0.IDE_Initialize_Device();

		console << "Initialize ATAPI Controller#1" << endl;
		Controller& ata1 = *controllers[1];
		ata1.IDE_Initialize_Device();

		PIC::RegisterInterruptHandler(14, raw_atapi_packet_interrupt<0,14>);
		PIC::RegisterInterruptHandler(15, raw_atapi_packet_interrupt<1,15>);
		PIC::EnableInterrupt(14);
		PIC::EnableInterrupt(15);

		return true;

	}

	static Device* getDeviceByIndex(int index) {
		int ctrl;
		int dev;
		switch(index) {
		case 0:
			ctrl = 0;
			dev = 0;
			break;
		case 1:
			ctrl = 0;
			dev = 1;
			break;
		case 2:
			ctrl = 1;
			dev = 0;
			break;
		case 3:
			ctrl = 1;
			dev = 1;
			break;
		default:
			return NULL;
		}
		return &controllers[ctrl]->getDevice(dev);
	}

	extern IMedia* getDevice(int index) {
		Device* device = getDeviceByIndex(index);
		if(device==NULL) return NULL;
		if(device->IDE_Media_AccessReady(false)!=MEDIA_Ready) {
			getConsole() << "Not Ready" << endl;
			return NULL;
		} else if(0!=device->IDE_Get_Media_Infomation()) {
			getConsole() << "FAILED" << endl;
			return NULL;
		}
		return device;
	}

	extern DeviceType getDeviceType(int index) {
		Device* device = getDeviceByIndex(index);
		if(device==NULL) return DEVICE_NON;
		return device->getDeviceType();
	}

	extern MediaType getMediaType(int index) {
		Device* device = getDeviceByIndex(index);
		if(device==NULL) return MEDIA_UNKNOWN;
		return device->getMediaType();
	}

}
