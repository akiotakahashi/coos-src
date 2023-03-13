using System;
using System.IO;

namespace CooS.CodeModels.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class float_info : cp_info {
		
		//public u4 bytes;
		public float Value;

		public float_info(BinaryReader reader) {
			//this.bytes = reader.ReadUInt32();
			this.Value = reader.ReadSingle();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Float;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("Float: {0}", this.Value);
		}

	}

}
