using System;

namespace CooS.Drivers.ATAPI {

	[Flags]
	public enum DeviceFeatures {
		None			= 0,
		Lock			= 1,
		Eject			= 2,
		Removable		= 4,
		MediaStatus		= 8,
		CFA				= 16,
	}

}
