using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;
using CooS.Reflection;
using CooS.Execution;
using CooS.Toolchains;

namespace CooS.Compile {

	public abstract class Compiler {

		public readonly Engine Engine;
		public readonly Domain Domain;

		protected Compiler(Engine engine, Domain domain) {
			this.Engine = engine;
			this.Domain = domain;
		}

		public World World {
			get {
				return this.Engine.World;
			}
		}

		public abstract CodeInfo Compile(MethodBase method);

	}

}
