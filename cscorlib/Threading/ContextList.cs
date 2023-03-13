using System;

namespace CooS.Threading {
	/// <summary>
	/// ContextList ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
	/// </summary>
	public class ContextList {

		private ThreadContext head;
		private int count = 0;

		public ContextList() {
		}

		public int Count {
			get {
				return this.count;
			}
		}

		public ThreadContext Head {
			get {
				return this.head;
			}
		}

		public void Rotate() {
			head = head.next;
		}

		public void Append(ThreadContext context) {
			if(context.owner!=null) throw new ArgumentException();
			context.owner = this;
			if(head==null) {
				context.next = context;
				context.prev = context;
				head = context;
			} else {
				context.prev = head.prev;
				context.next = head;
				context.prev.next = context;
				context.next.prev = context;
			}
			++count;
		}

		public void Remove(ThreadContext context) {
			if(context.owner!=this) throw new ArgumentException();
			--count;
			if(context.next==context) {
				context.next = null;
				context.prev = null;
				head = null;
			} else {
				context.prev.next = context.next;
				context.next.prev = context.prev;
				context.prev = null;
				context.next = null;
			}
			context.owner = null;
		}

	}

}
