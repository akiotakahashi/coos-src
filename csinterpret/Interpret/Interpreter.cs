using System;
using CooS.Reflection;

namespace CooS.Interpret {

	public abstract class Interpreter {

		public abstract Flow Execute(MethodBase method);

	}

}
