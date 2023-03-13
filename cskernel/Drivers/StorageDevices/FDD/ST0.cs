using System;

namespace CooS.Drivers.StorageDevices.FDD {

	public struct ST0 {

		public byte Value;
		/*
		byte us		: 2;
		byte hd		: 1;
		byte nr		: 1;
		byte ec		: 1;
		byte se		: 1;
		byte ic		: 2;
		*/

		public int us {
			get {
				return this.Value & 3;
			}
			set {
				this.Value &= 0xFC;
				this.Value |= (byte)value;
			}
		}

		public bool hd {
			get {
				return ((this.Value>>2)&1)!=0;
			}
		}
		
		public bool nr {
			get {
				return ((this.Value>>3)&1)!=0;
			}
		}
		
		public bool ec {
			get {
				return ((this.Value>>4)&1)!=0;
			}
		}
		
		public bool se {
			get {
				return ((this.Value>>5)&1)!=0;
			}
		}
		
		public int ic {
			get {
				return this.Value>>6;
			}
			set {
				this.Value &= 0x3F;
				this.Value |= (byte)(value<<6);
			}
		}

	}

}
