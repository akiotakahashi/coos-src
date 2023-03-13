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

	partial class CompilerImpl {
		
		#region Assembling Method Dispatcher

		static void Dispatch_add(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_add(inst); }
		static void Dispatch_and(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_and(inst); }
		static void Dispatch_arglist(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_arglist(inst); }
		static void Dispatch_beq(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_beq(inst); }
		static void Dispatch_bge(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_bge(inst); }
		static void Dispatch_bgt(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_bgt(inst); }
		static void Dispatch_ble(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ble(inst); }
		static void Dispatch_blt(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_blt(inst); }
		static void Dispatch_bne(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_bne(inst); }
		static void Dispatch_box(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_box(inst); }
		static void Dispatch_br(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_br(inst); }
		static void Dispatch_break(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_break(inst); }
		static void Dispatch_brfalse(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_brfalse(inst); }
		static void Dispatch_brtrue(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_brtrue(inst); }
		static void Dispatch_call(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_call(inst); }
		static void Dispatch_calli(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_calli(inst); }
		static void Dispatch_callvirt(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_callvirt(inst); }
		static void Dispatch_castclass(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_castclass(inst); }
		static void Dispatch_ceq(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ceq(inst); }
		static void Dispatch_cgt(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_cgt(inst); }
		static void Dispatch_ckfinite(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ckfinite(inst); }
		static void Dispatch_clt(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_clt(inst); }
		static void Dispatch_conv(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_conv(inst); }
		static void Dispatch_cpblk(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_cpblk(inst); }
		static void Dispatch_cpobj(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_cpobj(inst); }
		static void Dispatch_div(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_div(inst); }
		static void Dispatch_dup(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_dup(inst); }
		static void Dispatch_endfilter(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_endfilter(inst); }
		static void Dispatch_endfinally(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_endfinally(inst); }
		static void Dispatch_initblk(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_initblk(inst); }
		static void Dispatch_initobj(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_initobj(inst); }
		static void Dispatch_isinst(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_isinst(inst); }
		static void Dispatch_jmp(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_jmp(inst); }
		static void Dispatch_ldarg(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldarg(inst); }
		static void Dispatch_ldarga(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldarga(inst); }
		static void Dispatch_ldc(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldc(inst); }
		static void Dispatch_ldelem(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldelem(inst); }
		static void Dispatch_ldelema(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldelema(inst); }
		static void Dispatch_ldfld(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldfld(inst); }
		static void Dispatch_ldflda(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldflda(inst); }
		static void Dispatch_ldftn(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldftn(inst); }
		static void Dispatch_ldind(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldind(inst); }
		static void Dispatch_ldlen(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldlen(inst); }
		static void Dispatch_ldloc(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldloc(inst); }
		static void Dispatch_ldloca(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldloca(inst); }
		static void Dispatch_ldnull(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldnull(inst); }
		static void Dispatch_ldobj(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldobj(inst); }
		static void Dispatch_ldsfld(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldsfld(inst); }
		static void Dispatch_ldsflda(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldsflda(inst); }
		static void Dispatch_ldstr(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldstr(inst); }
		static void Dispatch_ldtoken(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldtoken(inst); }
		static void Dispatch_ldvirtftn(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ldvirtftn(inst); }
		static void Dispatch_leave(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_leave(inst); }
		static void Dispatch_localloc(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_localloc(inst); }
		static void Dispatch_mkrefany(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_mkrefany(inst); }
		static void Dispatch_mul(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_mul(inst); }
		static void Dispatch_neg(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_neg(inst); }
		static void Dispatch_newarr(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_newarr(inst); }
		static void Dispatch_newobj(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_newobj(inst); }
		static void Dispatch_nop(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_nop(inst); }
		static void Dispatch_not(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_not(inst); }
		static void Dispatch_or(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_or(inst); }
		static void Dispatch_pop(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_pop(inst); }
		static void Dispatch_prefix1(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix1(inst); }
		static void Dispatch_prefix2(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix2(inst); }
		static void Dispatch_prefix3(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix3(inst); }
		static void Dispatch_prefix4(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix4(inst); }
		static void Dispatch_prefix5(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix5(inst); }
		static void Dispatch_prefix6(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix6(inst); }
		static void Dispatch_prefix7(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefix7(inst); }
		static void Dispatch_prefixref(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_prefixref(inst); }
		static void Dispatch_refanytype(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_refanytype(inst); }
		static void Dispatch_refanyval(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_refanyval(inst); }
		static void Dispatch_rem(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_rem(inst); }
		static void Dispatch_rem_Un(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_rem_Un(inst); }
		static void Dispatch_ret(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_ret(inst); }
		static void Dispatch_rethrow(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_rethrow(inst); }
		static void Dispatch_shl(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_shl(inst); }
		static void Dispatch_shr(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_shr(inst); }
		static void Dispatch_sizeof(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_sizeof(inst); }
		static void Dispatch_starg(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_starg(inst); }
		static void Dispatch_stelem(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_stelem(inst); }
		static void Dispatch_stfld(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_stfld(inst); }
		static void Dispatch_stind(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_stind(inst); }
		static void Dispatch_stloc(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_stloc(inst); }
		static void Dispatch_stobj(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_stobj(inst); }
		static void Dispatch_stsfld(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_stsfld(inst); }
		static void Dispatch_sub(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_sub(inst); }
		static void Dispatch_switch(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_switch(inst); }
		static void Dispatch_tail(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_tail(inst); }
		static void Dispatch_throw(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_throw(inst); }
		static void Dispatch_unaligned(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_unaligned(inst); }
		static void Dispatch_unbox(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_unbox(inst); }
		static void Dispatch_volatile(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_volatile(inst); }
		static void Dispatch_xor(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_xor(inst); }
		//C#2.0
		static void Dispatch_constrained(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_constrained(inst); }
		static void Dispatch_readonly(CompilerImpl compiler, CompiledInstruction inst) { compiler.Assemble_readonly(inst); }
		
		#endregion

		#region Methematical Operations

		void Assemble_add(CompiledInstruction inst) {
			TypeInfo t2 = this.Engine.Realize(inst.StackState[0]);
			TypeInfo t1 = this.Engine.Realize(inst.StackState[1]);
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Add32(!inst.Instruction.Unsigned, inst.Instruction.Overflow);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Add64(!inst.Instruction.Unsigned, inst.Instruction.Overflow);
			} else {
				throw new BadCodeException("sizes mismatch: "+t1+" ("+t1.VariableSize+") and "+t2+" ("+t2.VariableSize+")");
			}
		}

		void Assemble_sub(CompiledInstruction inst) {
			TypeInfo t2 = this.Engine.Realize(inst.StackState[0]);
			TypeInfo t1 = this.Engine.Realize(inst.StackState[1]);
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Sub32(!inst.Instruction.Unsigned, inst.Instruction.Overflow);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Sub64(!inst.Instruction.Unsigned, inst.Instruction.Overflow);
			} else {
				throw new BadCodeException("sizes mismatch: "+t1+" ("+t1.VariableSize+") and "+t2+" ("+t2.VariableSize+")");
			}
		}

		void Assemble_mul(CompiledInstruction inst) {
			TypeInfo t2 = this.Engine.Realize(inst.StackState[0]);
			TypeInfo t1 = this.Engine.Realize(inst.StackState[1]);
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Mul32(!inst.Instruction.Unsigned, inst.Instruction.Overflow);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Mul64(!inst.Instruction.Unsigned, inst.Instruction.Overflow);
			} else {
				throw new BadCodeException("sizes mismatch");
			}
		}

		void Assemble_div(CompiledInstruction inst) {
			TypeInfo t2 = this.Engine.Realize(inst.StackState[0]);
			TypeInfo t1 = this.Engine.Realize(inst.StackState[1]);
			if(t1.VariableSize<=4 && t2.VariableSize<=4) {
				this.assembler.Div32(!inst.Instruction.Unsigned);
			} else if(t1.VariableSize==8 && t2.VariableSize==8) {
				this.assembler.Div64(!inst.Instruction.Unsigned);
			} else {
				throw new BadCodeException("sizes mismatch");
			}
		}

		void Assemble_neg(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Negate64();
			} else {
				this.assembler.Negate32();
			}
		}

		#endregion

		#region Logical Operations

		void Assemble_and(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.And64();
			} else {
				this.assembler.And32();
			}
		}

		void Assemble_xor(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Xor64();
			} else {
				this.assembler.Xor32();
			}
		}

		void Assemble_not(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Not64();
			} else {
				this.assembler.Not32();
			}
		}

		void Assemble_or(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState.Top)) {
				this.assembler.Or64();
			} else {
				this.assembler.Or32();
			}
		}

		#endregion

		void Assemble_arglist(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		private void AssertToEqualComparingOperands(CompiledInstruction inst) {
			if(inst.StackState.Length<2) throw new BadCodeException("Stack is too few to compare");
			if(inst.StackState[0]!=inst.StackState[1]) {
				// C# generates the code that compares UIntPtr with Int32*.
				//if(Synthesizer.GetIntrinsicType(inst.StackState[0])!=Synthesizer.GetIntrinsicType(inst.StackState[1])) {
				int tz0 = Architecture.GetStackingSize(this.Engine.Realize(inst.StackState[0]).VariableSize);
				int tz1 = Architecture.GetStackingSize(this.Engine.Realize(inst.StackState[1]).VariableSize);
				if(tz0!=tz1) {
					throw new BadCodeException("Not same size about comparing operands: "+inst.StackState[0].FullName+" and "+inst.StackState[1].FullName);
				}
			}
		}

		private void AssembleConditionalBranch(CompiledInstruction inst, Condition cond, bool signed) {
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
			case IntrinsicTypes.Int4:
			case IntrinsicTypes.NInt:
			case IntrinsicTypes.Objt:
			case IntrinsicTypes.Pter:
				this.assembler.BranchI32(cond, signed, this.GetInstructionByAddress(inst.Instruction.BrTarget));
				break;
			case IntrinsicTypes.Int8:
				this.assembler.BranchI64(cond, signed, this.GetInstructionByAddress(inst.Instruction.BrTarget));
				break;
			case IntrinsicTypes.Fp32:
				this.assembler.BranchR32(cond, this.GetInstructionByAddress(inst.Instruction.BrTarget));
				break;
			case IntrinsicTypes.Fp64:
				this.assembler.BranchR64(cond, this.GetInstructionByAddress(inst.Instruction.BrTarget));
				break;
			default:
				throw new NotImplementedException();
			}
		}

		#region Branch Operations

		void Assemble_beq(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.Equal, true);
		}

		void Assemble_bge(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.GreaterOrEqual, !inst.Instruction.Unsigned);
		}

		void Assemble_bgt(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.GreaterThan, !inst.Instruction.Unsigned);
		}

		void Assemble_ble(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.LessOrEqual, !inst.Instruction.Unsigned);
		}

		void Assemble_blt(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.LessThan, !inst.Instruction.Unsigned);
		}

		void Assemble_bne(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.NotEqual, false);
		}

		void Assemble_br(CompiledInstruction inst) {
			this.assembler.Branch(this.GetInstructionByAddress(inst.Instruction.BrTarget));
		}

		void Assemble_brfalse(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.False, false);
		}

		void Assemble_brtrue(CompiledInstruction inst) {
			this.AssembleConditionalBranch(inst, Condition.True, false);
		}

		#endregion

		void Assemble_box(CompiledInstruction inst) {
			this.assembler.Box(inst.TypeInfo);
		}

		void Assemble_break(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_call(CompiledInstruction inst) {
			this.assembler.Call(inst.MethodInfo);
		}

		void Assemble_calli(CompiledInstruction inst) {
			this.assembler.CallInd(inst.MethodInfo);
		}

		void Assemble_callvirt(CompiledInstruction inst) {
			MethodInfo method = inst.MethodInfo;
			this.assembler.CallVirt(this.Engine.Realize(inst.StackState[method.ParameterCount]), method);
		}

		void Assemble_castclass(CompiledInstruction inst) {
			this.assembler.CastClass(inst.TypeInfo);
		}

		private void AssembleCompare(CompiledInstruction inst, Condition cond, bool signed) {
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
			case IntrinsicTypes.Int4:
			case IntrinsicTypes.NInt:
			case IntrinsicTypes.Objt:
			case IntrinsicTypes.Pter:
				this.assembler.CompareI32(cond, signed);
				break;
			case IntrinsicTypes.Int8:
				this.assembler.CompareI64(cond, signed);
				break;
			case IntrinsicTypes.Fp32:
				this.assembler.CompareR32(cond);
				break;
			case IntrinsicTypes.Fp64:
				this.assembler.CompareR64(cond);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		#region Compare Operations

		void Assemble_ceq(CompiledInstruction inst) {
			this.AssembleCompare(inst, Condition.Equal, true);
		}

		void Assemble_cgt(CompiledInstruction inst) {
			this.AssembleCompare(inst, Condition.GreaterThan, !inst.Instruction.Unsigned);
		}

		void Assemble_ckfinite(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_clt(CompiledInstruction inst) {
			this.AssembleCompare(inst, Condition.LessThan, !inst.Instruction.Unsigned);
		}

		#endregion

		void Assemble_conv(CompiledInstruction inst) {
			this.assembler.Convert(this.Engine.Realize(inst.StackState.Top), inst.OpType);
		}

		void Assemble_cpblk(CompiledInstruction inst) {
			this.assembler.CopyBlock();
		}

		void Assemble_cpobj(CompiledInstruction inst) {
			this.assembler.LoadConstant(inst.TypeInfo.VariableSize);
			this.assembler.CopyBlock();
		}

		void Assemble_dup(CompiledInstruction inst) {
			this.assembler.Duplicate(this.Engine.Realize(inst.StackState[0]).VariableSize);
		}

		void Assemble_endfilter(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_endfinally(CompiledInstruction inst) {
			//TODO: 'endfinally' instruction
		}

		void Assemble_initblk(CompiledInstruction inst) {
			this.assembler.InitializeMemory();
		}

		void Assemble_initobj(CompiledInstruction inst) {
			this.assembler.ClearMemory(inst.TypeInfo.VariableSize);
		}

		void Assemble_isinst(CompiledInstruction inst) {
			this.assembler.IsInst(inst.TypeInfo);
		}

		void Assemble_jmp(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_ldarg(CompiledInstruction inst) {
			this.assembler.LoadArg(inst.Instruction.Number);
		}

		void Assemble_ldarga(CompiledInstruction inst) {
			this.assembler.LoadArgAddress(inst.Instruction.Number);
		}

		void Assemble_ldc(CompiledInstruction inst) {
			switch(inst.Instruction.OpType.Name) {
			case "Int32":
				this.assembler.LoadConstant(inst.Instruction.Number);
				break;
			case "Int64":
				this.assembler.LoadConstant(inst.Instruction.GetNumeric<long>());
				break;
			case "Single":
				this.assembler.LoadConstant(inst.Instruction.GetNumeric<float>());
				break;
			case "Double":
				this.assembler.LoadConstant(inst.Instruction.GetNumeric<double>());
				break;
			default:
				throw new UnexpectedException();
			}
		}

		void Assemble_ldelem(CompiledInstruction inst) {
			this.assembler.LoadElement(this.Engine.Realize(inst.StackState[1].ElementType));
		}

		void Assemble_ldelema(CompiledInstruction inst) {
			this.assembler.LoadElementAddress(this.Engine.Realize(inst.StackState[1].ElementType));
		}

		void Assemble_ldfld(CompiledInstruction inst) {
			FieldInfo fi = inst.FieldInfo;
			this.assembler.LoadField(fi);
		}

		void Assemble_ldflda(CompiledInstruction inst) {
			FieldInfo fi = inst.FieldInfo;
			this.assembler.LoadFieldAddress(fi);
		}

		void Assemble_ldftn(CompiledInstruction inst) {
			this.assembler.LoadEntryPoint(inst.MethodInfo);
		}

		void Assemble_ldind(CompiledInstruction inst) {
			this.assembler.LoadInd(inst.OpType);
		}

		void Assemble_ldlen(CompiledInstruction inst) {
			this.assembler.LoadLength();
		}

		void Assemble_ldloc(CompiledInstruction inst) {
			this.assembler.LoadVar(inst.Instruction.Number);
		}

		void Assemble_ldloca(CompiledInstruction inst) {
			this.assembler.LoadVarAddress(inst.Instruction.Number);
		}

		void Assemble_ldnull(CompiledInstruction inst) {
			this.assembler.LoadNull();
		}

		void Assemble_ldobj(CompiledInstruction inst) {
			this.assembler.LoadObject(inst.TypeInfo);
		}

		void Assemble_ldsfld(CompiledInstruction inst) {
			FieldInfo field = inst.FieldInfo;
			if(!field.IsStatic) throw new BadCodeException();
			this.assembler.LoadField(field);
		}

		void Assemble_ldsflda(CompiledInstruction inst) {
			FieldInfo field = inst.FieldInfo;
			if(!field.IsStatic) throw new BadCodeException();
			this.assembler.LoadFieldAddress(field);
		}

		void Assemble_ldstr(CompiledInstruction inst) {
			this.assembler.LoadString(inst.Instruction.StringIndex);
		}

		void Assemble_ldtoken(CompiledInstruction inst) {
			switch(inst.Instruction.Extension) {
			case "td":
			case "tr":
			case "ts":
				this.assembler.LoadToken(inst.TypeInfo);
				break;
			case "fd":
				FieldInfo field = inst.FieldInfo;
				this.assembler.LoadToken(field);
				break;
			case "md":
				MethodInfo method = inst.MethodInfo;
				this.assembler.LoadToken(method);
				break;
			case "mr":
				switch(inst.Instruction.MemberInfo.Kind) {
				case System.Reflection.MemberTypes.Field:
					this.assembler.LoadToken(inst.FieldInfo);
					break;
				case System.Reflection.MemberTypes.Method:
					this.assembler.LoadToken(inst.MethodInfo);
					break;
				default:
					throw new NotSupportedException();
				}
				break;
			default:
				throw new NotSupportedException();
			}
		}

		void Assemble_ldvirtftn(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_leave(CompiledInstruction inst) {
			CompiledInstruction target = this.GetInstructionByAddress(inst.Instruction.BrTarget);
			TypeInfo[] discards = new TypeInfo[inst.StackState.Length-target.StackState.Length];
			for(int i=0; i<discards.Length; ++i) {
				discards[i] = this.Engine.Realize(inst.StackState[i]);
			}
			this.assembler.Leave(target, discards);
		}

		void Assemble_localloc(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_mkrefany(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_newarr(CompiledInstruction inst) {
			this.assembler.AllocateSzArray(inst.TypeInfo);
		}

		void Assemble_newobj(CompiledInstruction inst) {
			this.assembler.AllocateObject(inst.MethodInfo);
		}

		void Assemble_nop(CompiledInstruction inst) {
			// NOP
		}

		void Assemble_pop(CompiledInstruction inst) {
			this.assembler.Pop(this.Engine.Realize(inst.StackState.Top).VariableSize);
		}

		void Assemble_prefix1(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix2(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix3(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix4(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix5(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix6(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefix7(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_prefixref(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_refanytype(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_refanyval(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_rem(CompiledInstruction inst) {
			this.assembler.Rem32(true);
		}

		void Assemble_rem_Un(CompiledInstruction inst) {
			this.assembler.Rem32(false);
		}

		void Assemble_ret(CompiledInstruction inst) {
			this.assembler.Return();
		}

		void Assemble_rethrow(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_shl(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState[1])) {
				this.assembler.Shl64();
			} else {
				this.assembler.Shl32();
			}
		}

		void Assemble_shr(CompiledInstruction inst) {
			if(Synthesizer.IsInt64Compatible(inst.StackState[1])) {
				this.assembler.Shr64(!inst.Instruction.Unsigned);
			} else {
				this.assembler.Shr32(!inst.Instruction.Unsigned);
			}
		}

		void Assemble_sizeof(CompiledInstruction inst) {
			this.assembler.LoadConstant(inst.TypeInfo.VariableSize);
		}

		void Assemble_starg(CompiledInstruction inst) {
			this.assembler.StoreArg(inst.Instruction.Number);
		}

		void Assemble_stelem(CompiledInstruction inst) {
			this.assembler.StoreElement(this.Engine.Realize(inst.StackState[0]));
		}

		void Assemble_stfld(CompiledInstruction inst) {
			this.assembler.StoreField(inst.FieldInfo);
		}

		void Assemble_stind(CompiledInstruction inst) {
			this.assembler.StoreInd(inst.OpType);
		}

		void Assemble_stloc(CompiledInstruction inst) {
			this.assembler.StoreVar(inst.Instruction.Number);
		}

		void Assemble_stobj(CompiledInstruction inst) {
			if(inst.TypeInfo.IsValueType) {
				this.assembler.StoreObject(inst.TypeInfo, this.Engine.Realize(inst.StackState[0]));
			} else {
				this.assembler.StoreInd(inst.OpType);
			}
		}

		void Assemble_stsfld(CompiledInstruction inst) {
			FieldInfo field = inst.FieldInfo;
			if(!field.IsStatic) throw new BadCodeException();
			this.assembler.StoreField(field);
		}

		void Assemble_switch(CompiledInstruction inst) {
			IBranchTarget[] targets = new CompiledInstruction[inst.Instruction.BrTargets.Length];
			for(int i=0; i<targets.Length; ++i) {
				targets[i] = this.GetInstructionByAddress(inst.Instruction.BrTargets[i]);
			}
			this.assembler.Switch(targets);
		}

		void Assemble_tail(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_throw(CompiledInstruction inst) {
			this.assembler.Throw();
		}

		void Assemble_unaligned(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_unbox(CompiledInstruction inst) {
			TypeInfo type = inst.TypeInfo;
			this.assembler.Unbox(type);
		}

		void Assemble_volatile(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_constrained(CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		void Assemble_readonly(CompiledInstruction inst) {
			throw new NotImplementedException();
		}


	}

}
