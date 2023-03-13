using System;
using CooS.Architectures;
using CooS.Drivers.Controllers;

namespace CooS.Drivers.PS2.Mouse {

	public struct MouseData {
		
		public byte Value;

		public bool LeftButtonPressed {
			get {
				return 0!=(this.Value&0x01);
			}
		}

		public bool RightButtonPressed {
			get {
				return 0!=(this.Value&0x02);
			}
		}

		public bool MinusX {
			get {
				return 0!=(this.Value&0x10);
			}
		}

		public bool MinusY {
			get {
				return 0!=(this.Value&0x20);
			}
		}

		public bool OverflowX {
			get {
				return 0!=(this.Value&0x40);
			}
		}

		public bool OverflowY {
			get {
				return 0!=(this.Value&0x80);
			}
		}

	}

	public delegate void MouseEventHandler(MouseData data, int dx, int dy, int dz);

	public class PS2Mouse {

		KeyboardController kbc;
		int irq;

		internal PS2Mouse(KeyboardController kbc, int irq) {
			this.kbc = kbc;
			this.irq = irq;
			this.LetEnabled(false);
			InterruptController.Register(irq, new InterruptHandler(HandleInterrupt));
		}

		public void LetEnabled(bool enabled) {
			if(enabled) {
				InterruptController.LetEnabled(this.irq, enabled);
				kbc.LetAuxiliaryInterruptEnabled(enabled);
				kbc.LetAuxiliaryDeviceEnabled(enabled);
				this.LetDeviceEnabled(enabled);
			} else {
				this.LetDeviceEnabled(enabled);
				kbc.LetAuxiliaryDeviceEnabled(enabled);
				kbc.LetAuxiliaryInterruptEnabled(enabled);
				InterruptController.LetEnabled(this.irq, enabled);
			}
		}

		private void LetDeviceEnabled(bool enabled) {
			if(enabled) {
				kbc.SendToAuxiliary(0xF4);
			} else {
				kbc.SendToAuxiliary(0xF5);
			}
		}

		public event MouseEventHandler OnReceive;

		private void HandleInterrupt(ref IntPtr sp) {
			if(kbc.ReadyData) {
				byte[] data = kbc.ReadData(3);
				MouseData md;
				md.Value = data[0];
				int dx = md.MinusX ? -(byte)~data[1] : data[1];
				int dy = md.MinusY ? -(byte)~data[2] : data[2];
				int dz = 0;
				if(OnReceive!=null) this.OnReceive(md, dx, dy, dz);
			}
			InterruptController.NotifyEndOfInterrupt(irq);
		}


	}

}
