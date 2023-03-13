using System;
using System.IO;

namespace CooS.Drivers.ATAPI {

	/// <summary>
	/// AT Attachment with Packet Interface (ATAPI) - 7 ��ATAPI������񋟂��܂��B
	/// </summary>
	public class ATAPIDevice : DeviceBase {

		public ATAPIDevice(ATAPIController controller, int devnum) : base(controller,devnum) {
		}

		public override byte[] IdentifyDevice() {
			byte[] identify_data = new byte[512];
			lock(this.controller) {
				this.Activate();
				// IDENTIFY PACKET DEVICE �R�}���h
				this.controller.SendCommand(0xA1, 0,0,0,0, false);
				this.controller.ReadBlocks(identify_data, 1);
				this.controller.DiscardData();
			}
			return identify_data;
		}

		public override void IdleDevice() {
			if(0!=(this.IdentityData[164]&8)) {
				/* Power Management feature set�T�|�[�g */
				lock(this.controller) {
					this.Activate();
					this.controller.SendCommand(0xE1, 0,0,0,0, true);
					this.controller.WaitForSuccess();
				}
			}
		}

		public override void InitializeParameters() {
			//NOP
		}

		public override MediaType GetMediaType() {
			if((this.IdentityData[1]&0x1F)==5) {
				// CD-ROM/DVD-ROM
				return MediaType.CDROM;
			} else {
				throw new NotSupportedException((this.IdentityData[1]&0x1F).ToString());
			}
			/* MO��SuperDisk�f�o�C�X�̔��������ꍇ */
			/* identify_data[device]���� Model number �Ȃǂ���f�o�C�X����� */
			/*	device_type[device]=device_type[device]|DEVICE_MO; */
			/*	device_type[device]=device_type[device]|DEVICE_LS; */
		}

		public int SendPacketCommand(byte[] buf, int index, int feature, int limit, byte[] atapi_cmd) {
			lock(this.controller) {
				this.Activate();
				return this.controller.SendPacketCommand(buf, index, feature, limit, atapi_cmd);
			}
		}

	}

}
