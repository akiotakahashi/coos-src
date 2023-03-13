using System;
using CooS.Reflection;
using CooS.CodeModels;
using CooS.Execution;
using CooS.Architectures.IA32;
using CooS.Drivers.ATAPI;
using CooS.Drivers.PS2;

namespace CooS {

	public class Architecture {

		private Architecture() {
		}

		public static Assembler CreateAssembler(MethodInfoImpl method, int workingsize) {
			return new Architectures.IA32.IA32Assembler(method, workingsize);
		}

		#region アーキテクチャ依存の計算

		/// <summary>
		/// 大きさ size のオブジェクトが占有するスタック上でのバイトサイズを計算します。
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int GetStackingSize(int size) {
			return (size+IntPtr.Size-1)&~(IntPtr.Size-1);
		}

		/// <summary>
		/// 大きさ size のオブジェクトが占有するスタック上でのスタック単位の個数を計算します。
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int GetStackingLength(int size) {
			return (size+IntPtr.Size-1)/IntPtr.Size;
		}

		/// <summary>
		/// 引数の数値を表現するする最小の2^nの数値を得ます。
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int GetAlignment(int size) {
			if(size<=1) {
				return 1;
			} else if(size<=2) {
				return 2;
			} else if(size<=4) {
				return 4;
			} else if(size<=8) {
				return 8;
			} else {
				return 8;
			}
		}

		/// <summary>
		/// offset の、最小の size の整数倍を得ます。
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int AlignOffset(int offset, int size) {
			int align = GetAlignment(size)-1;
			return (offset+align)&~align;
		}

		#endregion

		#region 基本的な周辺装置

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
