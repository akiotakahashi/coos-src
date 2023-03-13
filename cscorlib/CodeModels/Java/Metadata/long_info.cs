using System;
using System.IO;

namespace CooS.CodeModels.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class long_info : cp_info {
	
		//public u4 high_bytes;
		//public u4 low_bytes;
		public ulong Value;

		public long_info(BinaryReader reader) {
			//this.high_bytes = reader.ReadUInt32();
			//this.low_bytes = reader.ReadUInt32();
			this.Value = reader.ReadUInt64();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Long;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("Long: {0}", this.Value);
		}

	}

}
