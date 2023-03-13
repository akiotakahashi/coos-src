using System;
using System.IO;

class ls {
	static void Main(string[] args) {
		DirectoryInfo dir;
		if(args.Length==0) {
			dir = new DirectoryInfo(Directory.GetCurrentDirectory());
		} else {
			dir = new DirectoryInfo(args[0]);
		}
		foreach(FileSystemInfo fsi in dir.GetFileSystemInfos()) {
			Console.Write("{0,-32} ", fsi.Name);
			if(fsi is FileInfo) {
				FileInfo fi = (FileInfo)fsi;
				Console.Write("{0,10} B", fi.Length);
			} else {
				Console.Write("{0,10} -","");
			}
			Console.WriteLine();
		}
	}
}
