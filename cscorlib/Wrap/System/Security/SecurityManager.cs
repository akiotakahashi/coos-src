using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.CompilerServices;

namespace CooS.Wrap._System.Security {

	public class _SecurityManager {

		public static bool SecurityEnabled {
			get {
				return false;
			}			
			[SecurityPermission(SecurityAction.Demand, ControlPolicy=true)]
			set {
				throw new NotImplementedException();
			}
		}
 
	}

}
