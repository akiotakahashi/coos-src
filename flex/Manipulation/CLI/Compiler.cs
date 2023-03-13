using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using CooS.Formats.CLI;
using CooS.Reflection;
using CooS.Reflection.CLI.Metatype;

namespace CooS.Manipulation.CLI {

	delegate void InstructionAssembler(Compiler compiler, Instruction inst);

	class Compiler {
		
		static readonly InstructionAssembler[] assem1;
		static readonly Hashtable assem2 = new Hashtable();

		readonly AssemblyDefInfo assembly;
		readonly MethodDefInfo method;
		readonly Instruction[] instructions;
		readonly int workingsize;
		//Commutator this.assembler;
		Assembler assembler;

		static Compiler() {
			Console.WriteLine("Constructing Compiler");
			assem1 = new InstructionAssembler[256];
			#region OpCode Registrations
			foreach(FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(fi.FieldType!=typeof(OpCode)) continue;
				OpCode opcode = (OpCode)fi.GetValue(null);
				int i = opcode.Name.IndexOf('.');
				if(i<0) i=opcode.Name.Length;
				string methodname = "Dispatch_"+opcode.Name.Substring(0,i).ToLower();
				Delegate d = Delegate.CreateDelegate(typeof(InstructionAssembler), typeof(Compiler), methodname);
				Register(opcode, (InstructionAssembler)d);
			}
			#region
			/*
			Register(OpCodes.Add, new InstructionAssembler(Dispatch_add));
			Register(OpCodes.And, new InstructionAssembler(Dispatch_and));
			Register(OpCodes.Arglist, new InstructionAssembler(Dispatch_arglist));
			Register(OpCodes.Beq, new InstructionAssembler(Dispatch_beq));
			Register(OpCodes.Bge, new InstructionAssembler(Dispatch_bge));
			Register(OpCodes.Bgt, new InstructionAssembler(Dispatch_bgt));
			Register(OpCodes.Ble, new InstructionAssembler(Dispatch_ble));
			Register(OpCodes.Blt, new InstructionAssembler(Dispatch_blt));
			Register(OpCodes.Bne_Un, new InstructionAssembler(Dispatch_bne_Un));
			Register(OpCodes.Box, new InstructionAssembler(Dispatch_box));
			Register(OpCodes.Br, new InstructionAssembler(Dispatch_br));
			Register(OpCodes.Break, new InstructionAssembler(Dispatch_break));
			Register(OpCodes.Brfalse, new InstructionAssembler(Dispatch_brfalse));
			Register(OpCodes.Brtrue, new InstructionAssembler(Dispatch_brtrue));
			Register(OpCodes.Call, new InstructionAssembler(Dispatch_call));
			Register(OpCodes.Calli, new InstructionAssembler(Dispatch_calli));
			Register(OpCodes.Callvirt, new InstructionAssembler(Dispatch_callvirt));
			Register(OpCodes.Castclass, new InstructionAssembler(Dispatch_castclass));
			Register(OpCodes.Ceq, new InstructionAssembler(Dispatch_ceq));
			Register(OpCodes.Cgt, new InstructionAssembler(Dispatch_cgt));
			Register(OpCodes.Ckfinite, new InstructionAssembler(Dispatch_ckfinite));
			Register(OpCodes.Clt, new InstructionAssembler(Dispatch_clt));
			Register(OpCodes.Conv_I, new InstructionAssembler(Dispatch_conv));
			Register(OpCodes.Cpblk, new InstructionAssembler(Dispatch_cpblk));
			Register(OpCodes.Cpobj, new InstructionAssembler(Dispatch_cpobj));
			Register(OpCodes.Div, new InstructionAssembler(Dispatch_div));
			Register(OpCodes.Dup, new InstructionAssembler(Dispatch_dup));
			Register(OpCodes.Endfilter, new InstructionAssembler(Dispatch_endfilter));
			Register(OpCodes.Endfinally, new InstructionAssembler(Dispatch_endfinally));
			Register(OpCodes.Initblk, new InstructionAssembler(Dispatch_initblk));
			Register(OpCodes.Initobj, new InstructionAssembler(Dispatch_initobj));
			Register(OpCodes.Isinst, new InstructionAssembler(Dispatch_isinst));
			Register(OpCodes.Jmp, new InstructionAssembler(Dispatch_jmp));
			Register(OpCodes.Ldarg, new InstructionAssembler(Dispatch_ldarg));
			Register(OpCodes.Ldarga, new InstructionAssembler(Dispatch_ldarga));
			Register(OpCodes.Ldc, new InstructionAssembler(Dispatch_ldc));
			Register(OpCodes.Ldelem, new InstructionAssembler(Dispatch_ldelem));
			Register(OpCodes.Ldelema, new InstructionAssembler(Dispatch_ldelema));
			Register(OpCodes.Ldfld, new InstructionAssembler(Dispatch_ldfld));
			Register(OpCodes.Ldflda, new InstructionAssembler(Dispatch_ldflda));
			Register(OpCodes.Ldftn, new InstructionAssembler(Dispatch_ldftn));
			Register(OpCodes.Ldind_I, new InstructionAssembler(Dispatch_ldind));
			Register(OpCodes.Ldlen, new InstructionAssembler(Dispatch_ldlen));
			Register(OpCodes.Ldloc, new InstructionAssembler(Dispatch_ldloc));
			Register(OpCodes.Ldloca, new InstructionAssembler(Dispatch_ldloca));
			Register(OpCodes.Ldnull, new InstructionAssembler(Dispatch_ldnull));
			Register(OpCodes.Ldobj, new InstructionAssembler(Dispatch_ldobj));
			Register(OpCodes.Ldsfld, new InstructionAssembler(Dispatch_ldsfld));
			Register(OpCodes.Ldsflda, new InstructionAssembler(Dispatch_ldsflda));
			Register(OpCodes.Ldstr, new InstructionAssembler(Dispatch_ldstr));
			Register(OpCodes.Ldtoken, new InstructionAssembler(Dispatch_ldtoken));
			Register(OpCodes.Ldvirtftn, new InstructionAssembler(Dispatch_ldvirtftn));
			Register(OpCodes.Leave, new InstructionAssembler(Dispatch_leave));
			Register(OpCodes.Localloc, new InstructionAssembler(Dispatch_localloc));
			Register(OpCodes.Mkrefany, new InstructionAssembler(Dispatch_mkrefany));
			Register(OpCodes.Mul, new InstructionAssembler(Dispatch_mul));
			Register(OpCodes.Neg, new InstructionAssembler(Dispatch_neg));
			Register(OpCodes.Newarr, new InstructionAssembler(Dispatch_newarr));
			Register(OpCodes.Newobj, new InstructionAssembler(Dispatch_newobj));
			Register(OpCodes.Nop, new InstructionAssembler(Dispatch_nop));
			Register(OpCodes.Not, new InstructionAssembler(Dispatch_not));
			Register(OpCodes.Or, new InstructionAssembler(Dispatch_or));
			Register(OpCodes.Pop, new InstructionAssembler(Dispatch_pop));
			Register(OpCodes.Prefix1, new InstructionAssembler(Dispatch_prefix1));
			Register(OpCodes.Prefix2, new InstructionAssembler(Dispatch_prefix2));
			Register(OpCodes.Prefix3, new InstructionAssembler(Dispatch_prefix3));
			Register(OpCodes.Prefix4, new InstructionAssembler(Dispatch_prefix4));
			Register(OpCodes.Prefix5, new InstructionAssembler(Dispatch_prefix5));
			Register(OpCodes.Prefix6, new InstructionAssembler(Dispatch_prefix6));
			Register(OpCodes.Prefix7, new InstructionAssembler(Dispatch_prefix7));
			Register(OpCodes.Prefixref, new InstructionAssembler(Dispatch_prefixref));
			Register(OpCodes.Refanytype, new InstructionAssembler(Dispatch_refanytype));
			Register(OpCodes.Refanyval, new InstructionAssembler(Dispatch_refanyval));
			Register(OpCodes.Rem, new InstructionAssembler(Dispatch_rem));
			Register(OpCodes.Rem_Un, new InstructionAssembler(Dispatch_rem_Un));
			Register(OpCodes.Ret, new InstructionAssembler(Dispatch_ret));
			Register(OpCodes.Rethrow, new InstructionAssembler(Dispatch_rethrow));
			Register(OpCodes.Shl, new InstructionAssembler(Dispatch_shl));
			Register(OpCodes.Shr, new InstructionAssembler(Dispatch_shr));
			Register(OpCodes.Sizeof, new InstructionAssembler(Dispatch_sizeof));
			Register(OpCodes.Starg, new InstructionAssembler(Dispatch_starg));
			Register(OpCodes.Stelem_I, new InstructionAssembler(Dispatch_stelem));
			Register(OpCodes.Stfld, new InstructionAssembler(Dispatch_stfld));
			Register(OpCodes.Stind_I, new InstructionAssembler(Dispatch_stind));
			Register(OpCodes.Stloc, new InstructionAssembler(Dispatch_stloc));
			Register(OpCodes.Stobj, new InstructionAssembler(Dispatch_stobj));
			Register(OpCodes.Stsfld, new InstructionAssembler(Dispatch_stsfld));
			Register(OpCodes.Sub, new InstructionAssembler(Dispatch_sub));
			Register(OpCodes.Switch, new InstructionAssembler(Dispatch_switch));
			Register(OpCodes.Tailcall, new InstructionAssembler(Dispatch_tailcall));
			Register(OpCodes.Throw, new InstructionAssembler(Dispatch_throw));
			Register(OpCodes.Unaligned, new InstructionAssembler(Dispatch_unaligned));
			Register(OpCodes.Unbox, new InstructionAssembler(Dispatch_unbox));
			Register(OpCodes.Volatile, new InstructionAssembler(Dispatch_volatile));
			Register(OpCodes.Xor, new InstructionAssembler(Dispatch_xor));
			//*/
			#endregion
			#endregion
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

		public static CodeInfo Compile(MethodDefInfo method, Instruction[] insts, bool trap) {
			Compiler comp = new Compiler(method, insts);
			return comp.CompileInternal(trap);
		}

		private Compiler(MethodDefInfo method, Instruction[] insts) {
			this.assembly = method.MyAssembly;
			this.method = method;
			this.instructions = insts;
			int maxsize = 0;
			foreach(Instruction inst in insts) {
				if(inst.OpCode.Value==OpCodes.Newobj.Value) {
					MethodInfoImpl calling = this.assembly.ResolveMethod((MDToken)inst.Token);
					TypeImpl type = (TypeImpl)calling.DeclaringType;
					maxsize = Math.Max(maxsize, type.VariableSize);
				} else if(inst.OpCode.Value==OpCodes.Call.Value
					|| inst.OpCode.Value==OpCodes.Calli.Value
					|| inst.OpCode.Value==OpCodes.Callvirt.Value)
				{
					MethodInfoImpl calling = assembly.ResolveMethod(inst.Token);
					if(((TypeImpl)calling.ReturnType).VariableSize>8) {
						maxsize = Math.Max(maxsize, ((TypeImpl)calling.ReturnType).VariableSize);
					}
				}
			}
			this.workingsize = maxsize;
		}

		private unsafe CodeInfo CompileInternal(bool trap) {
			this.assembler = Architecture.CreateAssembler(this.method, this.workingsize);
			if(trap) this.assembler.Trap();
			this.assembler.Prologue();
			foreach(Instruction inst in this.instructions) {
				inst.NativeAddress = assembler.Position;
				if(inst.StackState==null) {
					//TODO: catch ブロック内の命令などはスタック状態を確定できていない。
					Console.WriteLine("Undetermined block> [{0,3:X}/{1,3:X}] {2}", inst.Address, inst.NativeAddress, inst.OpCode.Name);
				} else {
					Assemble(inst);
				}
			}
			this.assembler.Epilogue();
			CodeInfo ci = this.assembler.GenerateCode();
			fixed(byte* p = &ci.CodeBlock[0]) {
				foreach(Instruction inst in this.instructions) {
					inst.NativeAddress += (int)p;
				}
			}
			return ci;
		}

		private void Assemble(Instruction inst) {
			if(inst.OpCode.Size==1) {
				assem1[inst.OpCode.Value](this, inst);
			} else if(inst.OpCode.Size==2) {
				((InstructionAssembler)assem2[inst.OpCode.Value])(this, inst);
			} else {
				throw new BadILException();
			}
		}

		Instruction GetInstructionByAddress(int address) {
			foreach(Instruction inst in this.instructions) {
				if(inst.Address==address) return inst;
			}
			throw new ArgumentException("Instruction assigned at the specified address is not found.");
		}

		public static bool IsSigned(Type type) {
			return type.IsPrimitive && ((CooS.Reflection.CLI.Metatype.PrimitiveType)type).Signed;
		}

		#region Assembling Method Dispatcher

		static void Dispatch_add(Compiler compiler, Instruction inst) { compiler.Assemble_add(inst); }
		static void Dispatch_and(Compiler compiler, Instruction inst) { compiler.Assemble_and(inst); }
		static void Dispatch_arglist(Compiler compiler, Instruction inst) { compiler.Assemble_arglist(inst); }
		static void Dispatch_beq(Compiler compiler, Instruction inst) { compiler.Assemble_beq(inst); }
		static void Dispatch_bge(Compiler compiler, Instruction inst) { compiler.Assemble_bge(inst); }
		static void Dispatch_bgt(Compiler compiler, Instruction inst) { compiler.Assemble_bgt(inst); }
		static void Dispatch_ble(Compiler compiler, Instruction inst) { compiler.Assemble_ble(inst); }
		static void Dispatch_blt(Compiler compiler, Instruction inst) { compiler.Assemble_blt(inst); }
		static void Dispatch_bne(Compiler compiler, Instruction inst) { compiler.Assemble_bne(inst); }
		static void Dispatch_box(Compiler compiler, Instruction inst) { compiler.Assemble_box(inst); }
		static void Dispatch_br(Compiler compiler, Instruction inst) { compiler.Assemble_br(inst); }
		static void Dispatch_break(Compiler compiler, Instruction inst) { compiler.Assemble_break(inst); }
		static void Dispatch_brfalse(Compiler compiler, Instruction inst) { compiler.Assemble_brfalse(inst); }
		static void Dispatch_brtrue(Compiler compiler, Instruction inst) { compiler.Assemble_brtrue(inst); }
		static void Dispatch_call(Compiler compiler, Instruction inst) { compiler.Assemble_call(inst); }
		static void Dispatch_calli(Compiler compiler, Instruction inst) { compiler.Assemble_calli(inst); }
		static void Dispatch_callvirt(Compiler compiler, Instruction inst) { compiler.Assemble_callvirt(inst); }
		static void Dispatch_castclass(Compiler compiler, Instruction inst) { compiler.Assemble_castclass(inst); }
		static void Dispatch_ceq(Compiler compiler, Instruction inst) { compiler.Assemble_ceq(inst); }
		static void Dispatch_cgt(Compiler compiler, Instruction inst) { compiler.Assemble_cgt(inst); }
		static void Dispatch_ckfinite(Compiler compiler, Instruction inst) { compiler.Assemble_ckfinite(inst); }
		static void Dispatch_clt(Compiler compiler, Instruction inst) { compiler.Assemble_clt(inst); }
		static void Dispatch_conv(Compiler compiler, Instruction inst) { compiler.Assemble_conv(inst); }
		static void Dispatch_cpblk(Compiler compiler, Instruction inst) { compiler.Assemble_cpblk(inst); }
		static void Dispatch_cpobj(Compiler compiler, Instruction inst) { compiler.Assemble_cpobj(inst); }
		static void Dispatch_div(Compiler compiler, Instruction inst) { compiler.Assemble_div(inst); }
		static void Dispatch_dup(Compiler compiler, Instruction inst) { compiler.Assemble_dup(inst); }
		static void Dispatch_endfilter(Compiler compiler, Instruction inst) { compiler.Assemble_endfilter(inst); }
		static void Dispatch_endfinally(Compiler compiler, Instruction inst) { compiler.Assemble_endfinally(inst); }
		static void Dispatch_initblk(Compiler compiler, Instruction inst) { compiler.Assemble_initblk(inst); }
		static void Dispatch_initobj(Compiler compiler, Instruction inst) { compiler.Assemble_initobj(inst); }
		static void Dispatch_isinst(Compiler compiler, Instruction inst) { compiler.Assemble_isinst(inst); }
		static void Dispatch_jmp(Compiler compiler, Instruction inst) { compiler.Assemble_jmp(inst); }
		static void Dispatch_ldarg(Compiler compiler, Instruction inst) { compiler.Assemble_ldarg(inst); }
		static void Dispatch_ldarga(Compiler compiler, Instruction inst) { compiler.Assemble_ldarga(inst); }
		static void Dispatch_ldc(Compiler compiler, Instruction inst) { compiler.Assemble_ldc(inst); }
		static void Dispatch_ldelem(Compiler compiler, Instruction inst) { compiler.Assemble_ldelem(inst); }
		static void Dispatch_ldelema(Compiler compiler, Instruction inst) { compiler.Assemble_ldelema(inst); }
		static void Dispatch_ldfld(Compiler compiler, Instruction inst) { compiler.Assemble_ldfld(inst); }
		static void Dispatch_ldflda(Compiler compiler, Instruction inst) { compiler.Assemble_ldflda(inst); }
		static void Dispatch_ldftn(Compiler compiler, Instruction inst) { compiler.Assemble_ldftn(inst); }
		static void Dispatch_ldind(Compiler compiler, Instruction inst) { compiler.Assemble_ldind(inst); }
		static void Dispatch_ldlen(Compiler compiler, Instruction inst) { compiler.Assemble_ldlen(inst); }
		static void Dispatch_ldloc(Compiler compiler, Instruction inst) { compiler.Assemble_ldloc(inst); }
		static void Dispatch_ldloca(Compiler compiler, Instruction inst) { compiler.Assemble_ldloca(inst); }
		static void Dispatch_ldnull(Compiler compiler, Instruction inst) { compiler.Assemble_ldnull(inst); }
		static void Dispatch_ldobj(Compiler compiler, Instruction inst) { compiler.Assemble_ldobj(inst); }
		static void Dispatch_ldsfld(Compiler compiler, Instruction inst) { compiler.Assemble_ldsfld(inst); }
		static void Dispatch_ldsflda(Compiler compiler, Instruction inst) { compiler.Assemble_ldsflda(inst); }
		static void Dispatch_ldstr(Compiler compiler, Instruction inst) { compiler.Assemble_ldstr(inst); }
		static void Dispatch_ldtoken(Compiler compiler, Instruction inst) { compiler.Assemble_ldtoken(inst); }
		static void Dispatch_ldvirtftn(Compiler compiler, Instruction inst) { compiler.Assemble_ldvirtftn(inst); }
		static void Dispatch_leave(Compiler compiler, Instruction inst) { compiler.Assemble_leave(inst); }
		static void Dispatch_localloc(Compiler compiler, Instruction inst) { compiler.Assemble_localloc(inst); }
		static void Dispatch_mkrefany(Compiler compiler, Instruction inst) { compiler.Assemble_mkrefany(inst); }
		static void Dispatch_mul(Compiler compiler, Instruction inst) { compiler.Assemble_mul(inst); }
		static void Dispatch_neg(Compiler compiler, Instruction inst) { compiler.Assemble_neg(inst); }
		static void Dispatch_newarr(Compiler compiler, Instruction inst) { compiler.Assemble_newarr(inst); }
		static void Dispatch_newobj(Compiler compiler, Instruction inst) { compiler.Assemble_newobj(inst); }
		static void Dispatch_nop(Compiler compiler, Instruction inst) { compiler.Assemble_nop(inst); }
		static void Dispatch_not(Compiler compiler, Instruction inst) { compiler.Assemble_not(inst); }
		static void Dispatch_or(Compiler compiler, Instruction inst) { compiler.Assemble_or(inst); }
		static void Dispatch_pop(Compiler compiler, Instruction inst) { compiler.Assemble_pop(inst); }
		static void Dispatch_prefix1(Compiler compiler, Instruction inst) { compiler.Assemble_prefix1(inst); }
		static void Dispatch_prefix2(Compiler compiler, Instruction inst) { compiler.Assemble_prefix2(inst); }
		static void Dispatch_prefix3(Compiler compiler, Instruction inst) { compiler.Assemble_prefix3(inst); }
		static void Dispatch_prefix4(Compiler compiler, Instruction inst) { compiler.Assemble_prefix4(inst); }
		static void Dispatch_prefix5(Compiler compiler, Instruction inst) { compiler.Assemble_prefix5(inst); }
		static void Dispatch_prefix6(Compiler compiler, Instruction inst) { compiler.Assemble_prefix6(inst); }
		static void Dispatch_prefix7(Compiler compiler, Instruction inst) { compiler.Assemble_prefix7(inst); }
		static void Dispatch_prefixref(Compiler compiler, Instruction inst) { compiler.Assemble_prefixref(inst); }
		static void Dispatch_refanytype(Compiler compiler, Instruction inst) { compiler.Assemble_refanytype(inst); }
		static void Dispatch_refanyval(Compiler compiler, Instruction inst) { compiler.Assemble_refanyval(inst); }
		static void Dispatch_rem(Compiler compiler, Instruction inst) { compiler.Assemble_rem(inst); }
		static void Dispatch_rem_Un(Compiler compiler, Instruction inst) { compiler.Assemble_rem_Un(inst); }
		static void Dispatch_ret(Compiler compiler, Instruction inst) { compiler.Assemble_ret(inst); }
		static void Dispatch_rethrow(Compiler compiler, Instruction inst) { compiler.Assemble_rethrow(inst); }
		static void Dispatch_shl(Compiler compiler, Instruction inst) { compiler.Assemble_shl(inst); }
		static void Dispatch_shr(Compiler compiler, Instruction inst) { compiler.Assemble_shr(inst); }
		static void Dispatch_sizeof(Compiler compiler, Instruction inst) { compiler.Assemble_sizeof(inst); }
		static void Dispatch_starg(Compiler compiler, Instruction inst) { compiler.Assemble_starg(inst); }
		static void Dispatch_stelem(Compiler compiler, Instruction inst) { compiler.Assemble_stelem(inst); }
		static void Dispatch_stfld(Compiler compiler, Instruction inst) { compiler.Assemble_stfld(inst); }
		static void Dispatch_stind(Compiler compiler, Instruction inst) { compiler.Assemble_stind(inst); }
		static void Dispatch_stloc(Compiler compiler, Instruction inst) { compiler.Assemble_stloc(inst); }
		static void Dispatch_stobj(Compiler compiler, Instruction inst) { compiler.Assemble_stobj(inst); }
		static void Dispatch_stsfld(Compiler compiler, Instruction inst) { compiler.Assemble_stsfld(inst); }
		static void Dispatch_sub(Compiler compiler, Instruction inst) { compiler.Assemble_sub(inst); }
		static void Dispatch_switch(Compiler compiler, Instruction inst) { compiler.Assemble_switch(inst); }
		static void Dispatch_tail(Compiler compiler, Instruction inst) { compiler.Assemble_tail(inst); }
		static void Dispatch_throw(Compiler compiler, Instruction inst) { compiler.Assemble_throw(inst); }
		static void Dispatch_unaligned(Compiler compiler, Instruction inst) { compiler.Assemble_unaligned(inst); }
		static void Dispatch_unbox(Compiler compiler, Instruction inst) { compiler.Assemble_unbox(inst); }
		static void Dispatch_volatile(Compiler compiler, Instruction inst) { compiler.Assemble_volatile(inst); }
		static void Dispatch_xor(Compiler compiler, Instruction inst) { compiler.Assemble_xor(inst); }
		
		#endregion

		#region Methematical Operations

		void Assemble_add(Instruction inst) {
			SuperType t2 = inst.StackState[0];
			SuperType t1 = inst.StackState[1];
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Add32(!inst.Unsigned, inst.Overflow);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Add64(!inst.Unsigned, inst.Overflow);
			} else {
				throw new BadILException("sizes mismatch: "+t1+" ("+t1.VariableSize+") and "+t2+" ("+t2.VariableSize+")");
			}
		}

