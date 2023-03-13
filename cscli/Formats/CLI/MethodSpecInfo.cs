using System;
using System.Collections.Generic;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;
	using MethodAttributes=System.Reflection.MethodAttributes;
	using MethodImplAttributes=System.Reflection.MethodImplAttributes;

	class MethodSpecInfo : MethodDeclInfo {

		private MethodSpecRow row;
		private MethodSpecSig sig;

		internal MethodSpecInfo(AssemblyDefInfo assembly, MethodSpecRow row) : base(assembly) {
			this.row = row;
			this.sig = (MethodSpecSig)this.Assembly.LoadSignature(this.row.Instantiation, MethodSpecSig.Factory);
			if(this.sig.GenericArgumentCount!=this.GenericParameterCount) {
				throw new ArgumentException("Counts of Generic parameter are different.");
			}
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		public int Index {
			get {
				return this.RowIndex-1;
			}
		}

		public override TypeDeclInfo Type {
			get {
				return this.Method.Type;
			}
		}

		public override string Name {
			get {
				return this.Method.Name;
			}
		}

		internal override MethodSig Signature {
			get {
				return this.Method.Signature;
			}
		}

		public MethodDeclInfo Method {
			get {
				return this.Assembly.GetMethod(this.row.Method);
			}
		}

		public TypeDeclInfo GetGenericArgumentType(int index) {
			return this.Assembly.LookupType(this.sig.GetGenericArgumentType(index));
		}

		public IEnumerable<TypeDeclInfo> EnumGenericArgumentTypes() {
			for(int i=0; i<this.GenericParameterCount; ++i) {
				yield return this.GetGenericArgumentType(i);
			}
		}

	}

}
