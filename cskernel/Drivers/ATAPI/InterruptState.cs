using System;

namespace CooS.Drivers.ATAPI {

	public enum InterruptState {
		IDLE,
		READY,
		COMMAND,
		MESSAGE,
		DATATOHOST,
		DATAFROMHOST,
		STATUS,
		RELEASE,
	}

}
