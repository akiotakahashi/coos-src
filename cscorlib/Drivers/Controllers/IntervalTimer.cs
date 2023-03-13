using System;
using CooS.Architectures;

namespace CooS.Drivers.Controllers {

	class IntervalTimerController {

		IOPort1 ctlp;
		IOPort1 sysp;

		public IntervalTimerController(ushort ctrl, ushort sys) {
			ctlp = new IOPort1(ctrl);
			sysp = new IOPort1(sys);
		}

		public byte ControlPort {
			set {
				ctlp.Write(value);
			}
		}

		public byte SystemPort {
			get {
				return sysp.Read();
			}
			set {
				sysp.Write(value);
			}
		}

	}

	public class IntervalTimer {

		static IntervalTimerController master;
		static IntervalTimer irq0;
		static IntervalTimer beep;
		IntervalTimerController ctrl;
		byte channel;
		byte mode;
		ushort resetcount;
		IOPort1 cntp;

		private static IntervalTimerController Master {
			get {
				if(master==null) {
					master = new IntervalTimerController(0x43,0x61);
				}
				return master;
			}
		}

		public static IntervalTimer Irq0 {
			get {
				if(irq0==null) {
					irq0 = new IntervalTimer(Master,0,2,0x40);
				}
				return irq0;
			}
		}

		public static IntervalTimer Beep {
			get {
				if(beep==null) {
					beep = new IntervalTimer(Master,2,3,0x42);
				}
				return beep;
			}
		}
		
		public static bool SpeakerEnabled {
			get {
				return (master.SystemPort&2)!=0 ? true : false;
			}
			set {
				byte al = master.SystemPort;
				al &= 0x0D;
				if(value) al |= 3;
				master.SystemPort = al;
			}
		}

		private IntervalTimer(IntervalTimerController ctrl, byte channel, byte mode, ushort cntport) {
			this.ctrl = ctrl;
			this.channel = channel;
			this.mode = mode;
			this.resetcount = 11932;
			this.cntp = new IOPort1(cntport);
		}

		public uint ResetCount {
			get {
				return resetcount;
			}
			set {
				lock(ctrl) {
					ctrl.ControlPort = (byte)((channel<<5) | (byte)0x30 | (mode<<1));
					cntp.Write((byte)value);
					cntp.Write((byte)(value>>8));
				}
			}
		}

		public void SetFrequency(uint frequency) {
			ResetCount = 1193181/frequency;
		}

	}
}
