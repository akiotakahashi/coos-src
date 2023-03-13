using System;
using CooS.Reflection;
using CooS.Execution;
using CooS.Formats.CLI.CodeModel;

namespace CooS.Interpret.CLI {

	partial class InterpreterImpl {

		public Flow Interpret_nop(Machine machine, Frame frame, Instruction inst) {
			return Flow.Next;
		}

		public Flow Interpret_ldarg(Machine machine, Frame frame, Instruction inst) {
			if(!frame.Method.IsConstructor) {
				Block arg = machine.Stack[frame.BP, frame.Method.ArgumentCount-inst.Number-1];
				machine.Stack.Push(arg, true);
				return Flow.Next;
			} else {
				int i;
				if(inst.Number==0) {
					i = frame.Method.ParameterCount;
				} else {
					i = frame.Method.ArgumentCount-inst.Number;
				}
				Block arg = machine.Stack[frame.BP, i];
				machine.Stack.Push(arg, true);
				return Flow.Next;
			}
		}

		public Flow Interpret_newobj(Machine machine, Frame frame, Instruction inst) {
			MethodBase method = (MethodBase)frame.Method._Assembly.Realize(inst.MethodInfo);
			if(!method.IsConstructor) throw new MissingMethodException();
			if(method.IsStatic) throw new MissingMethodException();
			TypeInfo type = machine.Engine.Realize(method.Type);
			Address p = machine.Heap.AllocateObject(type);
			machine.Stack.Push(p);
			Flow ret = this.Execute(method);
			machine.Stack.Push(p);
			return ret;
		}

		public Flow Interpret_call(Machine machine, Frame frame, Instruction inst) {
			MethodBase method = (MethodBase)frame.Method._Assembly.Realize(inst.MethodInfo);
			return this.Execute(method);
		}

		public Flow Interpret_callvirt(Machine machine, Frame frame, Instruction inst) {
			MethodBase method = (MethodBase)frame.Method._Assembly.Realize(inst.MethodInfo);
			Block obj = machine[(Address)machine.Stack[method.ParameterCount]];
			//obj.Type.FindActualMethod(inst.MethodInfo);
			return this.Execute(method);
		}

		public Flow Interpret_ret(Machine machine, Frame frame, Instruction inst) {
			if(!frame.Method.HasReturnValue) {
				machine.Stack.Pop(frame.Method.ArgumentCount);
			} else {
				Block blk = machine.Stack.Pop();
				machine.Stack.Pop(frame.Method.ArgumentCount);
				machine.Stack.Push(blk, false);
			}
			return Flow.Return;
		}

		public Flow Interpret_stloc(Machine machine, Frame frame, Instruction inst) {
			Block val = machine.Stack.Pop();
			frame.Variables[inst.Number] = val;
			return Flow.Next;
		}

		public Flow Interpret_ldloc(Machine machine, Frame frame, Instruction inst) {
			Block val = frame.Variables[inst.Number];
			machine.Stack.Push(val, true);
			return Flow.Next;
		}

		public Flow Interpret_ldc(Machine machine, Frame frame, Instruction inst) {
			machine.Stack.Push(inst.Number);
			return Flow.Next;
		}

		public Flow Interpret_br(Machine machine, Frame frame, Instruction inst) {
			frame.Jump(inst.BrTarget);
			return Flow.Jump;
		}

		public Flow Interpret_brfalse(Machine machine, Frame frame, Instruction inst) {
			int value = (int)machine.Stack.Pop();
			if(value==0) {
				frame.Jump(inst.BrTarget);
				return Flow.Jump;
			} else {
				return Flow.Next;
			}
		}

		public Flow Interpret_brtrue(Machine machine, Frame frame, Instruction inst) {
			int value = (int)machine.Stack.Pop();
			if(value!=0) {
				frame.Jump(inst.BrTarget);
				return Flow.Jump;
			} else {
				return Flow.Next;
			}
		}

	}

}
