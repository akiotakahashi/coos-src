#pragma once

#include "stdtype.h"
#include "atapi-device.h"
#include "atapi-controller.h"



namespace Atapi {
	
	extern bool Initialize();
	extern IMedia* getDevice(int index);
	extern DeviceType getDeviceType(int index);
	extern MediaType getMediaType(int index);

}
