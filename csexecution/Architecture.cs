using System;
using CooS.Reflection;
using CooS.Execution;

namespace CooS {

	public abstract class Architecture {

		private static Architecture arc;

		static Architecture() {
			arc = new Architectures.IA32.ArchitectureImpl();
		}

		protected Architecture() {
		}

		public static Architecture Target {
			get {
				return arc;
			}
		}

		public abstract int AddressSize {
			get;
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

	}

}
