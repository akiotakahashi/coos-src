using System;
using System.IO;

class HexToBin {
	static void Main(string[] args) {
		Console.Error.WriteLine("Type hex numbers separated by space.");
		BinaryWriter writer = new BinaryWriter(new MemoryStream());
		string line;
		while((line=Console.ReadLine())!=null) {
			if(line.Length==0) {
				if(writer.BaseStream.Position>0) {
					break;
				}
			} else {
				foreach(string hex in line.Split(' ')) {
					writer.Write(Convert.ToByte(hex,16));
				}
			}
		}
		((MemoryStream)writer.BaseStream).WriteTo(Console.OpenStandardOutput());
		writer.Close();
	}
}

