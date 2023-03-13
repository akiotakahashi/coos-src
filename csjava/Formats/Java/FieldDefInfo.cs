using System;

namespace CooS.Formats.Java {
	using Metadata;

	public class FieldDefInfo : MemberDeclInfo {

		private readonly TypeDefInfo type;
		private readonly field_info info;
		private readonly int index;

		public FieldDefInfo(TypeDefInfo type, field_info info, int index) {
			this.type = type;
			this.info = info;
			this.index = index;
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

		public TypeDeclInfo FieldType {
			get {
				throw new NotImplementedException();
			}
		}

	}

}
