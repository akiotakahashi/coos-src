using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace CooS.Development {

	class CreateSynthesizerDispatch {

		public CreateSynthesizerDispatch(string[] args) {
		}

		public int Execute() {
			Console.WriteLine("using System;");
			Console.WriteLine();
			Console.WriteLine("namespace CooS.Compile.CLI {");
			Console.WriteLine();
			Console.WriteLine("\tpartial class Synthesizer {");
			Console.WriteLine();
			Dictionary<string, OpCode> cache = new Dictionary<string, OpCode>();
			foreach(FieldInfo field in typeof(OpCodes).GetFields()) {
				if(field.FieldType!=typeof(OpCode))
					continue;
				OpCode opcode = (OpCode)field.GetValue(null);
				string name = opcode.Name.Split('.')[0];
				name = name.Replace('.', '_');
				name = name.ToLower();
				if(!cache.ContainsKey(name)) {
					Console.WriteLine("\t\tpublic static void Dispatch_{0}(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)", name);
					Console.WriteLine("\t\t{{ syn.Eval_{0}(frame, inst); }}", name);
					cache[name] = opcode;
				}
			}
			Console.WriteLine();
			Console.WriteLine("\t}");
			Console.WriteLine();
			Console.WriteLine("}");
			return 0;
		}

	}

}
