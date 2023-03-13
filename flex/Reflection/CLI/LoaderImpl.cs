using System;
using CooS.Formats.DLL;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {

	class LoaderImpl : ILoader {

		#region ILoader �����o

		public AssemblyBase LoadAssembly(System.IO.Stream stream) {
			AssemblyDefInfo asmdef = AssemblyDefInfo.LoadAssembly(stream);
			return new AssemblyImpl(asmdef);
		}

		#endregion

	}

}
