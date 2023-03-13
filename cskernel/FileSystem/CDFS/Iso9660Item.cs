using System;

namespace CooS.FileSystem.CDFS {

	public abstract class Iso9660Item : Book, Page {
	
		protected readonly DirectoryRecord record;
		protected readonly Partition partition;

		protected Iso9660Item(DirectoryRecord record, Partition partition) {
			this.record = record;
			this.partition = partition;
		}

		#region Book ÉÅÉìÉo

		public BookInfo BookInfo {
			get {
				return this.record.ToBookInfo();
			}
		}

		public abstract Aspect QueryAspects(Type aspect);

		#endregion

		#region Aspect ÉÅÉìÉo

		public Page Page {
			get {
				return this;
			}
		}

		#endregion

		protected void ReadExtent(byte[] buf, int index, long start, int count) {
			this.partition.Read(buf, index, this.record.LocationOfExtent+start, count);
		}

	}

}
