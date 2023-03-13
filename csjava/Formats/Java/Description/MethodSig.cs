using System;
using System.IO;
using System.Collections.Generic;

namespace CooS.Formats.Java.Description {
	
	public class MethodSig {

		private List<FieldSig> parameters;
		private FieldSig rettype;

		public MethodSig(string text) {
			Reader reader = new Reader(text);
			this.MethodDescriptor(reader);
		}

		public override string ToString() {
			System.Text.StringBuilder build = new System.Text.StringBuilder();
			build.Append('(');
			foreach(FieldSig sig in this.parameters) {
				build.Append(sig.ToString());
			}
			build.Append(')');
			build.Append(rettype.ToString());
			return build.ToString();
		}

		public void MethodDescriptor(Reader reader) {
			// ( ParameterDescriptor* ) ReturnDescriptor
			if('('!=reader.Read()) throw new BadImageException();
			while(reader.Peek()!=')') {
				FieldSig sig = ParameterDescriptor(reader);
				if(this.parameters==null) {
					this.parameters = new List<FieldSig>();
				}
				this.parameters.Add(sig);
			}
			reader.Read();
			rettype = ReturnDescriptor(reader);
		}

		private FieldSig ParameterDescriptor(Reader reader) {
			// FieldType
			FieldSig sig = new FieldSig();
			sig.FieldType(reader);
			return sig;
		}

		private FieldSig ReturnDescriptor(Reader reader) {
			// FieldType
			// V
			if(reader.Peek()=='V') {
				reader.Read();
				return null;
			} else {
				FieldSig sig = new FieldSig();
				sig.FieldType(reader);
				return sig;
			}
		}

		public int ParameterCount {
			get {
				if(this.parameters==null) {
					return 0;
				} else {
					return this.parameters.Count;
				}
			}
		}

		public IList<FieldSig> Parameters {
			get {
				return this.parameters;
			}
		}

		public bool IsVoid {
			get {
				return this.rettype==null;
			}
		}

		public FieldSig ReturnType {
			get {
				return this.rettype;
			}
		}

	}

}
