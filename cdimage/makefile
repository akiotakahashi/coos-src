#---------------------------------------------------
#	CooS makefile
#---------------------------------------------------

target = $(TargetDir)\..\coos.iso
fdimg = ..\fdimage\fdimage.img
brg = ..\csbridge\Release\csbridge.dll
krl = ..\cskorlib\Release\cskorlib.dll
cor = ..\cscorlib\bin\Release\cscorlib.dll
grx = ..\csgraphics\bin\Release\csgraphics.dll
asm = ..\ia32assembler\bin\Release\ia32assembler.dll
ft2 = ..\freetype2\Release\freetype2.dll
drv = ..\driver\Release\driver.dll
app = ..\application\bin\Release\application.exe
doc = ..\cdimage\README.html
lic = ..\cdimage\LICENSE.html

#---------------------------------------------------
# ê∂ê¨ãKë•
#---------------------------------------------------

$(target): $(fdimg) $(brg) $(krl) $(cor) $(grx) $(asm) $(ft2) $(drv) $(app) $(doc) $(lic) makefile
	-mkdir $(TargetDir)
	copy /y $(fdimg) $(TargetDir)\bootldr.img > nul
	copy /y mscorlib.dll $(TargetDir) > nul
	copy /y System.dll $(TargetDir) > nul
	copy /y System.Drawing.dll $(TargetDir) > nul
	copy /y $(brg) $(TargetDir) > nul
	copy /y $(krl) $(TargetDir) > nul
	copy /y $(cor) $(TargetDir) > nul
	copy /y $(grx) $(TargetDir) > nul
	copy /y $(asm) $(TargetDir) > nul
	copy /y $(ft2) $(TargetDir) > nul
	copy /y $(drv) $(TargetDir) > nul
	copy /y $(app) $(TargetDir) > nul
	copy /y $(doc) $(TargetDir) > nul
	copy /y $(lic) $(TargetDir) > nul
	xcopy /d /e /i /y ..\resource $(TargetDir) > nul
	xcopy /d /e /i /y licenses $(TargetDir)\licenses > nul
	D:\Miscellaneous\cdrtools-1.11a12-win32-bin\mkisofs.exe -verbose -iso-level 3 -eltorito-boot bootldr.img -o $(target) $(TargetDir)

#---------------------------------------------------
# ÉNÉäÉA
#---------------------------------------------------

clean:
	-del $(target)
	-rmdir /S /Q $(TargetDir)

all: clean $(target)
