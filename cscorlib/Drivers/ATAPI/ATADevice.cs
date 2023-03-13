using System;
using System.IO;

namespace CooS.Drivers.ATAPI {

	/// <summary>
	/// AT Attachment with Packet Interface (ATAPI) - 7 ��ATA������񋟂��܂��B
	/// </summary>
	public class ATADevice : DeviceBase {

		public ATADevice(ATAPIController controller, int devnum) : base(controller,devnum) {
		}

		public override byte[] IdentifyDevice() {
			byte[] identify_data = new byte[512];
			lock(this.controller) {
				this.Activate();
				// IDENTIFY DEVICE �R�}���h
				this.controller.SendCommand(0xEC, 0,0,0,0, true);
				this.controller.ReadBlocks(identify_data, 1);
			}
			return identify_data;
		}

		public override void IdleDevice() {
			lock(this.controller) {
				this.Activate();
				this.controller.SendCommand(0xE3, 0,0,0,0, true);
				this.controller.WaitForSuccess();
			}
		}

		public override void InitializeParameters() {
			int head = this.IdentityData[6]-1;
			int sector = this.IdentityData[12];
			lock(this.controller) {
				this.Activate();
				// INITALIZE DEVICE PARAMATERS�R�}���h
				this.controller.SendCommand(0x91, 0xFF, sector, 0,0, false);
			}
		}

		public override MediaType GetMediaType() {
			bool lbaok;
			if(this.IdentityData[122]==0 && this.IdentityData[120]==0) {
				lbaok = false;	/* CHS�����Ή� */
			} else {
				lbaok = true;	/* LBA�����Ή� */
			}
			if(0!=(this.IdentityData[0]&0x40)) {
				/*  �񃊃��[�o�u���f�o�C�X */
				return lbaok ? MediaType.HDD_LBA : MediaType.HDD_CHS;
			} else {
				throw new NotSupportedException();
			}
			/* ZIP�f�o�C�X�̔��������ꍇ */
			/* identify_data[device]���� Model number �Ȃǂ���f�o�C�X����� */
			/*	device_type[device]|=DEVICE_ZIP; */
		}

		private int IDE_ata_read_sector(byte[] buf, int index, uint lba, ushort count) {
			throw new NotImplementedException();
		}

	}

}
