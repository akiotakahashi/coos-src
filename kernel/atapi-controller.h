#pragma once

#include "stdtype.h"
#include "io.h"
#include "atapi-device.h"


namespace Atapi {

	/* ATAエラーレジスタビット定義 */
	enum ATA_ERR_REG {
		BIT_NM		= 2,		/* メディアがない */
		BIT_MCR		= 8,		/* メディアチェンジが要求された */
		BIT_MC		= 0x20,		/* メディアがチェンジされた */
		BIT_WP		= 0x40,		/* メディアがライトプロテクト状態である */
	};

	/* ATAステータスレジスタ */
	struct StatusRegister {
		bool	err		: 1;
		byte	obs1	: 1;
		byte	obs2	: 1;
		bool	drq		: 1;
		byte	bit4	: 1;
		byte	bit5	: 1;
		bool	drdy	: 1;
		bool	bsy		: 1;
		StatusRegister() {
			*(byte*)this = 0;
		}
		StatusRegister(byte value) {
			*(byte*)this = value;
		}
	};

	struct ErrorRegister {
		bool	bit0	: 1;
		bool	bit1	: 1;
		bool	abrt	: 1;
		bool	bit3	: 1;
		bool	bit4	: 1;
		bool	bit5	: 1;
		bool	bit6	: 1;
		bool	bit7	: 1;
	};

	struct InterruptReason {
		bool	cd		: 1;
		bool	io		: 1;
		bool	rel		: 1;
		byte	tag		: 5;
		InterruptReason() {
			*(byte*)this = 0;
		}
		InterruptReason(byte value) {
			*(byte*)this = value;
		}
	};

	struct DeviceHeadRegister {
		byte	bit03	: 4;
		byte	dev		: 1;
		byte	obs5	: 1;
		byte	bit6	: 1;
		byte	obs7	: 1;
		DeviceHeadRegister() {
			*(byte*)this = 0;
		}
		DeviceHeadRegister(byte value) {
			*(byte*)this = value;
		}
	};

	/* ATAコマンド発行用 構造体 */
	struct STRUCT_ATA_CMD {
		byte feature;	/* フィ－チャーレジスタ */
		byte sec_cnt;	/* セクタカウントレジスタ */
		byte sec_no;	/* セクタナンバレジスタ */
		byte cyl_lo;	/* シリンダ下位レジスタ */
		byte cyl_hi;	/* シリンダ上位レジスタ */
		DeviceHeadRegister dev_hed;	/* デバイス/ヘッドレジスタ */
		byte command;	/* コマンドレジスタ */
		byte DRDY_Chk;	/* コマンドレジスタ */
	};
		
	/* ATAPIパケットコマンド発行用 構造体 */
	struct STRUCT_ATAPI_CMD {
		byte feature; 			/* overlap/DMAの設定 */
		byte dev_sel;			/* デバイスセレクト(0:デバイス0/1:デバイス1) */
		byte cmd_packet[12];	/* コマンドパケット */
	};

	/* パケットコマンドの処理を割り込みを使って行う場合 */
	enum InterruptState {
		ST_IDLE,
		ST_READY,
		ST_COMMAND,
		ST_MESSAGE,
		ST_DATATOHOST,
		ST_DATAFROMHOST,
		ST_STATUS,
		ST_RELEASE,
	};

