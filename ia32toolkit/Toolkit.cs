using System;
using System.IO;
using System.Xml;

namespace IA32Toolkit {

	class Toolkit {

		static void Main(string[] args) {
			for(int i=0; i<args.Length; ) {
				switch(args[i++]) {
				case "/iset":
					using(TextReader reader = new StreamReader(args[i++], System.Text.Encoding.Default)) {
						Assembler.IA32Generator.GenerateInstructionSet(reader);
					}
					break;
				case "/genasm":
					XmlDocument xdoc = new XmlDocument();
					xdoc.Load(args[i++]);
					Assembler.IA32Generator.GenerateAssembler(xdoc);
					break;
				default:
					Console.WriteLine("Unknown argument: {0}", args[i++]);
					break;
				}
			}
		}

	}

}
