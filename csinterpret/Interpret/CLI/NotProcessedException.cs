using System;
using CooS.Formats.CLI.CodeModel;

namespace CooS.Interpret.CLI {

	class NotProcessedException : NotImplementedException {

		public NotProcessedException(Instruction inst) : base(inst.CoreName) {
		}

	}

}
