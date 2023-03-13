#include "kernel.h"
#include "io.h"
#include "pic.h"
#include "keyboard.h"
#include "interrupt.h"
#include "stdlib.h"
#include "console.h"
#include "ilengine.h"

#define	KBD_PORT		0x60
#define	KBC_PORT		0x64

#define	KBCS_OB_FULL	0x01	// Output Buffer FULL
#define	KBCS_IB_FULL	0x02	// Input Buffer FULL


static KEYBOARDHANDLER* handler = NULL;

//スキャンコードを変換してバッファにコピー
extern const KeyCode ScanCodeToKeyCode[];
extern const KeyCode ExtendedScanCodeToKeyCode[];

static void callbackKeyHandler(unsigned char scancode) {
	static const KeyCode* keymap = ScanCodeToKeyCode;
	if(scancode == 0xfa) {
		//ACK
		getConsole() << "<KBACK>";
	} else if(scancode == 0xe0) {
		//特殊なキー
		keymap = ExtendedScanCodeToKeyCode;
	} else {
		//キーが押されたときかどうか
		KeyStatus keystat;
		if(scancode & 0x80) {
			scancode  &= ~0x80;
			keystat = KS_UP;
		} else {
			keystat = KS_DOWN;
		}
		//文字に変換
		KeyCode c = keymap[scancode];
		keymap = ScanCodeToKeyCode;
		//
		switch(c) {
		case KEY_RETURN:
			clrpanic("INTERRUPT BREAK");
			break;
		case KEY_ESC:
			if(keystat==KS_DOWN) {
				getConsole() << "\v************************************************* GOING TO RESET MACHINE";
			} else {
				outp8(KBC_PORT, 0xFE);
			}
			break;
		}
		//バッファへ保存
		KEYBOARDHANDLER* fp = handler;	// to avoid conflict by interrupt
		if(fp!=NULL) fp(c, keystat);
	}
}

//キーが押されたときに呼び出される割り込みハンドラ

static void waitForReadyToInput() {
	while(true) {
		byte value = inp8(KBC_PORT);
		if(KBCS_OB_FULL&value) {
			return;
		}
		if(KBCS_IB_FULL&value) {
			Delay(10);
		}
	}
}

static void waitForReadyToOutput() {
	for(int i=0; i<100; ++i) {
		byte value = inp8(KBC_PORT);
		if(0==((KBCS_OB_FULL|KBCS_IB_FULL)&value)) {
			return;
		} else {
			if(KBCS_OB_FULL&value) {
				// よく分からないデータ
				getConsole() << "<" << inp8(KBD_PORT) << ">";
			}
			if(KBCS_IB_FULL&value) {
				Delay(10);
			}
		}
	}
	getConsole() << "FDC Status: " << inp8(KBC_PORT);
	panic("Keyboard::waitForReadyToOutput was timeout");
}

static void confirmStable(const char* msg) {
	for(int i=0; i<100; ++i) {
		byte value = inp8(KBC_PORT);
		if(0==((KBCS_OB_FULL|KBCS_IB_FULL)&value)) {
			return;
		} else {
			if(KBCS_OB_FULL&value) {
				// よく分からないデータ
				getConsole() << "<THERE'S DATA:" << inp8(KBC_PORT) << ">";
				break;
			}
			if(KBCS_IB_FULL&value) {
				Delay(10);
			}
		}
	}
	panic(msg);
}

static void handleKeyInterrupt() {
	if(KBCS_OB_FULL&inp8(KBC_PORT)) {
		byte sc = inp8(KBD_PORT);
		callbackKeyHandler(sc);
	}
}

static __declspec(naked) void __stdcall raw_key_handler() {
	InterruptHandler_Prologue;
	handleKeyInterrupt();
	PIC::NotifyEndOfInterrupt(1);
	InterruptHandler_Epilogue;
}

namespace Keyboard {

