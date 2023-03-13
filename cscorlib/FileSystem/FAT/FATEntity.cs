using System;
using System.IO;
using System.Collections;

namespace CooS.FileSystem.FAT {

	public class FATEntity : FATContainer, Book {

		readonly FATArchive archive;
		readonly DirectoryEntry entry;
		readonly uint[] clusterchain;
		readonly string filename;

		public FATEntity(FATArchive archive, DirectoryEntry entry, string filename) {
			this.archive = archive;
			this.entry = entry;
			uint cluster = entry.FirstCluster;
			ArrayList clusters = new ArrayList();
			while(!archive.IsEOC(cluster)) {
				clusters.Add(cluster);
				cluster = archive.GetFATEntry(cluster);
			}
			this.clusterchain = (uint[])clusters.ToArray(typeof(uint));
			this.filename = filename;
		}

		public FATArchive Archive {
			get {
				return this.archive;
			}
		}

		#region Book ÉÅÉìÉo

		public BookInfo BookInfo {
			get {
				return new BookInfo(filename,FileAttributes.Directory);
			}
		}

		public Aspect QueryAspects(Type aspect) {
			if(0!=(this.entry.attributes&FATAttributes.Directory)) {
				if(aspect==typeof(DirectoryAspect)) {
					return new FATDirectory(this);
				}
			} else {
				if(aspect==typeof(FileAspect)) {
					return new FATFile(this);
				}
			}
			return null;
		}

		#endregion

		#region Page ÉÅÉìÉo

		public override int ClusterSize {
			get {
				return this.archive.ClusterSize;
			}
		}

		public override long ClusterCount {
			get {
				return clusterchain.Length;
			}
		}

		public override long Length {
			get {
				return entry.fileSize;
			}
		}

		#endregion

		public override void Read(byte[] buffer, int index, long start, int count) {
			for(long ci=start; ci<start+count; ++ci) {
				//Console.WriteLine("Reading cluster#{0} to {1}", clusterchain[ci], index);
				archive.ReadCluster(buffer, index, clusterchain[ci]);
				index += this.archive.ClusterSize;
			}
		}

	}

}
