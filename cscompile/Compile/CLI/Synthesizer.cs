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

	delegate void InstructionDispatcher(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst);
	delegate void InstructionEvaluator(EvaluationFrame frame, CompiledInstruction inst);
	
	partial class Synthesizer {

		static readonly InstructionDispatcher[] evals1;
		static readonly Dictionary<short, InstructionDispatcher> evals2 = new Dictionary<short, InstructionDispatcher>();
		
		static Synthesizer() {
			evals1 = new InstructionDispatcher[256];
			#region OpCode Registrations
			foreach(System.Reflection.FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(fi.FieldType!=typeof(OpCode)) continue;
				OpCode opcode = (OpCode)fi.GetValue(null);
				string methodname = "Dispatch_"+opcode.Name.Split('.')[0].ToLower();
				Delegate d = Delegate.CreateDelegate(typeof(InstructionDispatcher), typeof(Synthesizer), methodname);
				Register(opcode, (InstructionDispatcher)d);
			}
			#endregion
		}

		private static void Register(OpCode opcode, InstructionDispatcher eval) {
			if(opcode.Size==1) {
				evals1[opcode.Value] = eval;
			} else if(opcode.Size==2) {
				evals2[opcode.Value] = eval;
			} else {
				throw new NotSupportedException();
			}
		}

		public static CompiledInstruction[] Perform(Compiler compiler, MethodInfo method) {
			Synthesizer syn = new Synthesizer(compiler, method);
			return syn.Preprocess();
		}

	}
	
	partial class Synthesizer {

		readonly Compiler compiler;
		readonly World world;
		readonly Engine engine;
		readonly MethodInfo method;
		readonly AssemblyImpl assembly;

		private Synthesizer(Compiler compiler, MethodInfo method) {
			this.compiler = compiler;
			this.method = method;
			this.world = compiler.World;
			this.engine = compiler.Engine;
			this.assembly = (AssemblyImpl)method.Assembly;
		}

		private CompiledInstruction[] Preprocess() {
			List<CompiledInstruction> instlist = new List<CompiledInstruction>();
			foreach(Instruction inst in method.EnumInstructions()) {
				instlist.Add(new CompiledInstruction(this.compiler, this.assembly, this.method, inst));
			}
			CompiledInstruction[] insts = instlist.ToArray();
			EvaluationFrame frame = new EvaluationFrame(method, compiler);
			foreach(TypeBase arg in method.EnumArguments()) {
				frame.Stack.Push(arg);
			}
			DetermineStackState(insts, 0, frame);
			return insts;
		}

		private void DetermineStackState(CompiledInstruction[] insts, int startIndex, EvaluationFrame frame) {
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
						throw new BadCodeException("Evaluation Stack is not together at a interflow point: 0x"+insts[i].Address.ToString("X"));
					}
#endif
					return;
				} else {
					insts[i].StackState = (EvaluationStack)frame.Stack.Clone();
					//**********
					insts[i].Dump(Console.Out);
					//**********
					EvaluateInstruction(insts[i], frame);
					if(insts[i].OpCode.Value==OpCodes.Ret.Value) {
						return;
					} else if(insts[i].OpCode.Value==OpCodes.Throw.Value) {
						return;
					} else {
						OpCode opcode = insts[i].Instruction.OpCode;
						if(opcode.OperandType==OperandType.InlineBrTarget || opcode.OperandType==OperandType.ShortInlineBrTarget) {
							//Console.WriteLine("branch");
							DetermineStackState(insts, GetBrTargetIndex(insts, insts[i].Instruction.BrTarget), (EvaluationFrame)frame.Clone());
							if(insts[i].OpCode.Value==OpCodes.Br.Value) return;
							if(insts[i].OpCode.Value==OpCodes.Br_S.Value) return;
							if(insts[i].OpCode.Value==OpCodes.Leave.Value) return;
							if(insts[i].OpCode.Value==OpCodes.Leave_S.Value) return;
						} else if(opcode.OperandType==OperandType.InlineSwitch) {
							int[] targets = insts[i].Instruction.BrTargets;
							foreach(int target in targets) {
								//Console.WriteLine("branch");
								DetermineStackState(insts, GetBrTargetIndex(insts, target), (EvaluationFrame)frame.Clone());
							}
						}
					}
				}
			}
		}

		private TypeInfo nullType = null;

		public TypeInfo NullType {
			get {
				if(nullType==null) {
					nullType = this.engine.Realize(this.world.ResolveType("NullType", "CooS.Compile.CLI"));
				}
				return nullType;
			}
		}

		private int GetBrTargetIndex(CompiledInstruction[] insts, int target) {
			int i = Array.BinarySearch(insts, target, new AddressComparator());
			if(i<0) throw new BadCodeException("Branch target does not point to the beginning of instruction: 0x"+target.ToString("X"));
			return i;
		}
		
		class AddressComparator : System.Collections.IComparer {
			public int Compare(object x, object y) {
				return ((CompiledInstruction)x).Instruction.Address.CompareTo((int)y);
			}
		}

		private void EvaluateInstruction(CompiledInstruction inst, EvaluationFrame frame) {
			if(inst.OpCode.Size==1) {
				evals1[inst.OpCode.Value](this, frame, inst);
			} else if(inst.OpCode.Size==2) {
				evals2[inst.OpCode.Value](this, frame, inst);
			} else {
				throw new BadCodeException();
			}
		}

		public static bool IsInt32Suitable(TypeBase type) {
			if(type.IsEnum) {
				return true;
			} else if(type.IsPrimitive) {
				switch(type.Name) {
				case "Boolean":
				case "Char":
				case "SByte":
				case "Byte":
				case "Int16":
				case "Int32":
				case "UInt16":
				case "UInt32":
				case "IntPtr":
				case "UIntPtr":
					return true;
				default:
					return false;
				}
			} else {
				return false;
			}
		}

		public static bool IsInt64Compatible(TypeBase type) {
			return type.IsPrimitive && (type.Name=="Int64" || type.Name=="UInt64");
		}

		public static bool IsNIntCompatible(TypeBase type) {
			return (type.IsPrimitive /*&& type.VariableSize<=4*/)
				|| type.IsByRefPointer || type.IsByValPointer;
		}

		public static bool IsNIntSuitable(TypeBase type) {
			return (type.IsPrimitive && (type.Name=="IntPtr" || type.Name=="UIntPtr"))
				|| type.IsByRefPointer || type.IsByValPointer;
		}

		public static bool IsFloatCompatible(TypeBase type) {
			return (type.IsPrimitive && (type.Name=="Double" || type.Name=="Single"));
		}

		public static bool IsObjectCompatible(TypeBase type) {
			return !type.IsValueType;
		}

		public static IntrinsicTypes GetIntrinsicType(TypeBase type) {
			if(type.IsPointer) {
				return IntrinsicTypes.Pter;
			} else if(IsNIntSuitable(type)) {
				return IntrinsicTypes.NInt;
			} else if(IsInt32Suitable(type)) {
				return IntrinsicTypes.Int4;
			} else if(IsObjectCompatible(type)) {
				return IntrinsicTypes.Objt;
			} else if(IsInt64Compatible(type)) {
				return IntrinsicTypes.Int8;
			} else if(IsFloatCompatible(type)) {
				return type.Name=="Double" ? IntrinsicTypes.Fp64 : IntrinsicTypes.Fp32;
			} else if(type.IsEnum) {
				return IntrinsicTypes.Int4;
			} else if(type.IsValueType) {
				return IntrinsicTypes.Any;
			} else {
				throw new ArgumentException(type.FullName);
			}
		}

		private TypeBase GetStackingType(TypeBase type) {
			IntrinsicTypes it = GetIntrinsicType(type);
			if(it==IntrinsicTypes.Any) {
				return type;
			} else {
				return this.world.Resolve(it);
			}
		}

		private static bool IsEqualFamily(OpCode opcode) {
			return opcode.Value==OpCodes.Beq.Value
				|| opcode.Value==OpCodes.Beq_S.Value
				|| opcode.Value==OpCodes.Bne_Un.Value
				|| opcode.Value==OpCodes.Bne_Un_S.Value
				|| opcode.Value==OpCodes.Ceq.Value;
		}

		private void EvaluateBinaryNumericOperation(EvaluationFrame frame, Instruction inst) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicTypes.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.Int4));
					return;
				case IntrinsicTypes.NInt:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.NInt));
					return;
				case IntrinsicTypes.Pter:
					if(inst.CoreName=="add") {
						stack.Change(2, 0);
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int8:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.Int8));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.NInt));
					return;
				case IntrinsicTypes.Pter:
					if(inst.CoreName=="add") {
						stack.Change(2, 0);
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Fp32:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Fp32:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.Fp32));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Fp64:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Fp64:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.Fp64));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Pter:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
					if(inst.CoreName=="add" || inst.CoreName=="sub") {
						stack.Change(2, 1);
						return;
					} else {
						throw new BadOperandException();
					}
				case IntrinsicTypes.Pter:
					if(inst.CoreName=="sub") {
						stack.Change(2, this.world.Resolve(IntrinsicTypes.NInt));
						return;
					} else {
						throw new BadOperandException();
					}
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException("Invalid operand type: "+stack[1].FullName+" ("+GetIntrinsicType(stack[1])+")");
			}
		}

		private static void EvaluateUnaryNumericOperation(EvaluationStack stack, OpCode opcode) {
			switch(GetIntrinsicType(stack[0])) {
			case IntrinsicTypes.Int4:
			case IntrinsicTypes.Int8:
			case IntrinsicTypes.NInt:
			case IntrinsicTypes.Fp32:
			case IntrinsicTypes.Fp64:
				return;
			default:
				throw new BadOperandException();
			}
		}

		private void EvaluateBinaryComparisonOperation(EvaluationFrame frame, OpCode opcode) {
			TypeBase booltype = this.world.Resolve(PrimitiveTypes.Boolean);
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicTypes.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
				case IntrinsicTypes.Pter:
					stack.Change(2, booltype);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int8:
					stack.Change(2, booltype);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
				case IntrinsicTypes.Pter:
					stack.Change(2, booltype);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Fp32:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Fp32:
					stack.Change(2, booltype);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Fp64:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Fp64:
					stack.Change(2, booltype);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Pter:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
				case IntrinsicTypes.Pter:
					stack.Change(2, booltype);
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Objt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Objt:
					if(IsEqualFamily(opcode)) {
						stack.Change(2, booltype);
						return;
					} else if(opcode.Value==OpCodes.Cgt_Un.Value) {
						stack.Change(2, booltype);
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

		private void EvaluateBranchOperation(EvaluationFrame frame, OpCode opcode) {
			EvaluateBinaryComparisonOperation(frame, opcode);
			frame.Stack.Pop();
		}

		private void EvaluateIntegerOperation(EvaluationFrame frame) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicTypes.Int4:
//			case IntrinsicTypes.Pter:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
//				case IntrinsicTypes.Pter:
					stack.Change(2, this.world.Resolve(PrimitiveTypes.I4));
					return;
				case IntrinsicTypes.NInt:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.NInt));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int8:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.Int8));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
