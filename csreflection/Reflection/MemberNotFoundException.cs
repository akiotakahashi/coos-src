using System;

namespace CooS.Reflection {

	/// <summary>
	/// MemberNotFoundExcption ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
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
