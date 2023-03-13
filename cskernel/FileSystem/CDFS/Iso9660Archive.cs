using System;
using System.IO;

namespace CooS.FileSystem.CDFS {

	public class Iso9660Archive : ArchiveBase {
	
		readonly Partition partition;
		readonly VolumeDescriptor vd;
		Book rootdir;

		public Iso9660Archive(Partition partition) {
			this.partition = partition;
			byte[] buf = new byte[this.partition.BlockSize];
			for(int ivd=0; ivd<32; ++ivd) {
				this.partition.Read(buf, 0, 16+ivd, 1);
				VolumeDescriptorBase vdb = (VolumeDescriptorBase)buf;
				switch(vdb.VolumeDerscriptorType) {
				case 0:
					// Boot Record
					break;
				case 1:
				case 2:
					this.vd = new VolumeDescriptor(buf);
					break;
				case 3:
					// Volume Partition Descriptor
					break;
				case 255:
					return;
				default:
					// future standardization
					break;
				}
			}
			// Timeout
			throw new NotFoundException("Volume-Descriptor-Set-Terminator Not Found");
		}

		protected override Book OpenRootBook() {
			if(rootdir==null) {
				rootdir = new Iso9660Directory(this.vd.RootDirectory, this.partition);
			}
			return rootdir;
		}

	}

}
