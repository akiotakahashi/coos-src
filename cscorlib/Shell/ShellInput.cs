using System;
using System.IO;
using System.Collections;
using CooS.Drivers.PS2;
using CooS.Drivers.PS2.Keyboard;

namespace CooS.Shell {

	/// <summary>
	/// シェルへの入出力をサービスするストリームです。
	/// このストリームへの入出力は一般に画面表示にリダイレクトされます。
	/// </summary>
	public class ShellInput : TextReader {

		private readonly ShellBase shell;
		private readonly Queue keyqueue = new Queue();

		bool[] pushed = new bool[256];
		bool numlocked = false;
		bool capslocked = false;
		KeyCodeConverter converter = new KeyCodeConverter();

		public ShellInput(ShellBase shell) {
			this.shell = shell;
			KeyboardController kbc = Architecture.KeyboardController;
			kbc.Keyboard.OnReceive += new KeyboardEventHandler(keyboard_OnReceive);
			kbc.Keyboard.LetEnabled(true);
		}

		private bool ShiftPressed {
			get {
				return pushed[(int)KeyCode.LSHIFT]
					|| pushed[(int)KeyCode.RSHIFT];
			}
		}

		private bool CtrlPressed {
			get {
				return pushed[(int)KeyCode.LCTRL]
					|| pushed[(int)KeyCode.RCTRL];
			}
		}

		private bool AltPressed {
			get {
				return pushed[(int)KeyCode.LALT]
					|| pushed[(int)KeyCode.RALT];
			}
		}

		private void keyboard_OnReceive(KeyCode code, KeyStatus status) {
			switch(status) {
			default:
				throw new Exception("Unknown status: "+(int)status);
			case KeyStatus.Push:
				pushed[(int)code] = true;
				this.keyqueue.Enqueue(code);
				break;
			case KeyStatus.Release:
				pushed[(int)code] = false;
				break;
			}
		}

		private int ReadImmediately() {
			if(this.keyqueue.Count==0) {
				return -1;
			} else {
				KeyCode code = (KeyCode)this.keyqueue.Dequeue();
				if(code==KeyCode.PAD_NUMLOCK) numlocked=!numlocked;
				if(code==KeyCode.CAPSLOCK) capslocked=!capslocked;
				code = KeyCodeConverter.ProcessNumLock(code, numlocked, ShiftPressed);
				char ch = KeyCodeConverter.ToChar(code, capslocked, ShiftPressed);
				if(ch!='\0') {
					return ch;
				} else {
					return ReadImmediately();
				}
			}
		}

		bool peeked = false;
		int peekch = (int)'\0';

		public override int Peek() {
			if(!peeked) {
				if(this.keyqueue.Count==0) {
					return -1;
				}
				peekch = ReadImmediately();
				peeked = peekch>0;
			}
			return peekch;
		}

		public override int Read() {
			if(peeked) {
				peeked = false;
				return peekch;
			}
			int ch;
			do {
				while(this.keyqueue.Count==0) {
					CooS.Architectures.IA32.Instruction.hlt();
				}
				ch = ReadImmediately();
			} while(ch<=0);
			return ch;
		}

	}

}
