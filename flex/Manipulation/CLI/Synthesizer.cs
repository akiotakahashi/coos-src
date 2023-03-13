using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using CooS.Formats.CLI;
using CooS.Formats.CLI.Metadata;
using CooS.Reflection;
using CooS.Reflection.CLI;
using CooS.Reflection.CLI.Metatype;

namespace CooS.Manipulation.CLI {

	delegate void InstructionEvaluator(EvaluationFrame frame, Instruction inst);
	
	class Synthesizer {

		static readonly InstructionEvaluator[] evals1;
		static readonly Hashtable evals2 = new Hashtable();
		readonly AssemblyDefInfo assembly;

		static Synthesizer() {
			Console.WriteLine("Constructing Synthesizer");
			evals1 = new InstructionEvaluator[256];
			#region OpCode Registrations
			foreach(FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(fi.FieldType!=typeof(OpCode)) continue;
				OpCode opcode = (OpCode)fi.GetValue(null);
				int i = opcode.Name.IndexOf('.');
				if(i<0) i=opcode.Name.Length;
				string methodname = "Eval_"+opcode.Name.Substring(0,i).ToLower();
				Delegate d = Delegate.CreateDelegate(typeof(InstructionEvaluator), typeof(Synthesizer), methodname);
				Register(opcode, (InstructionEvaluator)d);
			}
			#endregion
		}

		private static void Register(OpCode opcode, InstructionEvaluator eval) {
			if(opcode.Size==1) {
				evals1[opcode.Value] = eval;
			} else if(opcode.Size==2) {
				evals2[opcode.Value] = eval;
			} else {
				throw new NotSupportedException();
			}
		}

		public static Instruction[] Perform(MethodDefInfo method, Stream stream) {
			Synthesizer syn = new Synthesizer(method.MyAssembly);
			return syn.Preprocess(method, stream);
		}

		private Synthesizer(AssemblyDefInfo assembly) {
			this.assembly = assembly;
		}

		private Instruction[] Preprocess(MethodDefInfo method, Stream stream) {
			ILStream ils = new ILStream(this.assembly, stream);
			ArrayList instlist = new ArrayList();
			while(!ils.AtEndOfStream) {
				instlist.Add(ils.Read());
				//Console.WriteLine(instlist[instlist.Count-1]);
			}
			Instruction[] insts = (Instruction[])instlist.ToArray(typeof(Instruction));
			EvaluationFrame frame = new EvaluationFrame(method);
			for(int i=0; i<method.ArgumentCount; ++i) {
				frame.Stack.Push(method.GetArgumentType(i));
			}
			DetermineStackState(insts, 0, frame);
			return insts;
		}

		private void DetermineStackState(Instruction[] insts, int startIndex, EvaluationFrame frame) {
			for(int i=startIndex; i<insts.Length; ++i) {
				if(insts[i].StackState!=null) {
					//Console.WriteLine("interflow");
					//スタックの合流検査
#if false
					if(!insts[i].StackState.IsCompatibleWith(frame.Stack)) {
						Console.WriteLine();
						insts[i].Dump(Console.Out);
						Console.Write("Actual: ");
						for(int j=0; j<frame.Stack.Length; ++j) {
							Console.Write(" {0}", frame.Stack[j].Name);
						}
						Console.WriteLine();
						throw new BadILException("Evaluation Stack is not together at a interflow point: 0x"+insts[i].Address.ToString("X"));
					}
#endif
					return;
				} else {
					insts[i].StackState = (EvaluationStack)frame.Stack.Clone();
					//**********
					//insts[i].Dump(Console.Out);
					//**********
					EvaluateInstruction(insts[i], frame);
					if(insts[i].OpCode.Value==OpCodes.Ret.Value) {
						return;
					} else if(insts[i].OpCode.Value==OpCodes.Throw.Value) {
						return;
					} else {
						if(insts[i].BrTarget>=0) {
							//Console.WriteLine("branch");
							DetermineStackState(insts, GetBrTargetIndex(insts, insts[i].BrTarget), (EvaluationFrame)frame.Clone());
							if(insts[i].OpCode.Value==OpCodes.Br.Value) return;
							if(insts[i].OpCode.Value==OpCodes.Br_S.Value) return;
							if(insts[i].OpCode.Value==OpCodes.Leave.Value) return;
							if(insts[i].OpCode.Value==OpCodes.Leave_S.Value) return;
						} else if(insts[i].BrTargets!=null) {
							int[] targets = (int[])insts[i].Operand;
							foreach(int target in targets) {
								//Console.WriteLine("branch");
								DetermineStackState(insts, GetBrTargetIndex(insts, target), (EvaluationFrame)frame.Clone());
							}
						}
					}
				}
			}
		}

