using System;

namespace CooS.Formats.Java {
	using Metadata;
	using Description;

	public class MethodDefInfo : MemberDeclInfo {

		private readonly TypeDefInfo type;
		private readonly method_info info;
		private readonly int index;
		private readonly MethodSig sig;
		private readonly TypeDeclInfo[] paramtypes;

		public MethodDefInfo(TypeDefInfo type, method_info method, int index) {
			this.type = type;
			this.info = method;
			this.index = index;
			this.sig = new MethodSig(((utf8_info)this.type.ConstantPool[this.info.descriptor_index]).Text);
			this.paramtypes = new TypeDeclInfo[this.sig.ParameterCount];
		}

		public AssemblyDefInfo Assembly {
			get {
				return this.type.Assembly;
			}
		}

		public TypeDefInfo Type {
			get {
				return this.type;
			}
		}

		public int Index {
			get {
				return this.index;
			}
		}

		public string Name {
			get {
				return this.type.LoadString(this.info.name_index);
			}
		}

		public bool IsStatic {
			get {
				throw new NotImplementedException();
			}
		}

		public TypeDeclInfo ReturnType {
			get {
				return this.Assembly.LoadType(this.sig.ReturnType);
			}
		}

		public int ParameterCount {
			get {
				return this.sig.ParameterCount;
			}
		}

		public TypeDeclInfo GetParameterType(int index) {
			if(this.paramtypes[index]!=null) {
				return this.paramtypes[index];
			} else {
				return this.paramtypes[index] = this.Assembly.LoadType(this.sig.Parameters[index]);
			}
		}

	}

}
