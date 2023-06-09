#include "kernel.h"
#include "scancode.h"


extern const KeyCode ScanCodeToKeyCode[] = {
		KEY_NONE,
	KEY_ESC,
	KEY_1,
	KEY_2,
	KEY_3,
	KEY_4,
	KEY_5,
	KEY_6,
	KEY_7,
	KEY_8,
	KEY_9,
	KEY_0,
	KEY_HYPHEN,		// -
	KEY_CARET,		// ^
	KEY_BACKSPACE,
	KEY_TAB,
	KEY_Q,
	KEY_W,
	KEY_E,
	KEY_R,
	KEY_T,
	KEY_Y,
	KEY_U,
	KEY_I,
	KEY_O,
	KEY_P,
	KEY_ATMARK,		// @
	KEY_BRACKET_L,	// [
	KEY_RETURN,
	KEY_LCTRL,
	KEY_A,
	KEY_S,
	KEY_D,
	KEY_F,
	KEY_G,
	KEY_H,
	KEY_J,
	KEY_K,
	KEY_L,
	KEY_SEMICOLON,	// ;
	KEY_COLON,		// :
	KEY_BRACKET_R,	// ]
	KEY_LSHIFT,
	KEY_BRACKET_R2,	// ]	?
	KEY_Z,
	KEY_X,
	KEY_C,
	KEY_V,
	KEY_B,
	KEY_N,
	KEY_M,
	KEY_COMMA,		// ,
	KEY_PERIOD,		// .
	KEY_SLASH,		// /
	KEY_RSHIFT,
	KEY_ASTERISK,	// *
	KEY_LALT,
	KEY_SPACE,		// ' '
	KEY_CAPSLOCK,
	KEY_F1,
	KEY_F2,
	KEY_F3,
	KEY_F4,
	KEY_F5,
	KEY_F6,
	KEY_F7,
	KEY_F8,
	KEY_F9,
	KEY_F10,
	KEY_PAD_NUMLOCK,
	KEY_SCRLOCK,
	KEY_PAD_7,
	KEY_PAD_8,
	KEY_PAD_9,
	KEY_PAD_MINUS,
	KEY_PAD_4,
	KEY_PAD_5,
	KEY_PAD_6,
	KEY_PAD_PLUS,
	KEY_PAD_1,
	KEY_PAD_2,
	KEY_PAD_3,
	KEY_PAD_0,
	KEY_PAD_PERIOD,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
	KEY_F11,
	KEY_F12,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE
};

extern const KeyCode ExtendedScanCodeToKeyCode[] = {
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
	KEY_PAD_ENTER,
	KEY_RCTRL,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
	KEY_PAD_DIVIDE,
		KEY_NONE,
	KEY_PRTSCRN,
	KEY_RALT,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
	KEY_HOME,
	KEY_ARROW_UP,
	KEY_PAGEUP,
		KEY_NONE,
	KEY_ARROW_LEFT,
		KEY_NONE,
	KEY_ARROW_RIGHT,
		KEY_NONE,
	KEY_END,
	KEY_ARROW_DOWN,
	KEY_PAGEDOWN,
	KEY_INS,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
		KEY_NONE,
	KEY_LWIN,
	KEY_RWIN,
	KEY_MENU,
		KEY_NONE,
		KEY_NONE
};
