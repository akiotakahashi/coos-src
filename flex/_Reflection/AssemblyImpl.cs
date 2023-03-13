using System;
using System.Reflection;
using CooS.Reflection;
/*
using CooS.Formats.CLI;
using CooS.Formats.CLI.Metadata;
using CooS.Formats.CLI.Signature;
*/

using System.Runtime.CompilerServices;

namespace CooS.Reflection {

	public class AssemblyImpl : Assembly {

		private readonly AssemblyBase assemblyinfo;

		public AssemblyImpl(AssemblyBase asminfo) {
			this.assemblyinfo = asminfo;
		}

		public override string ToString() {
			return this.GetName().ToString();
		}

		public AssemblyBase AssemblyInfo {
			get {
				return this.assemblyinfo;
			}
		}

		public override AssemblyName GetName(bool copiedName) {
			return this.assemblyinfo.GetName(copiedName);
		}

		public override MethodInfo EntryPoint {
			get {
				return this.assemblyinfo.EntryPoint;
			}
		}

	}

}
