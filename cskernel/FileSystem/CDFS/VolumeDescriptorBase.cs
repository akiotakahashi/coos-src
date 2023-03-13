using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.CDFS {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct VolumeDescriptorBase {
		
		public byte		VolumeDerscriptorType;
		public sbyte	StandardIdentifier0;
		public sbyte	StandardIdentifier1;
		public sbyte	StandardIdentifier2;
		public sbyte	StandardIdentifier3;
		public sbyte	StandardIdentifier4;
		public byte		VolumeDescriptorVersion;

		public VolumeDescriptorBase(BinaryReader reader) {
			this.VolumeDerscriptorType = reader.ReadByte();
			this.StandardIdentifier0 = reader.ReadSByte();
			this.StandardIdentifier1 = reader.ReadSByte();
			this.StandardIdentifier2 = reader.ReadSByte();
			this.StandardIdentifier3 = reader.ReadSByte();
			this.StandardIdentifier4 = reader.ReadSByte();
			this.VolumeDescriptorVersion = reader.ReadByte();
		}

		public static unsafe explicit operator VolumeDescriptorBase(byte[] buf) {
			fixed(byte* p = &buf[0]) {
				return *(VolumeDescriptorBase*)p;
			}
		}

	}

}
