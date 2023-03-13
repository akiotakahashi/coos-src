using System;
using System.Collections.Generic;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;

	sealed class GenericParamInfo : TypeDeclInfo {

		public readonly GenericSources SourceType;
		public readonly int Number;

		internal GenericParamInfo(AssemblyDefInfo assembly, TypeSig sig) : base(assembly) {
			switch(sig.ElementType) {
			case ElementType.Var:
				this.SourceType = GenericSources.Type;
				break;
			case ElementType.MVar:
				this.SourceType = GenericSources.Method;
				break;
			default:
				throw new ArgumentException();
			}
			this.Number = sig.Number;
		}

		public override int RowIndex {
			get {
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Name {
			get {
				switch(this.SourceType) {
				case GenericSources.Type:
					return "!"+this.Number;
				case GenericSources.Method:
					return "!!"+this.Number;
				default:
					throw new UnexpectedException();
				}
			}
		}

		public override string Namespace {
			get {
				return null;
			}
		}

		public override bool IsGenericType {
			get {
				return false;
			}
		}

		public override bool IsGenericParam {
			get {
				return true;
			}
		}

	}

}
