using System;
using CooS.Reflection;
using CooS.Execution;

namespace CooS {

	public abstract class Architecture {

		private static Architecture arc;

		static Architecture() {
			arc = new Architectures.IA32.ArchitectureImpl();
		}

		protected Architecture() {
		}

		public static Architecture Target {
			get {
				return arc;
			}
		}

		public abstract int AddressSize {
			get;
		}

		#region �A�[�L�e�N�`���ˑ��̌v�Z

		/// <summary>
		/// �傫�� size �̃I�u�W�F�N�g����L����X�^�b�N��ł̃o�C�g�T�C�Y���v�Z���܂��B
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int GetStackingSize(int size) {
			return (size+IntPtr.Size-1)&~(IntPtr.Size-1);
		}

		/// <summary>
		/// �傫�� size �̃I�u�W�F�N�g����L����X�^�b�N��ł̃X�^�b�N�P�ʂ̌����v�Z���܂��B
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int GetStackingLength(int size) {
			return (size+IntPtr.Size-1)/IntPtr.Size;
		}

		/// <summary>
		/// �����̐��l��\�����邷��ŏ���2^n�̐��l�𓾂܂��B
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int GetAlignment(int size) {
			if(size<=1) {
				return 1;
			} else if(size<=2) {
				return 2;
			} else if(size<=4) {
				return 4;
			} else if(size<=8) {
				return 8;
			} else {
				return 8;
			}
		}

		/// <summary>
		/// offset �́A�ŏ��� size �̐����{�𓾂܂��B
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int AlignOffset(int offset, int size) {
			int align = GetAlignment(size)-1;
			return (offset+align)&~align;
		}

		#endregion

	}

}
