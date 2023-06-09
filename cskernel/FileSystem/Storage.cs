using System;

namespace CooS.FileSystem {

	/// <summary>
	/// Storage は Drive, Disk, FlushMemory などの情報格納媒体を表します。 
	/// </summary>
	public interface Storage {
		string PrefixName {get;}
		//int BlockSize {get;}
		//long BlockCount {get;}
		int PartitionCount {get;}
		Partition GetPartition(int index);
	}

}
