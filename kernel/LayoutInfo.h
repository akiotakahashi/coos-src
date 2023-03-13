#pragma once

#include "stl.h"


struct LayoutInfo {
	std::deque<int> sizes;
	std::deque<int> offsets;
	int totalsize;
public:
	LayoutInfo();
	LayoutInfo(int capacity);
};
