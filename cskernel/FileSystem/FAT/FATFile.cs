using System;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	public class FATFile : FileAspect {

		FATEntity page;

		public FATFile(FATEntity page) {
			this.page = page;
		}

		#region FileAspect ƒƒ“ƒo

		public Page Page {
			get {
				return page;
			}
		}

		public long Length {
			get {
				return this.page.Length;
			}
		}

		public int ClusterSize {
			get {
				return this.page.ClusterSize;
			}
		}

		public void Read(byte[] buffer, int index, long start, int count) {
			this.page.Read(buffer,index,start,count);
		}

		#endregion

	}

}
