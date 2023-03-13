using System;
using CooS.Reflection;
using CooS.Toolchains;
using CooS.Toolchains.IA32;
using CooS.Architectures.IA32;
using CooS.Drivers.PS2;
using CooS.Drivers.ATAPI;

namespace CooS {

	public static class Machine {

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

	}

}
