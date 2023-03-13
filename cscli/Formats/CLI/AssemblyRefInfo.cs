using System;
using AssemblyName = System.Reflection.AssemblyName;
using CultureInfo = System.Globalization.CultureInfo;

namespace CooS.Formats.CLI {
	using Metadata;
	using Metadata.Rows;

	sealed class AssemblyRefInfo : AssemblyDeclInfo, IResolutionScope {

		private AssemblyDefInfo assembly;
		private AssemblyRefRow row;

		internal AssemblyRefInfo(AssemblyDefInfo assembly, AssemblyRefRow row) {
			this.assembly = assembly;
			this.row = row;
		}

		public override string Name {
			get {
				return this.assembly.LoadBlobString(this.row.Name);
			}
		}

		public override CultureInfo Culture {
			get {
				string name = this.assembly.LoadBlobString(this.row.Culture);
				if(name==null) return null;
				return new CultureInfo(name);
			}
		}

		public override Version Version {
			get {
				return new Version(this.row.MajorVersion, this.row.MinorVersion, this.row.BuildNumber, this.row.RevisionNumber);
			}
		}

		public override AssemblyName AssemblyName {
			get {
				AssemblyName name = new AssemblyName();
				name.Name = this.Name;
				name.CultureInfo = this.Culture;
				name.Version = this.Version;
				return name;
			}
		}

	}

}
