using System;
using System.IO;

namespace CooS.FileSystem.CDFS {

	public struct u32b {
	
		public uint l;
		public uint m;

		public u32b(BinaryReader reader) {
			this.l = reader.ReadUInt32();
			this.m = reader.ReadUInt32();
		}

		public static implicit operator uint(u32b op) {
			return op.l;
		}

	}

}
