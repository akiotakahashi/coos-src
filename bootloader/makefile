#---------------------------------------------------
#	CLIOS	makefile
#---------------------------------------------------

PATH	= $(PATH);C:\osdev\tool\lsic\BIN

#---------------------------------------------------
# ¶¬‹K‘¥
#---------------------------------------------------

all: bootloader.img

bootloader.img: makefile 1stboot.img 2ndboot.img 3rdboot.bin
	copy /b /y bpb.bin + 1stboot.img + 3rdboot.bin + 2ndboot.img bootloader.img

1stboot.img: 1stboot.obj
	lld -M -T 7C60 -TDATA 7DC0 -o 1stboot.com 1stboot.obj
	copy /y 1stboot.com 1stboot.img > nul
	fixsize 0x1A0 1stboot.img 55 AA

2ndboot.img: 2ndboot.obj
	lld -M -T 1200 -TDATA 1C00 -o 2ndboot.com 2ndboot.obj
	copy /y 2ndboot.com 2ndboot.img > nul
	fixsize 0xC00 2ndboot.img

#---------------------------------------------------
#	suffix rules
#---------------------------------------------------

.c.obj:
	lcc -O -o $@ -c $<

#---------------------------------------------------
# ƒNƒŠƒA
#---------------------------------------------------

clean:
	-del /f *.img > nul
	-del /f *.com > nul
	-del /f *.obj > nul
