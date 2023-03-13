using System;

namespace CooS.Drivers.ATAPI {

	public struct StatusRegister {
	
		public byte Value;

		public StatusRegister(byte value) {
			this.Value = value;
		}

		public override string ToString() {
			return "err="+this.err+", drq="+this.drq+", drdy="+this.drdy+", bsy="+this.bsy;
		}

		public static implicit operator StatusRegister(byte op) {
			return new StatusRegister(op);
		}

		/// <summary>
		/// Error / Check
		/// 
		/// ERR indicates that an error occurred during execution of the previous command. For the PACKET and
		/// SERVICE commands, this bit is defined as CHK and indicates that an exception condition exists (See 2.1).
		/// </summary>
		public bool err {
			get {
				return 0!=(this.Value&0x01);
			}
		}

		/// <summary>
		/// Data Request
		/// 
		/// DRQ indicates that the device is ready to transfer data between the host and the device. After the host has
		/// written the Command register the device shall either set the BSY bit to one or the DRQ bit to one, until
		/// command completion or the device has performed a bus release for an overlapped command.
		/// </summary>
		public bool drq {
			get {
				return 0!=(this.Value&0x08);
			}
		}

		/// <summary>
		/// Device Fault / Stream Error
		/// 
		/// Device Fault is implemented by many but not all commands (See Clause 6). A Device Fault is any event that
		/// prevents the device from completing a command that is not the result of an error described in the Error
		/// register. Recovery from Device Fault is device specific. See Streaming Command feature Set, (Clause 4.17)
		/// for description of SE bit.
		/// </summary>
		public bool df {
			get {
				return 0!=(this.Value&0x20);
			}
		}

		/// <summary>
		/// Device Ready
		/// 
		/// The DRDY bit shall be cleared to zero by the device:
		/// 	1)	when power-on, hardware, or software reset or DEVICE RESET or EXECUTE DEVICE
		/// 		DIAGNOSTIC commands for devices implementing the PACKET command feature set.
		/// 		
		/// When the DRDY bit is cleared to zero, the device shall accept and attempt to execute commands as
		/// described in Volume 2, Clause 3.
		/// 
		/// The DRDY bit shall be set to one by the device:
		///		1)	when the device is capable of accepting all commands for devices not implementing the
		///			PACKET command feature set;
		///		2)	prior to command completion except the DEVICE RESET or EXECUTE DEVICE
		///			DIAGNOSTIC command for devices implementing the PACKET command feature set.
		///			
		///	When the DRDY bit is set to one:
		///		1)	the device shall accept and attempt to execute all implemented commands;
		///		2)	devices that implement the Power Management feature set shall maintain the DRDY bit
		///			set to one when they are in the Idle or Standby modes.
		/// </summary>
		public bool drdy {
			get {
				return 0!=(this.Value&0x40);
			}
		}

		/// <summary>
		/// Busy
		/// 
		/// BSY is set to one to indicate that the device is busy. After the host has written the Command register the
		/// device shall have either the BSY bit set to one, or the DRQ bit set to one, until command completion or the
		/// device has performed a bus release for an overlapped command.
		/// </summary>
		public bool bsy {
			get {
				return 0!=(this.Value&0x80);
			}
		}

	}

}
