using System;

namespace CooS.FileSystem.CDFS {

	public class Iso9660 : FileSystemProvider {

		public override string Name {
			get {
				return "CDFS";
			}
		}

		public override string PreferredPrefix {
			get {
				return "cd";
			}
		}

		public override bool IsSuitable(Partition partition) {
			if(partition.BlockSize!=2048) return false;
			return true;
		}

		public override Archive Bind(Partition partition) {
			return new Iso9660Archive(partition);
		}

	}

}
