using System;

namespace CooS {

	class AppDomainImpl : AppDomain {

		private string friendlyName;
		private AppDomainSetup setupInfo;

		public AppDomainImpl(string friendlyName, AppDomainSetup info) {
			this.friendlyName = friendlyName;
			this.setupInfo = info;
		}

	}

}
