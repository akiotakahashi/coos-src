using System;
using System.Threading;
using System.Globalization;
using CooS.Threading;

class DummyThread {
	public static void DummyMain() {
	}
}

namespace CooS.Wrap._System.Threading {

	public class _Thread {

		static CultureInfo currentCulture;
		static Thread currentThread;

		static Thread CurrentThread_internal() {
			if(currentThread==null) {
				currentThread = new Thread(new ThreadStart(DummyThread.DummyMain));
			}
			return currentThread;
		}

		void SetCachedCurrentCulture(CultureInfo culture) {
			currentCulture = culture;
		}

		CultureInfo GetCachedCurrentCulture() {
			return currentCulture;
		}

		byte[] GetSerializedCurrentCulture() {
			return null;
		}

		public static int GetDomainID() {
			return Kernel.ObjectToValue(AppDomain.CurrentDomain).ToInt32();
		}

		static int next_handle = 1;

		private IntPtr Thread_internal(MulticastDelegate start) {
			Thread me = (Thread)(object)this;
			ThreadService.RegisterThread(me);
			ThreadService.LetThreadReady(me);
			ThreadService.LetService(true);
			return new IntPtr(next_handle++);
		}

	}

}
