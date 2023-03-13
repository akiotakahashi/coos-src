#include "stl.h"
#include "io.h"
#include "pic.h"
#include "kernel.h"
#include "console.h"
#include "atapi-device.h"
#include "atapi-controller.h"

#define RETRY_MAX		8			/* リトライ回数 */


namespace Atapi {

	Device::Device(Controller& ctrler, int devnum) : atapi(ctrler) {
		this->devnum = devnum;
		devtype = DEVICE_UNKOWN;
		devmedia = DEVICE_HDD;
		devfeat = 0;
		pio_mode = 0;
		dma_mode = -1;
		lbaok = false;
		memclr(identify_data, sizeof(identify_data));
	}

	Device::~Device() {
		getConsole() << "~Device" << endl;
	}

	void Device::Seek(uint32 block) {
		current_block = block;
	}

	void Device::Read(void* buf, uint count) {
		if(!Select()) {
			getConsole() << "{ATAAPI:DEV:SELECT:FAILED}";
		} else if(0!=IDE_Read_Sector(current_block,count,(byte*)buf)) {
			getConsole() << "{ATAAPI:DEV:READ:FAILED}";
		} else {
			//getConsole() << "OK" << endl;
		}
	}

	int Device::IDE_idle_device() {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;	/* (フィーチャー) */
		ata_cmd.sec_cnt = 0;	/* (セクタカウント) */
		ata_cmd.sec_no = 0;		/* (セクタナンバ) */
		ata_cmd.cyl_lo = 0;		/* (シリンダLo) */
		ata_cmd.cyl_hi = 0;		/* (シリンダHi) */
		ata_cmd.dev_hed = devnum<<4;	/* デバイス */
		if(devtype==DEVICE_ATA) {
			/* ATAデバイス用 IDLEコマンド */
			ata_cmd.command = 0xe3;
		} else if(devtype==DEVICE_ATAPI) {
			/* ATAPIデバイス用 IDLEコマンド */
			ata_cmd.command = 0xe1;
		} else {
			panic("Device::IDE_idle_device");
		}
		ata_cmd.DRDY_Chk = 1;		/* DRDYビットチェック */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

	int Device::IDE_init_device_paramaters(byte head, byte sector) {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0xff;		/* (フィーチャー) */
		ata_cmd.sec_cnt = sector;	/* 最大セクタ数 */
		ata_cmd.sec_no = 0;			/* (セクタナンバ) */
		ata_cmd.cyl_lo = 0;			/* (シリンダLo) */
		ata_cmd.cyl_hi = 0;			/* (シリンダHi) */
		ata_cmd.dev_hed = (devnum<<4)|(head & 0xf);	/* デバイス/ヘッド数 */
		ata_cmd.command = 0x91;		/* INITALIZE DEVICE PARAMATERSコマンド */
		ata_cmd.DRDY_Chk = 0;		/* DRDYビットチェック不要 */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

/* SET FEATURESコマンド用サブコマンド */
#define SET_TRANSFER	0x03	/* Set Transfer Mode */
#define ENA_MEDIASTATUS	0x95	/* Enable Media Status Notification */
#define DIS_MEDIASTATUS	0x31	/* Disable Media Status Notification */

	int Device::ide_initialize_device_mode_sub1(int PIO_mode, int DMA_mode) {
		if(devtype!=DEVICE_ATA && devtype!=DEVICE_ATAPI) return 0xFFFF;
		pio_mode = PIO_mode;
		dma_mode = DMA_mode;
		/* デバイスnモード初期化 */
		if(ide_ata_device_select()!=0) return -1;
		switch(devtype) {
		default:
			panic("Unknown Device Type");
		case DEVICE_ATA:
			getConsole() << "ATA>InitDevParam>";
			if(0!=IDE_init_device_paramaters((byte)(identify_data[3]-1),(byte)identify_data[6])) {
				/* INITALIZE DEVICE PARAMATERSコマンドエラー */
				return -2;
			}
			getConsole() << "Idle>";
			if(0!=IDE_idle_device()) {
				/* IDLEコマンドエラー */
				return -3;
			}
			break;
		case DEVICE_ATAPI:
			getConsole() << "APAPI>PwrMgmtFeat>";
			/* ATAPIなら IDLE IMMEDIATEコマンドサポートが必須になっているはずだが */
			if(identify_data[82]&8) {	/* Power Management feature setサポート */
				if(0!=IDE_idle_device()) return -4;
			}
		}
		/* PIO転送モード設定 */
		if(identify_data[49]&0x800) {
			getConsole() << "PIO>";
			/* IORDYサポート */
			/* PIOフローコントロールモード&PIO転送モードを設定 */
			if(0!=IDE_set_features(SET_TRANSFER, 0x08|(pio_mode&7))) {
				/* フローコントロール付きPIOモード 設定エラー */
				/* PIOデフォルト転送モード(エラー無視) */
				IDE_set_features(SET_TRANSFER, 0);
				/* PIO転送モードリセット */
				atapi.PIO_mode = 0;
			}
		} else {
			/* PIOフローコントロール未サポート */
			getConsole() << "NOPIO>";
			/* PIOデフォルト転送モード(エラー無視) */
			// Disable following code because Bochs reports an error.
			//IDE_set_features(SET_TRANSFER, 0);
			/* PIO転送モードリセット */
			atapi.PIO_mode = 0;
		}
		/* DMA転送モード設定 */
		if(dma_mode > 0) {
			getConsole() << "DMA" << dma_mode << ">";
			/* DMA転送対応 */
			if(0!=IDE_set_features(SET_TRANSFER, 0x20|(dma_mode&7))) {
				/* マルチワードDMA転送設定エラー */
				getConsole() << "ERROR>";
				/* DMA転送モード未使用 */
				atapi.DMA_mode = -1;
			}
		} else {
			getConsole() << "NODMA>";
		}
		/* Removable Media Status Notification サポートチェック */
		if(((identify_data[83]&0x10)==0x10)
		|| ((identify_data[127]&3)==1))
		{
			getConsole() << "RMMD>";
			if(0==IDE_set_features(ENA_MEDIASTATUS, 0)) {
				/* コマンド正常終了 */
				byte c = *atapi.reg_chr;
				if(c&2) devfeat|=DEVICE_LOCK;		/* ロック/アンロック機能あり */
				if(c&4) devfeat|=DEVICE_PEJECT;		/* パワーイジェクト機能あり */
			}
		}
		return ide_initialize_device_mode_sub2();
	}

	int Device::ide_initialize_device_mode_sub2() {
		devmedia = (MediaType)0;
		devfeat = (DeviceFeature)0;
		/* デバイスタイプ確認 & IDENTIFYコマンド再実行 */
		switch(devtype) {
		default:
			return -1;
		case DEVICE_ATA:
			/* ATAデバイス */
			if(identify_data[61]==0 && identify_data[60]==0) {
				lbaok = false;	/* CHS方式対応 */
			} else {
				lbaok = true;	/* LBA方式対応 */
			}
			if(identify_data[0]&0x40) {
				/*  非リムーバブルデバイス */
				devmedia = DEVICE_HDD;
			} else {
				panic("Unknown ATA Device");
			}
			/* ZIPデバイスの判定を入れる場合 */
			/* identify_data[device]中の Model number などからデバイスを特定 */
			/*	device_type[device]|=DEVICE_ZIP; */
			getConsole() << "IDENT>";
			if(IDE_identify_device(false)!=0) return -1;
			break;
		case DEVICE_ATAPI:
			/* ATAPIデバイス */
			if(((identify_data[0]>>8)&0x1f)==5) {
				/* CD-ROM/DVD-ROM device */
				devmedia = DEVICE_CDROM;
			} else {
				panic("Unknown ATAPI Device");
			}
			/* MOやSuperDiskデバイスの判定を入れる場合 */
			/* identify_data[device]中の Model number などからデバイスを特定 */
			/*	device_type[device]=device_type[device]|DEVICE_MO; */
			/*	device_type[device]=device_type[device]|DEVICE_LS; */
			getConsole() << "IDENT>";
			if(IDE_identify_device(true)!=0) return -1;
			break;
		}
		getConsole() << "ALZ>";
		if(identify_data[0]&0x80) {
			/*  リムーバブルデバイス */
			devfeat |= DEVICE_REMOV;
		}
		/* GET MEDIA STATUSコマンド対応確認 */
		if((identify_data[82]!=0xffff)&&((identify_data[82]&0x04)==0x04) /* Removable Media feature set */
		|| (identify_data[83]!=0xffff)&&((identify_data[83]&0x10)==0x10) /* Removable Media Status Notification feature set */
		|| ((identify_data[127]&3)==1) /* Removable Media Status Notification feature set */
		)
		{
			/* GET MEDIA STATUSコマンドに対応しているかもしれないので、実際にコマンドを発行してみる */
			getConsole() << "MDSTS>";
			if((IDE_get_media_status()&4)==0) {
				/* アボードが発生しなければGET MEDIA STATUSコマンド対応 */
				devfeat |= DEVICE_GETMeSt;
			}
		}
		/* CFAデバイス確認 */
		if((identify_data[0]==0x848a)	/* CFAシグネチャ */
		|| (identify_data[83]!=0xffff)
		&&((identify_data[83]&0x02)==0x02)) /* CFA feature set */
		{
			devmedia = DEVICE_CFA;		/* CFAデバイスである */
		}
		return 0;
	}

	int Device::IDE_get_media_status() {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;	/* (フィーチャー) */
		ata_cmd.sec_cnt = 0;	/* (セクタカウント) */
		ata_cmd.sec_no = 0;		/* (セクタナンバ) */
		ata_cmd.cyl_lo = 0;		/* (シリンダLo) */
		ata_cmd.cyl_hi = 0;		/* (シリンダHi) */
		ata_cmd.dev_hed = devnum<<4;	/* デバイス */
		ata_cmd.command = 0xda;	/* GET MEDIA STATUSコマンド */
		ata_cmd.DRDY_Chk=1;		/* DRDYビットチェック */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

	byte Device::ide_get_sct_parameter(uint32 lba) {
		if(lbaok) {
			/* LBA方式対応デバイス */
			return (byte)lba&0xff;
		} else {
			/* LBA方式未対応デバイス */
			return (byte)(lba%identify_data[56])+1;
		}
	}

	uint16 Device::ide_get_cyli_parameter(uint32 lba) {
		if(lbaok) {
			/* LBA方式対応デバイス */
			return (uint16)(lba>>8)&0xffff;
		} else {
			/* LBA方式未対応デバイス */
			return (uint16)(lba/(identify_data[56]*identify_data[55]));
		}
	}

	byte Device::ide_get_head_parameter(uint32 lba) {
		if(lbaok) {
			/* LBA方式対応デバイス */
			return 0x40	/*LBA_flg*/ | (byte)(devnum<<4) | ((byte)(lba>>24)&0xf);
		} else {
			/* LBA方式未対応デバイス */
			return (byte)(devnum<<4) | (byte)((lba/(identify_data[56]))%identify_data[55]);
		}
	}

	int Device::IDE_ata_read_sector(uint32 lba, uint16 count, void *buff) {
		struct STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;			/* (フィーチャー) */
		ata_cmd.sec_cnt = (byte)count;	/* セクタ数 */
		ata_cmd.sec_no = ide_get_sct_parameter(lba);	/* セクタナンバ */
		uint16 l = ide_get_cyli_parameter(lba);
		ata_cmd.cyl_lo = l&0xff;		/* シリンダLo */
		ata_cmd.cyl_hi = (l>>8)&0xff;	/* シリンダHi */
		ata_cmd.dev_hed = ide_get_head_parameter(lba);	/* デバイス/ヘッド */
		ata_cmd.command = 0x20;			/* セクタリードコマンド */
		ata_cmd.DRDY_Chk = 1;			/* DRDYビットチェック */
		return atapi.ide_ata_pio_datain_cmd(&ata_cmd,count,buff);	/* count=セクタ(ブロック)数 */
	}

	int Device::IDE_atapi_read10(uint32 lba, uint16 count, void *buff) {
		STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature = 0;							/* Non-overlap/non DMA */
		atapi_cmd.dev_sel = devnum;						/* デバイス */
		atapi_cmd.cmd_packet[0] = 0x28;					/* READ(10) */
		atapi_cmd.cmd_packet[2] = (byte)(lba>>24)&0xff;	/* 論理セクタ */
		atapi_cmd.cmd_packet[3] = (byte)(lba>>16)&0xff;
		atapi_cmd.cmd_packet[4] = (byte)(lba>>8)&0xff;
		atapi_cmd.cmd_packet[5] = (byte)lba&0xff;
		atapi_cmd.cmd_packet[7] = (count>>8)&0xff;	/* 読み出しセクタ数 */
		atapi_cmd.cmd_packet[8] = count&0xff;
		atapi.atapi_datatransfer = 0;					/* 転送データバイト数クリア */
		if(count*device_secter_size>0xFFFF) panic("Device::IDE_atapi_read10");
		if(0!=atapi.ide_atapi_packet_cmd(atapi_cmd,(uint16)(count*device_secter_size), buff)) {
			/* パケットコマンドの実行前/中/後にエラー */
			return -1;
		} else {
			/* パケットコマンドの実行自体は正常終了 */
			if(atapi.atapi_datatransfer==0) {
				getConsole() << "{ATAPI:READ10:NODATA}";
			} else if(atapi.atapi_datatransfer%device_secter_size) {
				getConsole() << "{ATAPI:READ10:LESS|" << atapi.atapi_datatransfer << "}";
				return 0;	// 少ない分には許す
			} else if(atapi.atapi_datatransfer>count*device_secter_size) {
				getConsole() << "{ATAPI:READ10:OVER|" << atapi.atapi_datatransfer << "}";
			} else {
				return 0;
			}
			return -1;
		}
	}

	int Device::IDE_set_features(byte subcom, byte mode) {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = subcom;		/* SET FEATURES sub command */
		ata_cmd.sec_cnt = mode;			/* データ転送モード値ほか */
		ata_cmd.sec_no = 0;				/* (セクタナンバ) */
		ata_cmd.cyl_lo = 0;				/* (シリンダLo) */
		ata_cmd.cyl_hi = 0;				/* (シリンダHi) */
		ata_cmd.dev_hed.dev = devnum;	/* デバイス */
		ata_cmd.command = 0xEF;			/* SET FEATURESコマンド */
		ata_cmd.DRDY_Chk = 1;			/* DRDYビットチェック */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

	int Device::IDE_Read_Sector(uint32 lba, uint32 count, byte* buff) {
		uint maxcount;
		switch(devtype) {
		case DEVICE_ATA:
			maxcount = 0x7FFF;
			break;
		case DEVICE_ATAPI:
			maxcount = 0x7FFF/device_secter_size;
			break;
		default:
			panic("Device::IDE_read");
		}
		while(count>0) {
			uint iocount = (count>maxcount) ? maxcount : count;
			//getConsole() << "LBA:" << lba << " - " << lba+iocount-1 << endl;
			switch(devtype) {
			case DEVICE_ATA:
				if(0!=IDE_ata_read_sector(lba,iocount,buff)) {
					return -1;
				}
			case DEVICE_ATAPI:
				if(0!=IDE_atapi_read10(lba,iocount,buff)) {
					return -1;
				}
				iocount = atapi.IDE_Get_Atapi_TransferByte()/device_secter_size;
			}
			buff += iocount*device_secter_size;
			lba += iocount;
			count -= iocount;
		}
		return 0;
	}

	int Device::IDE_atapi_test_unit() {
		STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature = 0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel = devnum;		/* デバイス */
		//getConsole() << "{ATAPI:TESTUNIT:";
		int rc = atapi.ide_atapi_packet_cmd(atapi_cmd, 0, NULL);
		//getConsole() << rc << "}";
		return rc;
	}

	int Device::IDE_atapi_request_sense(void *buff) {
		STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature = 0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel = devnum;		/* デバイス */
		atapi_cmd.cmd_packet[0] = 3;	/* REQUEST SENSE */
		atapi_cmd.cmd_packet[4] = 18;	/* 18バイト */
		atapi.atapi_datatransfer = 0;	/* 転送データバイト数クリア */
		//getConsole() << "{ATAPI:RQSENCE:";
		int i = atapi.ide_atapi_packet_cmd(atapi_cmd,18,buff);
		//getConsole() << i << "}";
		if((i!=0)||(atapi.atapi_datatransfer!=18)) return i;
		return 0;
	}

	MediaAttributes Device::IDE_Get_Atapi_DeviceReady() {
		if(devtype==DEVICE_ATAPI) {
			byte sense_buffer[18];
			memset(sense_buffer, 0xFF, sizeof(sense_buffer));
			/* ATAPIデバイスの場合 */
			for(int j=0; j<RETRY_MAX; j++) {
				/* Request Senseが失敗したときはリトライ */
				IDE_atapi_test_unit();/* ATAPI TEST UNIT */
				if(0==IDE_atapi_request_sense(sense_buffer)) {
					return (MediaAttributes)(
						((sense_buffer[2]&0xf)<<24)
						|(sense_buffer[12]<<16)
						|(sense_buffer[13]<<8));
				}
				Delay3(5);
			}
			return (MediaAttributes)0xffffff00;
		} else {
			return (MediaAttributes)0xffffff00;
		}
	}

	MediaAttributes Device::IDE_Get_MediaStatus_Immediate() {
		if(0==(devfeat&DEVICE_REMOV)) return MEDIA_Ready;	/* 常時レディ状態 */
		/* リムーバブルデバイスの場合 */
		if(devmedia==DEVICE_CDROM) {
			/* CD-ROMデバイスの場合 */
			/* ビット27～8にSenseKeyやASC/ASCQの状態を格納 */
			//getConsole() << "{ATAPI:CDROM:";
			uint32 l = IDE_Get_Atapi_DeviceReady();
			//getConsole() << l << "}";
			if(l==0) {
				/* レディ状態 */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_Ready);
			} else if((l&0x0fff0000)==0x06280000) {
				/* ディスクが交換された */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_Chg);
			} else if((l&0x0f000000)==0x02000000) {
				/* Notレディ状態 */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_NotReady);
			} else {
				/* 何らかのエラー状態 */
				/* CD-ROMは読み出し専用なのでMEDIA_Wpを付加 */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_Error);
			}
		} else {
			/* CD-ROM以外のデバイスの場合 */
			if(devfeat&DEVICE_GETMeSt) {
				/* GET MEDIA STATUSコマンド対応の場合 */
				int err = IDE_get_media_status();			/* GET MEDIA STATUSコマンド */
				if(err==0) {
					/* 正常終了(アクセスレディ状態) */
					return MEDIA_Ready;
				} else {
					/* エラー状態 */
					uint flags = 0;
					if(err&BIT_WP) flags|=MEDIA_Wp;		/* メディアがライトプロテクト状態である */
					if(err&BIT_NM) flags|=MEDIA_NoDisk;	/* メディアがない */
					if(err&BIT_MC) flags|=MEDIA_Chg;		/* メディアがチェンジされた */
					if(err&BIT_MCR) {
						flags |= MEDIA_ChgReq;				/* メディアチェンジが要求された */
						//TODO: IDE_Media_Eject();				/* メディアイジェクト(エラー無視) */
					}
					if(err&4/*BIT_ABRT*/) flags|=MEDIA_Error;	/* 何らかのエラー */
					return (MediaAttributes)flags;
				}
			} else {
				/* GET MEDIA STATUSコマンド非対応の場合 */
				if(devtype==DEVICE_ATAPI) {
					/* ATAPIデバイスの場合 */
					/* ビット27～8にSenseKeyやASC/ASCQの状態を格納 */
					return IDE_Get_Atapi_DeviceReady();
				} else {
					if (devtype==DEVICE_CFA) {	/* CFAデバイスの場合 */
						/* CompactFlashなどPCカード系のATAデバイス(リムーバブルデバイスなのにGET MEDIA STATUSコマンド非対応) */
						/* の場合は、不意にカードを抜かれる状況が考えられるので、ここでカードの接続状態を確認したほうが良い */
						/* PCカードやCFの場合は、CD1#/CD2#(カードデテクト信号)を調べてカードが抜かれたかどうかを判定する */
						/* TrueIDEモードの場合は… プラットホーム依存(^^;) */
					}
					/* リムーバブルメディアのフラグが立っているのにGET MEDIA STATUSコマンド非対応の場合は */
					/* ATAPIデバイスならTEST UNITパケットコマンドなどでデバイスの状態を調べ */
					/* とりあえずレディ状態として返す */
					return MEDIA_Ready;		/* レディ状態 */
				}
			}
		}
	}

