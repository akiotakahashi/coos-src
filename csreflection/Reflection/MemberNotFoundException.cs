using System;

namespace CooS.Reflection {

	/// <summary>
	/// MemberNotFoundExcption の概要の説明です。
	/// </summary>
	public class MemberNotFoundException : NotFoundException {

		public MemberNotFoundException() {
		}

		public MemberNotFoundException(string message) : base("Member Not Found: "+message) {
		}

		public MemberNotFoundException(MemberRefDesc desc) : base(desc.ToString()) {
		}

	}

}