		private static SuperType nullType = null;

		public static SuperType NullType {
			get {
				if(nullType==null) {
					nullType = (SuperType)CooS.Management.AssemblyResolver.Manager.ResolveType("CooS.Manipulation.CLI.NullType",true);
				}
				return nullType;
			}
		}

		private int GetBrTargetIndex(Instruction[] insts, int target) {
			int i = Array.BinarySearch(insts, target, new AddressComparator());
			if(i<0) throw new BadILException("Branch target does not point to the beginning of instruction: 0x"+target.ToString("X"));
			return i;
		}
		
		class AddressComparator : IComparer {
			public int Compare(object x, object y) {
				return ((Instruction)x).Address.CompareTo((int)y);
			}
		}

		private void EvaluateInstruction(Instruction inst, EvaluationFrame frame) {
			if(inst.OpCode.Size==1) {
				evals1[inst.OpCode.Value](frame, inst);
			} else if(inst.OpCode.Size==2) {
				((InstructionEvaluator)evals2[inst.OpCode.Value])(frame, inst);
			} else {
				throw new BadILException();
			}
		}

		public static bool IsInt32Suitable(Type _type) {
			TypeImpl type = (TypeImpl)_type;
			return type.IsEnum
				|| (type.IsPrimitive
				&& type.VariableSize<=4
				&& !type.IsByRefPointer
				&& !type.IsByValPointer);
		}

		public static bool IsInt64Compatible(Type _type) {
			TypeImpl type = (TypeImpl)_type;
			return type.IsPrimitive && (type.Name=="Int64" || type.Name=="UInt64");
		}

		public static bool IsNIntCompatible(Type _type) {
			TypeImpl type = (TypeImpl)_type;
			return (type.IsPrimitive && type.VariableSize<=4)
				|| type.IsByRefPointer || type.IsByValPointer;
		}

		public static bool IsNIntSuitable(Type _type) {
			TypeImpl type = (TypeImpl)_type;
			return (type.IsPrimitive && (type.Name=="IntPtr" || type.Name=="UIntPtr"))
				|| type.IsByRefPointer || type.IsByValPointer;
		}

		public static bool IsFloatCompatible(Type type) {
			return (type.IsPrimitive && (type.Name=="Double" || type.Name=="Single"));
		}

		public static bool IsObjectCompatible(Type type) {
			return !type.IsValueType;
		}

		public static IntrinsicType GetIntrinsicType(Type type) {
			if(type.IsPointer) {
				return IntrinsicType.Pter;
			} else if(IsNIntSuitable(type)) {
				return IntrinsicType.NInt;
			} else if(IsInt32Suitable(type)) {
				return IntrinsicType.Int4;
			} else if(IsObjectCompatible(type)) {
				return IntrinsicType.Objt;
			} else if(IsInt64Compatible(type)) {
				return IntrinsicType.Int8;
			} else if(IsFloatCompatible(type)) {
				return type.Name=="Double" ? IntrinsicType.Fp64 : IntrinsicType.Fp32;
			} else if(type.IsEnum) {
				return IntrinsicType.Int4;
			} else if(type.IsValueType) {
				return IntrinsicType.Any;
			} else {
				throw new ArgumentException(type.FullName);
			}
		}

		private static TypeImpl GetStackingType(SuperType type) {
			switch(GetIntrinsicType(type)) {
			case IntrinsicType.Pter:
				return type.MyAssembly.Manager.IntPtr;
			case IntrinsicType.Int4:
				return type.MyAssembly.Manager.Int32;
			case IntrinsicType.NInt:
				return type.MyAssembly.Manager.IntPtr;
			case IntrinsicType.Objt:
				return type;
			case IntrinsicType.Int8:
				return type.MyAssembly.Manager.Int64;
			case IntrinsicType.Fp32:
				return type.MyAssembly.Manager.Single;
			case IntrinsicType.Fp64:
				return type.MyAssembly.Manager.Double;
			case IntrinsicType.Any:
				return type;
			default:
				throw new NotSupportedException(type.ToString());
			}
		}

