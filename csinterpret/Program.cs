using System;
using CooS.Reflection;
using CooS.Execution;

namespace CooS.Interpret {

	class Program {

		static void Main(string[] args) {
			WorldImpl world = new WorldImpl();
			world.LoadAssembly("CLI", CLI.Test.MscorlibPath);
			world.FinishLoadingPrimaryAssemblies();
			world.LoadAssembly("CLI", CLI.Test.Test1Path);
			AssemblyBase assembly = world.LoadAssembly("CLI", CLI.Test.Test2Path);
			Engine engine = new Engine(world);
			Machine machine = new Machine(engine);
			machine.Run(assembly, null);
			machine.Dump();
		}

#if false
		static void EnumInstructions() {
			foreach(FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(field.FieldType!=typeof(OpCode))
					continue;
				OpCode op = (OpCode)field.GetValue(null);
				Console.Write("{0,-4:X02} {1,-16}", op.Value, op.Name);
				string operand;
				switch(op.OperandType) {
				case OperandType.InlineBrTarget:
					operand = "br";
					break;
				case OperandType.InlineField:
					operand = "field";
					break;
				case OperandType.InlineI:
					operand = "int";
					break;
				case OperandType.InlineI8:
					operand = "long";
					break;
				case OperandType.InlineMethod:
					operand = "method";
					break;
				case OperandType.InlineNone:
					operand = null;
					break;
				case OperandType.InlinePhi:
					operand = "reserved-phi";
					break;
				case OperandType.InlineR:
					operand = "double";
					break;
				case OperandType.InlineSig:
					operand = "sig";
					break;
				case OperandType.InlineString:
					operand = "string";
					break;
				case OperandType.InlineSwitch:
					operand = "switch";
					break;
				case OperandType.InlineTok:
					operand = "token";
					break;
				case OperandType.InlineType:
					operand = "type";
					break;
				case OperandType.InlineVar:
					operand = "var";
					break;
				case OperandType.ShortInlineBrTarget:
					operand = "br.s";
					break;
				case OperandType.ShortInlineI:
					operand = "int.s";
					break;
				case OperandType.ShortInlineR:
					operand = "signle";
					break;
				case OperandType.ShortInlineVar:
					operand = "var.s";
					break;
				default:
					throw new NotImplementedException();
				}
				if(operand!=null) {
					Console.Write("{0,-10}", "<"+operand+">");
				} else {
					Console.Write("{0,10}", "");
				}
				string[] pops = op.StackBehaviourPop.ToString().Split('_');
				string[] pushs = op.StackBehaviourPush.ToString().Split('_');
				if(pops[0]=="Pop0") {
					System.Diagnostics.Debug.Assert(pops.Length==1);
				} else {
					System.Array.Reverse(pushs);
					foreach(string pop in pops) {
						Console.Write(" {0}", pop);
					}
				}
				Console.Write(" ->");
				if(pushs[0]=="Push0") {
					System.Diagnostics.Debug.Assert(pushs.Length==1);
				} else {
					foreach(string push in pushs) {
						Console.Write(" {0}", push);
					}
				}
				Console.WriteLine();
			}
		}
#endif

	}

}
