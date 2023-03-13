using System;
using CooS.Formats.Java;

namespace CooS.Reflection.Java.Metatype {

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
