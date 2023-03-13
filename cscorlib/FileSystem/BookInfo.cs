using System;
using System.IO;

namespace CooS.FileSystem {

	public class BookInfo {

		public static readonly BookInfo RootDirectoryInfo = new BookInfo(PathInfo.RootDirectoryName, FileAttributes.Directory);

		public readonly string Name;
		public readonly long Length;
		public readonly FileAttributes Attributes;
		public readonly DateTime CreationTime;
		public readonly DateTime LastAccessTime;
		public readonly DateTime LastWriteTime;

		public BookInfo(string name, FileAttributes attr, long length, DateTime CreationTime, DateTime LastWriteTime, DateTime LastAccessTime) {
			this.Name = name;
			this.Length = length;
			this.Attributes = attr;
			this.CreationTime = CreationTime;
			this.LastWriteTime = LastWriteTime;
			this.LastAccessTime = LastAccessTime;
		}

		public BookInfo(string name, FileAttributes attr) {
			this.Name = name;
			this.Length = -1;
			this.Attributes = attr;
			this.CreationTime = DateTime.MinValue;
			this.LastWriteTime = DateTime.MinValue;
			this.LastAccessTime = DateTime.MinValue;
		}

		public MonoIOStat ToMonoIOStat() {
			MonoIOStat stat;
			stat.Name = this.Name;
			stat.Length = this.Length;
			stat.Attributes = this.Attributes;
			stat.CreationTime = this.CreationTime.ToFileTimeUtc();
			stat.LastWriteTime = this.LastWriteTime.ToFileTimeUtc();
			stat.LastAccessTime = this.LastAccessTime.ToFileTimeUtc();
			return stat;
		}

	}

}
