using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace CooS.Development {

	class GenerateInterpretMethodTemplate {

		public GenerateInterpretMethodTemplate(string[] args) {
		}

		public int Execute() {
			Console.WriteLine("using System;");
			Console.WriteLine("using CooS.Reflection;");
			Console.WriteLine("CooS.Formats.CLI.CodeModel;");
			Console.WriteLine();
			Console.WriteLine("namespace CooS.Interpret.CLI {");
			Console.WriteLine();
			Console.WriteLine("\tpartial class InterpreterImpl {");
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
					Console.WriteLine("\t\tpublic int? Interpret_{0}(Machine machine, Frame frame, Instruction inst) {{", name);
					Console.WriteLine("\t\t\tthrow new NotImplementedException(\"Not implemented: \"+inst.CoreName);");
					Console.WriteLine("\t\t}");
					Console.WriteLine();
					cache[name] = opcode;
				}
			}
			Console.WriteLine("\t}");
			Console.WriteLine();
			Console.WriteLine("}");
			return 0;
		}

	}

}
