using System;
using MemberTypes=System.Reflection.MemberTypes;

namespace CooS.Formats.CLI {

	abstract class MemberDeclInfo : ElementInfo {

		private readonly AssemblyDefInfo assembly;

		protected MemberDeclInfo(AssemblyDefInfo assembly) {
			this.assembly = assembly;
		}

		public override AssemblyDefInfo Assembly {
			get {
				return this.assembly;
			}
		}

		public abstract MemberTypes Kind {
			get;
		}

		public abstract string Name {
			get;
		}

		public abstract TypeDeclInfo Type {
			get;
		}

		public abstract bool ContainsGenericParameters {
			get;
		}

	}

}
