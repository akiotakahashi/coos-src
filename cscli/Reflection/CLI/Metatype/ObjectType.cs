using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	class ObjectType : ClassType {

		public ObjectType(AssemblyImpl assembly, TypeDefInfo entity) : base(assembly, entity) {
		}

		public override TypeBase BaseType {
			get {
				return null;
			}
		}

	}

}
