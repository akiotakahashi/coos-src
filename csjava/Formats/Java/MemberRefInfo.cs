using System;
using System.Collections.Generic;

namespace CooS.Formats.Java {
	using Metadata;

	public class MemberRefInfo : MemberDeclInfo {

		private readonly TypeDefInfo type;
		private readonly memberref_info info;

		public MemberRefInfo(TypeDefInfo type, memberref_info info) {
			this.type = type;
			this.info = info;
		}

		public string Class {
			get {
				class_info ci = (class_info)this.type.ConstantPool[this.info.class_index];
				return this.type.LoadString(ci.name_index);
			}
		}

		public string Name {
			get {
				nameandtype_info nat = (nameandtype_info)this.type.ConstantPool[this.info.name_and_type_index];
				return this.type.LoadString((string_info)this.type.ConstantPool[nat.name_index]);
			}
		}

		public string Signature {
			get {
				nameandtype_info nat = (nameandtype_info)this.type.ConstantPool[this.info.name_and_type_index];
				return this.type.LoadString((string_info)this.type.ConstantPool[nat.descriptor_index]);
			}
		}

	}

}
