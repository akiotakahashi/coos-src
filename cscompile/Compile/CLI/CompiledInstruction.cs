using System;
using CooS.Execution;
using CooS.Reflection;
using CooS.Reflection.CLI;
using OpCode=System.Reflection.Emit.OpCode;

namespace CooS.Compile.CLI {

	class CompiledInstruction : IBranchTarget {

		private readonly Engine engine;
		private readonly AssemblyImpl assembly;
		private readonly MethodInfo method;
		public Instruction Instruction;
		public EvaluationStack StackState;
		public int NativeAddress = -1;

		public CompiledInstruction(Compiler compiler, AssemblyImpl assembly, MethodInfo method, Instruction inst) {
			this.engine = compiler.Engine;
			this.assembly = assembly;
			this.method = method;
			this.Instruction = inst;
		}

		public TypeInfo TypeInfo {
			get {
				return this.engine.Realize(this.Instruction.TypeInfo.Instantiate(method.Base));
			}
		}

		public FieldInfo FieldInfo {
			get {
				return this.engine.Realize(this.Instruction.FieldInfo);
			}
		}

		public MethodInfo MethodInfo {
			get {
				return this.engine.Realize(this.Instruction.MethodInfo.Instantiate(method.Base));
			}
		}

		#region IBranchTarget ÉÅÉìÉo

		public IntPtr Address {
			get {
				return new IntPtr(this.NativeAddress);
			}
		}

		#endregion

		[Obsolete]
		public OpCode OpCode {
			get {
				return this.Instruction.OpCode;
			}
		}

		public TypeInfo OpType {
			get {
				return this.engine.Realize(this.Instruction.OpType);
			}
		}

		public void Dump(System.IO.TextWriter writer) {
			this.Instruction.Dump(writer);
			writer.Write("<");
			if(this.StackState==null) {
				writer.WriteLine(" (n/a)");
			} else {
				for(int i=0; i<this.StackState.Length; ++i) {
					TypeBase type = this.StackState[i];
					if(type==null) {
						writer.Write(" (null)");
					} else {
						writer.Write(" {0}", type.Name);
						if(type.GenericParameterCount>0) {
							Console.Write("<");
							Console.Write(type.GetGenericArgumentType(0).Name);
							for(int k=1; k<type.GenericParameterCount; ++k) {
								Console.Write(",{0}", type.GetGenericArgumentType(k).Name);
							}
							Console.Write(">");
						}
					}
				}
				writer.WriteLine();
			}
		}

	}

}
