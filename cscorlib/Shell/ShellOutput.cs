using System;
using System.IO;
using System.Collections;

namespace CooS.Shell {

	/// <summary>
	/// シェルへの入出力をサービスするストリームです。
	/// このストリームへの入出力は一般に画面表示にリダイレクトされます。
	/// </summary>
	public class ShellOutput : TextWriter {

		private readonly ShellBase shell;

		public ShellOutput(ShellBase shell) {
			this.shell = shell;
		}

		public override System.Text.Encoding Encoding {
			get {
				return System.Text.Encoding.ASCII;
			}
		}

		public override void Write(char value) {
			this.shell.Output(value);
		}

	}
}
