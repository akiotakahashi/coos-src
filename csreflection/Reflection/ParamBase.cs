using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Reflection {

	public abstract class ParamBase {

		/*
		 * ���̃N���X�̓p�����[�^�̌^�Ɋւ������񋟂��Ȃ��悤�ɁI
		 * �Ȃ��Ȃ瑍�̌^�̓��ꉻ���s�����\�b�h�ɂ����āA
		 * �N���X�����b�v����K�v���o�Ă��Ă��܂�����B
		 * c.f. SpecializedMethodBase:EnumParameterInfo
		 */

		public abstract MethodBase Method {
			get;
		}

		public abstract int Position {
			get;
		}

		public abstract string Name {
			get;
		}

		public string FullName {
			get {
				return this.Method.FullName+"+"+this.Name;
			}
		}

	}

}
