using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class utf8_info : cp_info {
	
		//public u2 length;
		public u1[] bytes;
		
		public utf8_info(BinaryReader reader) {
			u2 length = reader.ReadUInt16();
			this.bytes = reader.ReadBytes(length);
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Utf8;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("Utf8: {0}", this.Text);
		}

		public string Text {
			get {
				return System.Text.Encoding.UTF8.GetString(this.bytes);
			}
		}

	}

}
