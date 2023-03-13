using System;
using System.IO;
using CooS.FileSystem;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.CDFS {

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct DirectoryRecord {

		public byte	LengthOfDirectoryRecord;
		public byte	ExtendedAttributeRecordLength;
		public u32b	LocationOfExtent;
		public u32b	DataLength;
		public ShortDateTime RecordingDateTime;
		public FileFlags FileFlags;
		public byte	FileUnitSize;
		public byte	InterleaveGapSize;
		public u16b	VolumeSequenceNumber;
		public byte	LengthOfFileIdentifier;
		public byte[]	FileIdentifier;		// 1 byte at least

		public DirectoryRecord(BinaryReader reader) {
			this.LengthOfDirectoryRecord = reader.ReadByte();
			if(this.LengthOfDirectoryRecord==0) {
				this = new DirectoryRecord();
			} else {
				this.ExtendedAttributeRecordLength = reader.ReadByte();
				this.LocationOfExtent = new u32b(reader);
				this.DataLength = new u32b(reader);
				this.RecordingDateTime = new ShortDateTime(reader);
				this.FileFlags = (FileFlags)reader.ReadByte();
				this.FileUnitSize = reader.ReadByte();
				this.InterleaveGapSize = reader.ReadByte();
				this.VolumeSequenceNumber = new u16b(reader);
				this.LengthOfFileIdentifier = reader.ReadByte();
				this.FileIdentifier = reader.ReadBytes(this.LengthOfFileIdentifier);
				if(this.FileIdentifier==null) throw new Exception();
			}
		}

		public string GetFileName() {
			if(this.FileIdentifier.Length==1) {
				switch(this.FileIdentifier[0]) {
				case 0:
					return string.Empty;
				case 1:
					return string.Empty;
				}
			}
			string filename = System.Text.Encoding.ASCII.GetString(this.FileIdentifier);
			int suffix = filename.LastIndexOf(';');
			if(suffix<0) return filename;
			return filename.Substring(0,suffix);
		}

		public BookInfo ToBookInfo() {
			FileAttributes attr = 0;
			if(0==(this.FileFlags&FileFlags.Existence)) attr|=FileAttributes.Hidden;
			if(0!=(this.FileFlags&FileFlags.Directory)) attr|=FileAttributes.Directory;
			DateTime dt = RecordingDateTime.ToDateTime();
			return new BookInfo(GetFileName(),attr,this.DataLength,dt,dt,dt);
		}

	}

}
