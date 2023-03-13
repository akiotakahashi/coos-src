using System;
using CooS.Architectures;
using CooS.Drivers.Controllers;

namespace CooS.Drivers.PS2.Keyboard {

	public delegate void KeyboardEventHandler(KeyCode code, KeyStatus status);

	public class PS2Keyboard {

		KeyboardController kbc;
		int irq;
		ScanCodeConverter converter;
		
		internal PS2Keyboard(KeyboardController kbc, int irq) {
			this.kbc = kbc;
			this.irq = irq;
			this.LetEnabled(false);
			this.SelectScanCode(2);
			InterruptController.Register(irq, new InterruptHandler(HandleInterrupt));
		}

		public void LetEnabled(bool enabled) {
			if(enabled) {
				InterruptController.LetEnabled(this.irq, enabled);
				kbc.LetKeyboardInterruptEnabled(enabled);
				kbc.LetKeyboardDeviceEnabled(enabled);
			} else {
				kbc.LetKeyboardDeviceEnabled(enabled);
				kbc.LetKeyboardInterruptEnabled(enabled);
				InterruptController.LetEnabled(this.irq, enabled);
			}
		}

		public void SelectScanCode(int n) {
			if(n<1 || 3<n) throw new ArgumentException();
			kbc.SendToKeyboard(0xF0, n);
		}

		public event KeyboardEventHandler OnReceive;

		private bool on_ctrl = false;
		private bool on_alt = false;
		private bool on_del = false;

		private void HandleInterrupt(ref IntPtr sp) {
			if(!kbc.ReadyData) {
				//Console.Write("(--)");
			} else {
				byte scancode = kbc.ReadData();
				if(scancode==0xFA) {
					// ACK
					Console.Write("<KB-ACK>");
				} else if(scancode==0xE0) {
					// 特殊なキー
					converter = ScanCodeConverter.ExtendedCodeConverter;
				} else {
					// キーが押されたときかどうか
					KeyStatus keystat;
					if(0==(scancode&0x80)) {
						keystat = KeyStatus.Push;
					} else {
						scancode &= unchecked((byte)~0x80);
						keystat = KeyStatus.Release;
					}
					// 文字に変換
					if(converter==null) {
						converter = ScanCodeConverter.DefaultCodeConverter;
					}
					KeyCode c = converter.ToKeyCode(scancode);
					converter = ScanCodeConverter.DefaultCodeConverter;
					if(c==KeyCode.NONE) {
						Console.Write("(?{0:X2})", scancode);
					} else {
						// CTRL+ALT+DEL
						switch(c) {
						case KeyCode.LCTRL:
						case KeyCode.RCTRL:
							on_ctrl = keystat==KeyStatus.Push;
							break;
						case KeyCode.LALT:
						case KeyCode.RALT:
							on_alt = keystat==KeyStatus.Push;
							break;
						case KeyCode.DEL:
							on_del = keystat==KeyStatus.Push;
							break;
						}
						if(on_del && on_ctrl) {
							if(on_alt) {
								// Reset the PC/AT computer.
								Console.WriteLine();
								Console.Write("Resetting...");
								Machine.ResetMachine();
							} else {
								throw new SystemException("INTERRUPT BREAK");
							}
						}
						// イベント発行
						if(OnReceive!=null) OnReceive(c, keystat);
					}
				}
			}
			InterruptController.NotifyEndOfInterrupt(irq);
		}

	}

}
