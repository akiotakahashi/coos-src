using System;
using System.Threading;
using System.Collections;

namespace CooS.Threading {

	public class ThreadContext {

		public readonly Thread thread;
		public readonly byte[] stack;

		public ContextList owner;
		public ThreadContext prev;
		public ThreadContext next;
		public IntPtr sp;
		public ArrayList tls;

		public ThreadContext(Thread thread, int stacksize) {
			this.thread = thread;
			this.stack = new byte[stacksize];
		}

		public override bool Equals(object obj) {
			return thread.Equals(((ThreadContext)obj).thread);
		}

		public override int GetHashCode() {
			return this.thread.GetHashCode();
		}

	}

}
