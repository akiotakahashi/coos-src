#pragma once

#include "stdlib.h"
#include "memory.h"

#define _STLP_NO_IOSTREAMS			1
#define _STLP_NO_EXCEPTIONS			1
#define _STLP_NO_EXCEPTION_SPEC		1
#define _STLP_NO_THREADS			1

#define BOOST_DISABLE_THREADS		1

#define __declspec(dllimport)

#include <memory>
#include <string>
#include <vector>
#include <deque>
#include <stack>
#include <list>
#include <map>

#include <iterator>
#include <functional>
#include <algorithm>

#include <boost/smart_ptr.hpp>

#undef __declspec


inline static __declspec(noreturn) void panic(const std::string& msg) { panic(msg.c_str()); }
inline static __declspec(noreturn) void panic(const std::wstring& msg) { panic(msg.c_str()); }

inline static int stoi(const std::string& s) { return stoi(s.c_str()); }
inline static int stoi(const std::wstring& s) { return stoi(s.c_str()); }


template < typename char_t, int base, typename num_t >
std::basic_string<char_t> itos(num_t n, int digits = 0, char_t ch = ' ') {
	typedef std::basic_string<char_t> string;
	char_t buf[32];
	char_t* p = buf;
	bool minus = false;
	if(n==0) {
		*(p++) = '0';
	} else {
		unsigned long long N;
		if(n<0) {
			minus = true;
#pragma warning(disable: 4146)
			N = -n;
#pragma warning(default: 4146)
		} else {
			N = n;
		}
		while((unsigned long long)N>0) {
			*(p++) = (char)('0'+(N%base));
			if(p[-1]>'9') p[-1]+='A'-('9'+1);
			N /= base;
		}
	}
	digits -= p-buf;
	if(minus) {
		digits--;
		*(p++) = '-';
	}
	while((digits--)>0) {
		*(p++) = ch;
	}
	*p = '\0';
	return string(std::reverse_iterator<char_t*>(p),std::reverse_iterator<char_t*>(buf));
}


static inline std::wstring a2w(const std::string& s) {
	return std::wstring(s.begin(),s.end());
}
