using System;
using System.Globalization;

class Hex2Dbl {
	public static void Main(string[] args) {
		string line;
		while((line=Console.ReadLine())!=null) {
			if(line.Length==0) break;
			string val = line;
			if(val.StartsWith("0x")) val=val.Substring(2);
			ulong value = ulong.Parse(val, NumberStyles.HexNumber);
			double dbl = BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
			Console.WriteLine("{0} = {1}", line, dbl);
		}
	}
}