//				case IntrinsicTypes.Pter:
					stack.Change(2, this.world.Resolve(IntrinsicTypes.NInt));
					return;
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException();
			}
		}

		private void EvaluateShiftOperation(EvaluationFrame frame) {
			EvaluationStack stack = frame.Stack;
			switch(GetIntrinsicType(stack[1])) {
			case IntrinsicTypes.Int4:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
					stack.Change(2, this.world.Resolve(PrimitiveTypes.I4));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.Int8:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
					stack.Change(2, this.world.Resolve(PrimitiveTypes.I8));
					return;
				default:
					throw new BadOperandException();
				}
			case IntrinsicTypes.NInt:
				switch(GetIntrinsicType(stack[0])) {
				case IntrinsicTypes.Int4:
				case IntrinsicTypes.NInt:
					stack.Change(2, this.world.Resolve(PrimitiveTypes.I));
					return;
				default:
					throw new BadOperandException();
				}
			default:
				throw new BadOperandException();
			}
		}

		private void EvaluateConversionOperation(EvaluationFrame frame, Instruction inst) {
			EvaluationStack stack = frame.Stack;
			TypeBase type = inst.OpType;
			switch(GetIntrinsicType(stack[0])) {
			case IntrinsicTypes.Int4:
			case IntrinsicTypes.Int8:
			case IntrinsicTypes.NInt:
			case IntrinsicTypes.Fp32:
			case IntrinsicTypes.Fp64:
			case IntrinsicTypes.Pter:
				stack.Change(1, type);
				break;
			case IntrinsicTypes.Objt:
				if(IsPointerKind(type)) {
					stack.Change(1, type);
				} else {
					throw new BadOperandException("Operand "+stack[0]+" can't be converted to "+inst.OpType);
				}
				break;
			default:
				throw new NotSupportedException(stack[0].FullName);
			}
		}

		private static bool IsPointerKind(TypeBase type) {
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

		private static bool IsObjectKind(TypeBase type) {
			return !type.IsValueType || IsPointerKind(type);
		}

		public static void EvaluateBoolBranchOperation(EvaluationStack stack, Instruction inst) {
			switch(GetIntrinsicType(stack[0])) {
			case IntrinsicTypes.Int4:
			case IntrinsicTypes.NInt:
			case IntrinsicTypes.Objt:
				stack.Pop();
				break;
			default:
				throw new BadCodeException(stack[0].ToString());
			}
		}

		private MethodInfo EvaluateCallOperation(EvaluationFrame frame, Instruction inst, bool newobj) {
			MethodInfo method = this.engine.Realize(inst.MethodInfo);
			for(int i=0; i<method.ParameterCount; ++i) {
				TypeBase t1 = method.GetParameterType(i);
				TypeBase t2 = frame.Stack[method.ParameterCount-1-i];
				if(t1.IsAssignableFrom(t2)) continue;					// Safe-cast
				if(t1.IsPrimitive && t2.IsPrimitive) continue;			// short and int are compatible in stack
				if(t1.IsEnum && t2.IsPrimitive) continue;				// enum-type can be cast to primitive types
				if(t1.IsPrimitive && t2.IsEnum) continue;				// same above
				if(IsPointerKind(t1) && IsPointerKind(t2)) continue;	// CLI casts pointer implicitly.
				if(t2==this.world.Resolve(PrimitiveTypes.Object) && !t1.IsValueType) continue;	// maybe anonymous null Object loaded by ldnull
				Console.Error.WriteLine(method.ToString());
				throw new BadCodeException("call detects parameter mismatch #"+i+" expected="+t1.FullName+", actual="+t2.FullName);
			}
			if(newobj) {
				frame.Stack.Pop(method.ArgumentCount-1);
			} else {
				if(!method.IsStatic) {
					TypeBase type = frame.Stack[method.ParameterCount];
					if(type.IsPointer) {
						TypeBase etype = type.ElementType;
						if(!etype.IsValueType || !method.Type.IsAssignableFrom(etype)) {
							throw new BadCodeException("call detects this incompatible: "+type+" and "+method.Type);
						}
					} else {
						if(!method.Type.IsAssignableFrom(type)) {
							throw new BadCodeException("call detects this incompatible: "+type+" and "+method.Type);
						}
					}
				}
				frame.Stack.Pop(method.ArgumentCount);
				if(method.ReturnType!=this.world.Resolve(PrimitiveTypes.Void)) {
					frame.Stack.Push(method.ReturnType);
				}
			}
			return method;
		}
		
		private MethodInfo EvaluateCallOperation(EvaluationFrame frame, Instruction inst) {
			return EvaluateCallOperation(frame, inst, false);
		}

	}

}
