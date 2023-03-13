using System;
using CooS.FileSystem;

namespace CooS.Drivers.ATAPI.CDROM {

	/// <summary>
	/// ATAPI SFF-8020i の実装を提供します。
	/// </summary>
	public class CdromDevice {

		private readonly ATAPIDevice device;

		public CdromDevice(ATAPIDevice device) {
			this.device = device;
		}
        
		public const byte ATAPI_START	= 1;	// デバイススタート
		public const byte ATAPI_EJECT	= 2;	// イジェクト
		public const byte ATAPI_CLOSE	= 3;	// トレイクローズ

		public  void StartUnit(byte data) {
			byte[] atapi_cmd = new byte[12];
			atapi_cmd[0] = 0x1b;	// START UNIT
			atapi_cmd[4] = data;	// 設定値
			this.device.SendPacketCommand(null,0, 0,0, atapi_cmd);
		}

		public  void TestUnit() {
			byte[] atapi_cmd = new byte[12];
			atapi_cmd[0] = 0x00;	// TEST UNIT
			this.device.SendPacketCommand(null,0,0,0, atapi_cmd);
		}

		byte[] RequestSense() {
			byte[] buf = new byte[18];
			byte[] atapi_cmd = new byte[12];
			atapi_cmd[0] = 3;	/* REQUEST SENSE */
			atapi_cmd[4] = 18;	/* 18バイト */
			int iosize = this.device.SendPacketCommand(buf,0, 0, 18, atapi_cmd);
			if(iosize!=buf.Length) throw new ATAPIException("Transferred data is too short.");
			return buf;
		}

		MediaAttributes GetMediaAttributes() {
			byte[] buf = RequestSense();
			SenseKey sensekey = (SenseKey)(buf[2]&0xf);
			int asc = buf[12];		// Additional Sense Code
			int ascq = buf[13];		// Additional Sense Code Qualifier
			switch(sensekey) {
			case SenseKey.NoSense:
				// レディ
				return MediaAttributes.Ready|MediaAttributes.WriteProtect;
			case SenseKey.UnitAttention:
				// ディスクが交換された
				return MediaAttributes.Ready|MediaAttributes.MediaChanged|MediaAttributes.WriteProtect;
			case SenseKey.NotReady:
				// メディアなし
				return MediaAttributes.NoMedia;
			default:
				return MediaAttributes.Error;
			}
		}

		public byte[] ReadCapacity() {
			byte[] buf = new byte[8];
			byte[] atapi_cmd = new byte[12];
			atapi_cmd[0] = 0x25;	// READ CAPACITY
			int iosize = this.device.SendPacketCommand(buf, 0, 0, buf.Length, atapi_cmd);
			if(iosize!=buf.Length) throw new ATAPIException("Transferred data is too short.");
			return buf;
		}

		/// <summary>
		/// ATAPI READ10 コマンドを発行します。
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="index"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		public int Read10(byte[] buffer, int index, uint lba, ushort count, int sectorsize) {
			if(lba>0x4000) throw new ArgumentOutOfRangeException();
			byte[] atapi_cmd = new byte[12];
			atapi_cmd[0] = 0x28;				/* READ(10) */
			atapi_cmd[2] = (byte)(lba>>24);		/* 論理セクタ */
			atapi_cmd[3] = (byte)(lba>>16);
			atapi_cmd[4] = (byte)(lba>>8);
			atapi_cmd[5] = (byte)lba;
			atapi_cmd[7] = (byte)(count>>8);	/* 読み出しセクタ数 */
			atapi_cmd[8] = (byte)count;
			ushort datasize = checked((ushort)(count*sectorsize));
			int iosize = this.device.SendPacketCommand(buffer, index, 0, datasize, atapi_cmd);
			//Console.WriteLine("READ10 0x{0:X} bytes from #{1:X}h", iosize, lba);
			return iosize/sectorsize;
		}

		public bool IsMediaReady() {
			MediaAttributes attr = this.GetMediaAttributes();
			if(0!=(attr&MediaAttributes.MediaChanged)) {
				//TODO: 今までのメディアオブジェクトがあれば、それを破棄する。
			}
			return 0!=(attr&MediaAttributes.Ready);
		}

		public Storage GetMedia() {
			return new CdromMedia(this);
		}

	}

}
