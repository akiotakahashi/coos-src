using System;
using System.Reflection;
using System.Globalization;

namespace CooS.Reflection {
	
	public interface IDomain {

		AssemblyBase ResolveAssembly(string name, CultureInfo culture, Version version);

	}

}
