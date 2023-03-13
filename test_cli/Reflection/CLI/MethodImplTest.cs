using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

namespace CooS.Reflection.CLI.Test {
	using CooS.Formats.CLI;

	public class MethodImplTest {

		private AssemblyDefInfo LoadAssembly(World world, Stream stream) {
			AssemblyDefInfo asmdef = AssemblyDefInfo.LoadAssembly(stream);
			return new AssemblyImpl(world, asmdef);
		}

		public void Setup() {
			LoaderImpl loader = new CooS.Reflection.CLI.LoaderImpl();
		}

	}

}
