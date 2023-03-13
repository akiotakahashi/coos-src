using System;
using CooS.FileSystem;

namespace CooS.Drivers.StorageDevices.FDD {

	public class FloppyDisk : Storage {

		FloppyDiskDrive fdd;

		public FloppyDisk(FloppyDiskDrive fdd) {
			this.fdd = fdd;
		}

		public string PrefixName {
			get {
				return "fd";
			}
		}

		public int PartitionCount {
			get {
				return 1;
			}
		}

		FDPartition partition;

		public Partition GetPartition(int index) {
			if(index!=0) throw new ArgumentOutOfRangeException();
			if(partition==null) partition=new FDPartition(this);
			return partition;
		}

		class FDPartition : Partition {
			FloppyDisk fd;
			public FDPartition(FloppyDisk fd) {
				this.fd = fd;
			}
			public Storage Storage {
				get {
					return fd;
				}
			}
			public int BlockSize {
				get {
					return 512;
				}
			}
			public long BlockCount {
				get {
					return 0x0b40;
				}
			}
			public void Read(byte[] buffer, int index, long start, int count) {
				fd.fdd.Seek((uint)start);
				fd.fdd.Read(buffer, index, (uint)count);
			}
		}

	}

}
