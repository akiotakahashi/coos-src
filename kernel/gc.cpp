#include "gc.h"
#include "stdlib.h"


struct RawHandle;

struct Mark {
	RawHandle* handle;
	size_t size;
	bool fixed;
	bool used;
	//bool scanned;
	Mark() {
		fixed = false;
		used = false;
		//scanned = false;
	}
};

struct RawHandle {
	Mark* ptr;
	RawHandle(Mark* p) {
		ptr = p;
	}
};

namespace GC {

	/*
	Engine::Engine(void* base, size_t size) {
		membase = (byte*)base;
		memsize = size;
		marker = membase;
	}

	void Engine::Collect() {
		byte* p = membase;
		int offset = 0;
		while(p<marker) {
			Mark* q = reinterpret_cast<Mark*>(p);
			if(q->handle==NULL) {
				offset += q->size;
			} else {
				if(q->fixed) {
					// ŒÅ’è—Ìˆæ‚Í¢‚é‚È‚ B
					panic("fixed memory is detected");
				}
				if(offset>0) {
					q->handle->ptr = (Mark*)(p-offset);
					memmove(q->handle->ptr, q, q->size);
				}
			}
			p += sizeof(Mark)+q->size;
		}
		marker -= offset;
	}

	Handle Engine::Allocate(size_t size) {
		size += sizeof(Mark);
		Mark* pmark = reinterpret_cast<Mark*>(marker);
		memclr(pmark, size);
		pmark->size = size;
		marker += size;
		RawHandle& ptr = *new RawHandle(pmark);
		pmark->handle = &ptr;
		return Handle((int)&ptr);
	}
	*/

}
