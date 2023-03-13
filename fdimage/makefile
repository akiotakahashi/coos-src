#---------------------------------------------------
#	CLIOS	makefile
#---------------------------------------------------

cygwin		= C:\cygwin\bin
as		= $(cygwin)\nasm.exe
disas		= $(cygwin)\ndisasm.exe
strip		= $(cygwin)\strip.exe
cat		= $(cygwin)\cat.exe

# for nasm
ASFLAGS		= -f bin

#---------------------------------------------------
#	suffix definitions
#---------------------------------------------------

.SUFFIXES: .asm .bin .img

#---------------------------------------------------
# コンパイルするファイルを定義
#---------------------------------------------------

target = fdimage.img
kernel = kernel.img
knlasm = kernel.asm
bin = bootloader.bin bootkernel.bin fat.bin rootdirectory.bin
drv = ..\driver\Release\driver.dll
asm = $(bin:.bin=.asm)

#---------------------------------------------------
# 生成規則
#---------------------------------------------------

all: $(target)

$(target): $(bin) $(kernel) $(drv) makefile ..\bootloader\bootloader.img
	-vfd.bat close
	copy /b /y ..\bootloader\bootloader.img + fat.bin + rootdirectory.bin $(target)
	fixsize.exe 1474560 $(target)
	vfd.bat open fdimage.img /1.44 /w
	copy $(kernel) b: > nul
	copy $(drv) b: > nul
	attrib +h +s +r b:\$(kernel)
	$(cygwin)\sleep 1s
	vfd.bat close

#$(kernel): ..\kernel\$(kernel:.img=.dll) makefile
#	$(strip) -O binary ..\kernel\$(kernel:.img=.dll) -o $@
#	$(disas) -b 32 $@ > $(@:.img=.asm)

$(kernel): Z:\coos\kernel.img
	copy /y $< $@

$(bin): $(@:.bin=.asm)

#---------------------------------------------------
#	suffix rules
#---------------------------------------------------

.asm.bin:
	$(as) $(ASFLAGS) -o $@ $<

#---------------------------------------------------
# クリア
#---------------------------------------------------

clean:
	-vfd.bat close
	-del $(bin) > nul
	-del $(kernel) > nul
	-del $(target) > nul
