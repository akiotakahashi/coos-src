using System;

namespace CooS.Manipulation {

	public enum CodeLevel {
		None,
		DontCare,	// �R�[�h���x�����w�肵�܂���B
		IL,			// �C���^�[�v���^�B���̃��x���̃R�[�h�̓��K�V�[�J�[�l���̃C���^�v���^�Ɉˑ����܂��B
		Stub,		// �X�^�u�B���̃��x���̃R�[�h�̓R�[�h�}�l�[�W���փ��_�C���N�g�����\��������܂��B
		Native,		// �@�B��B���̃��x���̃R�[�h�͒��ڎ��s�\�ł����A�Ăяo���̓X�^�u�ł���\��������܂��B
		//PureExecutable,	// ���S���s�\�B���̃��x���̃R�[�h�́A���ʂ̃R�[�h�����ׂ�NativeCode�ł��邱�Ƃ�ۏ؂��܂��B
	}

}
