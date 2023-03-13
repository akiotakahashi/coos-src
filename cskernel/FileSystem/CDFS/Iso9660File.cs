using System;

namespace CooS.FileSystem.CDFS {

	public class Iso9660File : Iso9660Item, FileAspect {

		private readonly Iso9660Directory directory;

		public Iso9660File(DirectoryRecord record, Partition partition, Iso9660Directory directory) : base(record,partition) {
			this.directory = directory;
		}

		public override Aspect QueryAspects(Type aspect) {
			if(aspect==typeof(FileAspect)) {
				return this;
			} else {
				return null;
			}
		}

		#region FileAspect ÉÅÉìÉo

		public long Length {
			get {
				return this.record.DataLength;
			}
		}

		public int ClusterSize {
			get {
				return 2048;
			}
		}

		public void Read(byte[] buffer, int index, long start, int count) {
			base.ReadExtent(buffer, index, start, count);
		}

		#endregion

	}
}
