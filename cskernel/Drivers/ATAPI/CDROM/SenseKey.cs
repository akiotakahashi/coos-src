using System;

namespace CooS.Drivers.ATAPI.CDROM {

	public enum SenseKey {
		/// <summary>
		/// Indicates that there is no specific sense key information to be reported. This would be the case for a successful command.
		/// </summary>
		NoSense				= 0,
		/// <summary>
		/// Indicates that the last command completed successfully with some recovery action performed
		/// by the ATAPI CD-ROM Drive. Details may be determinable by examining the additional sense bytes and
		/// the information field. When multiple recovered errors occur during one command, the choice of which error to
		/// report (first, last, most severe, etc.) is device specific.
		/// </summary>
		RecoveredError		= 1,
		/// <summary>
		/// Indicates that the Device cannot be accessed. Operator intervention may be required to correct this condition.
		/// </summary>
		NotReady			= 2,
		/// <summary>
		/// Indicates that the command terminated with a non-recovered error condition that was probably
		/// caused by a flaw in the medium or an error in the recorded data. This sense key may also be returned if the ATAPI
		/// CD-ROM Drive is unable to distinguish between a flaw in the medium and a specific hardware failure (sense key 4h).
		/// </summary>
		MediumError			= 3,
		/// <summary>
		/// Indicates that the ATAPI CD-ROM Drive detected a non-recoverable hardware failure (for
		/// example, controller failure, device failure, parity error, etc.) while performing the command or during a self test.
		/// </summary>
		HardwareError		= 4,
		/// <summary>
		/// Indicates that there was an illegal parameter in the Command Packet or in the additional
		/// parameters supplied as data for some commands. If the ATAPI CD-ROM Drive detects an invalid parameter in the
		/// Command Packet, then it shall terminate the command without altering the medium. If the ATAPI CD-ROM
		/// Drive detects an invalid parameter in the additional parameters supplied as data, then the ATAPI CD-ROM Drive
		/// may have already altered the medium.
		/// </summary>
		IllegalRequest		= 5,
		/// <summary>
		/// Indicates that the removable medium may have been changed or the ATAPI CD-ROM Drive has been reset.
		/// </summary>
		UnitAttention		= 6,
		/// <summary>
		/// Indicates that a command that reads the medium was attempted on a block that is protected from
		/// this operation. The read operation is not performed.
		/// </summary>
		DataProtect			= 7,
		/// <summary>
		/// Indicates that the device has aborted the command. The Host may be able to recover by
		/// trying the command again. This error is reported for conditions such as an overrun etc.
		/// </summary>
		AbortedCommand		= 0xB,
		/// <summary>
		/// Indicates that the source data did not match the data read from the medium.
		/// </summary>
		Miscompare			= 0xE,
	}

}
