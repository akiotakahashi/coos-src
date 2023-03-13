using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class double_info : cp_info {
		
		//public u4 high_bytes;
		//public u4 low_bytes;
		public double Value;
		
		public double_info(BinaryReader reader) {
			//this.high_bytes = reader.ReadUInt64();
			//this.low_bytes = reader.ReadUInt32();
			this.Value = reader.ReadDouble();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Double;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("Double: {0}", this.Value);
		}

	}

}
