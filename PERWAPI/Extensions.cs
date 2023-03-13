using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace PERWAPI {

	public abstract partial class CILInstruction {

		public string Name
		{
			get
			{
				return this.GetInstName();
			}
		}

	}

	public partial class Instr {

		public uint Instruciton
		{
			get
			{
				return this.instr;
			}
		}

	}

    public partial class PEFile {

        /// <summary>
        /// Read a PE file and create all the data structures to represent it
        /// </summary>
        /// <param name="filename">The file name of the PE file</param>
        /// <returns>PEFile object representing "filename"</returns>
        public static PEFile ReadPEFile(Stream stream)
        {
            return PEReader.ReadPEFile(stream);
        }

    }

    internal partial class PEReader {

        private PEReader(PEFile pefile, System.IO.Stream file, bool refs)
            : base(file)
        {
            thisScope = pefile;
            refsOnly = refs;
            try {
                ReadDOSHeader();
            } catch(PEFileException except) {
                Console.WriteLine("Bad DOS header");
                return;
            }
            ReadFileHeader();
            ReadSectionHeaders();
            ReadCLIHeader();
            ReadMetaData();
            if(refsOnly)
                ReadMetaDataTableRefs();
            else {
                ReadMetaDataTables();
                pefile.metaDataTables = new MetaDataTables(tables);
            }
            file.Close();

            if(thisScope != null) {
                thisScope.buffer = this;
            }
        }

        public static PEFile ReadPEFile(string filename)
        {
            PEFile pefile = new PEFile(filename);
            using(System.IO.FileStream file = GetFile(filename)) {
                PEReader reader = new PEReader(pefile, file, false);
            }
            return pefile;
        }

        public static PEFile ReadPEFile(Stream stream)
        {
            PEFile pefile = new PEFile(string.Empty);
            PEReader reader = new PEReader(pefile, stream, false);
            return pefile;
        }

    }

}