		void Assemble_sub(Instruction inst) {
			SuperType t2 = inst.StackState[0];
			SuperType t1 = inst.StackState[1];
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Sub32(!inst.Unsigned, inst.Overflow);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Sub64(!inst.Unsigned, inst.Overflow);
			} else {
				throw new BadILException("sizes mismatch: "+t1+" ("+t1.VariableSize+") and "+t2+" ("+t2.VariableSize+")");
			}
		}

		void Assemble_mul(Instruction inst) {
			SuperType t2 = inst.StackState[0];
			SuperType t1 = inst.StackState[1];
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Mul32(!inst.Unsigned, inst.Overflow);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Mul64(!inst.Unsigned, inst.Overflow);
			} else {
				throw new BadILException("sizes mismatch");
			}
		}

		void Assemble_div(Instruction inst) {
			SuperType t2 = inst.StackState[0];
			SuperType t1 = inst.StackState[1];
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Div32(!inst.Unsigned);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Div64(!inst.Unsigned);
			} else {
				throw new BadILException("sizes mismatch");
			}
		}

		void Assemble_neg(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Negate64();
			} else {
				this.assembler.Negate32();
			}
		}

		#endregion

		#region Logical Operations

		void Assemble_and(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.And64();
			} else {
				this.assembler.And32();
			}
		}

		void Assemble_xor(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Xor64();
			} else {
				this.assembler.Xor32();
			}
		}

		void Assemble_not(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Not64();
			} else {
				this.assembler.Not32();
			}
		}

		void Assemble_or(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Or64();
			} else {
				this.assembler.Or32();
			}
		}

		#endregion

		void Assemble_arglist(Instruction inst) {
			throw new NotImplementedException();
		}

		private void AssertToEqualComparingOperands(Instruction inst) {
			if(inst.StackState.Length<2) throw new BadILException("Stack is too few to compare");
			if(inst.StackState[0]!=inst.StackState[1]) {
				// C# generates the code that compares UIntPtr with Int32*.
				//if(Synthesizer.GetIntrinsicType(inst.StackState[0])!=Synthesizer.GetIntrinsicType(inst.StackState[1])) {
				if(Architecture.GetStackingSize(inst.StackState[0].VariableSize)!=Architecture.GetStackingSize(inst.StackState[1].VariableSize)) {
					throw new BadILException("Not same size about comparing operands: "+inst.StackState[0].FullName+" and "+inst.StackState[1].FullName);
				}
			}
		}

		private void AssembleConditionalBranch(Instruction inst, Condition cond, bool signed) {
			switch(cond) {
			case Condition.Equal:
			case Condition.NotEqual:
			case Condition.LessThan:
			case Condition.LessOrEqual:
			case Condition.GreaterThan:
			case Condition.GreaterOrEqual:
				this.AssertToEqualComparingOperands(inst);
				break;
			}
			switch(Synthesizer.GetIntrinsicType(inst.StackState.Top)) {
			case IntrinsicType.Int4:
			case IntrinsicType.NInt:
			case IntrinsicType.Objt:
			case IntrinsicType.Pter:
				this.assembler.BranchI32(cond, signed, this.GetInstructionByAddress(inst.BrTarget));
				break;
			case IntrinsicType.Int8:
				this.assembler.BranchI64(cond, signed, this.GetInstructionByAddress(inst.BrTarget));
				break;
			case IntrinsicType.Fp32:
				this.assembler.BranchR32(cond, this.GetInstructionByAddress(inst.BrTarget));
				break;
			case IntrinsicType.Fp64:
				this.assembler.BranchR64(cond, this.GetInstructionByAddress(inst.BrTarget));
				break;
			default:
				throw new NotImplementedException();
			}
		}

		#region Branch Operations

		void Assemble_beq(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.Equal, true);
		}

		void Assemble_bge(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.GreaterOrEqual, !inst.Unsigned);
		}

		void Assemble_bgt(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.GreaterThan, !inst.Unsigned);
		}

		void Assemble_ble(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.LessOrEqual, !inst.Unsigned);
		}

		void Assemble_blt(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.LessThan, !inst.Unsigned);
		}

		void Assemble_bne(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.NotEqual, false);
		}

		void Assemble_br(Instruction inst) {
			this.assembler.Branch(this.GetInstructionByAddress(inst.BrTarget));
		}

		void Assemble_brfalse(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.False, false);
		}

		void Assemble_brtrue(Instruction inst) {
			this.AssembleConditionalBranch(inst, Condition.True, false);
		}

		#endregion

		void Assemble_box(Instruction inst) {
			this.assembler.Box(inst.TypeInfo);
		}

		void Assemble_break(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_call(Instruction inst) {
			this.assembler.Call(inst.MethodInfo);
		}

		void Assemble_calli(Instruction inst) {
			this.assembler.CallInd(inst.MethodInfo);
		}

		void Assemble_callvirt(Instruction inst) {
			MethodInfoImpl method = inst.MethodInfo;
			this.assembler.CallVirt(inst.StackState[method.ParameterCount], method);
		}

		void Assemble_castclass(Instruction inst) {
			this.assembler.CastClass(inst.TypeInfo);
		}

		private void AssembleCompare(Instruction inst, Condition cond, bool signed) {
			switch(cond) {
			case Condition.Equal:
			case Condition.LessThan:
			case Condition.GreaterThan:
				this.AssertToEqualComparingOperands(inst);
				break;
			case Condition.NotEqual:
			case Condition.LessOrEqual:
			case Condition.GreaterOrEqual:
			default:
				throw new NotSupportedException();
			}
			switch(Synthesizer.GetIntrinsicType(inst.StackState.Top)) {
			case IntrinsicType.Int4:
			case IntrinsicType.NInt:
			case IntrinsicType.Objt:
			case IntrinsicType.Pter:
				this.assembler.CompareI32(cond, signed);
				break;
			case IntrinsicType.Int8:
				this.assembler.CompareI64(cond, signed);
				break;
			case IntrinsicType.Fp32:
				this.assembler.CompareR32(cond);
				break;
			case IntrinsicType.Fp64:
				this.assembler.CompareR64(cond);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		#region Compare Operations

		void Assemble_ceq(Instruction inst) {
			this.AssembleCompare(inst, Condition.Equal, true);
		}

		void Assemble_cgt(Instruction inst) {
			this.AssembleCompare(inst, Condition.GreaterThan, !inst.Unsigned);
		}

		void Assemble_ckfinite(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_clt(Instruction inst) {
			this.AssembleCompare(inst, Condition.LessThan, !inst.Unsigned);
		}

		#endregion

		void Assemble_conv(Instruction inst) {
			this.assembler.Convert(inst.StackState.Top, inst.OpType);
		}

		void Assemble_cpblk(Instruction inst) {
			this.assembler.CopyBlock();
		}

		void Assemble_cpobj(Instruction inst) {
			this.assembler.LoadConstant(inst.TypeInfo.VariableSize);
			this.assembler.CopyBlock();
		}

		void Assemble_dup(Instruction inst) {
			this.assembler.Duplicate(inst.StackState[0].VariableSize);
		}

		void Assemble_endfilter(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_endfinally(Instruction inst) {
			//TODO: 'endfinally' instruction
		}

		void Assemble_initblk(Instruction inst) {
			this.assembler.InitializeMemory();
		}

		void Assemble_initobj(Instruction inst) {
			this.assembler.ClearMemory(inst.TypeInfo.VariableSize);
		}

		void Assemble_isinst(Instruction inst) {
			this.assembler.IsInst(this.assembly.ResolveType(inst.Token));
		}

		void Assemble_jmp(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_ldarg(Instruction inst) {
			this.assembler.LoadArg(inst.Number);
		}

		void Assemble_ldarga(Instruction inst) {
			this.assembler.LoadArgAddress(inst.Number);
		}

		void Assemble_ldc(Instruction inst) {
			switch(inst.OpType.Name) {
			case "Int32":
				this.assembler.LoadConstant(inst.Number);
				break;
			case "Int64":
				this.assembler.LoadConstant((long)inst.Operand);
				break;
			case "Single":
				this.assembler.LoadConstant((float)inst.Operand);
				break;
			case "Double":
				this.assembler.LoadConstant((double)inst.Operand);
				break;
			default:
				throw new UnexpectedException();
			}
		}

		void Assemble_ldelem(Instruction inst) {
			this.assembler.LoadElement((TypeImpl)inst.StackState[1].GetElementType());
		}

		void Assemble_ldelema(Instruction inst) {
			this.assembler.LoadElementAddress((TypeImpl)inst.StackState[1].GetElementType());
		}

		void Assemble_ldfld(Instruction inst) {
			FieldInfoImpl fi = this.assembly.ResolveField((MDToken)inst.Operand);
			this.assembler.LoadField(fi);
		}

		void Assemble_ldflda(Instruction inst) {
			FieldInfoImpl fi = this.assembly.ResolveField((MDToken)inst.Operand);
			this.assembler.LoadFieldAddress(fi);
		}

		void Assemble_ldftn(Instruction inst) {
			this.assembler.LoadEntryPoint(inst.MethodInfo);
		}

		void Assemble_ldind(Instruction inst) {
			this.assembler.LoadInd(inst.OpType);
		}

		void Assemble_ldlen(Instruction inst) {
			this.assembler.LoadLength();
		}

		void Assemble_ldloc(Instruction inst) {
			this.assembler.LoadVar(inst.Number);
		}

		void Assemble_ldloca(Instruction inst) {
			this.assembler.LoadVarAddress(inst.Number);
		}

		void Assemble_ldnull(Instruction inst) {
			this.assembler.LoadNull();
		}

		void Assemble_ldobj(Instruction inst) {
			SuperType type = this.assembly.ResolveType((MDToken)inst.Operand);
			this.assembler.LoadObject(type);
		}

		void Assemble_ldsfld(Instruction inst) {
			FieldInfoImpl field = this.assembly.ResolveField((MDToken)inst.Operand);
			if(!field.IsStatic) throw new BadILException();
			this.assembler.LoadField(field);
		}

		void Assemble_ldsflda(Instruction inst) {
			FieldInfoImpl field = this.assembly.ResolveField((MDToken)inst.Operand);
			if(!field.IsStatic) throw new BadILException();
			this.assembler.LoadFieldAddress(field);
		}

		void Assemble_ldstr(Instruction inst) {
			this.assembler.LoadString(((MDToken)inst.Operand).RID);
		}

		void Assemble_ldtoken(Instruction inst) {
			switch(inst.Token.TableId) {
			case TableId.TypeDef:
			case TableId.TypeRef:
			case TableId.TypeSpec:
				this.assembler.LoadToken(this.assembly.ResolveType(inst.Token));
				break;
			case TableId.Field:
			case TableId.Method:
			case TableId.MemberRef:
				MemberInfo member = this.assembly.ResolveMember(inst.Token);
				switch(member.MemberType) {
				case MemberTypes.Field:
					this.assembler.LoadToken((FieldInfoImpl)member);
					break;
				case MemberTypes.Method:
					this.assembler.LoadToken((MethodInfoImpl)member);
					break;
				default:
					throw new NotSupportedException();
				}
				break;
			default:
				throw new NotSupportedException();
			}
		}

		void Assemble_ldvirtftn(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_leave(Instruction inst) {
			Instruction target = this.GetInstructionByAddress(inst.BrTarget);
			TypeImpl[] discards = new TypeImpl[inst.StackState.Length-target.StackState.Length];
			for(int i=0; i<discards.Length; ++i) {
				discards[i] = inst.StackState[i];
			}
			this.assembler.Leave(target, discards);
		}

		void Assemble_localloc(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_mkrefany(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_newarr(Instruction inst) {
			this.assembler.AllocateSzArray(this.assembly.ResolveType(inst.Token));
		}

		void Assemble_newobj(Instruction inst) {
			this.assembler.AllocateObject(this.assembly.ResolveMethod((MDToken)inst.Operand));
		}

		void Assemble_nop(Instruction inst) {
			// NOP
		}

		void Assemble_pop(Instruction inst) {
			this.assembler.Pop(inst.StackState.Top.VariableSize);
		}

		void Assemble_prefix1(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix2(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix3(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix4(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix5(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix6(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix7(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefixref(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_refanytype(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_refanyval(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_rem(Instruction inst) {
			this.assembler.Rem32(true);
		}

		void Assemble_rem_Un(Instruction inst) {
			this.assembler.Rem32(false);
		}

		void Assemble_ret(Instruction inst) {
			this.assembler.Return();
		}

		void Assemble_rethrow(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_shl(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState[1])) {
				this.assembler.Shl64();
			} else {
				this.assembler.Shl32();
			}
		}

		void Assemble_shr(Instruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState[1])) {
				this.assembler.Shr64(!inst.Unsigned);
			} else {
				this.assembler.Shr32(!inst.Unsigned);
			}
		}

		void Assemble_sizeof(Instruction inst) {
			this.assembler.LoadConstant(inst.TypeInfo.VariableSize);
		}

		void Assemble_starg(Instruction inst) {
			this.assembler.StoreArg(inst.Number);
		}

		void Assemble_stelem(Instruction inst) {
			this.assembler.StoreElement(inst.StackState[0]);
		}

		void Assemble_stfld(Instruction inst) {
			this.assembler.StoreField(this.assembly.ResolveField((MDToken)inst.Operand));
		}

		void Assemble_stind(Instruction inst) {
			this.assembler.StoreInd(inst.OpType);
		}

		void Assemble_stloc(Instruction inst) {
			this.assembler.StoreVar(inst.Number);
		}

		void Assemble_stobj(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_stsfld(Instruction inst) {
			FieldInfoImpl field = this.assembly.ResolveField((MDToken)inst.Operand);
			if(!field.IsStatic) throw new BadILException();
			this.assembler.StoreField(field);
		}

		void Assemble_switch(Instruction inst) {
			IBranchTarget[] targets = new Instruction[inst.BrTargets.Length];
			for(int i=0; i<targets.Length; ++i) {
				targets[i] = this.GetInstructionByAddress(inst.BrTargets[i]);
			}
			this.assembler.Switch(targets);
		}

		void Assemble_tail(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_throw(Instruction inst) {
			this.assembler.Throw();
		}

		void Assemble_unaligned(Instruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_unbox(Instruction inst) {
			SuperType type = this.assembly.ResolveType((MDToken)inst.Operand);
			this.assembler.Unbox(type);
		}

		void Assemble_volatile(Instruction inst) {
			throw new NotImplementedException();
		}

	}

}
