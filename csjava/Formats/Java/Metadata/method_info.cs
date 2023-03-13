using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class method_info : member_info {
		
		public method_info(ClassFile file, BinaryReader reader) : base(file, reader) {
			// The only attributes defined by this specification as appearing
			// in the attributes table of a method_info structure are the:
			//		Code (Åò4.7.3),
			//		Exceptions (Åò4.7.4),
			//		Synthetic (Åò4.7.6),
			//		Deprecated (Åò4.7.10) attributes. 
		}

		public override void Dump() {
			base.Dump();
			this.Code.Dump(this.File);
		}

		public code_attribute Code {
			get {
				attribute_info attr = this["Code"];
				return new code_attribute(attr);
			}
		}

	}

}
