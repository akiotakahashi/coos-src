#pragma once

#include "storage.h"


namespace FAT {

	extern void Initialize();
	extern void Finalize();

	extern bool IsSuitableMedia(const byte* data);
	extern IFileSystemSP AttachMedia(IMedia* media);

}
