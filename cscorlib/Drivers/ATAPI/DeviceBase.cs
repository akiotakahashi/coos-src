using System;

namespace CooS.Drivers.ATAPI {

	public abstract class DeviceBase {

		// SET FEATURES�R�}���h�p�T�u�R�}���h
		const int SET_TRANSFER		= 0x03;		// Set Transfer Mode
		const int ENA_MEDIASTATUS	= 0x95;		// Enable Media Status Notification
		const int DIS_MEDIASTATUS	= 0x31;		// Disable Media Status Notification

		protected readonly ATAPIController controller;
		protected readonly int DeviceNumber;
		protected readonly byte[] IdentityData;
		protected int mode_pio;
		protected int mode_dma;
		
		public DeviceBase(ATAPIController controller, int devnum) {
			this.controller = controller;
			this.DeviceNumber = devnum;
			this.IdentityData = this.IdentifyDevice();
			/*
			for(int i=0; i<176; ++i) {
				byte e = this.IdentityData[i];
				Console.Write("{0:X2} ", e);
				if(i%16==15) Console.WriteLine();
			}
			*/
			this.mode_pio = DeterminePIOMode(this.IdentityData);
			this.mode_dma = DetermineDMAMode(this.IdentityData);
		}

		public bool RequireDMADIR {
			get {
				///If bit 15 of word 62 is set to 1, the device requires the use of the DMADIR bit for Packet DMA commands.
				return 0!=(0x80&this.IdentityData[124]);
			}
		}

		public int PIOMode {
			get {
				return this.mode_pio;
			}
		}

		public int DMAMode {
			get {
				return this.mode_dma;
			}
		}

		public abstract byte[] IdentifyDevice();
		public abstract void IdleDevice();
		public abstract void InitializeParameters();
		public abstract MediaType GetMediaType();

		/*
		public abstract bool IsMediaReady();
		public abstract MediaBase GetMedia();
		*/

		private static int DeterminePIOMode(byte[] identify_data) {
			if(0==(identify_data[0x6A]&2)) {
				// ���[�h�L�q�̈悪����
				return -1;
			} else if(0!=(identify_data[128]&2)) {
				return 4;
			} else if(0!=(identify_data[128]&1)) {
				return 3;
			} else {
				return 0;
			}
		}

		private static int DetermineDMAMode(byte[] identify_data) {
			if(0==(identify_data[106]&2)) {
				// ���[�h�L�q�̈悪����
				return -1;
			} else if(0==(identify_data[126]&7)) {
				// �}���`���[�hDMA�]����Ή�
				return -1;
			} else {
				// �}���`���[�hDMA�]���Ή�
				if(0!=(identify_data[126]&4)) {
					return 2;
				} else if(0!=(identify_data[126]&2)) {
					return 1;
				} else if(0!=(identify_data[126]&1)) {
					return 0;
				} else {
					return -1;
				}
			}
		}

		public void ValidateDeviceType() {
			try {
				for(int l=0; l<4;l++){
					this.controller.WaitForIdle();
					try {
						byte[] id1 = this.IdentifyDevice();
						byte[] id2 = this.IdentifyDevice();
						// �G���[�Ȃ�&IDENTIFY�f�[�^����擾
						// 2����s���ē�������
						return;
					} catch {
						//NOP
					}
				}
			} catch {
			}
			throw new NotFoundException();
		}

		protected void Activate() {
			this.controller.SelectDevice(this.DeviceNumber, false);
		}

		byte SetFeatures(int features, int mode) {
			lock(this.controller) {
				this.Activate();
				// SET FEATURES�R�}���h
				this.controller.SendCommand(0xEF, features, mode, 0,0, true);
				this.controller.WaitForSuccess();
				return this.controller.reg_chr.Read();
			}
		}

		public void SetTransferMode(int PIO_mode, int DMA_mode) {
			this.mode_pio = PIO_mode;
			this.mode_dma = DMA_mode;
			this.InitializeParameters();
			this.IdleDevice();
			/* PIO�]�����[�h�ݒ� */
			if(this.mode_pio>=0) {
				if(0!=(this.IdentityData[99]&0x8)) {
					/* IORDY�T�|�[�g */
					/* PIO�t���[�R���g���[�����[�h&PIO�]�����[�h��ݒ� */
					SetFeatures(SET_TRANSFER, 0x08|(this.mode_pio&7));
				}
			}
			/* DMA�]�����[�h�ݒ� */
			if(this.mode_dma>=0) {
				SetFeatures(SET_TRANSFER, 0x20|(this.mode_dma&7));
			}
		}

		public DeviceFeatures DetermineDeviceFeatures() {
			DeviceFeatures feat = DeviceFeatures.None;
			if(0!=(this.IdentityData[0]&0x80)) {
				feat |= DeviceFeatures.Removable;
			}
			// Removable Media Status Notification �T�|�[�g�`�F�b�N
			if(((this.IdentityData[166]&0x10)!=0) || ((this.IdentityData[254]&3)==1)) {
				byte c = SetFeatures(ENA_MEDIASTATUS, 0);
				if(0!=(c&2)) feat|=DeviceFeatures.Lock;		// ���b�N/�A�����b�N�@�\����
				if(0!=(c&4)) feat|=DeviceFeatures.Eject;	// �p���[�C�W�F�N�g�@�\����
			}
			// GET MEDIA STATUS�R�}���h�Ή��m�F
			if(this.IdentityData[164]!=0xff || this.IdentityData[165]!=0xff) {
				// Removable Media feature set
				if((this.IdentityData[164]&0x04)==0x04) {
					feat |= DeviceFeatures.MediaStatus;
				}
			}
			if(this.IdentityData[166]!=0xff || this.IdentityData[167]!=0xff) {
				// Removable Media Status Notification feature set
				if((this.IdentityData[166]&0x10)==0x10) {
					feat |= DeviceFeatures.MediaStatus;
				}
			}
			if((this.IdentityData[254]&3)==1) {
				// Removable Media Status Notification feature set
				feat |= DeviceFeatures.MediaStatus;
			}
			// CFA�f�o�C�X�m�F
			if(this.IdentityData[0]==0x8A && this.IdentityData[1]==0x84) {
				// CFA�V�O�l�`��
				feat |= DeviceFeatures.CFA;
			}
			if(this.IdentityData[166]!=0xff || this.IdentityData[167]!=0xff) {
				if((this.IdentityData[166]&0x02)==0x02) {
					// CFA feature set
					feat |= DeviceFeatures.CFA;
				}
			}
			return feat;
		}

	}

}
