#include <ft2build.h>
#include FT_FREETYPE_H
#include "freetype2.h"
#include <memory.h>

#pragma unmanaged
#include "../kernel/stdtype.h"
#pragma managed

using namespace System;
using namespace System::Drawing;


#include "include/freetype/internal/ftobjs.h"

// for GZIP library required by FreeType
extern "C" extern int z_verbose = 0;
extern "C" extern FILE _iob[3] = {};

extern "C" extern void z_error(char* msg) {
	abort();
}

extern "C" extern int fprintf(FILE*, const char*, ...) {
	abort();
}


namespace FreeType {

	public __delegate void FontDrawHandler(int x, int y, int level);

	public __gc class FreeTypeException : public SystemException {
	public:
		FreeTypeException(int error) : SystemException(String::Format("FreeType returns an error: 0x{0:X}", __box(error))) {
		}
	};

	public __value struct FontBitmap {
		IntPtr Buffer;
		int Width;
		int Height;
		int ScanlineSize;
	};

	public __gc class FontData {

		FT_Library	freetype;	// handle to library
		FT_Face		face;		// handle to face object
		FontDrawHandler* painter;

	public:

		FontData(byte fontdata __gc[]) {
			FT_Library	freetype;
			int error = FT_Init_FreeType(&freetype);
			if(error) throw new FreeTypeException(error);
			this->freetype = freetype;
			FT_Face		face;
			byte __pin* pfont = &fontdata[0];
			error = FT_New_Memory_Face(this->freetype,
				pfont, fontdata->Length,
				0 /*face_index*/, &face);
			pfont = NULL;
			this->face = face;
			SetSizeByPixel(0, 16);
		}

	public:

		__property int get_Height() {
			return face->height;
		}

		__property String* get_FamilyName() {
			return new String(face->family_name);
		}

	public:

		void SetSizeByPoint(double width, double height) {
			SetSizeByPoint(width, height, 96, 96);
		}

		void SetSizeByPoint(double width, double height, int resX, int resY) {
			int error = FT_Set_Char_Size(
				face,				/* handle to face object           */
				(int)(width*64),	/* char_width in 1/64th of points  */
				(int)(height*64),	/* char_height in 1/64th of points */
				resX,				/* horizontal device resolution    */
				resY);				/* vertical device resolution      */
			if(error) throw new FreeTypeException(error);
		}

		void SetSizeByPixel(int width, int height) {
			int error = FT_Set_Pixel_Sizes(
				face,		// handle to face object
				width,		// pixel_width
				height);	// pixel_height
			if(error) throw new FreeTypeException(error);
		}

		void SetPainter(FontDrawHandler* fp) {
			this->painter = fp;
		}

	private:

		void PrepareBitmap() {
			if(face->glyph->format!=FT_GLYPH_FORMAT_BITMAP) {
				int error = FT_Render_Glyph(face->glyph, FT_RENDER_MODE_NORMAL);
				if(error) throw new FreeTypeException(error);
			}
		}

	public:

		void LoadGlyph(wchar_t ch) {
			// load glyph image into the slot (erase previous one)
			int error = FT_Load_Char(face, ch, FT_LOAD_DEFAULT);
			if(error) throw new FreeTypeException(error);
		}

		__property Size get_AdvanceSize() {
			return Size(face->glyph->advance.x, face->glyph->advance.y);
		}

		__property Size get_BitmapSize() {
			PrepareBitmap();
			return Size(face->glyph->bitmap.width, face->glyph->bitmap.rows);
		}

		__property Size get_BearingSize() {
			PrepareBitmap();
			return Size(face->glyph->bitmap_left, face->glyph->bitmap_top);
		}

		void DrawGlyph() {
			PrepareBitmap();
			FT_Bitmap bitmap = face->glyph->bitmap;
			unsigned char* ppixel = bitmap.buffer;
			for(int y=0; y<bitmap.rows; ++y) {
				for(int x=0; x<bitmap.width; ++x) {
					this->painter(x, y, *(ppixel+x));
				}
				ppixel += bitmap.pitch;
			}
		}

		/*
		void DrawGlyph(FontBitmap bitmap) {
			FT_Bitmap bmp;
			bmp.pixel_mode = FT_PIXEL_MODE_GRAY;
			bmp.num_grays = 256;
			bmp.width = bitmap.Width;
			bmp.rows = bitmap.Height;
			bmp.pitch = bitmap.ScanlineSize;
			bmp.buffer = (byte*)bitmap.Buffer.ToPointer();
			memset(bmp.buffer, 0, bmp.rows*bmp.pitch);
			int error = FT_Get_Glyph_Bitmap(face, &bmp);
			if(error) throw new FreeTypeException(error);
		}
		*/

	};

}

using namespace System::Runtime::InteropServices;

extern "C" extern void* malloc(size_t size) {
	unsigned char buf __gc[] = new unsigned char __gc[size+sizeof(__int32)];
	GCHandle handle = GCHandle::Alloc(buf, GCHandleType::Pinned);
	void* p = handle.AddrOfPinnedObject().ToPointer();
	*(void**)p = GCHandle::op_Explicit(handle).ToPointer();
	return (void*)((char*)p+sizeof(void*));
}

extern "C" extern void free(void* p) {
	void* handle = *(void**)((char*)p-sizeof(void*));
	GCHandle::op_Explicit(handle).Free();
}

extern "C" extern void* realloc(void* p, size_t size) {
	void* q = malloc(size);
	if(p!=NULL) {
		void* ph = *(void**)((char*)p-sizeof(void*));
		GCHandle handle = GCHandle::op_Explicit(ph);
		unsigned char buf __gc[] = (unsigned char __gc[])handle.Target;
		if((int)size>buf->Length-sizeof(void*)) size=buf->Length-sizeof(void*);
		memcpy(q, p, size);
		free(p);
	}
	return q;
}
