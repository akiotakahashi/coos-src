using System;
using CooS.Execution;
using CooS.Compile;

namespace CooS.Toolchains.IA32 {

	public class ToolchainImpl : Toolchain {

		public override Assembler CreateAssembler(Compiler compiler, MethodInfo method, int workingsize) {
			return new IA32Assembler(compiler, method, workingsize);
		}

	}

}
