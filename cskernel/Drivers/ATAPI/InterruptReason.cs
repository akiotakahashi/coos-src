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
		/// 0のとき、データ転送中もしくはバス開放
		/// 1のとき、パケットコマンド要求中もしくはメッセージ転送中
		/// </summary>
		public bool cd {
			get {
				return 0!=(this.Value&0x01);
			}
		}

		/// <summary>
		/// 入出力方向表示ビット
		/// 0のとき、ホスト→デバイス
		/// 1のとき、デバイス→ホスト 
		/// </summary>
		public bool io {
			get {
				return 0!=(this.Value&0x02);
			}
		}

		/// <summary>
		/// バスがリリースされていたら1になる 
		/// </summary>
		public bool rel {
			get {
				return 0!=(this.Value&0x04);
			}
		}

		/// <summary>
		/// オーバーラップトパケットコマンドのときに、コマンド用のタグを格納
		/// </summary>
		public byte tag {
			get {
				return (byte)(this.Value >> 3);
			}
		}

	}

}
