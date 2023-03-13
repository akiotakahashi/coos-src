using System;

namespace CooS.Formats.CLI.Signature {

	public abstract class SignatureFactory {

		internal abstract object Parse(SignatureReader reader);

	}

}
