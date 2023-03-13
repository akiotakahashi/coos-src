using System;
using System.IO;
using System.Collections;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	public class FATRootDirectory : FATContainer, Book, Page {
	
		readonly FATArchive archive;
		readonly byte[] dirdata;

		public FATRootDirectory(FATArchive archive) {
			this.archive = archive;
			this.dirdata = new byte[archive.ReadRootDirectoryData(null,0)];
			archive.ReadRootDirectoryData(dirdata, (uint)dirdata.Length);
		}

		public FATArchive Archive {
			get {
				return this.archive;
			}
		}

		public override int ClusterSize {
			get {
				return archive.ClusterSize;
			}
		}

		public override long ClusterCount {
			get {
				int bpc = archive.ClusterSize;
				return (this.Length+bpc-1)/bpc;
			}
		}

		public override long Length {
			get {
				return dirdata.Length;
			}
		}

		public override void Read(byte[] buffer, int index, long start, int count) {
			int bpc = archive.ClusterSize;
			Buffer.BlockCopy(dirdata, (int)(start*bpc), buffer, index, count*bpc);
		}

		#region Book ÉÅÉìÉo

		public BookInfo BookInfo {
			get {
				return BookInfo.RootDirectoryInfo;
			}
		}

		public Aspect QueryAspects(Type aspect) {
			if(aspect==typeof(DirectoryAspect)) {
				return new FATDirectory(this);
			} else {
				return null;
			}
		}

		#endregion

	}

}
