using System;
using System.Collections;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	public class FATFileSystem : FileSystemProvider {

		public FATFileSystem() {
		}

		public override string Name {
			get {
				return "FAT";
			}
		}

		public override string PreferredPrefix {
			get {
				return "fat";
			}
		}

		public override Archive Bind(Partition partition) {
			return new FATArchive(partition);
		}

		public override bool IsSuitable(Partition partition) {
			if(partition.BlockSize!=512) return false;
			return true;
		}

	};

}
