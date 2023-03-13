using System;

namespace CooS.Drivers.StorageDevices.FDD {

	public struct ReadDataResult {
		public ST0 st0;
		public byte st1;
		public byte st2;
		public uint c;
		public uint h;
		public uint r;
		public uint n;
	}

}
