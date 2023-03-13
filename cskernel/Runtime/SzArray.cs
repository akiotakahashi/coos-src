using System;
using CooS.Execution;

namespace CooS.Runtime {

	public abstract class SzArray : Array {

		internal TypeInfo ElementType;
		internal uint ElementSize;
		internal uint Length;

	}

}
