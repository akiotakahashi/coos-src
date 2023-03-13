REM @ECHO OFF
SET QEMU=D:\\Miscellaneous\\qemu-0.9.1-windows
start %QEMU%\qemu.exe -L %QEMU% -m 64 -boot d -cdrom %1\\coos.iso -fda D:\\Repository\\clios\\fdimage\\fdimage.img
