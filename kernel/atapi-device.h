#pragma once

#include "stdtype.h"
#include "storage.h"

#define ATA_TIMEOUT		1000000L


namespace Atapi {

	/* デバイス種別定義 */
	enum DeviceType {
		DEVICE_NON		= 0,		/* 未接続 */
		DEVICE_UNKOWN	= 0x8000,	/* 不明デバイス */
		DEVICE_ATA		= 0x4000,	/* ATAデバイス */
		DEVICE_ATAPI	= 0x2000,	/* ATAPIデバイス */
	};

	enum MediaType {
		DEVICE_HDD		= 1,		/* HDD */
		DEVICE_CDROM	= 2,		/* CD-ROMドライブ */
		DEVICE_MO		= 4,		/* 3.5インチMO */
		DEVICE_LS		= 8,		/* SuperDisk LS-120/240 */
		DEVICE_ZIP		= 0x10,		/* ZIP100/250 */
		DEVICE_CFA		= 0x20,		/* Compact Flash feature setサポート */
		MEDIA_UNKNOWN	= 0xFF,
	};

	enum DeviceFeature {
		DEVICE_REMOV	= 0x1000,	/* リムーバブルフラグ */
		DEVICE_PEJECT	= 0x0800,	/* パワーイジェクト機能あり */
		DEVICE_LOCK		= 0x0400,	/* ロック/アンロック機能あり */
		DEVICE_GETMeSt	= 0x0200,	/* GET MEDIA STATUSコマンドサポート */
	};

	enum MediaAttributes {
		/* メディア状態定義 */
		MEDIA_Ready			= 1,		/* アクセスレディ状態 */
		MEDIA_Wp			= 2,		/* メディアがライトプロテクト状態である */
		MEDIA_NoDisk		= 4,		/* メディアがない */
		MEDIA_Chg			= 8,		/* メディアがチェンジされた */
		MEDIA_ChgReq		= 16,		/* メディアチェンジが要求された */
		MEDIA_NotReady		= 64,		/* ノットレディ状態 */
		MEDIA_Error			= 128,		/* 何らかのエラー */
	};

	class Controller;

	class Device : public IMedia {
		friend class Controller;
		Controller& atapi;
		int devnum;
		DeviceType devtype;
		MediaType devmedia;
		uint devfeat;
		int pio_mode;
		int dma_mode;
		bool lbaok;
		uint16 identify_data[256];
		uint32 device_secter_size;
		uint32 lbamax;
	private:
		uint32 current_block;
	public:
		Device(Controller& ctrler, int devnum);
		~Device();
	public:
		DeviceType getDeviceType() const { return devtype; }
		MediaType getMediaType() const { return devmedia; }
		bool Select();
		MediaAttributes IDE_Media_AccessReady(bool writable);
		int IDE_Get_Media_Infomation();
	public:
		virtual uint32 getBlockCount() const {return lbamax;}
		virtual size_t getBlockSize() const {return device_secter_size;}
		virtual void Seek(uint32 block);
		virtual void Read(void* buf, uint count);
	private:
		//int ide_ata_pio_datain_cmd(const STRUCT_ATA_CMD* ata_cmd, uint16 count, void* buf);
		int IDE_identify_device(bool packetdevice);
		bool ide_initialize_device_check_sub(byte s1, byte s2);
		void ide_initialize_device_modcheck();
		int ide_initialize_device_mode_sub1(int PIO_mode, int DMA_mode);
		int ide_initialize_device_mode_sub2();
		int IDE_init_device_paramaters(byte head, byte sector);
		int ide_ata_device_select();
		MediaAttributes IDE_Get_Atapi_DeviceReady();
		MediaAttributes IDE_Get_MediaStatus_Immediate();
		int IDE_Read_Sector(uint32 lba, uint32 count, byte* buff);
		int IDE_idle_device();
		int IDE_set_features(byte subcom, byte mode);
		int IDE_get_media_status();
		int IDE_ata_read_sector(uint32 lba, uint16 count, void *buff);
		byte ide_get_sct_parameter(uint32 lba);
		uint16 ide_get_cyli_parameter(uint32 lba);
		byte ide_get_head_parameter(uint32 lba);
		int IDE_atapi_read10(uint32 lba, uint16 count, void *buff);
		int IDE_atapi_test_unit();
		int IDE_atapi_request_sense(void *buff);
		int IDE_atapi_read_capacity(void *buff);
		int IDE_atapi_read_toc(int lba_msf, byte format, byte setion, uint16 len, void *buff);
		int IDE_atapi_start_unit(byte data);
		int IDE_device_reset();
	};

}
