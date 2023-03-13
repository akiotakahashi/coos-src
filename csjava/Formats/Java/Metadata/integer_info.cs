using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class integer_info : cp_info {

		public u4 bytes;

		public integer_info(BinaryReader reader) {
			this.bytes = reader.ReadUInt32();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Integer;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("Integer: {0}", this.bytes);
		}

	}

}
