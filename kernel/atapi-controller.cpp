#include "stl.h"
#include "io.h"
#include "pic.h"
#include "kernel.h"
#include "console.h"
#include "atapi-controller.h"


namespace Atapi {

	Controller::Controller(int irq, int reg, int ctl) :
	reg_dtr(reg+0),
	reg_err(reg+1),
	reg_ftr(reg+1),
	reg_scr(reg+2),
	reg_irr(reg+2),
	reg_snr(reg+3),
	reg_clr(reg+4),
	reg_blr(reg+4),
	reg_chr(reg+5),
	reg_bhr(reg+5),
	reg_dhr(reg+6),
	reg_str(reg+7),
	reg_cmr(reg+7),
	reg_asr(ctl+0),
	reg_dcr(ctl+0),
	dev0(*this, 0),
	dev1(*this, 1)
	{
		this->irq = irq;
		PIO_mode = 0;
		DMA_mode = -1;
		atapi_interrupt_buff = NULL;
		atapi_interrupt_limit = 0;
		if(sizeof(InterruptReason)!=1) panic("Wrong size of InterruptReason");
	}

	int Controller::ide_ata_non_data_cmd(STRUCT_ATA_CMD *ata_cmd) {
		if(!SelectDevice(ata_cmd->dev_hed.dev)) return -1;
		reg_dcr << 0x2;					/* 割り込み未使用 */
		reg_ftr << ata_cmd->feature;	/* フィーチャー */
		reg_scr << ata_cmd->sec_cnt;	/* セクタカウント */
		reg_snr << ata_cmd->sec_no;		/* セクタナンバ */
		reg_clr << ata_cmd->cyl_lo;		/* シリンダLo */
		reg_chr << ata_cmd->cyl_hi;		/* シリンダHi */
		if(ata_cmd->DRDY_Chk) {
			/* DRDYビットウェイト */
			if(!ide_wait_drdyset()) return -1;
		}
		reg_cmr << ata_cmd->command;	/* コマンド */
		Delay9(400);
		for(int l=0; l<ATA_TIMEOUT; l++) {
			StatusRegister str = *reg_str;
			if(!str.bsy) {
				if(str.err) {
					/* エラー終了 */
					return *reg_err;
				} else {
					/* コマンド正常実行終了時 */
					return 0;
				}
			}
		}
		return -2;		/* タイムアウトエラー */
	}

	int Controller::ide_ata_device_select(int dev_head) {
		for(int l=0; l<ATA_TIMEOUT; l++) {
			StatusRegister asr = *reg_asr;
			if(!asr.bsy && !asr.drq) {
				reg_dhr << dev_head;			/* デバイス選択 */
				Delay(400);
				for(int l=0; l<ATA_TIMEOUT; l++) {
					asr = *reg_asr;
					if(!asr.bsy && !asr.drq) {
						return 0;
					}
				}
				getConsole() << "{ATAPI:DEVSEL:INCMP}";
				return 2;
			}
		}
		getConsole() << "{ATAPI:DEVSEL:BUSY}";
		return 1;
	}

	bool Controller::ide_wait_drdyset() {
		for(int l=0; l<ATA_TIMEOUT; l++) {
			if((*reg_asr).drdy) {
				return true;
			}
		}
		//getConsole() << "{ATAPI:DRDYSET}";
		return false;
	}

	bool Controller::ide_wait_bsyclr() {
		for(int l=0; l<ATA_TIMEOUT; l++){
			if(!(*reg_asr).bsy) {
				return true;
			}
		}
		getConsole() << "{ATAPI:BSYCLR}";
		return false;
	}

