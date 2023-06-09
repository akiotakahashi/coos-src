
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
#if !COOS
using System.Collections;
#else
using System.Collections.Generic;
#endif
using System.Runtime.InteropServices;

#if !COOS
using Mono.PEToolkit.Metadata;
#endif

namespace Mono.PEToolkit {

	public class Image : IDisposable {

		internal DOSHeader dosHdr;
		internal COFFHeader coffHdr;
		internal PEHeader peHdr;

#if !COOS
		internal CorHeader corHdr;
#endif

#if !COOS
		internal Hashtable sections;
#else
		internal readonly Dictionary<string,Section> sections = new Dictionary<string,Section>();
#endif
		// File position right after PEHeader (NT Optional Header).
		protected long sectionsPos;

#if !COOS
		private MetaDataRoot mdRoot;
#endif

		private string name;
		private bool open;
		internal BinaryReader reader;

		public Image(string name)
		{
			this.name = name;
			open = false;
			reader = null;

#if !COOS
			mdRoot = null;
#endif

			dosHdr = new DOSHeader();
			coffHdr = new COFFHeader();
			peHdr = new PEHeader();
#if !COOS
			corHdr = new CorHeader();
#endif

#if !COOS
			sections = new Hashtable();
#endif
			sectionsPos = -1;
		}

#if COOS
		public Image(Stream stream) {
			this.name = null;
			open = true;
			reader = new BinaryReader(stream);

			dosHdr = new DOSHeader();
			coffHdr = new COFFHeader();
			peHdr = new PEHeader();

			sectionsPos = -1;
		}
#endif

		~Image()
		{
			Close();
		}


#if COOS
		public Stream Stream {
			get {
				return this.reader.BaseStream;
			}
		}
#endif

#if COOS
		public PEHeader Headers {
			get {
				return this.peHdr;
			}
		}
#endif

#if !COOS
		public Hashtable Sections {
			get {
				return sections;
			}
		}
#else
		public Dictionary<string,Section> Sections {
			get {
				return sections;
			}
		}
#endif

		public void Open()
		{
			lock (this) if (!open) {
				FileInfo pe = new FileInfo(name);
				if (!pe.Exists) {
					throw new Exception("Invalid file path.");
				}

				reader = new BinaryReader(new BufferedStream(pe.OpenRead()));
				if (!reader.BaseStream.CanSeek) {
					throw new Exception("Can't seek.");
				}

				open = true;
			}
		}

		public void Close()
		{
			lock (this) if (open) {
				reader.Close();
				open = false;
			}
		}

		// IDisposable
		public void Dispose()
		{
			Close();
		}


		public bool IsCLI {
			get {
				return peHdr.IsCLIImage;
			}
		}

#if !COOS
		public MetaDataRoot MetadataRoot {
			get {
				return mdRoot;
			}
		}
#endif

		/// <summary>
		/// </summary>
		public void ReadHeaders()
		{
			if (!open) {
				throw new Exception("You must open image before trying to read it.");
			}

			dosHdr.Read(reader);
			reader.BaseStream.Position = dosHdr.Lfanew;
			ExeSignature peSig = (ExeSignature) reader.ReadUInt16();
			if (peSig != ExeSignature.NT) {
				throw new Exception ("Invalid image format: cannot find PE signature.");
			}
			peSig = (ExeSignature) reader.ReadUInt16();
			if (peSig != ExeSignature.NT2) {
				throw new Exception ("Invalid image format: cannot find PE signature.");
			}

			coffHdr.Read(reader);
			peHdr.Read(reader);
		
			sectionsPos = reader.BaseStream.Position;
			ReadSections();
			
#if !COOS
			if (this.IsCLI) {
				
				reader.BaseStream.Position = RVAToVA(peHdr.CLIHdrDir.virtAddr);
				corHdr.Read (reader);

				mdRoot = new MetaDataRoot(this);
				reader.BaseStream.Position = RVAToVA(corHdr.MetaData.virtAddr);
				mdRoot.Read(reader);
			}
#endif
			
		}

		public void WriteHeaders (BinaryWriter writer)
		{
			dosHdr.Write (writer);
			writer.BaseStream.Position = dosHdr.Lfanew;
			writer.Write ((ushort)ExeSignature.NT);
			writer.Write ((ushort)ExeSignature.NT2);
			
			coffHdr.Write (writer);
			peHdr.Write (writer);
		
			WriteSections (writer);
			
#if !COOS
			if (this.IsCLI) {
				
				writer.BaseStream.Position = RVAToVA (peHdr.CLIHdrDir.virtAddr);
				corHdr.Write (writer);

				long pos = RVAToVA (corHdr.MetaData.virtAddr);
				writer.BaseStream.Position = pos;
				mdRoot.Write (writer);
				
			}
#endif
			
		}

		/// <summary>
		/// </summary>
		protected void ReadSections()
		{
			if (sectionsPos < 0) {
				throw new Exception("Read headers first.");
			}
			reader.BaseStream.Position = sectionsPos;

			int n = coffHdr.NumberOfSections;
			for (int i = n; --i >=0;) {
				Section sect = new Section();
				sect.Read(reader);
				sections [sect.Name] = sect;
			}
		}

		protected void WriteSections (BinaryWriter writer)
		{
			foreach (Section section in sections.Values) {
				section.Write (writer);
			}
		}

#if !COOS

		/// <summary>
		/// </summary>
		/// <param name="writer"></param>
		public void Dump(TextWriter writer)
		{
			writer.WriteLine (
				"COFF Header:" + Environment.NewLine +
				coffHdr.ToString() + Environment.NewLine +
				"PE Header:" + Environment.NewLine +
				peHdr.ToString() + Environment.NewLine +
				"Core Header:" + Environment.NewLine +
				corHdr.ToString()
				);
		}


		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

#endif

		/// <summary>
		///  Returns name of the section for the given RVA.
		/// </summary>
		/// <param name="rva"></param>
		/// <returns></returns>
		public string RVAToSectionName(RVA rva)
		{
			string res = null;
			foreach (Section s in Sections.Values) {
				RVA sva = s.VirtualAddress;
				if (rva >= sva && rva < sva + s.SizeOfRawData) {
					res = s.Name;
					break;
				}
			}
			return res;
		}

		public long RVAToVA(RVA rva)
		{
			string sectName = RVAToSectionName(rva);
			long res = 0;
			if (sectName != null) {
				Section s = (Section) Sections [sectName];
				res = rva + (s.PointerToRawData - s.VirtualAddress);
			}
			return res;
		}

#if !COOS
		public MetaDataRoot MetaDataRoot {
			get {
				return mdRoot;
			}
		}
#endif

#if !COOS
		public void DumpStreamHeader(TextWriter writer, string name)
		{
			if (mdRoot == null || name == null || name == String.Empty || writer == null) return;
			writer.Write(name + " header: ");
			MDStream s = MetaDataRoot.Streams[name] as MDStream;
			if (s != null) {
				writer.WriteLine();
				writer.WriteLine(s);
			} else {
				writer.WriteLine("not present.");
				writer.WriteLine();
			}
		}

		public void DumpStreamHeader(string name)
		{
			DumpStreamHeader(Console.Out, name);
		}
#endif

	}

}

