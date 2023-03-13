using System;

namespace CooS.CodeModels.Java.Metadata {

	public enum ConstantTag : byte {
		Class				= 7,
		Fieldref			= 9,
		Methodref			= 10,
		InterfaceMethodref	= 11,
		String				= 8,
		Integer				= 3,
		Float				= 4,
		Long				= 5,
		Double				= 6,
		NameAndType			= 12,  
		Utf8				= 1,
	}

}
