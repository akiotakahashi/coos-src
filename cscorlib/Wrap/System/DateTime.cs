using System;

namespace CooS.Wrap._System {

	public class _DateTime {

		public static DateTime Now {
			get {
				return UtcNow.AddHours(9);
			}
		}
 
		public static DateTime UtcNow {
			get {
				return new DateTime(1999,1,2,3,4,5);
			}
		}

		internal static long GetNow() {
			return UtcNow.Ticks;
		} 

	}

}