	int Device::IDE_atapi_start_unit(byte data) {
		getConsole() << "{ATAPI:STARTUNIT}";
		struct STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature=0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel=devnum;		/* デバイス */
		atapi_cmd.cmd_packet[0]=0x1b;	/* START UNIT */
		atapi_cmd.cmd_packet[4]=data;	/* 設定値 */
		return atapi.ide_atapi_packet_cmd(atapi_cmd,0,NULL);
	}

/* START/STOP UNIT(START)コマンドパラメータ */
#define ATAPI_START		1	/* ATAPIデバイススタート */
#define ATAPI_EJECT		2	/* イジェクト */
#define ATAPI_CLOSE		3	/* トレイクローズ */

	MediaAttributes Device::IDE_Media_AccessReady(bool writable) {
		MediaAttributes k = (MediaAttributes)(MEDIA_Ready | (writable ? MEDIA_Wp : 0));
		for(int j=0; j<RETRY_MAX; j++) {
			uint32 ms = IDE_Get_MediaStatus_Immediate();
			if(k==(ms&k)) return k;
			Delay3(5);
			if(devmedia==DEVICE_CDROM) {
				/* CD-ROMドライブの場合 */
				IDE_atapi_start_unit(ATAPI_START);
			}
		}
		return MEDIA_Error;
	}

