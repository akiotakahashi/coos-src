using System;
using System.IO;

class test {
	static void Main(string[] args) {
		foreach(string arg in args) {
			Console.WriteLine(Path.GetFileName(arg));
			Console.WriteLine(Path.GetDirectoryName(arg));
			Console.WriteLine(Path.GetFullPath(arg));
		}
	}
}
