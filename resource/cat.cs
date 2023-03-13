using System;
using System.IO;


class cat {
	static void Main(string[] args) {
		using(TextReader reader = new StreamReader(args[0])) {
			string line;
			while((line=reader.ReadLine())!=null) {
				Console.WriteLine(line);
			}
		}
	}
}
