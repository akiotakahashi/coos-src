using System;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public struct Utf8Index {
		
		public u2 Value;

		public Utf8Index(u2 value) {
			this.Value = value;
		}

		public static implicit operator u2(Utf8Index ni) {
			return ni.Value;
		}

		public static implicit operator Utf8Index(u2 value) {
			return new Utf8Index(value);
		}

	}

}
