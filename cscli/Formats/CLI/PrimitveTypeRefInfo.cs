using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {
	using Metadata;

	sealed class PrimitiveTypeRefInfo : TypeRefInfo {

		private AssemblyRefInfo mscorlib;
		private string name;
		private string ns;

		internal PrimitiveTypeRefInfo(AssemblyDefInfo assembly, AssemblyRefInfo mscorlib, string name, string ns) : base(assembly) {
			this.mscorlib = mscorlib;
			this.name = name;
			this.ns = ns;
		}

		public override int RowIndex {
			get {
				throw new NotSupportedException();
			}
		}

		public override string Name {
			get {
				return this.name;
			}
		}

		public override string Namespace {
			get {
				return this.ns;
			}
		}

		public override bool IsNested {
			get {
				return false;
			}
		}

		public override IResolutionScope ResolutionScope {
			get {
				return this.mscorlib;
			}
		}

	}

}
