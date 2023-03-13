#include "kernel.h"
#include "pe.h"
#include "metadata.h"
#include "console.h"
#include "utility.h"

#pragma warning(disable: 4018)


namespace PE {

	const byte* Section::getData() const {
		return image.getImageBase()+virtualAddress;
	}

	Image::Image(const void* p) {
		Console& console = getConsole();
		const byte* data = (const byte*)p;

		const Raw::Header& header = *reinterpret_cast<const Raw::Header*>(data+*(int*)(data+0x3c)+4);
		this->header = &header;
		/*
		console << "num of sec: " << header.numberOfSections << endl;
		console << "size of opt hdr: " << header.sizeOfOptionalHeader << endl;
		console << "characteristics: " << header.characteristics << endl;
		//*/

		const Raw::OptionalHeader& opthdr = *reinterpret_cast<const Raw::OptionalHeader*>(&header+1);
		this->optionalHeader = &opthdr;
		/*
		console << "size of image: " << opthdr.sizeOfImage << endl;
		for(int i=0; i<static_cast<int>(opthdr.numberOfRvaAndSizes); ++i) {
			console << "dde: " << opthdr.dataDirectory[i].rva << " (" << opthdr.dataDirectory[i].size << ")\r\n";
		}
		//*/

		image = NULL;
		image = new byte[opthdr.sizeOfImage];
		//console << "image: " << (void*)image << endl;

		const Raw::SectionHeader* sections = reinterpret_cast<const Raw::SectionHeader*>((byte*)(&header+1)+header.sizeOfOptionalHeader);
		for(int i=0; i<header.numberOfSections; ++i) {
			const Raw::SectionHeader& section = sections[i];
			this->sections.push_back(Section(*this, section));
			/*
			console << "name: " << section.name << endl;
			console << "virtual size: " << section.virtualSize << endl;
			console << "virtual address: " << section.virtualAddress << endl;
			console << "size of raw data: " << section.sizeOfRawData << endl;
			console << "ptr to raw data: " << section.pointerToRawData << endl;
			*/
			memcpy(image+section.virtualAddress, data+section.pointerToRawData, section.sizeOfRawData);
		}

		const Raw::DataDirectory& dd = opthdr.dataDirectory[IMAGE_DDE_BASERELOCATION];
		const byte* preloc = image+dd.rva;
		while(preloc-(image+dd.rva) < dd.size) {
			const Raw::FixupBlock* fb = reinterpret_cast<const Raw::FixupBlock*>(preloc);
			int c = (fb->size-sizeof(*fb)) / sizeof(Raw::BlockSize);
			for(int i=0; i<c; ++i) {
				const Raw::BlockSize* bs = reinterpret_cast<const Raw::BlockSize*>(fb+1)+i;
				if(bs->type==0x3) {
					DWORD* p = reinterpret_cast<DWORD*>(image+fb->rva+bs->offset);
					//console << "rva:" << "[" << p << "] " <<fb->rva << " offset:" << bs->offset << " mem:" << *p;
					*p += (DWORD)image-opthdr.imageBase;
					//console << " -> " << *p << endl;
				}
			}
			preloc += fb->size;
		}

	}

	Image::~Image() {
		delete [] image;
	}

	const Section& Image::getSection(const char* name) const {
		for(int i=0; i<sections.size(); ++i) {
			const Section& section = sections[i];
			if(streql(name,section.name)) return section;
		}
		panic("PE Section not found");
	}

}
