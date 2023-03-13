using System;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	public abstract class FATContainer : Page {
	
		public FATContainer() {
		}

		#region Page ƒƒ“ƒo

		public abstract int ClusterSize {get;}
		public abstract long ClusterCount {get;}
		public abstract long Length {get;}

		#endregion

		public abstract void Read(byte[] buffer, int index, long start, int count);

	}
}