	class Controller {
		friend class Device;
		int irq;
		IO::Port<uint16>					reg_dtr;
		IO::Port<byte>						reg_err;
		IO::Port<byte>						reg_ftr;
		IO::Port<byte>						reg_scr;
		IO::Port<byte,InterruptReason>		reg_irr;
		IO::Port<byte>						reg_snr;
		IO::Port<byte>						reg_clr;
		IO::Port<byte>						reg_blr;
		IO::Port<byte>						reg_chr;
		IO::Port<byte>						reg_bhr;
		IO::Port<byte,DeviceHeadRegister>	reg_dhr;
		IO::Port<byte,StatusRegister>		reg_str;
		IO::Port<byte>						reg_cmr;
		IO::Port<byte,StatusRegister>		reg_asr;
		IO::Port<byte>						reg_dcr;
		Device dev0;
		Device dev1;
		int PIO_mode;
		int DMA_mode;
		volatile InterruptState atapi_interrupt_state;		/* 割り込み処理ステート状態変数 */
		volatile uint32 atapi_datatransfer;		/* ATAPIで転送したバイト数 */
		byte* atapi_interrupt_buff;	/* 割り込み処理ルーチンで使用するバッファポインタ */
		uint16 atapi_interrupt_limit;	/* 割り込み処理ルーチンで使用する最大転送バイト数 */
	public:
		Controller(int irq, int reg, int ctl);
		void IDE_atapi_packet_interrupt();
	public:
		int IDE_Initialize_Device();
		Device& getDevice(int i) { return i==0 ? dev0 : dev1; }
	private:
		void ide_ata_reset();
		void ide_initialize_device_check();
	private:
		/*************************/
		/* 物理I/Oアクセスレベル */
		/*************************/
		byte getDeviceSignature(int devnum);
	private:
		/*******************/
		/* ATAコマンド発行 */
		/*******************/
		/* EXECUTE DEVICE DIAGNOSTICコマンド発行 */
		int IDE_execute_device_diagnostic(void);
		/* GET MEDIA STATUSコマンド発行 */
		int IDE_get_media_status(int);
		/* DOOR/MEDIA UNLOCKコマンド発行 */
		int IDE_media_unlock(int);
		/* DOOR/MEDIA LOCKコマンド発行 */
		int IDE_media_lock(int);
		/* MEDIA EJECTコマンド発行 */
		int IDE_media_eject(int);
	private:
		/*****************************/
		/* ATAPIパケットコマンド発行 */
		/*****************************/
		/* READ CAPACITYコマンドパケット発行 */
		int IDE_atapi_read_capacity(int, void *);
		/* INQUIRYコマンドパケット発行 */
		int IDE_atapi_inquiry(int, byte, void *);
		/* PREVENT ALLOW MEDIAUM REMOVALコマンドパケット発行 */
		int IDE_prevent_allow_mediaum_removal(int, int);
		/* READ TOCコマンドパケット発行 */
		int IDE_atapi_read_toc(int, int, byte, byte, uint16, void *);
		/* PLAY AUDIO MSFコマンドパケット発行 */
		int IDE_atapi_play_audio_msf(int, byte, byte, byte, byte, byte, byte);
		/* STOP PLAYBACKコマンドパケット発行 */
		int IDE_atapi_stop_playback(int);
		/* PAUSE/RESUMEコマンドパケット発行 */
		int IDE_atapi_pause_resume(int, int);
		/* READ SUBCHANNLコマンドパケット発行 */
		int IDE_atapi_read_subchannel(int, int, int, byte, byte, uint16, void *);
	private:
		/************************************/
		/* ATA/ATAPI デバイス初期化ルーチン */
		/************************************/
		/* デバイス詳細情報テーブル取得 */
		void IDE_Get_Identify_Infomation(int, uint16 *);
		/* 現在のPIO転送モードを取得 */
		int IDE_Get_PIO_Mode(void);
		/* 現在のDMA転送モードを取得 */
		int IDE_Get_DMA_Mode(void);
		/* メディア状態チェック(保存情報から返す) */
		uint32 IDE_Get_MediaStatus(int);
		/* メディアロック設定/アンロック設定 */
		int IDE_Media_LockUnlock(int, int);
		/* メディアイジェクト */
		int IDE_Media_Eject(int);
		/* メディアトレイクローズ */
		int IDE_Media_TrayClose(int);
		/* 直前のATAPIコマンドで転送されたバイト数取得 */
		uint32 IDE_Get_Atapi_TransferByte();
	private:
		void ide_ata_read_data(void* buf, int size) {
			size >>= 1;
			if(buf==NULL) {
				/* データの空読み */
				while(size-->0){
					*reg_dtr;
				}
			} else {
				/* データバッファへ書き込み */
				uint16* p = (uint16*)buf;
				while(size-->0){
					*(p++) = *reg_dtr;
				}
			}
		}
		void ide_ata_write_data(const void* buf, int size) {
			size >>= 1;
			if(buf==NULL) {
				/* ダミーデータ(00h)の書き込み */
				while(size-->0){
					reg_dtr << 0;
				}
			} else {		/* データバッファから書き込み */
				const uint16* p = (const uint16*)buf;
				while(size-->0){
					reg_dtr << *(p++);
				}
			}
		}
		int ide_ata_device_select(int dev_head);
		bool ide_wait_drdyset();
		bool ide_wait_bsyclr();
		int ide_ata_non_data_cmd(STRUCT_ATA_CMD *ata_cmd);
		int ide_ata_pio_datain_cmd(const STRUCT_ATA_CMD *ata_cmd, uint16 count, void* buf);
		int ide_atapi_packet_cmd(const STRUCT_ATAPI_CMD& atapi_cmd, uint16 limit, void* buf);
	private:
		int active_device;
		bool SelectDevice(int devnum) {
			if(active_device!=devnum) {
				// 現在選択されているドライブでなければ
				if(0!=ide_ata_device_select(devnum)) {
					active_device = -1;
					return false;
				} else {
					active_device = devnum;
					return true;
				}
			} else {
				// デバイスセレクションフェーズは行わずにDevice/Headレジスタへ書き込み
				DeviceHeadRegister dhr(0);
				dhr.dev = devnum;
				reg_dhr << dhr;
				active_device = devnum;
				return true;
			}
		}
	};

}
