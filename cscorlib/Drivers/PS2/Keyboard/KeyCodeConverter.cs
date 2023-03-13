using System;

namespace CooS.Drivers.PS2.Keyboard {

	public class KeyCodeConverter {
	
		public KeyCodeConverter() {
		}

		const int offset_A_to_a = 'a'-'A';

		public static KeyCode ProcessNumLock(KeyCode code, bool numlock, bool shift) {
			bool pad_console = numlock==shift;
			switch(code) {
			case KeyCode.PAD_0:
				if(pad_console) return KeyCode.INS;
				break;
			case KeyCode.PAD_1:
				if(pad_console) return KeyCode.END;
				break;
			case KeyCode.PAD_2:
				if(pad_console) return KeyCode.ARROW_DOWN;
				break;
			case KeyCode.PAD_3:
				if(pad_console) return KeyCode.PAGEDOWN;
				break;
			case KeyCode.PAD_4:
				if(pad_console) return KeyCode.ARROW_LEFT;
				break;
			case KeyCode.PAD_5:
				if(pad_console) return KeyCode.NONE;
				break;
			case KeyCode.PAD_6:
				if(pad_console) return KeyCode.ARROW_RIGHT;
				break;
			case KeyCode.PAD_7:
				if(pad_console) return KeyCode.HOME;
				break;
			case KeyCode.PAD_8:
				if(pad_console) return KeyCode.ARROW_UP;
				break;
			case KeyCode.PAD_9:
				if(pad_console) return KeyCode.PAGEUP;
				break;
			case KeyCode.PAD_PERIOD:
				if(pad_console) return KeyCode.DEL;
				break;
			}
			return code;
		}

		public static char ToChar(KeyCode code, bool capslock, bool shift) {
			switch(code) {
			default:
				return '\0';
			case KeyCode.TAB:			return '\t';
			case KeyCode.PAD_7:			return '7';
			case KeyCode.PAD_8:			return '8';
			case KeyCode.PAD_9:			return '9';
			case KeyCode.PAD_MINUS:		return '-';
			case KeyCode.PAD_4:			return '4';
			case KeyCode.PAD_5:			return '5';
			case KeyCode.PAD_6:			return '6';
			case KeyCode.PAD_PLUS:		return '+';
			case KeyCode.PAD_1:			return '1';
			case KeyCode.PAD_2:			return '2';
			case KeyCode.PAD_3:			return '3';
			case KeyCode.PAD_0:			return '0';
			case KeyCode.PAD_PERIOD:	return '.';
			case KeyCode.PAD_DIVIDE:	return '/';
			case KeyCode.PAD_ASTERISK:	return '*';
			case KeyCode.D0:
				if(shift) return '\0';
				return '0';
			case KeyCode.D1:
				if(shift) return '!';
				return '1';
			case KeyCode.D2:
				if(shift) return '"';
				return '2';
			case KeyCode.D3:
				if(shift) return '#';
				return '3';
			case KeyCode.D4:
				if(shift) return '$';
				return '4';
			case KeyCode.D5:
				if(shift) return '%';
				return '5';
			case KeyCode.D6:
				if(shift) return '\'';
				return '6';
			case KeyCode.D7:
				if(shift) return '\'';
				return '7';
			case KeyCode.D8:
				if(shift) return '(';
				return '8';
			case KeyCode.D9:
				if(shift) return ')';
				return '9';
			case KeyCode.A:
				if(shift==capslock)
					return 'a';
				return 'A';
			case KeyCode.B:
				if(shift==capslock)
					return 'b';
				return 'B';
			case KeyCode.C:
				if(shift==capslock)
					return 'c';
				return 'C';
			case KeyCode.D:
				if(shift==capslock)
					return 'd';
				return 'D';
			case KeyCode.E:
				if(shift==capslock)
					return 'e';
				return 'E';
			case KeyCode.F:
				if(shift==capslock)
					return 'f';
				return 'F';
			case KeyCode.G:
				if(shift==capslock)
					return 'g';
				return 'G';
			case KeyCode.H:
				if(shift==capslock)
					return 'h';
				return 'H';
			case KeyCode.I:
				if(shift==capslock)
					return 'i';
				return 'I';
			case KeyCode.J:
				if(shift==capslock)
					return 'j';
				return 'J';
			case KeyCode.K:
				if(shift==capslock)
					return 'k';
				return 'K';
			case KeyCode.L:
				if(shift==capslock)
					return 'l';
				return 'L';
			case KeyCode.M:
				if(shift==capslock)
					return 'm';
				return 'M';
			case KeyCode.N:
				if(shift==capslock)
					return 'n';
				return 'N';
			case KeyCode.O:
				if(shift==capslock)
					return 'o';
				return 'O';
			case KeyCode.P:
				if(shift==capslock)
					return 'p';
				return 'P';
			case KeyCode.Q:
				if(shift==capslock)
					return 'q';
				return 'Q';
			case KeyCode.R:
				if(shift==capslock)
					return 'r';
				return 'R';
			case KeyCode.S:
				if(shift==capslock)
					return 's';
				return 'S';
			case KeyCode.T:
				if(shift==capslock)
					return 't';
				return 'T';
			case KeyCode.U:
				if(shift==capslock)
					return 'u';
				return 'U';
			case KeyCode.V:
				if(shift==capslock)
					return 'v';
				return 'V';
			case KeyCode.W:
				if(shift==capslock)
					return 'w';
				return 'W';
			case KeyCode.X:
				if(shift==capslock)
					return 'x';
				return 'X';
			case KeyCode.Y:
				if(shift==capslock)
					return 'y';
				return 'Y';
			case KeyCode.Z:
				if(shift==capslock)
					return 'z';
				return 'Z';
			case KeyCode.HYPHEN:
				if(shift) return '=';
				return '-';
			case KeyCode.CARET:
				if(shift) return '~';
				return '^';
			case KeyCode.ATMARK:
				if(shift) return '`';
				return '@';
			case KeyCode.BRACKET_L:
				if(shift) return '{';
				return '[';
			case KeyCode.SEMICOLON:
				if(shift) return '+';
				return ';';
			case KeyCode.COLON:
				if(shift) return '*';
				return ':';
			case KeyCode.BRACKET_R:
				if(shift) return '}';
				return ']';
			case KeyCode.COMMA:
				if(shift) return '<';
				return ',';
			case KeyCode.PERIOD:
				if(shift) return '>';
				return '.';
			case KeyCode.SLASH:
				if(shift) return '?';
				return '/';
			case KeyCode.SPACE:
				return ' ';
			case KeyCode.UNDERSCORE:
				if(shift) return '_';
				return '\\';
			case KeyCode.BACKSPACE:
				return '\b';
			case KeyCode.ENTER:
			case KeyCode.PAD_ENTER:
				return '\n';
			}
		}

	}

}
