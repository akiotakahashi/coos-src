using System;
using System.Globalization;

namespace CooS.Wrap._System {

	internal class _CurrentTimeZone : TimeZone {

		public override string DaylightName {
			get {
				return string.Empty;
			}
		}

		public override DaylightTime GetDaylightChanges(int year) {
			throw new NotSupportedException();
		}

		public override TimeSpan GetUtcOffset(DateTime time) {
			return TimeSpan.FromHours(9);
		}

		public override string StandardName {
			get {
				return "JST";
			}
		}

	}

}
