using System;
using CooS.Architectures;
using CooS.Architectures.IA32;
using CooS.Drivers.Controllers;

namespace CooS.Drivers.PS2 {
	using Keyboard;
	using Mouse;

	public class KeyboardController {

		const int KBCS_OB_FULL	= 0x01;		// Output Buffer FULL
		const int KBCS_IB_FULL	= 0x02;		// Input Buffer FULL

		IOPort1 KBC_PORT;
		IOPort1 KBD_PORT;
		int irq_kbd;
		int irq_aux;
		//int cmdreg = 0;

		PS2Keyboard keyboard;
		PS2Mouse mouse;

		internal KeyboardController(int ctrlport, int dataport, int irq_kbd, int irq_aux) {
			this.KBC_PORT = new IOPort1(ctrlport);
			this.KBD_PORT = new IOPort1(dataport);
			this.irq_kbd = irq_kbd;
			this.irq_aux = irq_aux;
			//bool old1 = InterruptController.LetEnabled(irq_kbd, false);
			//bool old2 = InterruptController.LetEnabled(irq_aux, false);
		}

		public PS2Keyboard Keyboard {
			get {
				if(this.keyboard==null) {
					this.keyboard = new PS2Keyboard(this, irq_kbd);
				}
				return this.keyboard;
			}
		}

		public PS2Mouse Mouse {
			get {
				if(this.mouse==null) {
					this.mouse = new PS2Mouse(this, irq_aux);
				}
				return this.mouse;
			}
		}

		private void WaitForReadyToInput() {
			while(true) {
				byte value = KBC_PORT.Read();
				if(0!=(KBCS_OB_FULL&value)) {
					// KBCがホストに出力している。
					return;
				}
				if(0!=(KBCS_IB_FULL&value)) {
					// KBCがホストからの入力を受け取っている。
					Kernel.Delay(0,1,0);
				}
			}
		}

		private void WaitForReadyToOutput() {
			for(int i=0; i<100; ++i) {
				byte value = KBC_PORT.Read();
				if(0==((KBCS_OB_FULL|KBCS_IB_FULL)&value)) {
					return;
				} else {
					if(0!=(KBCS_OB_FULL&value)) {
						// よく分からないデータ
						byte data = KBD_PORT.Read();
						Console.Write("<KBD:{0:X2}>", data);
					}
					if(0!=(KBCS_IB_FULL&value)) {
						Kernel.Delay(0,1,0);
					}
				}
			}
			throw new SystemException("FDC Timeout");
		}

		private void ValidateStable() {
			for(;;) {
				byte value = KBC_PORT.Read();
				if(0!=(KBCS_OB_FULL&value)) throw new Exception();
				if(0==(KBCS_IB_FULL&value)) break;
				Kernel.Delay(0,1,0);
			}
		}

		private void IssueCommand(int cmd) {
			WaitForReadyToOutput();
			KBC_PORT.Write(cmd);
		}

		private void IssueCommand(int cmd, params int[] args) {
			WaitForReadyToOutput();
			KBC_PORT.Write(cmd);
			foreach(int data in args) {
				WaitForReadyToOutput();
				KBD_PORT.Write(data);
			}
		}

		private byte ReadCommandRegister() {
			WaitForReadyToOutput();
			bool old_aux = InterruptController.LetEnabled(this.irq_aux, false);
			bool old_kbd = InterruptController.LetEnabled(this.irq_kbd, false);
			KBC_PORT.Write(0x20);
			// このときに割り込みが起こるんだけど、それってなんかおかしくね？
			// と言っても仕方ないので、この関数呼ぶときは割り込み禁止で。
			WaitForReadyToInput();
			byte value = KBD_PORT.Read();
			InterruptController.LetEnabled(this.irq_kbd, old_kbd);
			InterruptController.LetEnabled(this.irq_aux, old_aux);
			return value;
		}

		private void WriteCommandRegister(byte value) {
			// コマンドレジスタを設定
			IssueCommand(0x60, value);
		}

		private bool SetCommandRegister(int bit, bool turnon) {
			bit = 1<<bit;
			byte cmdreg = ReadCommandRegister();
			bool old = 0!=(cmdreg&bit);
			if(turnon) {
				cmdreg |= (byte)bit;
			} else {
				cmdreg &= (byte)~bit;
			}
			WriteCommandRegister(cmdreg);
			return old;
		}

		public bool LetKeyboardInterruptEnabled(bool enabled) {
			return SetCommandRegister(0, enabled);
		}

		public bool LetAuxiliaryInterruptEnabled(bool enabled) {
			return SetCommandRegister(1, enabled);
		}

		public void LetKeyboardDeviceEnabled(bool enabled) {
			if(enabled) {
				this.IssueCommand(0xAE);
			} else {
				this.IssueCommand(0xAD);
			}
		}

		public void LetAuxiliaryDeviceEnabled(bool enabled) {
			if(enabled) {
				this.IssueCommand(0xA8);
			} else {
				this.IssueCommand(0xA7);
			}
		}

		public bool ReadyData {
			get {
				return 0!=(KBCS_OB_FULL&KBC_PORT.Read());
			}
		}

		public byte ReadData() {
			WaitForReadyToInput();
			return KBD_PORT.Read();
		}

		public byte[] ReadData(int size) {
			byte[] buf = new byte[size];
			for(int i=0; i<size; ++i) {
				buf[i] = ReadData();
			}
			return buf;
		}

		public void SendToKeyboard(params int[] data) {
			foreach(int b in data) {
				byte ret;
				do {
					WaitForReadyToOutput();
					bool old = this.LetKeyboardInterruptEnabled(false);
					KBD_PORT.Write(b);
					ret = ReadData();
					this.LetKeyboardInterruptEnabled(old);
				} while(ret!=0xFA);
			}
		}

		public void SendToAuxiliary(params byte[] data) {
			WaitForReadyToOutput();
			bool old = this.LetAuxiliaryInterruptEnabled(false);
			foreach(byte b in data) {
				byte ret;
				do {
					IssueCommand(0xD4, b);
					ret = ReadData();
				} while(ret!=0xFA);
			}
			this.LetAuxiliaryInterruptEnabled(old);
		}

		public void PulseOutputPort(int port) {
			if(port<0 || 15<port) throw new ArgumentOutOfRangeException();
			this.IssueCommand(0xF0|port);
		}

	}

}
