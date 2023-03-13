using System;
using CooS.Architectures.IA32;
using System.Runtime.CompilerServices;

namespace CooS {

	public class E<U,V> {
		public U vU;
		public V vV;
	}

	public class C<S> {
		public S fC;
		public void f<X>(S s, X x) {
		}
		public class D<T> : E<S,T> {
			public S vDs;
			public T vDt;
			public void g<Y>(S s, T t, Y y) {
			}
		}
	}

	public class Kernel {

		internal static readonly World World = new CooS.Execution.CLI.WorldImpl(1);
		internal static readonly Engine Engine;
		
		private static void Main() {
			Console.WriteLine("CONFIRMED RECEIVING CONTROL");
			CooS.Core.FileSystemManager.Initialize();
		}

		/// <summary>
		/// CooSランタイムによる実行サポートがあることを示します。
		/// </summary>
		public static bool Infrastructured {
			get {
				return false;
			}
		}

		/// <summary>
		/// ソフトウェアが特権モードで動作していることを示します。
		/// </summary>
		public static bool Privileged {
			get {
				return false;
			}
		}

		public static void SetDebugMode(bool enabled) {
			//throw new SystemException("VESデバッグモードを設定しようとしましたが、システムは現在VESから切り離されています。");
		}

		public static void Panic(string msg) {
			Console.WriteLine("*** PANIC ***");
			Console.WriteLine(msg);
			while(true) Instruction.hlt();
		}

		public static void Delay(int miliseconds, int microseconds, int nanoseconds) {
			microseconds += miliseconds*1000;
			nanoseconds += microseconds*1000;
			nanoseconds /= 1000;
			for(int i=0; i<=nanoseconds; ++i) {
				//NOP
			}
		}

		public static void Halt() {
			Instruction.hlt();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern unsafe object ValueToObject(void* value);
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object ValueToObject(IntPtr value);
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ObjectToValue(object value);

		#region 例外

		public static void DumpException(Exception ex) {
			Console.Error.WriteLine("*******************************************************************");
			Console.Error.WriteLine("throw: {0}", ex.GetType().FullName);
			Console.Error.WriteLine("  msg: {0}", ex.Message);
		}

		#endregion

	}

}