	int Controller::ide_atapi_packet_cmd(const STRUCT_ATAPI_CMD& atapi_cmd, uint16 limit, void* buf) {
		// コマンド送信
		reg_dcr << 0;					// 割り込み使用
		reg_ftr << atapi_cmd.feature;	// Overlapped I/O
		reg_scr << 0;
		reg_blr << (byte)(limit&0xff);	// バイトカウント low
		reg_bhr << (limit>>8);			// バイトカウント high
		reg_cmr << 0xA0;				// PACKETコマンド
		Delay(400);						// 400nsウェイト
		for(int l=0; l<ATA_TIMEOUT; l++) {
			StatusRegister asr = *reg_asr;
			if(!asr.bsy) {
				// コマンド実行終了
				if(asr.err) {
					/* PACKETコマンド実行エラー */
					atapi_interrupt_buff = NULL;
					return -3;
				}
				InterruptReason irr = *reg_irr;
				if(asr.drq && !irr.io && irr.cd) {
					/* パケットコマンド実行 */
					atapi_interrupt_state = ST_IDLE;
					atapi_interrupt_limit = limit;
					atapi_interrupt_buff = (byte*)buf;
					//getConsole() << "{ATAPI:PACKET";
					ide_ata_write_data(&atapi_cmd.cmd_packet, 12);
					//getConsole() << "}";
					/* パケットコマンド実行終了待ち */
					for(int l=0; l<ATA_TIMEOUT; l++) {
						/* ステータスリード */
						asr = *reg_asr;
						if(!asr.bsy && atapi_interrupt_state==ST_STATUS) {
							break;
						}
					}
					/* パケットコマンド実行終了 */
					if(asr.err) {
						/* パケットコマンド実行エラー */
						atapi_interrupt_state = ST_IDLE;
						return 0x1000|*reg_err;
					} else if(atapi_interrupt_state==ST_STATUS) {
						/* パケットコマンド正常終了 */
						atapi_interrupt_buff = NULL;
						atapi_interrupt_state = ST_IDLE;
						return 0;
					} else {
						panic("Controller::ide_atapi_packet_cmd("+itos<char,10>((int)atapi_interrupt_state)+")");
						atapi_interrupt_state = ST_IDLE;
						return -4;
					}
				} else {
					panic("break");
				}
			}
		}
		atapi_interrupt_buff = NULL;
		return -2;
	}

	int Controller::ide_ata_pio_datain_cmd(const STRUCT_ATA_CMD *ata_cmd, uint16 count, void* buf) {
		reg_dcr << 0x2;					/* 割り込み未使用 */
		reg_ftr << ata_cmd->feature;	/* フィーチャー */
		reg_scr << ata_cmd->sec_cnt;	/* セクタカウント */
		reg_snr << ata_cmd->sec_no;		/* セクタナンバ */
		reg_clr << ata_cmd->cyl_lo;		/* シリンダLo */
		reg_chr << ata_cmd->cyl_hi;		/* シリンダHi */
		if(ata_cmd->DRDY_Chk) {
			/* DRDYビットウェイト */
			if(!ide_wait_drdyset()) return -1;
		}
		reg_cmr << ata_cmd->command;	/* コマンド */
		Delay(400);
		byte* p = (byte*)buf;
		for(int i=0; i<count; i++) {
			/* 読み出しブロック数ループ */
			for(int l=0; l<=ATA_TIMEOUT; l++) {
				if(l==ATA_TIMEOUT) return -2;
				if(!(*reg_str).bsy) break;	/* コマンド実行終了 */
			}
			StatusRegister str = *reg_str;
			if(str.err) {
				/* コマンド実行エラー */
				return 0x1000|*reg_err;
			} else if(!str.drq) {
				/* なぜかデータが用意されていない */
				/* コマンド未実行エラー */
				return -3;
			}
			/* データが用意されている */
			/* ブロック読み出し(256ワード) */
			ide_ata_read_data(p, 512);
			p += 512;
		}
		return 0;
	}

	/* ソフトウェアリセット */
	void Controller::ide_ata_reset() {
		reg_dcr << 0x6;
		Delay6(10);			// > 5us
		reg_dcr << 0x2;		// the host shall clear SRST in the Device Control register to zero.
		Delay3(5);			// > 2ms
		if(!ide_wait_bsyclr()) {
			getConsole() << "{ATAPI:RESET}";
		}
	}

	byte Controller::getDeviceSignature(int devnum) {
		DeviceHeadRegister dhr;
		dhr.dev = devnum;
		reg_dhr << dhr;
		Delay(400);
		if(0xFF==reg_str.read() || 0xFF==reg_str.read()) {
			/* 未接続と判定 */
			return 0x7F;
		} else if(!ide_wait_bsyclr()) {
			/* デバイス未接続と判定 */
			return 0x7F;
		} else {
			byte err = *reg_err;
			if(err==1) {
				if((*reg_dhr).dev==dhr.dev) {
					return 1;
				}
				return 0x7F;
			} else {
				return err;
			}
		}
	}

