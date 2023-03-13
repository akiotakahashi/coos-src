using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI.Signature {

	public class ArrayShape {

		public readonly int rank;
		public readonly int[] sizes;
		public readonly int[] lbounds;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal ArrayShape(SignatureReader reader) {
			rank = reader.ReadInt32();
			sizes = new int[reader.ReadInt32()];
			for(int i=0; i<sizes.Length; ++i) {
				sizes[i] = reader.ReadInt32();
			}
			lbounds = new int[reader.ReadInt32()];
			for(int i=0; i<lbounds.Length; ++i) {
				lbounds[i] = reader.ReadInt32();
			}
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new ArrayShape(reader);
			}

		}

	}

}
