using System;
using System.IO;

namespace CooS.Reflection {

	public interface ILoader {

		AssemblyBase LoadAssembly(Stream stream);

	}

}
