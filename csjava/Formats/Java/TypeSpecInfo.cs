using System;

namespace CooS.Formats.Java {
	using Description;

	public class TypeSpecInfo : TypeDeclInfo {

		private readonly FieldSig sig;

		public TypeSpecInfo(AssemblyDefInfo assembly, string description) : base(assembly) {
			this.sig = new FieldSig(description);
		}

		public override string Name {
			get {
				return this.sig.ToString();
			}
		}

		public override string Namespace {
			get {
				return null;
			}
		}

	}

}
