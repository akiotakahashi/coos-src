using System;
using CooS.Reflection;
using CooS.Execution;
using CooS.Compile;
using CooS.Toolchains;

namespace CooS {

	public abstract class Toolchain {

		private static Toolchain arc;

		static Toolchain() {
			arc = new CooS.Toolchains.IA32.ToolchainImpl();
		}

		protected Toolchain() {
		}

		public static Toolchain Current {
			get {
				return arc;
			}
		}

		public abstract Assembler CreateAssembler(Compiler compiler, MethodInfo method, int workingsize);

#if false
		#region Šî–{“I‚ÈŽü•Ó‘•’u

		static KeyboardController kbc;

		public static KeyboardController KeyboardController {
			get {
				if(kbc==null) {
					kbc = new KeyboardController(0x64, 0x60, 1, 12);
				}
				return kbc;
			}
		}

		static ATAPIController atapicont0;
		static ATAPIController atapicont1;

		public static ATAPIController ATAPIPrimaryController {
			get {
				if(atapicont0==null) {
					Console.WriteLine("Creating primary ATAPI controller");
					atapicont0 = new ATAPIController(14,0x1F0,0x3F6);
					Console.WriteLine("Initialize the controller");
					atapicont0.InitializeDevice();
				}
				return atapicont0;
			}
		}

		public static ATAPIController ATAPISecondaryController {
			get {
				if(atapicont1==null) {
					Console.WriteLine("Creating secondary ATAPI controller");
					atapicont1 = new ATAPIController(15,0x170,0x376);
					Console.WriteLine("Initialize the controller");
					atapicont1.InitializeDevice();
				}
				return atapicont1;
			}
		}

		public static void ResetMachine() {
			KeyboardController kbc = KeyboardController;
			kbc.LetKeyboardInterruptEnabled(false);
			Instruction.cli();
			kbc.PulseOutputPort(0xE);
		}

		#endregion
#endif

	}

}
