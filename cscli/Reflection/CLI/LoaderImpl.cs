using System;
using System.IO;
using CooS.Formats.DLL;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {

	public class LoaderImpl : ILoader {

		public LoaderImpl() {
		}

		#region ILoader ÉÅÉìÉo

		public AssemblyBase LoadAssembly(World world, Stream stream) {
			AssemblyDefInfo asmdef = AssemblyDefInfo.LoadAssembly(stream);
			return new AssemblyImpl(world, asmdef);
		}

		public AssemblyBase LoadAssembly(World world, string path) {
			if(!File.Exists(path)) throw new FileNotFoundException(path);
			Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			try {
				return this.LoadAssembly(world, stream);
			} catch {
				stream.Dispose();
				throw;
			}
		}

		#endregion

	}

}