	extern void Initialize() {
		// コマンドレジスタを読む
		waitForReadyToOutput();
		outp8(KBC_PORT, 0x20);
		waitForReadyToInput();
		byte cmdreg = inp8(KBD_PORT);
		confirmStable("Read Cmd Reg");
		// コマンドレジスタを設定
		waitForReadyToOutput();
		outp8(KBC_PORT, 0x60);
		/*
			Keyboard Interrupt		: disabled
			Auxiliary Interrupt		: disabled
			Status Register bit#2	: 0
			bit#3					: reserved
			Keyboard Disable		: 0
			Auxiliary Disable		: 1
			KeyCode Conversion		: 1
			bit#7					: reserved
		*/
		waitForReadyToOutput();
		outp8(KBD_PORT, 0x60);
		confirmStable("Write Cmd Reg");
		// SCANCODE#2を選択
		waitForReadyToOutput();	outp8(KBD_PORT, 0xF0);
		waitForReadyToInput();	inp8(KBD_PORT);
		waitForReadyToOutput();	outp8(KBD_PORT, 0x02);
		waitForReadyToInput();	inp8(KBD_PORT);
		confirmStable("Select Scancode");
		// 割り込みを許可
		PIC::RegisterInterruptHandler(1, raw_key_handler);
		PIC::EnableInterrupt(1);	// enable keyboard interrupt
		confirmStable("Enable Interrupt");
		// コマンドレジスタを設定
		waitForReadyToOutput();
		outp8(KBC_PORT, 0x60);
		waitForReadyToOutput();
		outp8(KBD_PORT, 0x61 /* enabled interrupt */);
		confirmStable("Write Cmd Reg");
	}

	extern char KeyCodeToChar(KeyCode keycode) {
		switch(keycode) {
		default:				return '\0';
		case KEY_TAB:			return '\t';
		case KEY_PAD_7:			return '7';
		case KEY_PAD_8:			return '8';
		case KEY_PAD_9:			return '9';
		case KEY_PAD_MINUS:		return '-';
		case KEY_PAD_4:			return '4';
		case KEY_PAD_5:			return '5';
		case KEY_PAD_6:			return '6';
		case KEY_PAD_PLUS:		return '+';
		case KEY_PAD_1:			return '1';
		case KEY_PAD_2:			return '2';
		case KEY_PAD_3:			return '3';
		case KEY_PAD_0:			return '0';
		case KEY_PAD_PERIOD:	return '.';
		case KEY_PAD_DIVIDE:	return '/';
		case KEY_0:				return '0';
		case KEY_1:				return '1';
		case KEY_2:				return '2';
		case KEY_3:				return '3';
		case KEY_4:				return '4';
		case KEY_5:				return '5';
		case KEY_6:				return '6';
		case KEY_7:				return '7';
		case KEY_8:				return '8';
		case KEY_9:				return '9';
		case KEY_A:				return 'a';
		case KEY_B:				return 'b';
		case KEY_C:				return 'c';
		case KEY_D:				return 'd';
		case KEY_E:				return 'e';
		case KEY_F:				return 'f';
		case KEY_G:				return 'g';
		case KEY_H:				return 'h';
		case KEY_I:				return 'i';
		case KEY_J:				return 'j';
		case KEY_K:				return 'k';
		case KEY_L:				return 'l';
		case KEY_M:				return 'm';
		case KEY_N:				return 'n';
		case KEY_O:				return 'o';
		case KEY_P:				return 'p';
		case KEY_Q:				return 'q';
		case KEY_R:				return 'r';
		case KEY_S:				return 's';
		case KEY_T:				return 't';
		case KEY_U:				return 'u';
		case KEY_V:				return 'v';
		case KEY_W:				return 'w';
		case KEY_X:				return 'x';
		case KEY_Y:				return 'y';
		case KEY_Z:				return 'z';
		case KEY_HYPHEN:		return '-';
		case KEY_CARET:			return '^';
		case KEY_ATMARK:		return '@';
		case KEY_BRACKET_L:		return '[';
		case KEY_SEMICOLON:		return ';';
		case KEY_COLON:			return ':';
		case KEY_BRACKET_R:		return ']';
		case KEY_BRACKET_R2:	return ']';
		case KEY_COMMA:			return ',';
		case KEY_PERIOD:		return '.';
		case KEY_SLASH:			return '/';
		case KEY_ASTERISK:		return '*';
		case KEY_SPACE:			return ' ';
		}
	}

	extern KEYBOARDHANDLER* getKeyboardHandler() {
		return handler;
	}

	extern void setKeyboardHandler(KEYBOARDHANDLER* fp) {
		handler = fp;
	}

}