	int Device::IDE_identify_device(bool packetdevice) {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;	/* (フィーチャー) */
		ata_cmd.sec_cnt = 0;	/* (セクタカウント) */
		ata_cmd.sec_no = 0;		/* (セクタナンバ) */
		ata_cmd.cyl_lo = 0;		/* (シリンダLo) */
		ata_cmd.cyl_hi = 0;		/* (シリンダHi) */
		ata_cmd.dev_hed = (devnum<<4);	/* デバイス */
		if(!packetdevice) {
			ata_cmd.DRDY_Chk = true;		/* DRDYビットチェック */
			ata_cmd.command = 0xEC;			/* IDENTIFY DEVICEコマンド */
		} else {
			ata_cmd.DRDY_Chk = false;		/* DRDYビットチェック不要 */
			ata_cmd.command = 0xA1;			/* IDENTIFY PACKET DEVICEコマンド */
		}
		return atapi.ide_ata_pio_datain_cmd(&ata_cmd,1,identify_data);	// 1ブロック(256ワード)
	}

	void Device::ide_initialize_device_modcheck() {
		if(devtype!=DEVICE_ATA && devtype!=DEVICE_ATAPI) {
			/* デバイス未接続 or 非ATA/ATAPIデバイス */
			/* 未接続や非ATA/ATAPIデバイスなら無視 */
			pio_mode = 255;
			dma_mode = 255;
		} else {
			if(0==(identify_data[53]&2)) {
				/* 対応モードが不明な場合 */
				pio_mode = 0;		/* PIOモード 0 */
				dma_mode = -1;		/* マルチワードDMA非対応 */
			} else {
				/* ワード53 ビット1=1 ワード64～70有効 */
				if(identify_data[64]&2) {
					pio_mode = 4;	/* PIOモード4 */
				} else if(identify_data[64]&1) {
					pio_mode = 3;	/* PIOモード3 */
				} else {
					pio_mode = 0;	/* 基本はPIOモード0 */
				}
				if(0==(identify_data[63]&7)) {
					/* マルチワードDMA非対応 */
					dma_mode = -1;
				} else {
					/* マルチワードDMA転送対応 */
					if(identify_data[63]&4) {
						dma_mode = 2;	/* モード2 */
					} else if(identify_data[63]&2) {
						dma_mode = 1;	/* モード1 */
					} else if(identify_data[63]&1) {
						dma_mode = 0;	/* モード0 */
					} else {
						dma_mode = -1;
					}
				}
			}
		}
	}

