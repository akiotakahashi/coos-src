#pragma once

#include "stdtype.h"
#include "storage.h"


namespace Iso9660 {

	extern bool Initialize();
	extern IFileSystemSP AttachMedia(IMedia* media);

}
