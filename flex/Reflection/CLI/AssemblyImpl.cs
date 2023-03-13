using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {
	
	public class AssemblyImpl : AssemblyBase {

		private readonly AssemblyDefInfo entity;
		private readonly TypeImpl[] types;

		internal AssemblyImpl(AssemblyDefInfo assembly) {
			this.entity = assembly;
			this.types = new TypeImpl[assembly.TypeDefCount];
		}

		protected override void Dispose() {
			this.entity.Dispose();
		}

		public override string Name {
			get {
				return this.entity.AssemblyName.Name;
			}
		}

		public override System.Globalization.CultureInfo Culture {
			get {
				return this.entity.AssemblyName.CultureInfo;
			}
		}

		public override Version Version {
			get {
				return this.entity.AssemblyName.Version;
			}
		}

		internal TypeBase Realize(TypeDefInfo typedef) {
			if(types[typedef.Index]==null) {
				types[typedef.Index] = new TypeImpl(this, typedef);
			}
			return types[typedef.Index];
		}

		public override System.Collections.Generic.IEnumerable<TypeBase> EnumType() {
			foreach(TypeDefInfo typedef in this.entity.TypeDefCollection) {
				yield return this.Realize(typedef);
			}
		}

		public override TypeBase FindType(string name, string ns) {
			TypeDefInfo typedef = this.entity.SearchTypeDef(name, ns);
			return Realize(typedef);
		}

		internal AssemblyBase Resolve(AssemblyRefInfo asmref, IDomain domain) {
			return domain.ResolveAssembly(asmref.Name, asmref.Culture, asmref.Version);
		}

	}

}
