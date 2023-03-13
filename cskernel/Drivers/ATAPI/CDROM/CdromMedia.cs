using System;
using CooS.FileSystem;

namespace CooS.Drivers.ATAPI.CDROM {

	public class CdromMedia : MediaBase, Storage, Partition {

		private readonly CdromDevice device;
		private readonly int sectorsize;
		private readonly uint sectorcount;

		public CdromMedia(CdromDevice device) {
			this.device = device;
			byte[] buffer = this.device.ReadCapacity();
			this.sectorsize
				= (int)buffer[4]<<24
				| (int)buffer[5]<<16
				| (int)buffer[6]<<8
				| (int)buffer[7];
			this.sectorcount
				= (uint)buffer[0]<<24
				| (uint)buffer[1]<<16
				| (uint)buffer[2]<<8
				| (uint)buffer[3];
			if(this.sectorsize!=2048) {
				Console.WriteLine("CD-ROMメディアのセクタサイズが2048バイトではありません。");
				this.sectorsize = 2048;
			}
		}

		public override int SectorSize {
			get {
				return this.sectorsize;
			}
		}

		public override uint SectorCount {
			get {
				return this.sectorcount;
			}
		}

		public override int MaxReadableCount {
			get {
				return 0x7FFF/this.SectorSize;
			}
		}

		protected override int ReadSector(byte[] buf, int index, uint lba, ushort count) {
			this.device.StartUnit(CdromDevice.ATAPI_START);
			//Console.WriteLine("EXECUTE TO READ {0} SECTORS FROM #{1:X}h TO BUFFER #{2:X}h", count, lba, index);
			return this.device.Read10(buf, index, lba, count, this.SectorSize);
		}

		#region Storage メンバ

		public string PrefixName {
			get {
				return "cd";
			}
		}

		public int PartitionCount {
			get {
				return 1;
			}
		}

		public Partition GetPartition(int index) {
			return this;
		}

		#endregion

		#region Partition メンバ

		public Storage Storage {
			get {
				return this;
			}
		}

		public int BlockSize {
			get {
				return this.sectorsize;
			}
		}

		public long BlockCount {
			get {
				return this.sectorcount;
			}
		}

		public void Read(byte[] buffer, int index, long start, int count) {
			base.Read(buffer, index, (int)start, count);
		}

		#endregion

	}

}
