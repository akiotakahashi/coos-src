using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;
using CooS.Reflection;
using CooS.Reflection.CLI;
using CooS.Execution;
using BindingFlags=System.Reflection.BindingFlags;
using CooS.Formats;

namespace CooS.Compile.CLI {
		
	partial class Synthesizer {

		public void Eval_add(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryNumericOperation(frame, inst.Instruction);
		}

		public void Eval_and(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateIntegerOperation(frame);
		}

		public void Eval_arglist(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_beq(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public void Eval_bge(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public void Eval_bgt(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public void Eval_ble(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public void Eval_blt(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public void Eval_bne(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public void Eval_box(EvaluationFrame frame, CompiledInstruction inst) {
			TypeBase type = inst.TypeInfo.Base;
			if(!type.IsValueType) throw new BadCodeException("box can't perform about non-valuetype: "+type);
			/*
			if(!type.IsAssignableFrom(GetStackingType(frame.Stack.Top))) {
				Console.WriteLine("{0}", type.FullName);
				Console.WriteLine("{0}", GetStackingType(frame.Stack.Top).FullName);
				throw new BadCodeException("box operands are not assignable: "+type+" from "+frame.Stack.Top);
			}
			*/
			if(type.IsEnum) {
				// <enum type>.ToString() は、
				//		box <enum-type>
				//		call Enum.ToString()
				// に変換されるので、、、
				frame.Stack.Change(1, this.world.Resolve(PrimitiveTypes.Enum));
			} else {
				frame.Stack.Change(1, this.world.Resolve(PrimitiveTypes.Object));
			}
		}

		public void Eval_br(EvaluationFrame frame, CompiledInstruction inst) {
			//No change of frame.Stack-state
			//TODO: check the range of br-target.
		}

		public void Eval_break(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_brfalse(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBoolBranchOperation(frame.Stack, inst.Instruction);
		}

		public void Eval_brtrue(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBoolBranchOperation(frame.Stack, inst.Instruction);
		}

		public void Eval_call(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateCallOperation(frame, inst.Instruction);
		}

		public void Eval_calli(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsPointerKind(frame.Stack.Pop())) throw new BadCodeException();
			EvaluateCallOperation(frame, inst.Instruction);
		}

		public void Eval_callvirt(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateCallOperation(frame, inst.Instruction);
		}

		public void Eval_castclass(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Pop();
			frame.Stack.Push(inst.TypeInfo.Base);
		}

		public void Eval_ckfinite(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_ceq(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryComparisonOperation(frame, inst.OpCode);
		}

		public void Eval_cgt(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryComparisonOperation(frame, inst.OpCode);
		}

		public void Eval_clt(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryComparisonOperation(frame, inst.OpCode);
		}

		public void Eval_conv(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateConversionOperation(frame, inst.Instruction);
		}

		public void Eval_cpblk(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(frame.Stack[0])) throw new BadCodeException();
			if(!IsPointerKind(frame.Stack[1])) throw new BadCodeException();
			if(!IsPointerKind(frame.Stack[2])) throw new BadCodeException();
			frame.Stack.Pop(3);
		}

		public void Eval_cpobj(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsPointerKind(frame.Stack[1])) throw new BadCodeException();
			if(!IsPointerKind(frame.Stack[2])) throw new BadCodeException();
			frame.Stack.Pop(2);
		}

		public void Eval_div(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryNumericOperation(frame, inst.Instruction);
		}

		public void Eval_dup(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(inst.StackState[0]);
		}

		public void Eval_endfilter(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_endfinally(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_initblk(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(inst.StackState[0])) throw new BadCodeException();
			if(!IsInt32Suitable(inst.StackState[1])) throw new BadCodeException();
			if(!IsPointerKind(inst.StackState[2])) throw new BadCodeException();
			frame.Stack.Pop(3);
		}

		public void Eval_initobj(EvaluationFrame frame, CompiledInstruction inst) {
			if(!inst.TypeInfo.IsValueType) throw new BadCodeException();
			frame.Stack.Pop(1);
		}

		public void Eval_isinst(EvaluationFrame frame, CompiledInstruction inst) {
			TypeBase type = inst.TypeInfo.Base;
			if(type.IsValueType) {
				// 値型はObjectのまま
			} else {
				frame.Stack.Change(1, type);
			}
		}

		public void Eval_jmp(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_ldarg(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.LoadArg(inst.Instruction.Number);
		}

		public void Eval_ldarga(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.LoadArg(inst.Instruction.Number);
			TypeBase type = frame.Stack.Pop();
			frame.Stack.Push(type.GetByRefPointerType());
		}

		public void Eval_ldc(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(inst.OpType.Base);
		}

		public void Eval_ldelem(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(frame.Stack[0])) throw new BadCodeException("1st operand must be Index");
			if(!frame.Stack[1].IsArray) throw new BadCodeException("2nd operand must be Array");
			//TODO: Skip the check whether if element-type matches the target type because Char is loaded by using ldelem.u2.
			//if(stack[1].GetElementType()!=type) throw new BadCodeException("Type incompatible");
			frame.Stack.Change(2, frame.Stack[1].ElementType);
		}

		public void Eval_ldelema(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(frame.Stack[0])) throw new BadCodeException("1st operand must be Index");
			if(!frame.Stack[1].IsArray) throw new BadCodeException("2nd operand must be Array");
			frame.Stack.Change(2, (frame.Stack[1].ElementType).GetByRefPointerType());
		}

		public void Eval_ldfld(EvaluationFrame frame, CompiledInstruction inst) {
			FieldBase fi = inst.FieldInfo.Base;
			TypeBase type = frame.Stack.Pop();
			if(!(IsObjectKind(type) || type.IsPointer || type.IsValueType)) { throw new BadCodeException("ldfld requires object as its operand."); }
			frame.Stack.Push(fi.ReturnType);
		}

		public void Eval_ldflda(EvaluationFrame frame, CompiledInstruction inst) {
			FieldBase field = inst.FieldInfo.Base;
			if(field.IsStatic) throw new BadCodeException();
			TypeBase type = frame.Stack.Pop();
			if(type.IsPointer) {
				type = type.ElementType;
			}
			if(!field.Type.IsAssignableFrom(type)) throw new BadCodeException();
			frame.Stack.Push(field.ReturnType.GetByRefPointerType());
		}

		public void Eval_ldftn(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(this.world.Resolve(PrimitiveTypes.U1).GetByRefPointerType());
		}

		public void Eval_ldind(EvaluationFrame frame, CompiledInstruction inst) {
			TypeBase address = frame.Stack.Pop();
			if(address.IsByRefPointer) {
				//ldind.ref < InterfaceBase[]& InterfaceBase[]& ConcreteType
				//ldloc.s   < Object InterfaceBase[]& ConcreteType
				frame.Stack.Push(address.ElementType);
			} else if(IsPointerKind(address)) {
				//ldind.i4  < IKernel* UInt32 IKernel* UInt32
				//ldc.i4.s  < IKernel UInt32 IKernel* UInt32
				//frame.Stack.Push(address.GetElementType());
				frame.Stack.Push(inst.OpType.Base);
			} else if(address==this.world.Resolve(PrimitiveTypes.I4)) {
				//COOS: Microsoft .NET Framework may allows Int32 as a pointer.
				frame.Stack.Push(inst.OpType.Base);
			} else {
				throw new BadCodeException();
			}
		}

		public void Eval_ldlen(EvaluationFrame frame, CompiledInstruction inst) {
			if(!frame.Stack.Top.IsArray) throw new BadCodeException("ldlen's operand must be Array");
			frame.Stack.Change(1, this.world.Resolve(PrimitiveTypes.I4));
		}

		public void Eval_ldloc(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(frame.Method.GetVariableType(inst.Instruction.Number));
		}

		public void Eval_ldloca(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push((frame.Method.GetVariableType(inst.Instruction.Number)).GetByRefPointerType());
		}

		public void Eval_ldnull(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(this.world.Resolve(PrimitiveTypes.Object));
		}

		public void Eval_ldobj(EvaluationFrame frame, CompiledInstruction inst) {
			TypeBase type = inst.TypeInfo.Base;
			if(!type.IsValueType) throw new BadCodeException("ldobj can't performs about "+type+".");
			if(!IsPointerKind(frame.Stack.Pop())) throw new BadCodeException("ldobj requires pointer-kind value.");
			frame.Stack.Push(type);
		}

		public void Eval_ldsfld(EvaluationFrame frame, CompiledInstruction inst) {
			FieldBase field = inst.FieldInfo.Base;
			if(!field.IsStatic) throw new BadCodeException();
			frame.Stack.Push(field.ReturnType);
		}

		public void Eval_ldsflda(EvaluationFrame frame, CompiledInstruction inst) {
			FieldBase field = inst.FieldInfo.Base;
			if(!field.IsStatic) throw new BadCodeException();
			frame.Stack.Push((field.ReturnType).GetByRefPointerType());
		}

		public void Eval_ldstr(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(this.world.Resolve(PrimitiveTypes.String));
		}

		public void Eval_ldtoken(EvaluationFrame frame, CompiledInstruction inst) {
			switch(inst.Instruction.Extension) {
			case "td":
			case "tr":
			case "ts":
				frame.Stack.Push(this.world.ResolveType("RuntimeTypeHandle","System"));
				break;
			case "fd":
				frame.Stack.Push(this.world.ResolveType("RuntimeFieldHandle", "System"));
				break;
			case "md":
				frame.Stack.Push(this.world.ResolveType("RuntimeMethodHandle", "System"));
				break;
			case "mr":
				MemberBase member = inst.Instruction.MemberInfo;
				switch(member.Kind) {
				case System.Reflection.MemberTypes.Field:
					frame.Stack.Push(this.world.ResolveType("RuntimeFieldHandle","System"));
					break;
				case System.Reflection.MemberTypes.Method:
					frame.Stack.Push(this.world.ResolveType("RuntimeMethodHandle","System"));
					break;
				default:
					throw new NotSupportedException(member.Name);
				}
				break;
			}
		}

		public void Eval_ldvirtftn(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_leave(EvaluationFrame frame, CompiledInstruction inst) {
			// The leave instruction empties the evaluation stack and ensures that
			// the appropriate surrounding finally blocks are executed.
			frame.Stack.Pop(frame.Stack.Length-frame.Method.ArgumentCount);
		}

		public void Eval_localloc(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_mkrefany(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_mul(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryNumericOperation(frame, inst.Instruction);
		}

		public void Eval_neg(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateUnaryNumericOperation(frame.Stack, inst.OpCode);
		}

		public void Eval_newarr(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(frame.Stack.Pop())) throw new BadCodeException();
			TypeBase type = inst.TypeInfo.Base;
			frame.Stack.Push(type.GetSzArrayType());
		}

		public void Eval_newobj(EvaluationFrame frame, CompiledInstruction inst) {
			MethodInfo method = EvaluateCallOperation(frame, inst.Instruction, true);
			frame.Stack.Push(method.Type);
		}

		public void Eval_nop(EvaluationFrame frame, CompiledInstruction inst) {
			// NOP
		}

		public void Eval_not(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(frame.Stack.Top) && !IsInt64Compatible(frame.Stack.Top)) {
				throw new BadCodeException();
			}
		}

		public void Eval_or(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateIntegerOperation(frame);
		}

		public void Eval_pop(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Pop();
		}

		public void Eval_prefix1(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefix2(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefix3(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefix4(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefix5(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefix6(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefix7(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_prefixref(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_refanytype(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_refanyval(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_rem(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryNumericOperation(frame, inst.Instruction);
			//? EvaluateIntegerOperation(frame);
		}

		public void Eval_ret(EvaluationFrame frame, CompiledInstruction inst) {
			if(frame.Method.ReturnType!=this.world.Resolve(PrimitiveTypes.Void)) {
				frame.Stack.Pop();
			}
		}

		public void Eval_rethrow(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_shl(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateShiftOperation(frame);
		}

		public void Eval_shr(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateShiftOperation(frame);
		}

		public void Eval_sizeof(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.Push(this.world.Resolve(PrimitiveTypes.I4));
		}

		public void Eval_starg(EvaluationFrame frame, CompiledInstruction inst) {
			frame.Stack.StoreArg(inst.Instruction.Number);
		}

		public void Eval_stelem(EvaluationFrame frame, CompiledInstruction inst) {
			if(!frame.Stack[2].IsArray) throw new BadCodeException();
			if(!IsInt32Suitable(frame.Stack[1])) throw new BadCodeException();
			// stelem.i2 < Int32 Int32 Char[]
			/*
			if(!frame.Stack[2].GetElementType().IsAssignableFrom(frame.Stack[0])) {
				throw new BadCodeException();
			}
			*/
			frame.Stack.Pop(3);
		}

		public void Eval_stfld(EvaluationFrame frame, CompiledInstruction inst) {
			FieldBase fi = inst.FieldInfo.Base;
			if(fi.IsStatic) throw new BadCodeException("Don't use stfld for static field");
			//if(!IsObjectKind(frame.Stack.Top)) throw new BadCodeException("stfld requires object as its operand.");
			frame.Stack.Pop();
			frame.Stack.Pop();
		}

		public void Eval_stind(EvaluationFrame frame, CompiledInstruction inst) {
			if(!Synthesizer.IsPointerKind(inst.StackState[1])) throw new BadCodeException();
			//if(inst.StackState[0].VariableSize!=(inst.StackState[1].GetElementType()).VariableSize) throw new BadCodeException();
			frame.Stack.Pop();
			frame.Stack.Pop();
		}

		public void Eval_stloc(EvaluationFrame frame, CompiledInstruction inst) {
			//TODO: check type compatibility for method's local variable.
			//TODO: check whether if the index is in correct range.
			frame.Stack.Pop();
		}

		public void Eval_stobj(EvaluationFrame frame, CompiledInstruction inst) {
			if(inst.TypeInfo.IsValueType) {
				if(!frame.Stack[0].IsValueType) { throw new BadCodeException(); }
				if(!IsPointerKind(frame.Stack[1])) { throw new BadCodeException(); }
				frame.Stack.Pop(2);
			} else {
				this.Eval_stind(frame, inst);
			}
		}

		public void Eval_stsfld(EvaluationFrame frame, CompiledInstruction inst) {
			FieldBase fi = inst.FieldInfo.Base;
			if(!fi.IsStatic) throw new BadCodeException("Don't use stsfld for instance field");
			//if(!IsObjectKind(frame.Stack.Top)) throw new BadCodeException("stfld requires object as its operand.");
			frame.Stack.Pop();
		}

		public void Eval_sub(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateBinaryNumericOperation(frame, inst.Instruction);
		}

		public void Eval_switch(EvaluationFrame frame, CompiledInstruction inst) {
			if(!IsInt32Suitable(inst.StackState[0]) && !inst.StackState[0].IsEnum) {
				throw new BadCodeException();
			}
			frame.Stack.Pop();
		}

		public void Eval_tail(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_throw(EvaluationFrame frame, CompiledInstruction inst) {
			if(!this.world.ResolveType("Exception","System").IsAssignableFrom(inst.StackState[0])) {
				throw new BadCodeException();
			}
			frame.Stack.Pop();
		}

		public void Eval_unaligned(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_unbox(EvaluationFrame frame, CompiledInstruction inst) {
			TypeBase type = inst.TypeInfo.Base;
			if(!type.IsValueType) throw new BadCodeException("unbox requires the token indicates ValueType.");
			if(frame.Stack.Pop().IsValueType) throw new BadOperandException("unbox can apply to Object value.");
			//TODO: check type compatibility.
			frame.Stack.Push(type.GetByRefPointerType());
		}

		public void Eval_volatile(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_xor(EvaluationFrame frame, CompiledInstruction inst) {
			EvaluateIntegerOperation(frame);
		}

		public void Eval_constrained(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

		public void Eval_readonly(EvaluationFrame frame, CompiledInstruction inst) {
			throw new NotImplementedException();
		}

	}

}
