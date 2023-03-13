using System;
using CooS.Formats.Java;

namespace CooS.Reflection.Java {

	public class SimpleLoader : ILoader {

		public SimpleLoader() {
		}

		#region ILoader ÉÅÉìÉo

		public AssemblyBase LoadAssembly(World world, System.IO.Stream stream) {
			ClassFile file = new ClassFile(stream);
			AssemblyDefInfo assembly = new AssemblyDefInfo("TestJ", new ClassFile[] { file });
			return new AssemblyImpl(world, assembly);
		}

		public AssemblyBase LoadAssembly(World world, string path) {
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

	}

}
