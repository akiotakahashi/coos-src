using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class interfacemethodref_info : memberref_info {

		public interfacemethodref_info(BinaryReader reader) : base(reader) {
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.InterfaceMethodref;
			}
		}

	}

}