		private static bool IsEqualFamily(OpCode opcode) {
			return opcode.Value==OpCodes.Beq.Value
				|| opcode.Value==OpCodes.Beq_S.Value
				|| opcode.Value==OpCodes.Bne_Un.Value
				|| opcode.Value==OpCodes.Bne_Un_S.Value
				|| opcode.Value==OpCodes.Ceq.Value;
		}

		private static void EvaluateBinaryNumericOperation(EvaluationFrame frame, Instruction inst) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicType.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
					stack.Change(2, frame.Assembly.Manager.Int32);
					return;
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.IntPtr);
					return;
				case IntrinsicType.Pter:
					if(inst.CoreName=="add") {
						stack.Change(2, 0);
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int8:
					stack.Change(2, frame.Assembly.Manager.Int64);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.IntPtr);
					return;
				case IntrinsicType.Pter:
					if(inst.CoreName=="add") {
						stack.Change(2, 0);
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Fp32:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Fp32:
					stack.Change(2, frame.Assembly.Manager.Single);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Fp64:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Fp64:
					stack.Change(2, frame.Assembly.Manager.Double);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Pter:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
					if(inst.CoreName=="add" || inst.CoreName=="sub") {
						stack.Change(2, 1);
						return;
					} else {
						throw new BadOperandException();
					}
				case IntrinsicType.Pter:
					if(inst.CoreName=="sub") {
						stack.Change(2, frame.Assembly.Manager.IntPtr);
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException();
			}
		}

		private static void EvaluateUnaryNumericOperation(EvaluationStack stack, OpCode opcode) {
			switch(GetIntrinsicType(stack[0])) {
			case IntrinsicType.Int4:
			case IntrinsicType.Int8:
			case IntrinsicType.NInt:
			case IntrinsicType.Fp32:
			case IntrinsicType.Fp64:
				return;
			default:
				throw new BadOperandException();
			}
		}