	bool Device::ide_initialize_device_check_sub(byte s1, byte s2) {
		getConsole() << "(" << s1 << "," << s2 << ") ";
		devtype = DEVICE_UNKOWN;
		bool packetdevice;
		if ((s1==0)&&(s2==0)) {
			/* デバイスnはATAデバイスの可能性あり */
			packetdevice = false;
		} else if ((s1==0x14)&&(s2==0xEB)) {
			/* デバイスnはATAPIデバイスの可能性あり */
			packetdevice = true;
		} else {
			return false;
		}
		if(!atapi.ide_wait_bsyclr()) {
			/* BSYビットがクリアされない */
			return false;
		} else {
			/* タイムアウト以内にBSYビットクリア */
			for(int l=0; l<RETRY_MAX;l++){
				Delay3(5); int i=IDE_identify_device(packetdevice);
				Delay3(5); int k=IDE_identify_device(packetdevice);
				if(k!=i) continue;
				/* 2回実行して同じ結果 */
				switch(i) {
				case 0:
					/* エラーなし&IDENTIFYデータ正常取得 */
					devtype = packetdevice ? DEVICE_ATAPI : DEVICE_ATA;
					return true;
				case 1:	/* コマンド未実行終了 */
				case 2: /* アボートエラー */
					/* デバイスnは未接続 */
					getConsole() << "{ABORT}";
					devtype = DEVICE_NON;
					return false;
				default:
					/* その他のエラーはリトライ */
					//getConsole() << "{FAILED}";
					devtype = DEVICE_NON;
					return false;
				}
			}
			/* デバイスnは不明デバイス */
			return false;
		}
	}


