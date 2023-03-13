#pragma once

#include "stdtype.h"
#include "io.h"
#include "atapi-device.h"


namespace Atapi {

	/* ATA�G���[���W�X�^�r�b�g��` */
	enum ATA_ERR_REG {
		BIT_NM		= 2,		/* ���f�B�A���Ȃ� */
		BIT_MCR		= 8,		/* ���f�B�A�`�F���W���v�����ꂽ */
		BIT_MC		= 0x20,		/* ���f�B�A���`�F���W���ꂽ */
		BIT_WP		= 0x40,		/* ���f�B�A�����C�g�v���e�N�g��Ԃł��� */
	};

	/* ATA�X�e�[�^�X���W�X�^ */
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

	/* ATA�R�}���h���s�p �\���� */
	struct STRUCT_ATA_CMD {
		byte feature;	/* �t�B�|�`���[���W�X�^ */
		byte sec_cnt;	/* �Z�N�^�J�E���g���W�X�^ */
		byte sec_no;	/* �Z�N�^�i���o���W�X�^ */
		byte cyl_lo;	/* �V�����_���ʃ��W�X�^ */
		byte cyl_hi;	/* �V�����_��ʃ��W�X�^ */
		DeviceHeadRegister dev_hed;	/* �f�o�C�X/�w�b�h���W�X�^ */
		byte command;	/* �R�}���h���W�X�^ */
		byte DRDY_Chk;	/* �R�}���h���W�X�^ */
	};
		
	/* ATAPI�p�P�b�g�R�}���h���s�p �\���� */
	struct STRUCT_ATAPI_CMD {
		byte feature; 			/* overlap/DMA�̐ݒ� */
		byte dev_sel;			/* �f�o�C�X�Z���N�g(0:�f�o�C�X0/1:�f�o�C�X1) */
		byte cmd_packet[12];	/* �R�}���h�p�P�b�g */
	};

	/* �p�P�b�g�R�}���h�̏��������荞�݂��g���čs���ꍇ */
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
		volatile InterruptState atapi_interrupt_state;		/* ���荞�ݏ����X�e�[�g��ԕϐ� */
		volatile uint32 atapi_datatransfer;		/* ATAPI�œ]�������o�C�g�� */
		byte* atapi_interrupt_buff;	/* ���荞�ݏ������[�`���Ŏg�p����o�b�t�@�|�C���^ */
		uint16 atapi_interrupt_limit;	/* ���荞�ݏ������[�`���Ŏg�p����ő�]���o�C�g�� */
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
		/* ����I/O�A�N�Z�X���x�� */
		/*************************/
		byte getDeviceSignature(int devnum);
	private:
		/*******************/
		/* ATA�R�}���h���s */
		/*******************/
		/* EXECUTE DEVICE DIAGNOSTIC�R�}���h���s */
		int IDE_execute_device_diagnostic(void);
		/* GET MEDIA STATUS�R�}���h���s */
		int IDE_get_media_status(int);
		/* DOOR/MEDIA UNLOCK�R�}���h���s */
		int IDE_media_unlock(int);
		/* DOOR/MEDIA LOCK�R�}���h���s */
		int IDE_media_lock(int);
		/* MEDIA EJECT�R�}���h���s */
		int IDE_media_eject(int);
	private:
		/*****************************/
		/* ATAPI�p�P�b�g�R�}���h���s */
		/*****************************/
		/* READ CAPACITY�R�}���h�p�P�b�g���s */
		int IDE_atapi_read_capacity(int, void *);
		/* INQUIRY�R�}���h�p�P�b�g���s */
		int IDE_atapi_inquiry(int, byte, void *);
		/* PREVENT ALLOW MEDIAUM REMOVAL�R�}���h�p�P�b�g���s */
		int IDE_prevent_allow_mediaum_removal(int, int);
		/* READ TOC�R�}���h�p�P�b�g���s */
		int IDE_atapi_read_toc(int, int, byte, byte, uint16, void *);
		/* PLAY AUDIO MSF�R�}���h�p�P�b�g���s */
		int IDE_atapi_play_audio_msf(int, byte, byte, byte, byte, byte, byte);
		/* STOP PLAYBACK�R�}���h�p�P�b�g���s */
		int IDE_atapi_stop_playback(int);
		/* PAUSE/RESUME�R�}���h�p�P�b�g���s */
		int IDE_atapi_pause_resume(int, int);
		/* READ SUBCHANNL�R�}���h�p�P�b�g���s */
		int IDE_atapi_read_subchannel(int, int, int, byte, byte, uint16, void *);
	private:
		/************************************/
		/* ATA/ATAPI �f�o�C�X���������[�`�� */
		/************************************/
		/* �f�o�C�X�ڍ׏��e�[�u���擾 */
		void IDE_Get_Identify_Infomation(int, uint16 *);
		/* ���݂�PIO�]�����[�h���擾 */
		int IDE_Get_PIO_Mode(void);
		/* ���݂�DMA�]�����[�h���擾 */
		int IDE_Get_DMA_Mode(void);
		/* ���f�B�A��ԃ`�F�b�N(�ۑ���񂩂�Ԃ�) */
		uint32 IDE_Get_MediaStatus(int);
		/* ���f�B�A���b�N�ݒ�/�A�����b�N�ݒ� */
		int IDE_Media_LockUnlock(int, int);
		/* ���f�B�A�C�W�F�N�g */
		int IDE_Media_Eject(int);
		/* ���f�B�A�g���C�N���[�Y */
		int IDE_Media_TrayClose(int);
		/* ���O��ATAPI�R�}���h�œ]�����ꂽ�o�C�g���擾 */
		uint32 IDE_Get_Atapi_TransferByte();
	private:
		void ide_ata_read_data(void* buf, int size) {
			size >>= 1;
			if(buf==NULL) {
				/* �f�[�^�̋�ǂ� */
				while(size-->0){
					*reg_dtr;
				}
			} else {
				/* �f�[�^�o�b�t�@�֏������� */
				uint16* p = (uint16*)buf;
				while(size-->0){
					*(p++) = *reg_dtr;
				}
			}
		}
		void ide_ata_write_data(const void* buf, int size) {
			size >>= 1;
			if(buf==NULL) {
				/* �_�~�[�f�[�^(00h)�̏������� */
				while(size-->0){
					reg_dtr << 0;
				}
			} else {		/* �f�[�^�o�b�t�@���珑������ */
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
				// ���ݑI������Ă���h���C�u�łȂ����
				if(0!=ide_ata_device_select(devnum)) {
					active_device = -1;
					return false;
				} else {
					active_device = devnum;
					return true;
				}
			} else {
				// �f�o�C�X�Z���N�V�����t�F�[�Y�͍s�킸��Device/Head���W�X�^�֏�������
				DeviceHeadRegister dhr(0);
				dhr.dev = devnum;
				reg_dhr << dhr;
				active_device = devnum;
				return true;
			}
		}
	};

}
