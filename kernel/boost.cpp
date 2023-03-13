#include "stl.h"


type_info::~type_info() {
}

void boost::throw_exception(std::exception const & e) {
	panic("boost::throw_exception");
}

bool type_info::operator==(const type_info& rhs) const {
	panic("type_info::operator==");
}

#pragma message("  BOOST_COMPILER: " BOOST_COMPILER)
#pragma message("  BOOST_PLATFORM: " BOOST_PLATFORM)
#pragma message("  BOOST_STDLIB  : " BOOST_STDLIB)