	int Device::IDE_device_reset() {
		STRUCT_ATA_CMD ata_cmd;
		memclr(&ata_cmd, sizeof(ata_cmd));
		ata_cmd.feature=0;		/* (フィーチャー) */
		ata_cmd.sec_cnt=0;		/* (セクタカウント) */
		ata_cmd.sec_no=0;		/* (セクタナンバ) */
		ata_cmd.cyl_lo=0;		/* (シリンダLo) */
		ata_cmd.cyl_hi=0;		/* (シリンダHi) */
		ata_cmd.dev_hed=(devnum<<4);	/* デバイス */
		ata_cmd.command=0x08;	/* DEVICE RESETコマンド */
		ata_cmd.DRDY_Chk=0;		/* DRDYビットチェック不要 */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

	int Device::ide_ata_device_select() {
		return atapi.ide_ata_device_select(devnum<<4);
	}

	bool Device::Select() {
		return 0==ide_ata_device_select();
	}

	int Device::IDE_atapi_read_capacity(void *buff) {
		STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature=0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel=devnum;		/* デバイス */
		atapi_cmd.cmd_packet[0]=0x25;	/* READ CAPACITY */
		atapi.atapi_datatransfer=0;			/* 転送データバイト数クリア */
		int i=atapi.ide_atapi_packet_cmd(atapi_cmd,8,buff);
		if ((i==0)&&(atapi.atapi_datatransfer==8)) return 0;	/* 正常終了時=0 */
		return i;						/* エラー終了時!=0 */
	}

	int Device::IDE_Get_Media_Infomation() {
		if(devtype==DEVICE_ATA) {
			/* 選択されたドライブはATAデバイス */
			/* ATAデバイスは1セクタ512バイト */
			device_secter_size = 512;
			if(devfeat & DEVICE_REMOV) {
				/* リムーバブルデバイス */
				for(int j=0; j<RETRY_MAX; j++){
					if(0==IDE_identify_device(false)) {
						lbamax = identify_data[61]<<16|identify_data[60];
						return 0;
					}
				}
				return -1;
			} else {
				/* 非リムーバブルデバイス */
				lbamax = identify_data[61]<<16|identify_data[60];
				return 0;
			}
		} else if (devtype==DEVICE_ATAPI) {
			/* 選択されたドライブはATAPIデバイス */
			for(int j=0; j<RETRY_MAX; j++){
				byte buffer[8];
				if(0==IDE_atapi_read_capacity(buffer)) {
					lbamax
						= (uint32)buffer[0]<<24
						| (uint32)buffer[1]<<16
						| (uint32)buffer[2]<<8
						| (uint32)buffer[3];
					device_secter_size
						= (uint32)buffer[4]<<24
						| (uint32)buffer[5]<<16
						| (uint32)buffer[6]<<8
						| (uint32)buffer[7];
					if(devmedia==DEVICE_CDROM && device_secter_size!=2048) {
						getConsole() << "{ATAPI:MEDIAINFO:SECSIZEERR|" << device_secter_size << "}";
						device_secter_size = 2048;
					}
					return 0;
				}
			}
			return -1;
		} else {
			return -1;
		}
	}

	int Device::IDE_atapi_read_toc(int lba_msf, byte format, byte setion, uint16 len, void *buff) {
		struct STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature=0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel=devnum;		/* デバイス */
		atapi_cmd.cmd_packet[0]=0x43;	/* READ TOC */
		atapi_cmd.cmd_packet[1]=(lba_msf&1)<<1;/* select LBA/MSF */
		atapi_cmd.cmd_packet[2]=format&7;/* format */
		atapi_cmd.cmd_packet[6]=setion;	/* track/setion */
		atapi_cmd.cmd_packet[7]=(len>>8)&0xff;	/* 読み出しバイト数 */
		atapi_cmd.cmd_packet[8]=len&0xff;
		atapi.atapi_datatransfer=0;			/* 転送データバイト数クリア */
		int i=atapi.ide_atapi_packet_cmd(atapi_cmd,len,buff);
		if ((i==0)&&(atapi.atapi_datatransfer==len)) return 0;	/* 正常終了時=0 */
		return i;						/* エラー終了時!=0 */
	}

}
