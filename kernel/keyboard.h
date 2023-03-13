#pragma once


typedef void KEYBOARDHANDLER(KeyCode, KeyStatus);

namespace Keyboard {

	extern void Initialize();
	extern char KeyCodeToChar(KeyCode keycode);
	extern KEYBOARDHANDLER* getKeyboardHandler();
	extern void setKeyboardHandler(KEYBOARDHANDLER* fp);

}
