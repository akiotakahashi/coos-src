using System;
using CooS.Execution;

namespace CooS.Architectures.IA32 {

	public class ArchitectureImpl : Architecture {

		public override int AddressSize {
			get {
				return 4;
			}
		}

	}

}
