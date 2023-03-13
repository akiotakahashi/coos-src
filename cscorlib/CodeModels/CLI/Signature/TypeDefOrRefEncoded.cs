using System;
using CooS.CodeModels.CLI.Metatype;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	struct TypeDefOrRefEncoded {

		MDToken token;

		public TypeDefOrRefEncoded(SignatureReader reader) {
			token = TabsDecoder.DecodeToken(CodedTokenId.TypeDefOrRef, reader.ReadInt32());
		}

		public override string ToString() {
			return token.ToString();
		}


		public MDToken Token {
			get {
				return this.token;
			}
		}

		public SuperType ResolveTypeAt(AssemblyDef assembly) {
			return assembly.ResolveType(this.token);
		}

	}

}