		private static void EvaluateBinaryComparisonOperation(EvaluationFrame frame, OpCode opcode) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicType.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
				case IntrinsicType.Pter:
					stack.Change(2, frame.Assembly.Manager.Boolean);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int8:
					stack.Change(2, frame.Assembly.Manager.Boolean);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
				case IntrinsicType.Pter:
					stack.Change(2, frame.Assembly.Manager.Boolean);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Fp32:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Fp32:
					stack.Change(2, frame.Assembly.Manager.Boolean);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Fp64:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Fp64:
					stack.Change(2, frame.Assembly.Manager.Boolean);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Pter:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
				case IntrinsicType.Pter:
					stack.Change(2, frame.Assembly.Manager.Boolean);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Objt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Objt:
					if(IsEqualFamily(opcode)) {
						stack.Change(2, frame.Assembly.Manager.Boolean);
						return;
					} else if(opcode.Value==OpCodes.Cgt_Un.Value) {
						stack.Change(2, frame.Assembly.Manager.Boolean);
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException();
			}
		}

		private static void EvaluateBranchOperation(EvaluationFrame frame, OpCode opcode) {
			EvaluateBinaryComparisonOperation(frame, opcode);
			frame.Stack.Pop();
		}

		private static void EvaluateIntegerOperation(EvaluationFrame frame) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicType.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
					stack.Change(2, frame.Assembly.Manager.Int32);
					return;
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.IntPtr);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int8:
					stack.Change(2, frame.Assembly.Manager.Int64);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.IntPtr);
					return;
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException();
			}
		}

		private static void EvaluateShiftOperation(EvaluationFrame frame) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicType.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.Int32);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.Int64);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicType.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicType.Int4:
				case IntrinsicType.NInt:
					stack.Change(2, frame.Assembly.Manager.IntPtr);
					return;
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException();
			}
		}

		private static void EvaluateConversionOperation(EvaluationFrame frame, Instruction inst) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[0])) {
			case IntrinsicType.Int4:
			case IntrinsicType.Int8:
			case IntrinsicType.NInt:
			case IntrinsicType.Fp32:
			case IntrinsicType.Fp64:
			case IntrinsicType.Pter:
				stack.Change(1, inst.OpType);
				break;
			case IntrinsicType.Objt:
				if(IsPointerKind(inst.OpType)) {
					stack.Change(1, inst.OpType);
				} else {
					throw new BadOperandException("Operand "+stack[0]+" can't be converted to "+inst.OpType);
				}
				break;
			default:
				throw new NotSupportedException(stack[0].FullName);
			}
		}

		private static bool IsPointerKind(Type type) {
			if(type.IsPointer) {
				return true;
			} else if(type.IsPrimitive && type.Name=="Int32") {
				return true;
			} else if(type.IsPrimitive && type.Name=="IntPtr") {
				return true;
			} else if(type.IsPrimitive && type.Name=="UIntPtr") {
				return true;
			} else {
				return false;
			}
		}

		private static bool IsObjectKind(Type type) {
			return !type.IsValueType || IsPointerKind(type);
		}

		public static void Eval_add(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryNumericOperation(frame, inst);
		}

		public static void Eval_and(EvaluationFrame frame, Instruction inst) {
			EvaluateIntegerOperation(frame);
		}

		public static void Eval_arglist(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_beq(EvaluationFrame frame, Instruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public static void Eval_bge(EvaluationFrame frame, Instruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public static void Eval_bgt(EvaluationFrame frame, Instruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public static void Eval_ble(EvaluationFrame frame, Instruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public static void Eval_blt(EvaluationFrame frame, Instruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public static void Eval_bne(EvaluationFrame frame, Instruction inst) {
			EvaluateBranchOperation(frame, inst.OpCode);
		}

		public static void Eval_box(EvaluationFrame frame, Instruction inst) {
			MDToken token = (MDToken)inst.Operand;
			SuperType type = frame.Assembly.ResolveType(token);
			if(!type.IsValueType) throw new BadILException("box can't perform about non-valuetype: "+type);
			/*
			if(!type.IsAssignableFrom(GetStackingType(frame.Stack.Top))) {
				Console.WriteLine("{0}", type.FullName);
				Console.WriteLine("{0}", GetStackingType(frame.Stack.Top).FullName);
				throw new BadILException("box operands are not assignable: "+type+" from "+frame.Stack.Top);
			}
			*/
			if(type.IsEnum) {
				// <enum type>.ToString() は、
				//		box <enum-type>
				//		call Enum.ToString()
				// に変換されるので、、、
				frame.Stack.Change(1, frame.Assembly.Manager.Enum);
			} else {
				frame.Stack.Change(1, frame.Assembly.Manager.Object);
			}
		}

		public static void Eval_br(EvaluationFrame frame, Instruction inst) {
			//No change of frame.Stack-state
			//TODO: check the range of br-target.
		}

		public static void Eval_break(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void EvaluateBoolBranchOperation(EvaluationStack stack, Instruction inst) {
			switch(GetIntrinsicType(stack[0])) {
			case IntrinsicType.Int4:
			case IntrinsicType.NInt:
			case IntrinsicType.Objt:
				stack.Pop();
				break;
			default:
				throw new BadILException(stack[0].ToString());
			}
		}

		public static void Eval_brfalse(EvaluationFrame frame, Instruction inst) {
			EvaluateBoolBranchOperation(frame.Stack, inst);
		}

		public static void Eval_brtrue(EvaluationFrame frame, Instruction inst) {
			EvaluateBoolBranchOperation(frame.Stack, inst);
		}

		private static MethodInfoImpl EvaluateCallOperation(EvaluationFrame frame, Instruction inst, bool newobj) {
			MDToken token = (MDToken)inst.Operand;
			MethodInfoImpl method = frame.Assembly.ResolveMethod(token);
			for(int i=0; i<method.ParameterCount; ++i) {
				TypeImpl t1 = (TypeImpl)method.GetParameterType(i);
				TypeImpl t2 = frame.Stack[method.ParameterCount-1-i];
				if(t1.IsAssignableFrom(t2)) continue;					// Safe-cast
				if(t1.IsPrimitive && t2.IsPrimitive) continue;			// short and int are compatible in stack
				if(t1.IsEnum && t2.IsPrimitive) continue;				// enum-type can be cast to primitive types
				if(t1.IsPrimitive && t2.IsEnum) continue;				// same above
				if(IsPointerKind(t1) && IsPointerKind(t2)) continue;	// CLI casts pointer implicitly.
				if(t2==frame.Assembly.Manager.Object && !t1.IsValueType) continue;	// maybe anonymous null Object loaded by ldnull
				Console.Error.WriteLine(method.ToString());
				throw new BadILException("call detects parameter mismatch #"+i+" expected="+t1.FullName+", actual="+t2.FullName);
			}
			if(newobj) {
				frame.Stack.Pop(method.ArgumentCount-1);
			} else {
				if(!method.IsStatic) {
					SuperType type = frame.Stack[method.ParameterCount];
					if(type.IsPointer) {
						Type etype = type.GetElementType();
						if(!etype.IsValueType || !method.DeclaringType.IsAssignableFrom(etype)) {
							throw new BadILException("call detects this incompatible: "+type+" and "+method.DeclaringType);
						}
					} else {
						if(!method.DeclaringType.IsAssignableFrom(type)) {
							throw new BadILException("call detects this incompatible: "+type+" and "+method.DeclaringType);
						}
					}
				}
				frame.Stack.Pop(method.ArgumentCount);
				if(method.ReturnType!=frame.Assembly.Manager.Void) {
					frame.Stack.Push(method.ReturnType);
				}
			}
			return method;
		}
		
		private static MethodInfoImpl EvaluateCallOperation(EvaluationFrame frame, Instruction inst) {
			return EvaluateCallOperation(frame, inst, false);
		}

		public static void Eval_call(EvaluationFrame frame, Instruction inst) {
			EvaluateCallOperation(frame, inst);
		}

		public static void Eval_calli(EvaluationFrame frame, Instruction inst) {
			if(!IsPointerKind(frame.Stack.Pop())) throw new BadILException();
			EvaluateCallOperation(frame, inst);
		}

		public static void Eval_callvirt(EvaluationFrame frame, Instruction inst) {
			EvaluateCallOperation(frame, inst);
		}

		public static void Eval_castclass(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Pop();
			frame.Stack.Push(frame.Assembly.ResolveType(inst.Token));
		}

		public static void Eval_ckfinite(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_ceq(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryComparisonOperation(frame, inst.OpCode);
		}

		public static void Eval_cgt(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryComparisonOperation(frame, inst.OpCode);
		}

		public static void Eval_clt(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryComparisonOperation(frame, inst.OpCode);
		}

		public static void Eval_conv(EvaluationFrame frame, Instruction inst) {
			EvaluateConversionOperation(frame, inst);
		}

		public static void Eval_cpblk(EvaluationFrame frame, Instruction inst) {
			if(!IsInt32Suitable(frame.Stack[0])) throw new BadILException();
			if(!IsPointerKind(frame.Stack[1])) throw new BadILException();
			if(!IsPointerKind(frame.Stack[2])) throw new BadILException();
			frame.Stack.Pop(3);
		}

		public static void Eval_cpobj(EvaluationFrame frame, Instruction inst) {
			if(!IsPointerKind(frame.Stack[1])) throw new BadILException();
			if(!IsPointerKind(frame.Stack[2])) throw new BadILException();
			frame.Stack.Pop(2);
		}

		public static void Eval_div(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryNumericOperation(frame, inst);
		}

		public static void Eval_dup(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(inst.StackState[0]);
		}

		public static void Eval_endfilter(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_endfinally(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_initblk(EvaluationFrame frame, Instruction inst) {
			if(!IsInt32Suitable(inst.StackState[0])) throw new BadILException();
			if(!IsInt32Suitable(inst.StackState[1])) throw new BadILException();
			if(!IsPointerKind(inst.StackState[2])) throw new BadILException();
			frame.Stack.Pop(3);
		}

		public static void Eval_initobj(EvaluationFrame frame, Instruction inst) {
			if(!IsPointerKind(inst.StackState[0])) throw new BadILException();
			if(!inst.TypeInfo.IsValueType) throw new BadILException();
			frame.Stack.Pop(1);
		}

		public static void Eval_isinst(EvaluationFrame frame, Instruction inst) {
			TypeImpl type = inst.TypeInfo;
			if(type.IsValueType) {
				// 値型はObjectのまま
			} else {
				frame.Stack.Change(1, type);
			}
		}

		public static void Eval_jmp(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_ldarg(EvaluationFrame frame, Instruction inst) {
			frame.Stack.LoadArg(inst.Number);
		}

		public static void Eval_ldarga(EvaluationFrame frame, Instruction inst) {
			frame.Stack.LoadArg(inst.Number);
			SuperType type = frame.Stack.Pop();
			frame.Stack.Push(type.GetByRefPointerType());
		}

		public static void Eval_ldc(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(inst.OpType);
		}

		public static void Eval_ldelem(EvaluationFrame frame, Instruction inst) {
			if(!IsInt32Suitable(frame.Stack[0])) throw new BadILException("1st operand must be Index");
			if(!frame.Stack[1].IsArray) throw new BadILException("2nd operand must be Array");
			//TODO: Skip the check whether if element-type matches the target type because Char is loaded by using ldelem.u2.
			//if(stack[1].GetElementType()!=type) throw new BadILException("Type incompatible");
			frame.Stack.Change(2, frame.Stack[1].GetElementType());
		}

		public static void Eval_ldelema(EvaluationFrame frame, Instruction inst) {
			if(!IsInt32Suitable(frame.Stack[0])) throw new BadILException("1st operand must be Index");
			if(!frame.Stack[1].IsArray) throw new BadILException("2nd operand must be Array");
			frame.Stack.Change(2, ((TypeImpl)frame.Stack[1].GetElementType()).GetByRefPointerType());
		}

		public static void Eval_ldfld(EvaluationFrame frame, Instruction inst) {
			FieldInfoImpl fi = inst.FieldInfo;
			SuperType type = frame.Stack.Pop();
			if(!IsObjectKind(type) && !type.IsPointer) throw new BadILException("ldfld requires object as its operand.");
			frame.Stack.Push(fi.FieldType);
		}

		public static void Eval_ldflda(EvaluationFrame frame, Instruction inst) {
			FieldInfo field = inst.FieldInfo;
			if(field.IsStatic) throw new BadILException();
			TypeImpl type = frame.Stack.Pop();
			if(field.DeclaringType.IsValueType) {
				type = (TypeImpl)type.GetElementType();
			}
			if(!field.DeclaringType.IsAssignableFrom(type)) throw new BadILException();
			frame.Stack.Push(((SuperType)field.FieldType).GetByRefPointerType());
		}

		public static void Eval_ldftn(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(frame.Assembly.Manager.Byte.GetByRefPointerType());
		}

		public static void Eval_ldind(EvaluationFrame frame, Instruction inst) {
			SuperType address = frame.Stack.Pop();
			if(address.IsByRefPointer) {
				//ldind.ref < InterfaceBase[]& InterfaceBase[]& ConcreteType
				//ldloc.s   < Object InterfaceBase[]& ConcreteType
				frame.Stack.Push(address.GetElementType());
			} else if(IsPointerKind(address)) {
				//ldind.i4  < IKernel* UInt32 IKernel* UInt32
				//ldc.i4.s  < IKernel UInt32 IKernel* UInt32
				//frame.Stack.Push(address.GetElementType());
				frame.Stack.Push(inst.OpType);
			} else if(address==frame.Assembly.Manager.Int32) {
				//COOS: Microsoft .NET Framework may allows Int32 as a pointer.
				frame.Stack.Push(inst.OpType);
			} else {
				throw new BadILException();
			}
		}

		public static void Eval_ldlen(EvaluationFrame frame, Instruction inst) {
			if(!frame.Stack.Top.IsArray) throw new BadILException("ldlen's operand must be Array");
			frame.Stack.Change(1, frame.Assembly.Manager.Int32);
		}

		public static void Eval_ldloc(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(frame.Method.GetVariableType(inst.Number));
		}

		public static void Eval_ldloca(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(((TypeImpl)frame.Method.GetVariableType(inst.Number)).GetByRefPointerType());
		}

		public static void Eval_ldnull(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(frame.Assembly.Manager.Object);
		}

		public static void Eval_ldobj(EvaluationFrame frame, Instruction inst) {
			MDToken token = (MDToken)inst.Operand;
			SuperType type = frame.Assembly.ResolveType(token);
			if(!type.IsValueType) throw new BadILException("ldobj can't performs about "+type+".");
			if(!IsPointerKind(frame.Stack.Pop())) throw new BadILException("ldobj requires pointer-kind value.");
			frame.Stack.Push(type);
		}

		public static void Eval_ldsfld(EvaluationFrame frame, Instruction inst) {
			FieldInfo field = frame.Assembly.ResolveField((MDToken)inst.Operand);
			if(!field.IsStatic) throw new BadILException();
			frame.Stack.Push(field.FieldType);
		}

		public static void Eval_ldsflda(EvaluationFrame frame, Instruction inst) {
			FieldInfo field = frame.Assembly.ResolveField((MDToken)inst.Operand);
			if(!field.IsStatic) throw new BadILException();
			frame.Stack.Push(((SuperType)field.FieldType).GetByRefPointerType());
		}

		public static void Eval_ldstr(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(frame.Assembly.Manager.String);
		}

		public static void Eval_ldtoken(EvaluationFrame frame, Instruction inst) {
			MDToken token = inst.Token;
			switch(token.TableId) {
			case TableId.TypeDef:
			case TableId.TypeRef:
			case TableId.TypeSpec:
				TypeImpl type = frame.Assembly.ResolveType(token);
				frame.Stack.Push(frame.Assembly.Manager.ResolveType("System.RuntimeTypeHandle",true));
				break;
			case TableId.Field:
			case TableId.Method:
			case TableId.MemberRef:
				MemberInfo member = frame.Assembly.ResolveMember(token);
				switch(member.MemberType) {
				case MemberTypes.Field:
					frame.Stack.Push(frame.Assembly.Manager.ResolveType("System.RuntimeFieldHandle",true));
					break;
				case MemberTypes.Method:
					frame.Stack.Push(frame.Assembly.Manager.ResolveType("System.RuntimeMethodHandle",true));
					break;
				default:
					throw new NotSupportedException(member.Name);
				}
				break;
			}
		}

		public static void Eval_ldvirtftn(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_leave(EvaluationFrame frame, Instruction inst) {
			// The leave instruction empties the evaluation stack and ensures that
			// the appropriate surrounding finally blocks are executed.
			frame.Stack.Pop(frame.Stack.Length-frame.Method.ArgumentCount);
		}

		public static void Eval_localloc(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_mkrefany(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_mul(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryNumericOperation(frame, inst);
		}

		public static void Eval_neg(EvaluationFrame frame, Instruction inst) {
			EvaluateUnaryNumericOperation(frame.Stack, inst.OpCode);
		}

		public static void Eval_newarr(EvaluationFrame frame, Instruction inst) {
			if(!IsInt32Suitable(frame.Stack.Pop())) throw new BadILException();
			SuperType type = frame.Assembly.ResolveType(inst.Token);
			frame.Stack.Push(type.GetSzArrayType());
		}

		public static void Eval_newobj(EvaluationFrame frame, Instruction inst) {
			MethodInfoImpl method = EvaluateCallOperation(frame, inst, true);
			frame.Stack.Push(method.DeclaringType);
		}

		public static void Eval_nop(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_not(EvaluationFrame frame, Instruction inst) {
			EvaluateIntegerOperation(frame);
		}

		public static void Eval_or(EvaluationFrame frame, Instruction inst) {
			EvaluateIntegerOperation(frame);
		}

		public static void Eval_pop(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Pop();
		}

		public static void Eval_prefix1(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefix2(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefix3(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefix4(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefix5(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefix6(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefix7(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_prefixref(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_refanytype(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_refanyval(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_rem(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryNumericOperation(frame, inst);
			//? EvaluateIntegerOperation(frame);
		}

		public static void Eval_ret(EvaluationFrame frame, Instruction inst) {
			if(frame.Method.ReturnType!=frame.Assembly.Manager.Void) {
				frame.Stack.Pop();
			}
		}

		public static void Eval_rethrow(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_shl(EvaluationFrame frame, Instruction inst) {
			EvaluateShiftOperation(frame);
		}

		public static void Eval_shr(EvaluationFrame frame, Instruction inst) {
			EvaluateShiftOperation(frame);
		}

		public static void Eval_sizeof(EvaluationFrame frame, Instruction inst) {
			frame.Stack.Push(frame.Assembly.Manager.Int32);
		}

		public static void Eval_starg(EvaluationFrame frame, Instruction inst) {
			frame.Stack.StoreArg(inst.Number);
		}

		public static void Eval_stelem(EvaluationFrame frame, Instruction inst) {
			if(!frame.Stack[2].IsArray) throw new BadILException();
			if(!IsInt32Suitable(frame.Stack[1])) throw new BadILException();
			// stelem.i2 < Int32 Int32 Char[]
			/*
			if(!frame.Stack[2].GetElementType().IsAssignableFrom(frame.Stack[0])) {
				throw new BadILException();
			}
			*/
			frame.Stack.Pop(3);
		}

		public static void Eval_stfld(EvaluationFrame frame, Instruction inst) {
			FieldInfoImpl fi = frame.Assembly.ResolveField((MDToken)inst.Operand);
			if(fi.IsStatic) throw new BadILException("Don't use stfld for static field");
			//if(!IsObjectKind(frame.Stack.Top)) throw new BadILException("stfld requires object as its operand.");
			frame.Stack.Pop();
			frame.Stack.Pop();
		}

		public static void Eval_stind(EvaluationFrame frame, Instruction inst) {
			if(!Synthesizer.IsPointerKind(inst.StackState[1])) throw new BadILException();
			//if(inst.StackState[0].VariableSize!=((TypeImpl)inst.StackState[1].GetElementType()).VariableSize) throw new BadILException();
			frame.Stack.Pop();
			frame.Stack.Pop();
		}

		public static void Eval_stloc(EvaluationFrame frame, Instruction inst) {
			//TODO: check type compatibility for method's local variable.
			//TODO: check whether if the index is in correct range.
			frame.Stack.Pop();
		}

		public static void Eval_stobj(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_stsfld(EvaluationFrame frame, Instruction inst) {
			FieldInfoImpl fi = frame.Assembly.ResolveField((MDToken)inst.Operand);
			if(!fi.IsStatic) throw new BadILException("Don't use stsfld for instance field");
			//if(!IsObjectKind(frame.Stack.Top)) throw new BadILException("stfld requires object as its operand.");
			frame.Stack.Pop();
		}

		public static void Eval_sub(EvaluationFrame frame, Instruction inst) {
			EvaluateBinaryNumericOperation(frame, inst);
		}

		public static void Eval_switch(EvaluationFrame frame, Instruction inst) {
			if(inst.StackState[0]!=frame.Assembly.Manager.Int32
			&& inst.StackState[0]!=frame.Assembly.Manager.UInt32
			&& !inst.StackState[0].IsEnum)
			{
				inst.Dump(Console.Out);
				throw new BadILException();
			}
			frame.Stack.Pop();
		}

		public static void Eval_tail(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_throw(EvaluationFrame frame, Instruction inst) {
			if(!frame.Assembly.Manager.ResolveType("System.Exception",true).IsAssignableFrom(inst.StackState[0])) throw new BadILException();
			frame.Stack.Pop();
		}

		public static void Eval_unaligned(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_unbox(EvaluationFrame frame, Instruction inst) {
			MDToken token = (MDToken)inst.Operand;
			SuperType type = frame.Assembly.ResolveType(token);
			if(!type.IsValueType) throw new BadILException("unbox requires the token indicates ValueType.");
			if(frame.Stack.Pop().IsValueType) throw new BadOperandException("unbox can apply to Object value.");
			//TODO: check type compatibility.
			frame.Stack.Push(type.GetByRefPointerType());
		}

		public static void Eval_volatile(EvaluationFrame frame, Instruction inst) {
			throw new NotImplementedException();
		}

		public static void Eval_xor(EvaluationFrame frame, Instruction inst) {
			EvaluateIntegerOperation(frame);
		}

	}

}
