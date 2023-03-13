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
		reg_dcr << 0x2;					/* ���荞�ݖ��g�p */
		reg_ftr << ata_cmd->feature;	/* �t�B�[�`���[ */
		reg_scr << ata_cmd->sec_cnt;	/* �Z�N�^�J�E���g */
		reg_snr << ata_cmd->sec_no;		/* �Z�N�^�i���o */
		reg_clr << ata_cmd->cyl_lo;		/* �V�����_Lo */
		reg_chr << ata_cmd->cyl_hi;		/* �V�����_Hi */
		if(ata_cmd->DRDY_Chk) {
			/* DRDY�r�b�g�E�F�C�g */
			if(!ide_wait_drdyset()) return -1;
		}
		reg_cmr << ata_cmd->command;	/* �R�}���h */
		Delay9(400);
		for(int l=0; l<ATA_TIMEOUT; l++) {
			StatusRegister str = *reg_str;
			if(!str.bsy) {
				if(str.err) {
					/* �G���[�I�� */
					return *reg_err;
				} else {
					/* �R�}���h������s�I���� */
					return 0;
				}
			}
		}
		return -2;		/* �^�C���A�E�g�G���[ */
	}

	int Controller::ide_ata_device_select(int dev_head) {
		for(int l=0; l<ATA_TIMEOUT; l++) {
			StatusRegister asr = *reg_asr;
			if(!asr.bsy && !asr.drq) {
				reg_dhr << dev_head;			/* �f�o�C�X�I�� */
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
		// �R�}���h���M
		reg_dcr << 0;					// ���荞�ݎg�p
		reg_ftr << atapi_cmd.feature;	// Overlapped I/O
		reg_scr << 0;
		reg_blr << (byte)(limit&0xff);	// �o�C�g�J�E���g low
		reg_bhr << (limit>>8);			// �o�C�g�J�E���g high
		reg_cmr << 0xA0;				// PACKET�R�}���h
		Delay(400);						// 400ns�E�F�C�g
		for(int l=0; l<ATA_TIMEOUT; l++) {
			StatusRegister asr = *reg_asr;
			if(!asr.bsy) {
				// �R�}���h���s�I��
				if(asr.err) {
					/* PACKET�R�}���h���s�G���[ */
					atapi_interrupt_buff = NULL;
					return -3;
				}
				InterruptReason irr = *reg_irr;
				if(asr.drq && !irr.io && irr.cd) {
					/* �p�P�b�g�R�}���h���s */
					atapi_interrupt_state = ST_IDLE;
					atapi_interrupt_limit = limit;
					atapi_interrupt_buff = (byte*)buf;
					//getConsole() << "{ATAPI:PACKET";
					ide_ata_write_data(&atapi_cmd.cmd_packet, 12);
					//getConsole() << "}";
					/* �p�P�b�g�R�}���h���s�I���҂� */
					for(int l=0; l<ATA_TIMEOUT; l++) {
						/* �X�e�[�^�X���[�h */
						asr = *reg_asr;
						if(!asr.bsy && atapi_interrupt_state==ST_STATUS) {
							break;
						}
					}
					/* �p�P�b�g�R�}���h���s�I�� */
					if(asr.err) {
						/* �p�P�b�g�R�}���h���s�G���[ */
						atapi_interrupt_state = ST_IDLE;
						return 0x1000|*reg_err;
					} else if(atapi_interrupt_state==ST_STATUS) {
						/* �p�P�b�g�R�}���h����I�� */
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
		reg_dcr << 0x2;					/* ���荞�ݖ��g�p */
		reg_ftr << ata_cmd->feature;	/* �t�B�[�`���[ */
		reg_scr << ata_cmd->sec_cnt;	/* �Z�N�^�J�E���g */
		reg_snr << ata_cmd->sec_no;		/* �Z�N�^�i���o */
		reg_clr << ata_cmd->cyl_lo;		/* �V�����_Lo */
		reg_chr << ata_cmd->cyl_hi;		/* �V�����_Hi */
		if(ata_cmd->DRDY_Chk) {
			/* DRDY�r�b�g�E�F�C�g */
			if(!ide_wait_drdyset()) return -1;
		}
		reg_cmr << ata_cmd->command;	/* �R�}���h */
		Delay(400);
		byte* p = (byte*)buf;
		for(int i=0; i<count; i++) {
			/* �ǂݏo���u���b�N�����[�v */
			for(int l=0; l<=ATA_TIMEOUT; l++) {
				if(l==ATA_TIMEOUT) return -2;
				if(!(*reg_str).bsy) break;	/* �R�}���h���s�I�� */
			}
			StatusRegister str = *reg_str;
			if(str.err) {
				/* �R�}���h���s�G���[ */
				return 0x1000|*reg_err;
			} else if(!str.drq) {
				/* �Ȃ����f�[�^���p�ӂ���Ă��Ȃ� */
				/* �R�}���h�����s�G���[ */
				return -3;
			}
			/* �f�[�^���p�ӂ���Ă��� */
			/* �u���b�N�ǂݏo��(256���[�h) */
			ide_ata_read_data(p, 512);
			p += 512;
		}
		return 0;
	}

	/* �\�t�g�E�F�A���Z�b�g */
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
			/* ���ڑ��Ɣ��� */
			return 0x7F;
		} else if(!ide_wait_bsyclr()) {
			/* �f�o�C�X���ڑ��Ɣ��� */
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

		/* �\�t�g�E�F�A���Z�b�g���s */
		//console << "Resetting...";
		ide_ata_reset();
		//console << "OK" << endl;
		
		/* �f�o�C�X�V�O�l�`���擾 */
		byte sig0 = getDeviceSignature(0);
		byte s0l = *reg_clr;
		byte s0h = *reg_chr;
		byte sig1 = getDeviceSignature(1);
		byte s1l = *reg_clr;
		byte s1h = *reg_chr;

		/* �f�o�C�X�m�� */
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

		/* �f�o�C�X�m�� */
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
			/* �f�o�C�X0��ATA�܂���ATAPI�f�o�C�X���ڑ�����Ă��� */
			/* �f�o�C�X0��1�̗����ɐڑ�����Ă���ꍇ�̓f�t�H���g���f�o�C�X0 */
			active_device = 0;
		} else if(dev1.devtype==DEVICE_ATAPI) {
			/* �f�o�C�X1��ATA�܂���ATAPI�f�o�C�X���ڑ�����Ă��� */
			active_device = 1;
		} else if(dev0.devtype==DEVICE_ATA) {
			/* �f�o�C�X0��ATA�܂���ATAPI�f�o�C�X���ڑ�����Ă��� */
			/* �f�o�C�X0��1�̗����ɐڑ�����Ă���ꍇ�̓f�t�H���g���f�o�C�X0 */
			active_device = 0;
		} else if(dev1.devtype==DEVICE_ATA) {
			/* �f�o�C�X1��ATA�܂���ATAPI�f�o�C�X���ڑ�����Ă��� */
			active_device = 1;
		} else {
			/* �ǂ�������ڑ��Ȃ�h���C�u0�ɂ��Ă��� */
			active_device = 0;
			return;
		}

		/* �f�t�H���g�̃h���C�u�I�� */
		reg_dhr << (active_device<<4);
		Delay9(400);
		ide_wait_bsyclr();

		/* �f�o�C�X�ڑ��`�F�b�N�̂��ߑ��݂��Ȃ��h���C�u�ɑ΂���IDENTIFY DEVICE�R�}���h�𔭍s����� */
		/* �f�o�C�X�I����߂����Ƃ��ɃR�}���h�����s����Ă���!?�f�o�C�X������������ */
		if((*reg_asr).drq) {
			/* DRQ�r�b�g�������Ă�����Ƃ肠�����f�o�C�X���Z�b�g */
			//devices[active_device]->IDE_device_reset();
			panic("Unexpected DRQ Assertion");
		}

	}

	int Controller::IDE_Initialize_Device() {
		/* �f�o�C�X�ڑ��`�F�b�N */
		ide_initialize_device_check();
		/* �f�o�C�X���[�h���� */
		dev0.ide_initialize_device_modcheck();
		dev1.ide_initialize_device_modcheck();
		/* ���[�h�̐ݒ� */
		PIO_mode = std::min(dev0.pio_mode, dev1.pio_mode);
		DMA_mode = std::min(dev0.dma_mode, dev1.dma_mode);
		getConsole() << "Controller Modes: PIO=" << PIO_mode << ", DMA=" << DMA_mode << endl;
		/* �f�o�C�X0���[�h������ & IDENTIFY�R�}���h�Ď��s */
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
		/* �f�o�C�X1���[�h������ & IDENTIFY�R�}���h�Ď��s */
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
		/* ATAPI�œ]�������o�C�g�� */
		return atapi_datatransfer;
	}

	void Controller::IDE_atapi_packet_interrupt() {
		InterruptReason irr = *reg_irr;
		StatusRegister str = *reg_str;
		if(str.bsy) {
			getConsole() << "{ATAPI:INT:BSY}";
		} else if(!str.drq) {
			if(!irr.io && !irr.cd) {
				/* �o�X�����[�X */
				/* �I�[�o�[���b�v�R�}���h���g�p */
				atapi_interrupt_state = ST_RELEASE;
			} else if(irr.io && irr.cd) {
				if(!str.drdy) {
					getConsole() << "{ATAPI:INT:C}";
				} else {
					/* �R�}���h���s�I�� */
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
			// �f�[�^�]���v��
			if(irr.cd) {
				if(!irr.io) {
					/* �R�}���h�p�P�b�g��M�ҋ@��� */
					atapi_interrupt_state = ST_COMMAND;
					/* �p�P�b�g�R�}���h�͔񊄂荞�݃��[�`�����Ń|�[�����O�ɂ��]�� */
				} else {
					getConsole() << "{ATAPI:INT:MESSAGE}";
					atapi_interrupt_state = ST_MESSAGE;
					atapi_datatransfer = (*reg_bhr<<8)|*reg_blr;
					ide_ata_read_data(NULL, atapi_datatransfer+1);
				}
			} else {
				if(!irr.io) {
					/* �z�X�g���f�o�C�X�f�[�^�]�� */
					atapi_datatransfer = (*reg_bhr<<8)|*reg_blr;
					atapi_interrupt_state = ST_DATAFROMHOST;
					if(atapi_datatransfer>atapi_interrupt_limit) {
						/* �]���\��o�C�g�����]���o�C�g���������ꍇ */
						getConsole() << "{ATAPI:INT:H2D:SIZE}";
						/* Data���W�X�^�������� */
						ide_ata_write_data(atapi_interrupt_buff, (atapi_interrupt_limit+1)/2);
						/* ������Ȃ��f�[�^�́A�Ƃ肠�����d���Ȃ��̂�00h�Ŗ��߂�c */
						ide_ata_write_data(NULL, ((atapi_datatransfer-atapi_interrupt_limit)+1)/2);
					} else {
						/* Data���W�X�^�������� */
						ide_ata_write_data(atapi_interrupt_buff, (atapi_datatransfer+1)/2);
					}
				} else {
					/* �f�o�C�X���z�X�g�f�[�^�]�� */
					uint16 datasize = ((uint16)*reg_bhr<<8)|*reg_blr;
					atapi_interrupt_state = ST_DATATOHOST;
					if(atapi_datatransfer+datasize>atapi_interrupt_limit) {
						getConsole() << "{ATAPI:INT:D2H:SIZE|" << atapi_datatransfer << "," << atapi_interrupt_limit << "}";
						/* �]���\��o�C�g�����]���o�C�g���������ꍇ */
						if(atapi_interrupt_limit>atapi_datatransfer) {
							uint16 rest = atapi_interrupt_limit-(uint16)atapi_datatransfer;
							ide_ata_read_data(atapi_interrupt_buff, rest);
							atapi_interrupt_buff += rest;
							atapi_datatransfer += rest;
							datasize -= rest;
						}
						/* �f�[�^�͋�ǂ� */
						ide_ata_read_data(NULL, datasize);
						atapi_datatransfer += datasize;
					} else {
						//getConsole() << "{ATAPI:INT:*|" << datasize << "}";
						/* Data���W�X�^�ǂݏo�� */
						ide_ata_read_data(atapi_interrupt_buff, datasize);
						atapi_interrupt_buff += datasize;
						atapi_datatransfer += datasize;
					}
				}
			}
		}
	}

}
