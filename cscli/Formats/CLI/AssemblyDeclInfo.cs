using System;
using System.Reflection;
using System.Globalization;

namespace CooS.Formats.CLI {

	abstract class AssemblyDeclInfo {

		protected AssemblyDeclInfo() {
		}

		public abstract string Name {
			get;
		}

		public abstract CultureInfo Culture {
			get;
		}

		public abstract Version Version {
			get;
		}

		public abstract AssemblyName AssemblyName {
			get;
		}
	
	}

}
