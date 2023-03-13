using System;

namespace CooS.Drivers.PS2.Keyboard {

	public class ScanCodeConverter {
		
		static ScanCodeConverter defaultconv = null;
		static ScanCodeConverter extendedconv = null;

		private KeyCode[] keycodemap;

		public static ScanCodeConverter DefaultCodeConverter {
			get {
				if(defaultconv==null) {
					defaultconv = new ScanCodeConverter(ScanCodeToKeyCode);
				}
				return defaultconv;
			}
		}

		public static ScanCodeConverter ExtendedCodeConverter {
			get {
				if(extendedconv==null) {
					extendedconv = new ScanCodeConverter(ExtendedScanCodeToKeyCode);
				}
				return extendedconv;
			}
		}

		private ScanCodeConverter(KeyCode[] codemap) {
			this.keycodemap = codemap;
		}

		public KeyCode ToKeyCode(int scancode) {
			return keycodemap[scancode];
		}

		static KeyCode[] ScanCodeToKeyCode = new KeyCode[]{
															  KeyCode.NONE,
															  KeyCode.ESC,
															  KeyCode.D1,
															  KeyCode.D2,
															  KeyCode.D3,
															  KeyCode.D4,
															  KeyCode.D5,
															  KeyCode.D6,
															  KeyCode.D7,
															  KeyCode.D8,
															  KeyCode.D9,
															  KeyCode.D0,
															  KeyCode.HYPHEN,		// -
															  KeyCode.CARET,		// ^
															  KeyCode.BACKSPACE,
															  KeyCode.TAB,
															  KeyCode.Q,
															  KeyCode.W,
															  KeyCode.E,
															  KeyCode.R,
															  KeyCode.T,
															  KeyCode.Y,
															  KeyCode.U,
															  KeyCode.I,
															  KeyCode.O,
															  KeyCode.P,
															  KeyCode.ATMARK,		// @
															  KeyCode.BRACKET_L,	// [
															  KeyCode.ENTER,
															  KeyCode.LCTRL,
															  KeyCode.A,
															  KeyCode.S,
															  KeyCode.D,
															  KeyCode.F,
															  KeyCode.G,
															  KeyCode.H,
															  KeyCode.J,
															  KeyCode.K,
															  KeyCode.L,
															  KeyCode.SEMICOLON,	// ;
															  KeyCode.COLON,		// :
															  KeyCode.ZENKAKU,		// ‘SŠp”¼Šp
															  KeyCode.LSHIFT,
															  KeyCode.BRACKET_R,	// ]
															  KeyCode.Z,
															  KeyCode.X,
															  KeyCode.C,
															  KeyCode.V,
															  KeyCode.B,
															  KeyCode.N,
															  KeyCode.M,
															  KeyCode.COMMA,		// ,
															  KeyCode.PERIOD,		// .
															  KeyCode.SLASH,		// /
															  KeyCode.RSHIFT,
															  KeyCode.PAD_ASTERISK,	// *
															  KeyCode.LALT,
															  KeyCode.SPACE,		// ' '
															  KeyCode.CAPSLOCK,
															  KeyCode.F1,
															  KeyCode.F2,
															  KeyCode.F3,
															  KeyCode.F4,
															  KeyCode.F5,
															  KeyCode.F6,
															  KeyCode.F7,
															  KeyCode.F8,
															  KeyCode.F9,
															  KeyCode.F10,
															  KeyCode.PAD_NUMLOCK,
															  KeyCode.SCRLOCK,
															  KeyCode.PAD_7,
															  KeyCode.PAD_8,
															  KeyCode.PAD_9,
															  KeyCode.PAD_MINUS,
															  KeyCode.PAD_4,
															  KeyCode.PAD_5,
															  KeyCode.PAD_6,
															  KeyCode.PAD_PLUS,
															  KeyCode.PAD_1,
															  KeyCode.PAD_2,
															  KeyCode.PAD_3,
															  KeyCode.PAD_0,
															  KeyCode.PAD_PERIOD,
															  KeyCode.SYSREQ,
															  KeyCode.NONE,
															  KeyCode.NONE,
															  KeyCode.F11,
															  KeyCode.F12,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.HIRAGANA,
															  KeyCode.NONE,
															  KeyCode.NONE,
															  KeyCode.UNDERSCORE,
															  KeyCode.NONE,
															  KeyCode.NONE, KeyCode.NONE, KeyCode.NONE, KeyCode.NONE,
															  KeyCode.HENKAN,
															  KeyCode.NONE,
															  KeyCode.MUHENKAN,
															  KeyCode.NONE,
															  KeyCode.BACKSLASH,
															  KeyCode.NONE,
															  KeyCode.NONE,
		};

		static KeyCode[] ExtendedScanCodeToKeyCode = new KeyCode[256];

		static ScanCodeConverter() {
			ExtendedScanCodeToKeyCode[0x1C] = KeyCode.PAD_ENTER;
			ExtendedScanCodeToKeyCode[0x1D] = KeyCode.RCTRL;
			ExtendedScanCodeToKeyCode[0x35] = KeyCode.PAD_DIVIDE;
			ExtendedScanCodeToKeyCode[0x37] = KeyCode.PRTSCRN;
			ExtendedScanCodeToKeyCode[0x38] = KeyCode.RALT;
			ExtendedScanCodeToKeyCode[0x45] = KeyCode.PAD_NUMLOCK;
			ExtendedScanCodeToKeyCode[0x46] = KeyCode.BREAK;
			ExtendedScanCodeToKeyCode[0x47] = KeyCode.HOME;
			ExtendedScanCodeToKeyCode[0x48] = KeyCode.ARROW_UP;
			ExtendedScanCodeToKeyCode[0x49] = KeyCode.PAGEUP;
			ExtendedScanCodeToKeyCode[0x4B] = KeyCode.ARROW_LEFT;
			ExtendedScanCodeToKeyCode[0x4D] = KeyCode.ARROW_RIGHT;
			ExtendedScanCodeToKeyCode[0x4F] = KeyCode.END;
			ExtendedScanCodeToKeyCode[0x50] = KeyCode.ARROW_DOWN;
			ExtendedScanCodeToKeyCode[0x51] = KeyCode.PAGEDOWN;
			ExtendedScanCodeToKeyCode[0x52] = KeyCode.INS;
			ExtendedScanCodeToKeyCode[0x53] = KeyCode.DEL;
			ExtendedScanCodeToKeyCode[0x5B] = KeyCode.LWIN;
			ExtendedScanCodeToKeyCode[0x5C] = KeyCode.RWIN;
			ExtendedScanCodeToKeyCode[0x5D] = KeyCode.MENU;
		}

	}

}
