using System;

namespace CooS.Drivers.ATAPI {

	[Flags]
	public enum MediaAttributes {
		Ready			= 1,	// �A�N�Z�X���f�B���
		WriteProtect	= 2,	// ���f�B�A�����C�g�v���e�N�g��Ԃł���
		NoMedia			= 4,	// ���f�B�A���Ȃ�
		MediaChanged	= 8,	// ���f�B�A���`�F���W���ꂽ
		RequestChange	= 16,	// ���f�B�A�`�F���W���v�����ꂽ
		NotReady		= 64,	// �m�b�g���f�B���
		Error			= 128,	// ���炩�̃G���[
	}

}
