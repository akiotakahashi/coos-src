using System;

namespace CooS.CodeModels {

	public enum CodeLevel {
		None,
		DontCare,	// コードレベルを指定しません。
		IL,			// インタープリタ。このレベルのコードはレガシーカーネルのインタプリタに依存します。
		Stub,		// スタブ。このレベルのコードはコードマネージャへリダイレクトされる可能性があります。
		Native,		// 機械語。このレベルのコードは直接実行可能ですが、呼び出しはスタブである可能性があります。
		//PureExecutable,	// 完全実行可能。このレベルのコードは、下位のコードがすべてNativeCodeであることを保証します。
	}

}
