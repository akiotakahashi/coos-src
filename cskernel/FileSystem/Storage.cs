using System;

namespace CooS.FileSystem {

	/// <summary>
	/// Storage ‚Í Drive, Disk, FlushMemory ‚È‚Ç‚Ìî•ñŠi”[”}‘Ì‚ğ•\‚µ‚Ü‚·B 
	/// </summary>
	public interface Storage {
		string PrefixName {get;}
		//int BlockSize {get;}
		//long BlockCount {get;}
		int PartitionCount {get;}
		Partition GetPartition(int index);
	}

}
