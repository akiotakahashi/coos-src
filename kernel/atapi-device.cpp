#include "stl.h"
#include "io.h"
#include "pic.h"
#include "kernel.h"
#include "console.h"
#include "atapi-device.h"
#include "atapi-controller.h"

#define RETRY_MAX		8			/* ���g���C�� */


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
		ata_cmd.feature = 0;	/* (�t�B�[�`���[) */
		ata_cmd.sec_cnt = 0;	/* (�Z�N�^�J�E���g) */
		ata_cmd.sec_no = 0;		/* (�Z�N�^�i���o) */
		ata_cmd.cyl_lo = 0;		/* (�V�����_Lo) */
		ata_cmd.cyl_hi = 0;		/* (�V�����_Hi) */
		ata_cmd.dev_hed = devnum<<4;	/* �f�o�C�X */
		if(devtype==DEVICE_ATA) {
			/* ATA�f�o�C�X�p IDLE�R�}���h */
			ata_cmd.command = 0xe3;
		} else if(devtype==DEVICE_ATAPI) {
			/* ATAPI�f�o�C�X�p IDLE�R�}���h */
			ata_cmd.command = 0xe1;
		} else {
			panic("Device::IDE_idle_device");
		}
		ata_cmd.DRDY_Chk = 1;		/* DRDY�r�b�g�`�F�b�N */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

	int Device::IDE_init_device_paramaters(byte head, byte sector) {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0xff;		/* (�t�B�[�`���[) */
		ata_cmd.sec_cnt = sector;	/* �ő�Z�N�^�� */
		ata_cmd.sec_no = 0;			/* (�Z�N�^�i���o) */
		ata_cmd.cyl_lo = 0;			/* (�V�����_Lo) */
		ata_cmd.cyl_hi = 0;			/* (�V�����_Hi) */
		ata_cmd.dev_hed = (devnum<<4)|(head & 0xf);	/* �f�o�C�X/�w�b�h�� */
		ata_cmd.command = 0x91;		/* INITALIZE DEVICE PARAMATERS�R�}���h */
		ata_cmd.DRDY_Chk = 0;		/* DRDY�r�b�g�`�F�b�N�s�v */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

/* SET FEATURES�R�}���h�p�T�u�R�}���h */
#define SET_TRANSFER	0x03	/* Set Transfer Mode */
#define ENA_MEDIASTATUS	0x95	/* Enable Media Status Notification */
#define DIS_MEDIASTATUS	0x31	/* Disable Media Status Notification */

	int Device::ide_initialize_device_mode_sub1(int PIO_mode, int DMA_mode) {
		if(devtype!=DEVICE_ATA && devtype!=DEVICE_ATAPI) return 0xFFFF;
		pio_mode = PIO_mode;
		dma_mode = DMA_mode;
		/* �f�o�C�Xn���[�h������ */
		if(ide_ata_device_select()!=0) return -1;
		switch(devtype) {
		default:
			panic("Unknown Device Type");
		case DEVICE_ATA:
			getConsole() << "ATA>InitDevParam>";
			if(0!=IDE_init_device_paramaters((byte)(identify_data[3]-1),(byte)identify_data[6])) {
				/* INITALIZE DEVICE PARAMATERS�R�}���h�G���[ */
				return -2;
			}
			getConsole() << "Idle>";
			if(0!=IDE_idle_device()) {
				/* IDLE�R�}���h�G���[ */
				return -3;
			}
			break;
		case DEVICE_ATAPI:
			getConsole() << "APAPI>PwrMgmtFeat>";
			/* ATAPI�Ȃ� IDLE IMMEDIATE�R�}���h�T�|�[�g���K�{�ɂȂ��Ă���͂����� */
			if(identify_data[82]&8) {	/* Power Management feature set�T�|�[�g */
				if(0!=IDE_idle_device()) return -4;
			}
		}
		/* PIO�]�����[�h�ݒ� */
		if(identify_data[49]&0x800) {
			getConsole() << "PIO>";
			/* IORDY�T�|�[�g */
			/* PIO�t���[�R���g���[�����[�h&PIO�]�����[�h��ݒ� */
			if(0!=IDE_set_features(SET_TRANSFER, 0x08|(pio_mode&7))) {
				/* �t���[�R���g���[���t��PIO���[�h �ݒ�G���[ */
				/* PIO�f�t�H���g�]�����[�h(�G���[����) */
				IDE_set_features(SET_TRANSFER, 0);
				/* PIO�]�����[�h���Z�b�g */
				atapi.PIO_mode = 0;
			}
		} else {
			/* PIO�t���[�R���g���[�����T�|�[�g */
			getConsole() << "NOPIO>";
			/* PIO�f�t�H���g�]�����[�h(�G���[����) */
			// Disable following code because Bochs reports an error.
			//IDE_set_features(SET_TRANSFER, 0);
			/* PIO�]�����[�h���Z�b�g */
			atapi.PIO_mode = 0;
		}
		/* DMA�]�����[�h�ݒ� */
		if(dma_mode > 0) {
			getConsole() << "DMA" << dma_mode << ">";
			/* DMA�]���Ή� */
			if(0!=IDE_set_features(SET_TRANSFER, 0x20|(dma_mode&7))) {
				/* �}���`���[�hDMA�]���ݒ�G���[ */
				getConsole() << "ERROR>";
				/* DMA�]�����[�h���g�p */
				atapi.DMA_mode = -1;
			}
		} else {
			getConsole() << "NODMA>";
		}
		/* Removable Media Status Notification �T�|�[�g�`�F�b�N */
		if(((identify_data[83]&0x10)==0x10)
		|| ((identify_data[127]&3)==1))
		{
			getConsole() << "RMMD>";
			if(0==IDE_set_features(ENA_MEDIASTATUS, 0)) {
				/* �R�}���h����I�� */
				byte c = *atapi.reg_chr;
				if(c&2) devfeat|=DEVICE_LOCK;		/* ���b�N/�A�����b�N�@�\���� */
				if(c&4) devfeat|=DEVICE_PEJECT;		/* �p���[�C�W�F�N�g�@�\���� */
			}
		}
		return ide_initialize_device_mode_sub2();
	}

	int Device::ide_initialize_device_mode_sub2() {
		devmedia = (MediaType)0;
		devfeat = (DeviceFeature)0;
		/* �f�o�C�X�^�C�v�m�F & IDENTIFY�R�}���h�Ď��s */
		switch(devtype) {
		default:
			return -1;
		case DEVICE_ATA:
			/* ATA�f�o�C�X */
			if(identify_data[61]==0 && identify_data[60]==0) {
				lbaok = false;	/* CHS�����Ή� */
			} else {
				lbaok = true;	/* LBA�����Ή� */
			}
			if(identify_data[0]&0x40) {
				/*  �񃊃��[�o�u���f�o�C�X */
				devmedia = DEVICE_HDD;
			} else {
				panic("Unknown ATA Device");
			}
			/* ZIP�f�o�C�X�̔��������ꍇ */
			/* identify_data[device]���� Model number �Ȃǂ���f�o�C�X����� */
			/*	device_type[device]|=DEVICE_ZIP; */
			getConsole() << "IDENT>";
			if(IDE_identify_device(false)!=0) return -1;
			break;
		case DEVICE_ATAPI:
			/* ATAPI�f�o�C�X */
			if(((identify_data[0]>>8)&0x1f)==5) {
				/* CD-ROM/DVD-ROM device */
				devmedia = DEVICE_CDROM;
			} else {
				panic("Unknown ATAPI Device");
			}
			/* MO��SuperDisk�f�o�C�X�̔��������ꍇ */
			/* identify_data[device]���� Model number �Ȃǂ���f�o�C�X����� */
			/*	device_type[device]=device_type[device]|DEVICE_MO; */
			/*	device_type[device]=device_type[device]|DEVICE_LS; */
			getConsole() << "IDENT>";
			if(IDE_identify_device(true)!=0) return -1;
			break;
		}
		getConsole() << "ALZ>";
		if(identify_data[0]&0x80) {
			/*  �����[�o�u���f�o�C�X */
			devfeat |= DEVICE_REMOV;
		}
		/* GET MEDIA STATUS�R�}���h�Ή��m�F */
		if((identify_data[82]!=0xffff)&&((identify_data[82]&0x04)==0x04) /* Removable Media feature set */
		|| (identify_data[83]!=0xffff)&&((identify_data[83]&0x10)==0x10) /* Removable Media Status Notification feature set */
		|| ((identify_data[127]&3)==1) /* Removable Media Status Notification feature set */
		)
		{
			/* GET MEDIA STATUS�R�}���h�ɑΉ����Ă��邩������Ȃ��̂ŁA���ۂɃR�}���h�𔭍s���Ă݂� */
			getConsole() << "MDSTS>";
			if((IDE_get_media_status()&4)==0) {
				/* �A�{�[�h���������Ȃ����GET MEDIA STATUS�R�}���h�Ή� */
				devfeat |= DEVICE_GETMeSt;
			}
		}
		/* CFA�f�o�C�X�m�F */
		if((identify_data[0]==0x848a)	/* CFA�V�O�l�`�� */
		|| (identify_data[83]!=0xffff)
		&&((identify_data[83]&0x02)==0x02)) /* CFA feature set */
		{
			devmedia = DEVICE_CFA;		/* CFA�f�o�C�X�ł��� */
		}
		return 0;
	}

	int Device::IDE_get_media_status() {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;	/* (�t�B�[�`���[) */
		ata_cmd.sec_cnt = 0;	/* (�Z�N�^�J�E���g) */
		ata_cmd.sec_no = 0;		/* (�Z�N�^�i���o) */
		ata_cmd.cyl_lo = 0;		/* (�V�����_Lo) */
		ata_cmd.cyl_hi = 0;		/* (�V�����_Hi) */
		ata_cmd.dev_hed = devnum<<4;	/* �f�o�C�X */
		ata_cmd.command = 0xda;	/* GET MEDIA STATUS�R�}���h */
		ata_cmd.DRDY_Chk=1;		/* DRDY�r�b�g�`�F�b�N */
		return atapi.ide_ata_non_data_cmd(&ata_cmd);
	}

	byte Device::ide_get_sct_parameter(uint32 lba) {
		if(lbaok) {
			/* LBA�����Ή��f�o�C�X */
			return (byte)lba&0xff;
		} else {
			/* LBA�������Ή��f�o�C�X */
			return (byte)(lba%identify_data[56])+1;
		}
	}

	uint16 Device::ide_get_cyli_parameter(uint32 lba) {
		if(lbaok) {
			/* LBA�����Ή��f�o�C�X */
			return (uint16)(lba>>8)&0xffff;
		} else {
			/* LBA�������Ή��f�o�C�X */
			return (uint16)(lba/(identify_data[56]*identify_data[55]));
		}
	}

	byte Device::ide_get_head_parameter(uint32 lba) {
		if(lbaok) {
			/* LBA�����Ή��f�o�C�X */
			return 0x40	/*LBA_flg*/ | (byte)(devnum<<4) | ((byte)(lba>>24)&0xf);
		} else {
			/* LBA�������Ή��f�o�C�X */
			return (byte)(devnum<<4) | (byte)((lba/(identify_data[56]))%identify_data[55]);
		}
	}

	int Device::IDE_ata_read_sector(uint32 lba, uint16 count, void *buff) {
		struct STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;			/* (�t�B�[�`���[) */
		ata_cmd.sec_cnt = (byte)count;	/* �Z�N�^�� */
		ata_cmd.sec_no = ide_get_sct_parameter(lba);	/* �Z�N�^�i���o */
		uint16 l = ide_get_cyli_parameter(lba);
		ata_cmd.cyl_lo = l&0xff;		/* �V�����_Lo */
		ata_cmd.cyl_hi = (l>>8)&0xff;	/* �V�����_Hi */
		ata_cmd.dev_hed = ide_get_head_parameter(lba);	/* �f�o�C�X/�w�b�h */
		ata_cmd.command = 0x20;			/* �Z�N�^���[�h�R�}���h */
		ata_cmd.DRDY_Chk = 1;			/* DRDY�r�b�g�`�F�b�N */
		return atapi.ide_ata_pio_datain_cmd(&ata_cmd,count,buff);	/* count=�Z�N�^(�u���b�N)�� */
	}

	int Device::IDE_atapi_read10(uint32 lba, uint16 count, void *buff) {
		STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature = 0;							/* Non-overlap/non DMA */
		atapi_cmd.dev_sel = devnum;						/* �f�o�C�X */
		atapi_cmd.cmd_packet[0] = 0x28;					/* READ(10) */
		atapi_cmd.cmd_packet[2] = (byte)(lba>>24)&0xff;	/* �_���Z�N�^ */
		atapi_cmd.cmd_packet[3] = (byte)(lba>>16)&0xff;
		atapi_cmd.cmd_packet[4] = (byte)(lba>>8)&0xff;
		atapi_cmd.cmd_packet[5] = (byte)lba&0xff;
		atapi_cmd.cmd_packet[7] = (count>>8)&0xff;	/* �ǂݏo���Z�N�^�� */
		atapi_cmd.cmd_packet[8] = count&0xff;
		atapi.atapi_datatransfer = 0;					/* �]���f�[�^�o�C�g���N���A */
		if(count*device_secter_size>0xFFFF) panic("Device::IDE_atapi_read10");
		if(0!=atapi.ide_atapi_packet_cmd(atapi_cmd,(uint16)(count*device_secter_size), buff)) {
			/* �p�P�b�g�R�}���h�̎��s�O/��/��ɃG���[ */
			return -1;
		} else {
			/* �p�P�b�g�R�}���h�̎��s���̂͐���I�� */
			if(atapi.atapi_datatransfer==0) {
				getConsole() << "{ATAPI:READ10:NODATA}";
			} else if(atapi.atapi_datatransfer%device_secter_size) {
				getConsole() << "{ATAPI:READ10:LESS|" << atapi.atapi_datatransfer << "}";
				return 0;	// ���Ȃ����ɂ͋���
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
		ata_cmd.sec_cnt = mode;			/* �f�[�^�]�����[�h�l�ق� */
		ata_cmd.sec_no = 0;				/* (�Z�N�^�i���o) */
		ata_cmd.cyl_lo = 0;				/* (�V�����_Lo) */
		ata_cmd.cyl_hi = 0;				/* (�V�����_Hi) */
		ata_cmd.dev_hed.dev = devnum;	/* �f�o�C�X */
		ata_cmd.command = 0xEF;			/* SET FEATURES�R�}���h */
		ata_cmd.DRDY_Chk = 1;			/* DRDY�r�b�g�`�F�b�N */
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
		atapi_cmd.dev_sel = devnum;		/* �f�o�C�X */
		//getConsole() << "{ATAPI:TESTUNIT:";
		int rc = atapi.ide_atapi_packet_cmd(atapi_cmd, 0, NULL);
		//getConsole() << rc << "}";
		return rc;
	}

	int Device::IDE_atapi_request_sense(void *buff) {
		STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature = 0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel = devnum;		/* �f�o�C�X */
		atapi_cmd.cmd_packet[0] = 3;	/* REQUEST SENSE */
		atapi_cmd.cmd_packet[4] = 18;	/* 18�o�C�g */
		atapi.atapi_datatransfer = 0;	/* �]���f�[�^�o�C�g���N���A */
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
			/* ATAPI�f�o�C�X�̏ꍇ */
			for(int j=0; j<RETRY_MAX; j++) {
				/* Request Sense�����s�����Ƃ��̓��g���C */
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
		if(0==(devfeat&DEVICE_REMOV)) return MEDIA_Ready;	/* �펞���f�B��� */
		/* �����[�o�u���f�o�C�X�̏ꍇ */
		if(devmedia==DEVICE_CDROM) {
			/* CD-ROM�f�o�C�X�̏ꍇ */
			/* �r�b�g27�`8��SenseKey��ASC/ASCQ�̏�Ԃ��i�[ */
			//getConsole() << "{ATAPI:CDROM:";
			uint32 l = IDE_Get_Atapi_DeviceReady();
			//getConsole() << l << "}";
			if(l==0) {
				/* ���f�B��� */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_Ready);
			} else if((l&0x0fff0000)==0x06280000) {
				/* �f�B�X�N���������ꂽ */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_Chg);
			} else if((l&0x0f000000)==0x02000000) {
				/* Not���f�B��� */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_NotReady);
			} else {
				/* ���炩�̃G���[��� */
				/* CD-ROM�͓ǂݏo����p�Ȃ̂�MEDIA_Wp��t�� */
				return (MediaAttributes)(MEDIA_Wp|MEDIA_Error);
			}
		} else {
			/* CD-ROM�ȊO�̃f�o�C�X�̏ꍇ */
			if(devfeat&DEVICE_GETMeSt) {
				/* GET MEDIA STATUS�R�}���h�Ή��̏ꍇ */
				int err = IDE_get_media_status();			/* GET MEDIA STATUS�R�}���h */
				if(err==0) {
					/* ����I��(�A�N�Z�X���f�B���) */
					return MEDIA_Ready;
				} else {
					/* �G���[��� */
					uint flags = 0;
					if(err&BIT_WP) flags|=MEDIA_Wp;		/* ���f�B�A�����C�g�v���e�N�g��Ԃł��� */
					if(err&BIT_NM) flags|=MEDIA_NoDisk;	/* ���f�B�A���Ȃ� */
					if(err&BIT_MC) flags|=MEDIA_Chg;		/* ���f�B�A���`�F���W���ꂽ */
					if(err&BIT_MCR) {
						flags |= MEDIA_ChgReq;				/* ���f�B�A�`�F���W���v�����ꂽ */
						//TODO: IDE_Media_Eject();				/* ���f�B�A�C�W�F�N�g(�G���[����) */
					}
					if(err&4/*BIT_ABRT*/) flags|=MEDIA_Error;	/* ���炩�̃G���[ */
					return (MediaAttributes)flags;
				}
			} else {
				/* GET MEDIA STATUS�R�}���h��Ή��̏ꍇ */
				if(devtype==DEVICE_ATAPI) {
					/* ATAPI�f�o�C�X�̏ꍇ */
					/* �r�b�g27�`8��SenseKey��ASC/ASCQ�̏�Ԃ��i�[ */
					return IDE_Get_Atapi_DeviceReady();
				} else {
					if (devtype==DEVICE_CFA) {	/* CFA�f�o�C�X�̏ꍇ */
						/* CompactFlash�Ȃ�PC�J�[�h�n��ATA�f�o�C�X(�����[�o�u���f�o�C�X�Ȃ̂�GET MEDIA STATUS�R�}���h��Ή�) */
						/* �̏ꍇ�́A�s�ӂɃJ�[�h�𔲂����󋵂��l������̂ŁA�����ŃJ�[�h�̐ڑ���Ԃ��m�F�����ق����ǂ� */
						/* PC�J�[�h��CF�̏ꍇ�́ACD1#/CD2#(�J�[�h�f�e�N�g�M��)�𒲂ׂăJ�[�h�������ꂽ���ǂ����𔻒肷�� */
						/* TrueIDE���[�h�̏ꍇ�́c �v���b�g�z�[���ˑ�(^^;) */
					}
					/* �����[�o�u�����f�B�A�̃t���O�������Ă���̂�GET MEDIA STATUS�R�}���h��Ή��̏ꍇ�� */
					/* ATAPI�f�o�C�X�Ȃ�TEST UNIT�p�P�b�g�R�}���h�ȂǂŃf�o�C�X�̏�Ԃ𒲂� */
					/* �Ƃ肠�������f�B��ԂƂ��ĕԂ� */
					return MEDIA_Ready;		/* ���f�B��� */
				}
			}
		}
	}

	int Device::IDE_atapi_start_unit(byte data) {
		getConsole() << "{ATAPI:STARTUNIT}";
		struct STRUCT_ATAPI_CMD atapi_cmd;
		memclr(&atapi_cmd, sizeof(atapi_cmd));
		atapi_cmd.feature=0;			/* Non-overlap/non DMA */
		atapi_cmd.dev_sel=devnum;		/* �f�o�C�X */
		atapi_cmd.cmd_packet[0]=0x1b;	/* START UNIT */
		atapi_cmd.cmd_packet[4]=data;	/* �ݒ�l */
		return atapi.ide_atapi_packet_cmd(atapi_cmd,0,NULL);
	}

/* START/STOP UNIT(START)�R�}���h�p�����[�^ */
#define ATAPI_START		1	/* ATAPI�f�o�C�X�X�^�[�g */
#define ATAPI_EJECT		2	/* �C�W�F�N�g */
#define ATAPI_CLOSE		3	/* �g���C�N���[�Y */

	MediaAttributes Device::IDE_Media_AccessReady(bool writable) {
		MediaAttributes k = (MediaAttributes)(MEDIA_Ready | (writable ? MEDIA_Wp : 0));
		for(int j=0; j<RETRY_MAX; j++) {
			uint32 ms = IDE_Get_MediaStatus_Immediate();
			if(k==(ms&k)) return k;
			Delay3(5);
			if(devmedia==DEVICE_CDROM) {
				/* CD-ROM�h���C�u�̏ꍇ */
				IDE_atapi_start_unit(ATAPI_START);
			}
		}
		return MEDIA_Error;
	}

	int Device::IDE_identify_device(bool packetdevice) {
		STRUCT_ATA_CMD ata_cmd;
		ata_cmd.feature = 0;	/* (�t�B�[�`���[) */
		ata_cmd.sec_cnt = 0;	/* (�Z�N�^�J�E���g) */
		ata_cmd.sec_no = 0;		/* (�Z�N�^�i���o) */
		ata_cmd.cyl_lo = 0;		/* (�V�����_Lo) */
		ata_cmd.cyl_hi = 0;		/* (�V�����_Hi) */
		ata_cmd.dev_hed = (devnum<<4);	/* �f�o�C�X */
		if(!packetdevice) {
			ata_cmd.DRDY_Chk = true;		/* DRDY�r�b�g�`�F�b�N */
			ata_cmd.command = 0xEC;			/* IDENTIFY DEVICE�R�}���h */
		} else {
			ata_cmd.DRDY_Chk = false;		/* DRDY�r�b�g�`�F�b�N�s�v */
			ata_cmd.command = 0xA1;			/* IDENTIFY PACKET DEVICE�R�}���h */
		}
		return atapi.ide_ata_pio_datain_cmd(&ata_cmd,1,identify_data);	// 1�u���b�N(256���[�h)
	}

	void Device::ide_initialize_device_modcheck() {
		if(devtype!=DEVICE_ATA && devtype!=DEVICE_ATAPI) {
			/* �f�o�C�X���ڑ� or ��ATA/ATAPI�f�o�C�X */
			/* ���ڑ����ATA/ATAPI�f�o�C�X�Ȃ疳�� */
			pio_mode = 255;
			dma_mode = 255;
		} else {
			if(0==(identify_data[53]&2)) {
				/* �Ή����[�h���s���ȏꍇ */
				pio_mode = 0;		/* PIO���[�h 0 */
				dma_mode = -1;		/* �}���`���[�hDMA��Ή� */
			} else {
				/* ���[�h53 �r�b�g1=1 ���[�h64�`70�L�� */
				if(identify_data[64]&2) {
					pio_mode = 4;	/* PIO���[�h4 */
				} else if(identify_data[64]&1) {
					pio_mode = 3;	/* PIO���[�h3 */
				} else {
					pio_mode = 0;	/* ��{��PIO���[�h0 */
				}
				if(0==(identify_data[63]&7)) {
					/* �}���`���[�hDMA��Ή� */
					dma_mode = -1;
				} else {
					/* �}���`���[�hDMA�]���Ή� */
					if(identify_data[63]&4) {
						dma_mode = 2;	/* ���[�h2 */
					} else if(identify_data[63]&2) {
						dma_mode = 1;	/* ���[�h1 */
					} else if(identify_data[63]&1) {
						dma_mode = 0;	/* ���[�h0 */
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
			/* �f�o�C�Xn��ATA�f�o�C�X�̉\������ */
			packetdevice = false;
		} else if ((s1==0x14)&&(s2==0xEB)) {
			/* �f�o�C�Xn��ATAPI�f�o�C�X�̉\������ */
			packetdevice = true;
		} else {
			return false;
		}
		if(!atapi.ide_wait_bsyclr()) {
			/* BSY�r�b�g���N���A����Ȃ� */
			return false;
		} else {
			/* �^�C���A�E�g�ȓ���BSY�r�b�g�N���A */
			for(int l=0; l<RETRY_MAX;l++){
				Delay3(5); int i=IDE_identify_device(packetdevice);
				Delay3(5); int k=IDE_identify_device(packetdevice);
				if(k!=i) continue;
				/* 2����s���ē������� */
				switch(i) {
				case 0:
					/* �G���[�Ȃ�&IDENTIFY�f�[�^����擾 */
					devtype = packetdevice ? DEVICE_ATAPI : DEVICE_ATA;
					return true;
				case 1:	/* �R�}���h�����s�I�� */
				case 2: /* �A�{�[�g�G���[ */
					/* �f�o�C�Xn�͖��ڑ� */
					getConsole() << "{ABORT}";
					devtype = DEVICE_NON;
					return false;
				default:
					/* ���̑��̃G���[�̓��g���C */
					//getConsole() << "{FAILED}";
					devtype = DEVICE_NON;
					return false;
				}
			}
			/* �f�o�C�Xn�͕s���f�o�C�X */
			return false;
		}
	}


	int Device::IDE_device_reset() {
		STRUCT_ATA_CMD ata_cmd;
		memclr(&ata_cmd, sizeof(ata_cmd));
		ata_cmd.feature=0;		/* (�t�B�[�`���[) */
		ata_cmd.sec_cnt=0;		/* (�Z�N�^�J�E���g) */
		ata_cmd.sec_no=0;		/* (�Z�N�^�i���o) */
		ata_cmd.cyl_lo=0;		/* (�V�����_Lo) */
		ata_cmd.cyl_hi=0;		/* (�V�����_Hi) */
		ata_cmd.dev_hed=(devnum<<4);	/* �f�o�C�X */
		ata_cmd.command=0x08;	/* DEVICE RESET�R�}���h */
		ata_cmd.DRDY_Chk=0;		/* DRDY�r�b�g�`�F�b�N�s�v */
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
		atapi_cmd.dev_sel=devnum;		/* �f�o�C�X */
		atapi_cmd.cmd_packet[0]=0x25;	/* READ CAPACITY */
		atapi.atapi_datatransfer=0;			/* �]���f�[�^�o�C�g���N���A */
		int i=atapi.ide_atapi_packet_cmd(atapi_cmd,8,buff);
		if ((i==0)&&(atapi.atapi_datatransfer==8)) return 0;	/* ����I����=0 */
		return i;						/* �G���[�I����!=0 */
	}

	int Device::IDE_Get_Media_Infomation() {
		if(devtype==DEVICE_ATA) {
			/* �I�����ꂽ�h���C�u��ATA�f�o�C�X */
			/* ATA�f�o�C�X��1�Z�N�^512�o�C�g */
			device_secter_size = 512;
			if(devfeat & DEVICE_REMOV) {
				/* �����[�o�u���f�o�C�X */
				for(int j=0; j<RETRY_MAX; j++){
					if(0==IDE_identify_device(false)) {
						lbamax = identify_data[61]<<16|identify_data[60];
						return 0;
					}
				}
				return -1;
			} else {
				/* �񃊃��[�o�u���f�o�C�X */
				lbamax = identify_data[61]<<16|identify_data[60];
				return 0;
			}
		} else if (devtype==DEVICE_ATAPI) {
			/* �I�����ꂽ�h���C�u��ATAPI�f�o�C�X */
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
		atapi_cmd.dev_sel=devnum;		/* �f�o�C�X */
		atapi_cmd.cmd_packet[0]=0x43;	/* READ TOC */
		atapi_cmd.cmd_packet[1]=(lba_msf&1)<<1;/* select LBA/MSF */
		atapi_cmd.cmd_packet[2]=format&7;/* format */
		atapi_cmd.cmd_packet[6]=setion;	/* track/setion */
		atapi_cmd.cmd_packet[7]=(len>>8)&0xff;	/* �ǂݏo���o�C�g�� */
		atapi_cmd.cmd_packet[8]=len&0xff;
		atapi.atapi_datatransfer=0;			/* �]���f�[�^�o�C�g���N���A */
		int i=atapi.ide_atapi_packet_cmd(atapi_cmd,len,buff);
		if ((i==0)&&(atapi.atapi_datatransfer==len)) return 0;	/* ����I����=0 */
		return i;						/* �G���[�I����!=0 */
	}

}
