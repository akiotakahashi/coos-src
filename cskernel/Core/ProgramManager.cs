using System;
using System.Collections.Generic;

namespace CooS.Core {

	public static class ProgramManager {

		#region �R�}���h���C��

		public static unsafe string[] BuildCommandArguments(char* pcmdline, bool oldstyle) {
			if(pcmdline==null)
				return new string[0];
			string cmdline = new string(pcmdline).Trim();
			if(cmdline.Length==0)
				return new string[0];
			List<string> args = new List<string>();
			int s = 0;
			while(s<cmdline.Length) {
				int l = cmdline.IndexOf(' ', s);
				if(l<0)
					l=cmdline.Length;
				if(!oldstyle || s>0) {
					// �I�[���h�X�^�C���̈�Ԗڂ̓v���O������������X�L�b�v
					args.Add(cmdline.Substring(s, l-s));
				}
				s = l+1;
			}
			return args.ToArray();
		}

		#endregion

	}

}
