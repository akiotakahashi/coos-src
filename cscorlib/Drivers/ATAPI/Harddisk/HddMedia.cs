using System;

namespace CooS.Drivers.ATAPI.Harddisk {

	public class HddMedia : MediaBase {
	
		//private readonly HarddiskDevice device;
		//private readonly int sectorcount;

		public HddMedia(HarddiskDevice device) {
			throw new NotImplementedException();
#if false
			this.device = device;
			byte[] idbuf = this.IdentityData;
			mi.LBA
				= (uint)(idbuf[123]<<24)
				| (uint)(idbuf[122]<<16)
				| (uint)(idbuf[121]<<8)
				| (uint)idbuf[120];
			return mi;
#endif
			}
		
		public override int MaxReadableCount {
			get {
				return 0x7FFF;
			}
		}

		public override int SectorSize {
			get {
				return 512;
			}
		}

		public override uint SectorCount {
			get {
				return 0;
			}
		}

		protected override int ReadSector(byte[] buf, int index, uint lba, ushort count) {
			return 0;
		}

	
	}

}