	void Controller::ide_initialize_device_check() {

		Console& console = getConsole();

		/* ソフトウェアリセット実行 */
		//console << "Resetting...";
		ide_ata_reset();
		//console << "OK" << endl;
		
		/* デバイスシグネチャ取得 */
		byte sig0 = getDeviceSignature(0);
		byte s0l = *reg_clr;
		byte s0h = *reg_chr;
		byte sig1 = getDeviceSignature(1);
		byte s1l = *reg_clr;
		byte s1h = *reg_chr;

		/* デバイス確定 */
		console << "Device#0 ";
		if(1==(sig0&0x7F)) {
			reg_dhr << (0<<4);
			Delay9(400);
			if(!dev0.ide_initialize_device_check_sub(s0l,s0h)) {
				console << "error, ";
			} else {
				console << "passed, ";
			}
		} else {
			console << "failed, ";
		}

		/* デバイス確定 */
		console << "Device#1 ";
		if(0x80<=sig0 && sig0<=0xFF) {
			console << "failed.";
		} else if(1==(sig1&0x7F)) {
			reg_dhr << (1<<4);
			Delay9(400);
			if(!dev1.ide_initialize_device_check_sub(s1l,s1h)) {
				console << "error.";
			} else {
				console << "passed.";
			}
		} else {
			console << "failed.";
		}

		console << endl;

		if(dev0.devtype==DEVICE_ATAPI) {
			/* デバイス0にATAまたはATAPIデバイスが接続されている */
			/* デバイス0と1の両方に接続されている場合はデフォルトをデバイス0 */
			active_device = 0;
		} else if(dev1.devtype==DEVICE_ATAPI) {
			/* デバイス1にATAまたはATAPIデバイスが接続されている */
			active_device = 1;
		} else if(dev0.devtype==DEVICE_ATA) {
			/* デバイス0にATAまたはATAPIデバイスが接続されている */
			/* デバイス0と1の両方に接続されている場合はデフォルトをデバイス0 */
			active_device = 0;
		} else if(dev1.devtype==DEVICE_ATA) {
			/* デバイス1にATAまたはATAPIデバイスが接続されている */
			active_device = 1;
		} else {
			/* どちらも未接続ならドライブ0にしておく */
			active_device = 0;
			return;
		}

		/* デフォルトのドライブ選択 */
		reg_dhr << (active_device<<4);
		Delay9(400);
		ide_wait_bsyclr();

		/* デバイス接続チェックのため存在しないドライブに対してIDENTIFY DEVICEコマンドを発行すると */
		/* デバイス選択を戻したときにコマンドが実行されている!?デバイスがあったため */
		if((*reg_asr).drq) {
			/* DRQビットが立っていたらとりあえずデバイスリセット */
			//devices[active_device]->IDE_device_reset();
			panic("Unexpected DRQ Assertion");
		}

	}

	int Controller::IDE_Initialize_Device() {
		/* デバイス接続チェック */
		ide_initialize_device_check();
		/* デバイスモード判定 */
		dev0.ide_initialize_device_modcheck();
		dev1.ide_initialize_device_modcheck();
		/* モードの設定 */
		PIO_mode = std::min(dev0.pio_mode, dev1.pio_mode);
		DMA_mode = std::min(dev0.dma_mode, dev1.dma_mode);
		getConsole() << "Controller Modes: PIO=" << PIO_mode << ", DMA=" << DMA_mode << endl;
		/* デバイス0モード初期化 & IDENTIFYコマンド再実行 */
		getConsole() << "Identify Dev#0...";
		switch(dev0.ide_initialize_device_mode_sub1(PIO_mode,DMA_mode)) {
		case 0:
			getConsole() << "OK" << endl;
			break;
		case 0xFFFF:
			getConsole() << "PASSED" << endl;
			break;
		default:
			getConsole() << "FAILED" << endl;
			return -1;
		}
		/* デバイス1モード初期化 & IDENTIFYコマンド再実行 */
		getConsole() << "Identify Dev#1...";
		switch(dev1.ide_initialize_device_mode_sub1(PIO_mode,DMA_mode)) {
		case 0:
			getConsole() << "OK" << endl;
			break;
		case 0xFFFF:
			getConsole() << "PASSED" << endl;
			break;
		default:
			getConsole() << "FAILED" << endl;
			return -150;
		}
		return 0;
	}

	uint32 Controller::IDE_Get_Atapi_TransferByte() {
		/* ATAPIで転送したバイト数 */
		return atapi_datatransfer;
	}

