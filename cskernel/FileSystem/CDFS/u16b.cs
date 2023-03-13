using System;
using System.IO;

namespace CooS.FileSystem.CDFS {

	public struct u16b {

		public ushort l;
		public ushort m;

		public u16b(BinaryReader reader) {
			this.l = reader.ReadUInt16();
			this.m = reader.ReadUInt16();
		}

		public static implicit operator ushort(u16b op) {
			return op.l;
		}

	}

}
