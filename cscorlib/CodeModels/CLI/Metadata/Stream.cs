/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace CooS.CodeModels.CLI.Metadata {

	/// <summary>
	/// Metadata stream.
	/// </summary>
	public class MDStream {

		/// <summary>
		/// Metadata stream header as described
		/// in ECMA CLI specs, Partition II Metadata, 23.1.2
		/// </summary>
		private uint offs;
		private uint size;
		private string name;

		public MDStream() {
		}

		public uint Offset {
			get {
				return offs;
			}
			set {
				offs = value;
			}
		}

		public uint Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}

		/// <summary>
		/// Name of the stream.
		/// </summary>
		/// <remarks>
		/// Stored on-disk as a null-terminated ASCII string,
		/// rounded up to 4-byte boundary.
		/// </remarks>
		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// Reads stream header and body from supplied BinaryReader.
		/// </summary>
		/// <remarks>
		/// Reader must be positioned at the first byte of metadata stream.
		/// </remarks>
		/// <param name="reader"></param>
		unsafe public void Read(MetadataRoot root, BinaryReader reader) {
			offs = reader.ReadUInt32 ();
			size = reader.ReadUInt32 ();
			name = CooS.CodeModels.DLL.ImageUtility.ReadString(reader,16);
			if(name==null || name.Length==0) throw new BadImageException();
			// Round up to dword boundary.
			long pos = reader.BaseStream.Position;
			pos -= root.BasePosition;
			pos = (pos+3)&~3;
			pos += root.BasePosition;
			// Advance file pointer.
			reader.BaseStream.Position = pos;
		}

		/// <summary>
		/// </summary>
		public void Dump(TextWriter writer) {
			writer.WriteLine("Name        : {0}", name);
			writer.WriteLine("Offset      : 0x{1:x8}", offs);
			writer.WriteLine("Size        : 0x{2:x8}", size);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter ();
			Dump(sw);
			return sw.ToString();
		}

	}
}