	void Controller::IDE_atapi_packet_interrupt() {
		InterruptReason irr = *reg_irr;
		StatusRegister str = *reg_str;
		if(str.bsy) {
			getConsole() << "{ATAPI:INT:BSY}";
		} else if(!str.drq) {
			if(!irr.io && !irr.cd) {
				/* バスリリース */
				/* オーバーラップコマンド未使用 */
				atapi_interrupt_state = ST_RELEASE;
			} else if(irr.io && irr.cd) {
				if(!str.drdy) {
					getConsole() << "{ATAPI:INT:C}";
				} else {
					/* コマンド実行終了 */
					atapi_interrupt_state = ST_STATUS;
				}
			} else if(!irr.cd && irr.io && str.drdy && !str.err) {
				if(str.bit5) {
					getConsole() << "{ATAPI:INT:FAULT?}";
				} else {
					getConsole() << "{ATAPI:INT:BREAK?}";
				}
			} else {
				getConsole() << "*********************************************************" << endl;
				getConsole() << "IRR=" << (byte&)irr << ",CD=" << irr.cd << ",IO=" << irr.io << endl;
				getConsole() << "STR=" << (byte&)str << ",DRQ=" << str.drq << ",DRDY=" << str.drdy << ",ERR=" << str.err << endl;
				getConsole() << "ILLEGAL ATAPI PACKET INTERRUPT" << endl;
				getConsole() << "*********************************************************" << endl;
				//panic("ILLEGAL ATAPI PACKET INTERRUPT");
			}
		} else {
			// データ転送要求
			if(irr.cd) {
				if(!irr.io) {
					/* コマンドパケット受信待機状態 */
					atapi_interrupt_state = ST_COMMAND;
					/* パケットコマンドは非割り込みルーチン内でポーリングにより転送 */
				} else {
					getConsole() << "{ATAPI:INT:MESSAGE}";
					atapi_interrupt_state = ST_MESSAGE;
					atapi_datatransfer = (*reg_bhr<<8)|*reg_blr;
					ide_ata_read_data(NULL, atapi_datatransfer+1);
				}
			} else {
				if(!irr.io) {
					/* ホスト→デバイスデータ転送 */
					atapi_datatransfer = (*reg_bhr<<8)|*reg_blr;
					atapi_interrupt_state = ST_DATAFROMHOST;
					if(atapi_datatransfer>atapi_interrupt_limit) {
						/* 転送予定バイト数より転送バイト数が多い場合 */
						getConsole() << "{ATAPI:INT:H2D:SIZE}";
						/* Dataレジスタ書き込み */
						ide_ata_write_data(atapi_interrupt_buff, (atapi_interrupt_limit+1)/2);
						/* ↑足りないデータは、とりあえず仕方ないので00hで埋める… */
						ide_ata_write_data(NULL, ((atapi_datatransfer-atapi_interrupt_limit)+1)/2);
					} else {
						/* Dataレジスタ書き込み */
						ide_ata_write_data(atapi_interrupt_buff, (atapi_datatransfer+1)/2);
					}
				} else {
					/* デバイス→ホストデータ転送 */
					uint16 datasize = ((uint16)*reg_bhr<<8)|*reg_blr;
					atapi_interrupt_state = ST_DATATOHOST;
					if(atapi_datatransfer+datasize>atapi_interrupt_limit) {
						getConsole() << "{ATAPI:INT:D2H:SIZE|" << atapi_datatransfer << "," << atapi_interrupt_limit << "}";
						/* 転送予定バイト数より転送バイト数が多い場合 */
						if(atapi_interrupt_limit>atapi_datatransfer) {
							uint16 rest = atapi_interrupt_limit-(uint16)atapi_datatransfer;
							ide_ata_read_data(atapi_interrupt_buff, rest);
							atapi_interrupt_buff += rest;
							atapi_datatransfer += rest;
							datasize -= rest;
						}
						/* データは空読み */
						ide_ata_read_data(NULL, datasize);
						atapi_datatransfer += datasize;
					} else {
						//getConsole() << "{ATAPI:INT:*|" << datasize << "}";
						/* Dataレジスタ読み出し */
						ide_ata_read_data(atapi_interrupt_buff, datasize);
						atapi_interrupt_buff += datasize;
						atapi_datatransfer += datasize;
					}
				}
			}
		}
	}

}
