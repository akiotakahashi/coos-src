using System;

namespace CooS.Wrap._System {

	public class _Environment {

		public static string NewLine {
			get {
				return "\r\n";
			}
		}

		public static int Platform {
			get {
				return -1;
			}
		}

		public static string GetMachineConfigPath() {
			return "/coos/machine.config";
		}

		public static int TickCount {
			get {
				ulong tsc = CooS.Architectures.IA32.Instruction.rdtsc();
				return (int)(tsc/(3.4*1024*1024));
			}
		}

		public static string GetEnvironmentVariable(string name) {
			return null;
		}

	}

}
