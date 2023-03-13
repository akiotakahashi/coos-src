using System;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI {
	
	public sealed class ParamImpl : ParamBase {

		private MethodImpl method;
		private ParameterDefInfo entity;

		internal ParamImpl(MethodImpl method, ParameterDefInfo entity) {
			this.method = method;
			this.entity = entity;
		}

		public override MethodBase Method {
			get {
				return method;
			}
		}

		public override int Position {
			get {
				return this.entity.Sequence-1;
			}
		}

		public override string Name {
			get {
				return this.entity.Name;
			}
		}

	}

}
