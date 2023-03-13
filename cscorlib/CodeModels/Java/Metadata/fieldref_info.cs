using System;
using System.IO;

namespace CooS.CodeModels.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class fieldref_info : memberref_info {
		
		public fieldref_info(BinaryReader reader) : base(reader) {
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Fieldref;
			}
		}

	}

}
