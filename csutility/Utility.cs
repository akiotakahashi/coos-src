using System;
using System.IO;

namespace CooS {

	public class Utility {

		#region コンソール出力

		public unsafe static void Dump(TextWriter writer, IntPtr address, int start, int count) {
			byte* p = (byte*)address.ToPointer();
			for(int i=0; i<count; ++i) {
				if(i%16==0) {
					writer.Write("{0:X8}| ", address.ToInt32()+start+i);
				}
				writer.Write("{0:X2} ", p[start+i]);
				if(i%16==15) {
					writer.WriteLine();
				}
			}
			if(count%16>0) {
				writer.WriteLine();
			}
		}

		public static void Dump(TextWriter writer, byte[] buf, int start, int count) {
			for(int i=0; i<count; ++i) {
				if(i%16==0) {
					writer.Write("{0:X4}| ", start+i);
				}
				writer.Write("{0:X2} ", buf[start+i]);
				if(i%16==15) {
					writer.WriteLine();
				}
			}
			if(count%16>0) {
				writer.WriteLine();
			}
		}

		public static void Dump(TextWriter writer, byte[] buf) {
			Dump(writer, buf, 0, buf.Length);
		}

		public static unsafe void Output(char* p) {
			string msg = new string(p);
			Console.Error.Write(msg);
		}

		#endregion

	}

}
