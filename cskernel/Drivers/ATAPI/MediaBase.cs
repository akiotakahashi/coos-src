using System;

namespace CooS.Drivers.ATAPI {

	public abstract class MediaBase {

		protected MediaBase() {
		}
	
		public abstract int SectorSize {get;}
		public abstract uint SectorCount {get;}
		public abstract int MaxReadableCount {get;}
		protected abstract int ReadSector(byte[] buf, int index, uint lba, ushort count);

		public void Read(byte[] buffer, int index, int start, int count) {
			int maxcount = this.MaxReadableCount;
			while(count>0) {
				int iocount = (count>maxcount) ? maxcount : count;
				//Console.WriteLine("READ {0,3} SECTORS FROM #{1}", iocount, start);
				iocount = this.ReadSector(buffer,index,(uint)start,(ushort)iocount);
				if(iocount<=0) throw new ATAPIException();
				index += iocount*SectorSize;
				start += iocount;
				count -= iocount;
			}
		}
	
	}

}
