using System;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using System.Runtime.InteropServices;

namespace CooS.Execution {

	//
	// Sync with [kernel] Processing::RunState
	//

	struct _RunState {
		public IntPtr Thread;
		public IntPtr Context;
		public IntPtr AppDomain;
	}

	struct RunState {

		public Thread Thread;
		public Context Context;
		public AppDomain AppDomain;
		
		private RunState(_RunState rs) {
			this.Thread = (Thread)Kernel.ValueToObject(rs.Thread);
			this.Context = (Context)Kernel.ValueToObject(rs.Context);
			this.AppDomain = (AppDomain)Kernel.ValueToObject(rs.AppDomain);
		}

		public static RunState Current {
			get {
				return new RunState(GetCurrent());
			}
			set {
				_RunState rs;
				GCHandle ht = GCHandle.Alloc(value.Thread);
				GCHandle hc = GCHandle.Alloc(value.Context);
				GCHandle ha = GCHandle.Alloc(value.AppDomain);
				rs.Thread = ht.AddrOfPinnedObject();
				rs.Context = hc.AddrOfPinnedObject();
				rs.AppDomain = ha.AddrOfPinnedObject();
				SetCurrent(rs);
				ht.Free();
				hc.Free();
				ha.Free();
			}
		}

		private static unsafe _RunState GetCurrent() {
			return *(_RunState*)0x1100;
		}

		private static unsafe void SetCurrent(_RunState rs) {
			*(_RunState*)0x1100 = rs;
		}

	}

}
