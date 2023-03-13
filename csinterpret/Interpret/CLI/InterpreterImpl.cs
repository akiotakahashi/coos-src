using System;
using CooS.Execution;
using CooS.Reflection;
using CooS.Formats.CLI;
using CooS.Formats.CLI.CodeModel;
using CooS.Reflection.CLI;
using System.Collections.Generic;

namespace CooS.Interpret.CLI {

	delegate Flow AssembleDelegate(Machine machine, Frame frame, Instruction inst);

	public partial class InterpreterImpl : Interpreter {

		private Dictionary<string, AssembleDelegate> dispatch = new Dictionary<string,AssembleDelegate>();
		public readonly Machine Machine;
		public readonly World World;

		public InterpreterImpl(Machine machine) {
			this.Machine = machine;
			this.World = machine.World;
			foreach(System.Reflection.MethodInfo method in this.GetType().GetMethods(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance)) {
				if(!method.Name.StartsWith("Interpret_")) continue;
				Delegate dg = Delegate.CreateDelegate(typeof(AssembleDelegate), this, method);
				dispatch.Add(method.Name.Substring("Interpret_".Length), (AssembleDelegate)dg);
			}
		}

		public override Flow Execute(MethodBase method) {
			Frame frame = new Frame(this.Machine, (MethodImpl)method);
			try {
				bool exit = false;
				while(!exit) {
					Instruction inst = frame.Current;
					try {
						Console.WriteLine(inst);
						Flow flow = dispatch[inst.CoreName](this.Machine, frame, inst);
						switch(flow) {
						case Flow.Next:
							frame.Next();
							break;
						case Flow.Jump:
							break;
						case Flow.Return:
							exit = true;
							break;
						}
					} catch(NotProcessedException) {
						Console.Error.WriteLine("skipped: "+inst);
						frame.Next();
					}
				}
				return Flow.Next;
			} catch(KeyNotFoundException) {
				Console.WriteLine("Instruction Method Not Found: "+frame.Current.CoreName);
				Console.WriteLine(frame);
			}
			return Flow.Exception;
		}

	}

}
