using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using CooS.CodeModels;
using CooS.Reflection;
using CooS.Architectures;
using CooS.Drivers.Controllers;

namespace CooS.Threading {

	public class ThreadMother {

		private ThreadStart entrypoint = null;

		public ThreadMother(ThreadStart ts) {
			this.entrypoint = ts;
		}

		public void Engage() {
			bool flag = false;
			InterruptController.MakeInterrupt(0);
			if(flag) entrypoint();
			flag = true;
		}

	}

	public class ThreadService {

		private static ContextList readys = new ContextList();
		private static ContextList sleeps = new ContextList();
		private static Hashtable table = new Hashtable();

		static ThreadService() {
			if(Engine.Privileged) {
				//InterruptController.LetEnabled(0, false);
				InterruptController.Register(0, new InterruptHandler(HandleTimerInterrupt));
			}
		}

		public static void LetService(bool enabled) {
			InterruptController.LetEnabled(0, enabled);
		}

		private static unsafe void HandleTimerInterrupt(ref IntPtr sp) {
			/*
			if(readys.Count>0) {
				Console.Write("*");
				readys.Head.sp = sp;
				readys.Rotate();
				sp = readys.Head.sp;
			}
			*/
			byte* p = (byte*)new IntPtr(0xB8000).ToPointer();
			p[79*2+0]++;
			p[79*2+1] = 15;
			InterruptController.NotifyEndOfInterrupt(0);
		}

		private static ThreadContext creating_context = null;
		private static ThreadStart creating_entrypoint = null;

		private static void HandleRegisterInterrupt(ref IntPtr sp) {
			IntPtr target = Kernel.ObjectToValue(creating_entrypoint.Target);
			IntPtr method = Assist.GetFunctionPointer(creating_entrypoint);
			InterruptManager.CopyContext(sp, creating_context.stack);
			// We don't notify the end of interrupt for PIC,
			// since this function is called via software interruption.
		}

		public static void RegisterThread(Thread thread) {
			ThreadContext cxt = new ThreadContext(thread, 1024*1024);
			creating_entrypoint = (ThreadStart)Assist.GetField(thread, "threadstart");
			throw new NotImplementedException();
#if false
			creating_context = cxt;
			Console.WriteLine("RegisterThread");
			throw new Exception();
			bool old = InterruptController.LetEnabled(0, false);
			InterruptController.Register(0, new InterruptHandler(HandleRegisterInterrupt));
			ThreadMother mother = new ThreadMother(creating_entrypoint);
			mother.Engage();
			creating_context = null;
			creating_entrypoint = null;
			sleeps.Append(cxt);
			table[thread] = cxt;
			InterruptController.Register(0, new InterruptHandler(HandleTimerInterrupt));
			InterruptController.LetEnabled(0, old);
#endif
		}

		public static void UnregisterThread(Thread thread) {
			if(thread==Thread.CurrentThread) throw new ArgumentException("Running thread can't be unregistered.");
			ThreadContext cxt = (ThreadContext)table[thread];
			bool old = InterruptController.LetEnabled(0, false);
			cxt.owner.Remove(cxt);
			InterruptController.LetEnabled(0, old);
		}

		public static void SwitchThread() {
			InterruptController.MakeInterrupt(0);
		}

		public static void LetThreadReady(Thread thread) {
			ThreadContext cxt = (ThreadContext)table[thread];
			if(cxt.owner==readys) return;
			cxt.owner.Remove(cxt);
			bool old = InterruptController.LetEnabled(0, false);
			readys.Append(cxt);
			InterruptController.LetEnabled(0, old);
		}

		public static void LetThreadSleep(Thread thread) {
			ThreadContext cxt = (ThreadContext)table[thread];
			if(cxt.owner==sleeps) return;
			bool old = InterruptController.LetEnabled(0, false);
			cxt.owner.Remove(cxt);
			InterruptController.LetEnabled(0, old);
			sleeps.Append(cxt);
		}

	}

}
