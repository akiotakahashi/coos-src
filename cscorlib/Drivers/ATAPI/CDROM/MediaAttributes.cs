using System;

namespace CooS.Drivers.ATAPI {

	[Flags]
	public enum MediaAttributes {
		Ready			= 1,	// アクセスレディ状態
		WriteProtect	= 2,	// メディアがライトプロテクト状態である
		NoMedia			= 4,	// メディアがない
		MediaChanged	= 8,	// メディアがチェンジされた
		RequestChange	= 16,	// メディアチェンジが要求された
		NotReady		= 64,	// ノットレディ状態
		Error			= 128,	// 何らかのエラー
	}

}
