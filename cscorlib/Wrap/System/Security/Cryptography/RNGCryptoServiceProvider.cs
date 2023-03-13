using System;
using System.Collections;

namespace CooS.Wrap._System.Security.Cryptography {

	public class _RNGCryptoServiceProvider {

		static Hashtable table = new Hashtable();
		static int nextidx = 1;

		private static bool RngOpen() {
			return true;
		}

		private static void RngClose(IntPtr handle) {
			table.Remove(handle);
		}

		private static IntPtr RngInitialize(byte[] seed) {
			IntPtr handle = new IntPtr(nextidx++);
			uint[] key;
			if(seed!=null) {
				key = new uint[(seed.Length+3)&~3];
				Buffer.BlockCopy(seed, 0, key, 0, seed.Length);
			} else {
				key = new uint[]{(uint)(DateTime.Now-new DateTime(1981,1,1)).TotalSeconds};
			}
			CooS.MersenneTwister mt = new CooS.MersenneTwister(key);
			table[handle] = mt;
			return handle;
		}

		private static unsafe IntPtr RngGetBytes(IntPtr handle, byte[] data) {
			CooS.MersenneTwister mt = (CooS.MersenneTwister)table[handle];
			if(mt==null) throw new ArgumentException("Not found MT object");
			fixed(byte* p = &data[0]) {
				byte* q = p;
				int i = 0;
				while(i<data.Length-3) {
					*(uint*)q = mt.Generate();
					q += 4;
					i += 4;
				}
				int rest = data.Length-i;
				if(rest>0) {
					uint n = mt.Generate();
					switch(rest) {
					case 3:
						*(q++) = (byte)n;
						n >>= 8;
						goto case 2;
					case 2:
						*(ushort*)q = (ushort)n;
						break;
					case 1:
						*q = (byte)n;
						break;
					default:
						throw new InvalidOperationException();
					}
				}
			}
			return handle;
		}

	}

}
