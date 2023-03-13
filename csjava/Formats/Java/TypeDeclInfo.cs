using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.Java {

	public abstract class TypeDeclInfo {

		private readonly AssemblyDefInfo assembly;

		public TypeDeclInfo(AssemblyDefInfo assembly) {
			this.assembly = assembly;
		}

		public AssemblyDefInfo Assembly {
			get {
				return this.assembly;
			}
		}

		public abstract string Name {
			get;
		}

		public abstract string Namespace {
			get;
		}

	}

}
