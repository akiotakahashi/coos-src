using System;
using System.IO;
using System.Collections.Generic;

namespace CooS.Reflection {
	
	public sealed class DomainImpl : IDomain {

		private static readonly Dictionary<string, ILoader> loaders = new Dictionary<string,ILoader>();
		private static readonly List<AssemblyBase> assemblies = new List<AssemblyBase>();

		static DomainImpl() {
			loaders["CLI"] = new CLI.LoaderImpl();
		}

		#region IDomain ÉÅÉìÉo

		public AssemblyBase ResolveAssembly(string name) {
			return ResolveAssembly(name, null, ZeroVersion);
		}

		public AssemblyBase ResolveAssembly(string name, System.Globalization.CultureInfo culture) {
			return ResolveAssembly(name, culture, ZeroVersion);
		}

		public AssemblyBase ResolveAssembly(string name, System.Globalization.CultureInfo culture, Version version) {
			foreach(AssemblyBase assembly in assemblies) {
				if(IsMatch(assembly, name, culture, version)) {
					return assembly;
				}
			}
			return null;
		}

		private static Version ZeroVersion = new Version();

		private bool IsMatch(AssemblyBase assembly, string name, System.Globalization.CultureInfo culture, Version version) {
			if(assembly.Name!=name) {
				return false;
			}
			if(culture!=null && !culture.IsNeutralCulture && !assembly.Culture.IsNeutralCulture) {
				if(!assembly.Culture.Equals(culture))
					return false;
			}
			if(version!=ZeroVersion) {
				if(assembly.Version!=version) {
					return false;
				}
			}
			return true;
		}

		#endregion

		public AssemblyBase LoadAssembly(string type, string filepath) {
			Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
			return LoadAssembly(type, stream);
		}

		public AssemblyBase LoadAssembly(string type, Stream stream) {
			AssemblyBase assembly = loaders[type].LoadAssembly(stream);
			assemblies.Add(assembly);
			return assembly;
		}

	}

}
