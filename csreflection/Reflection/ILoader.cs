using System;
using System.IO;

namespace CooS.Reflection {

	public interface ILoader {

		AssemblyBase LoadAssembly(World world, Stream stream);
		AssemblyBase LoadAssembly(World world, string path);

	}

}
