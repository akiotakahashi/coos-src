using System;

namespace CooS.Drivers.ATAPI {

	public struct InterruptReason {
	
		public byte Value;

		public InterruptReason(byte value) {
			this.Value = value;
		}

		public override string ToString() {
			return "cd="+this.cd+", io="+this.io+", rel="+this.rel+", tag="+this.tag;
		}

		public static implicit operator InterruptReason(byte op) {
			return new InterruptReason(op);
		}
		
		/// <summary>
		/// 0�̂Ƃ��A�f�[�^�]�����������̓o�X�J��
		/// 1�̂Ƃ��A�p�P�b�g�R�}���h�v�����������̓��b�Z�[�W�]����
		/// </summary>
		public bool cd {
			get {
				return 0!=(this.Value&0x01);
			}
		}

		/// <summary>
		/// ���o�͕����\���r�b�g
		/// 0�̂Ƃ��A�z�X�g���f�o�C�X
		/// 1�̂Ƃ��A�f�o�C�X���z�X�g 
		/// </summary>
		public bool io {
			get {
				return 0!=(this.Value&0x02);
			}
		}

		/// <summary>
		/// �o�X�������[�X����Ă�����1�ɂȂ� 
		/// </summary>
		public bool rel {
			get {
				return 0!=(this.Value&0x04);
			}
		}

		/// <summary>
		/// �I�[�o�[���b�v�g�p�P�b�g�R�}���h�̂Ƃ��ɁA�R�}���h�p�̃^�O���i�[
		/// </summary>
		public byte tag {
			get {
				return (byte)(this.Value >> 3);
			}
		}

	}

}
