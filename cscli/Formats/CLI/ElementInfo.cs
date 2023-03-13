using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {

	abstract class ElementInfo {

		public abstract AssemblyDefInfo Assembly {
			get;
		}

		public abstract int RowIndex {
			get;
		}

	}

}
