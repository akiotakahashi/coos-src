using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;
using CooS.Reflection;
using CooS.Reflection.CLI;
using CooS.Execution;
using CooS.Toolchains;
using BindingFlags=System.Reflection.BindingFlags;
using BadCodeException=CooS.Formats.BadCodeException;

namespace CooS.Compile.CLI {

	delegate void InstructionAssembler(CompilerImpl compiler, CompiledInstruction inst);

	partial class CompilerImpl : Compiler {
		
		static readonly InstructionAssembler[] assem1;
		static readonly Dictionary<short,InstructionAssembler> assem2 = new Dictionary<short,InstructionAssembler>();

		static CompilerImpl() {
			assem1 = new InstructionAssembler[256];
			foreach(System.Reflection.FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(fi.FieldType!=typeof(OpCode)) continue;
				OpCode opcode = (OpCode)fi.GetValue(null);
				int i = opcode.Name.IndexOf('.');
				if(i<0) i=opcode.Name.Length;
				string methodname = "Dispatch_"+opcode.Name.Substring(0,i).ToLower();
				Delegate d = Delegate.CreateDelegate(typeof(InstructionAssembler), typeof(CompilerImpl), methodname);
				Register(opcode, (InstructionAssembler)d);
			}
		}

		private static void Register(OpCode opcode, InstructionAssembler eval) {
			if(opcode.Size==1) {
				assem1[opcode.Value] = eval;
			} else if(opcode.Size==2) {
				assem2[opcode.Value] = eval;
			} else {
				throw new NotSupportedException();
			}
		}

	}

	partial class CompilerImpl {

		AssemblyImpl assembly;
		MethodInfo method;
		CompiledInstruction[] instructions;
		int workingsize;
		Assembler assembler;

		public CompilerImpl(Engine engine, Domain domain) : base(engine,domain) {
		}

		public override CodeInfo Compile(MethodBase _method) {
			MethodInfo method = this.Engine.Realize(_method);
			CompiledInstruction[] insts = Synthesizer.Perform(this, method);
			return this.Compile(method, insts, false);
		}

		private CodeInfo Compile(MethodInfo method, CompiledInstruction[] insts, bool trap) {
			this.assembly = (AssemblyImpl)method.Assembly;
			this.method = method;
			this.instructions = insts;
			this.Prepare();
			return this.CompileInternal(trap);
		}

		private void Prepare() {
			int maxsize = 0;
			foreach(CompiledInstruction inst in this.instructions) {
				if(inst.Instruction.OpCode.Value==OpCodes.Newobj.Value) {
					MethodInfo calling = inst.MethodInfo;
					maxsize = Math.Max(maxsize, this.Engine.Realize(calling.Type).VariableSize);
				} else if(inst.Instruction.OpCode.Value==OpCodes.Call.Value
					|| inst.Instruction.OpCode.Value==OpCodes.Calli.Value
					|| inst.Instruction.OpCode.Value==OpCodes.Callvirt.Value)
				{
					MethodInfo calling = inst.MethodInfo;
					if(this.Engine.Realize(calling.ReturnType).VariableSize>8) {
						maxsize = Math.Max(maxsize, this.Engine.Realize(calling.ReturnType).VariableSize);
					}
				}
			}
			this.workingsize = maxsize;
		}

		private unsafe CodeInfo CompileInternal(bool trap) {
			this.assembler = Toolchain.Current.CreateAssembler(this, this.method, this.workingsize);
			if(trap) this.assembler.Trap();
			this.assembler.Prologue();
			foreach(CompiledInstruction inst in this.instructions) {
				inst.NativeAddress = assembler.Position;
				if(inst.StackState==null) {
					//TODO: catch ブロック内の命令などはスタック状態を確定できていない。
					Console.WriteLine("Undetermined block> [{0,3:X}/{1,3:X}] {2}"
						, inst.Instruction.Address, inst.NativeAddress
						, inst.Instruction.OpCode.Name);
				} else {
					Assemble(inst);
				}
			}
			this.assembler.Epilogue();
			CodeInfo ci = this.assembler.GenerateCode();
			fixed(byte* p = &ci.CodeBlock[0]) {
				foreach(CompiledInstruction inst in this.instructions) {
					inst.NativeAddress += (int)p;
				}
			}
			return ci;
		}

		private void Assemble(CompiledInstruction inst) {
			if(inst.Instruction.OpCode.Size==1) {
				assem1[inst.Instruction.OpCode.Value](this, inst);
			} else if(inst.Instruction.OpCode.Size==2) {
				assem2[inst.Instruction.OpCode.Value](this, inst);
			} else {
				throw new UnexpectedException();
			}
		}

		private CompiledInstruction GetInstructionByAddress(int address) {
			foreach(CompiledInstruction inst in this.instructions) {
				if(inst.Instruction.Address==address) return inst;
			}
			throw new ArgumentException("Instruction assigned at the specified address is not found.");
		}

		public static bool IsSigned(TypeInfo type) {
			return type.Base.IsPrimitive && type.Base.IsSigned;
		}

#if false
		private MemberInfo Resolve(MemberDeclInfo member) {
			throw new NotImplementedException();
		}
#endif

	}

}
