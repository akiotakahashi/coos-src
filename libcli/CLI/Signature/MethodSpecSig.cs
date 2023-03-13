using System;

namespace CooS.Formats.CLI.Signature {

	public class MethodSpecSig {

		const byte GENERICINST = 0x0A;

		private readonly TypeSig[] types;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal MethodSpecSig(SignatureReader reader) {
			if(GENERICINST!=reader.ReadByte()) {
				reader.Dump(Console.Out);
				throw new BadSignatureException("MethodSpecSig has no GENERICINST marker");
			}
			int args = reader.ReadInt32();
			this.types = new TypeSig[args];
			for(int i=0; i<args; ++i) {
				this.types[i] = new TypeSig(reader);
			}
		}

		public int GenericArgumentCount {
			get {
				return types.Length;
			}
		}

		public TypeSig GetGenericArgumentType(int index) {
			return this.types[index];
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new MethodSpecSig(reader);
			}

		}

	}

}
