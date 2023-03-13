using System;
using System.IO;
using System.Collections;

namespace CooS.Shell {

	/// <summary>
	/// �V�F���ւ̓��o�͂��T�[�r�X����X�g���[���ł��B
	/// ���̃X�g���[���ւ̓��o�͈͂�ʂɉ�ʕ\���Ƀ��_�C���N�g����܂��B
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
