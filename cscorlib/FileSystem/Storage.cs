using System;

namespace CooS.FileSystem {

	/// <summary>
	/// Storage �� Drive, Disk, FlushMemory �Ȃǂ̏��i�[�}�̂�\���܂��B 
	/// </summary>
	public interface Storage {
		string PrefixName {get;}
		//int BlockSize {get;}
		//long BlockCount {get;}
		int PartitionCount {get;}
		Partition GetPartition(int index);
	}

}
