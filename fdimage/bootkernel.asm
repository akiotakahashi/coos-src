%include "define.mac"
%define BASE_SEGMENT_OF_KERNEL_IMAGE	0x1500
%define BASE_SEGMENT_OF_FAT		0x6000
%define MEMORY_SIZE_KB			0x1000
%define VESA_INFO			0x0800
%define VESA_INFO_DETAIL		0x0900

[ORG 0]
[BITS 16]
	jmp	start

; ���b�Z�[�W�\��
print:
	push	ax
	push	bx
.print:
	lodsb				; ds:si�̈ʒu����al�֒l�����o���BSI��+1�����
	cmp	al, 0			; AL��0��������I��
	je	printend
	mov	ah, 0eh 		; al�̕�����BIOS�ŕ\��
	mov	bx, 7
	int	10h
	jmp	.print			; ���̕�����\��
printend:
	pop	bx
	pop	ax
	ret

printn:
	pusha
	mov	si, 0
printn_loop:
	cmp	si, 8
	jae	printn_end
	push	eax
	shr	eax, 28
	cmp	ax, 10
	jae	printn_alphabet
	add	ax, '0'
	jmp	printn_next
printn_alphabet:
	sub	ax, 10
	add	ax, 'A'
printn_next:
	mov	ah, 0xE
	mov	bx, 7
	int	0x10
	pop	eax
	inc	si
	shl	eax, 4
	jmp	printn_loop
printn_end:
	popa
	ret

;-----------------------------------------------------------------------------------------
; �G���g���|�C���g
;-----------------------------------------------------------------------------------------
start:
	; �Z�O�����g���W�X�^�̏�����
	mov	ax, cs
	mov	ds, ax		; CS = DS = 0x10000
	; �J�[�\��������
	mov	ah, 0x01
	mov	ch, 0x20
	int	0x10
	; �����������T�C�Y�擾
	mov	ax, 0xE801
	int	0x15
	xor	bx, bx
	mov	es, bx
	and	edx, 0xffff
	and	ecx, 0xffff
	and	ebx, 0xffff
	and	eax, 0xffff
	shl	edx, 6
	mov	[es:MEMORY_SIZE_KB], edx	; store extended memory size
 	add	[es:MEMORY_SIZE_KB], ecx	; and add lower memory into total size.
	; ���[�h�V�[�P���X
	call	loadkernel	; �J�[�l�����[�h
	call	a20enable	; A20���C����L���ɂ���
	;call	enable_a20
	call	vesa_mode	; SVGA�Ɉڍs
	; �v���e�N�g���[�h�Ɉڍs
	jmp	enter_protectmode

;-----------------------------------------------------------------------------------------
; �J�[�l�����[�h
;-----------------------------------------------------------------------------------------
loadkernel:
	mov	si, loadmsg
	call	print		; ���b�Z�[�W�̕\��

	; �܂��̓��[�g�f�B���N�g���G���g����ǂݍ��݂܂��B
	xor	dx, dx
	mov	cx, dx
	mov	bx, dx
	mov	ax, BASE_SEGMENT_OF_KERNEL_IMAGE
	mov	es, ax
	mov	ax, SectorsPerFAT
	mov	cl, NumberOfFAT
	mul	cx
	add	ax, ReservedSectorCount
	mov	di, RootEntryCount
	shr	di, 4				 ; RootEntryCount / (BytesPerSector/32)
	add	di, ReservedSectorCount
	call	readsector

	; ���[�g�f�B���N�g���G���g���̒�����J�[�l���C���[�W��T���܂��B
	xor	di, di
	mov	bx, RootEntryCount
kernel_next:
	mov	si, bname
	mov	cx, 0x000b
	push	di
	rep	cmpsb
	pop	di
	je	kernel_found
	add	di, 32				; ���̃G���g���ֈړ����܂��B
	dec	bx				; �c�G���g���J�E���^�����炵�܂��B
	jnz	kernel_next
	jmp	file_not_found

	; ���[�g�f�B���N�g���G���g���̒��ɃJ�[�l���C���[�W�������܂����B
kernel_found:
	mov	ecx, [es:di+28]			; �t�@�C���T�C�Y(+28-4)��ۑ����Ă����܂��B
	mov	ax, 0
	mov	gs, ax
	mov	[gs:0x7c00], ecx
	push	word [es:di+26]			; �N���X�^�ʒu(+26-2)��ۑ����Ă����܂��B

	; FAT�̈��ǂݍ��݂܂��B
	mov	bx, 0				; ���̃A�h���X�ɓǂݍ��݂܂��B
	mov	ax, BASE_SEGMENT_OF_FAT
	mov	es, ax
	mov	ax, ReservedSectorCount		; �\��̈�̎���FAT�̈�ł��B
	mov	di, SectorsPerFAT
	call	readsector

	; �J�[�l���C���[�W��ǂݍ��݂܂��B
	mov	ax, BASE_SEGMENT_OF_KERNEL_IMAGE
	mov	es, ax
	mov	bx, 0
	pop	cx			; �N���X�^�ʒu�𕜋A���܂��B
kernel_load:
	mov	ax, cx			; �ǂݍ��ރN���X�^�ʒu�ł��B
	sub	ax, 2
	mov	si, SectorsPerCluster
	mul	si
	add	ax, FirstDataSector	; �ǂݍ��݃Z�N�^�ʒu�ł��B
	mov	di, SectorsPerCluster	; �P�N���X�^�ǂݍ��݂܂��B
	call	readsector
	mov	si, readmsg
	call	print
	; �㑱�̃N���X�^�����邩���ׂ܂��B
	push	bx
	mov	bx, cx
	call	get_fat
	pop	bx
	cmp	ax, 0x0ff8
	jge	end_of_kernel
	; �㑱�̃N���X�^��ǂݍ��݂܂��B
	mov	cx, ax
	add	bx, BytesPerSector
	jnc	kernel_load
	mov	si, overseg
	call	print
	mov	bx, es
	add	bh, 0x10
	mov	es, bx
	xor	bx, bx
	jmp	kernel_load
end_of_kernel:
	mov	si, cmplmsg
	call	print
	ret

; BX: �N���X�^�ԍ�
get_fat:
	mov	si, bx
	shr	si, 1
	mov	ax, BASE_SEGMENT_OF_FAT
	mov	gs, ax
	mov	ax, [gs:si+bx]
	jnc	get_fat_with_even_cluster
	; �ǂݍ��ނ̂͊�N���X�^�G���g���ł��B
	shr	ax, 4
get_fat_with_even_cluster:
	and	ah, 0x0f
	ret

file_not_found:
	; �J�[�l���C���[�W���f�B���N�g���G���g���̒��Ɍ�����܂���ł����B
	mov	si, not_found
	call	print
	jmp	stop

; readsector
;   ax = start sector, di = number of sectors to read
;   es:bx = read address (es = 64kb align, bx =	512 bytes align)
readsector:
	pusha
	push	es
	;
_read0:	mov	si, bx
	neg	si
	dec	si
	shr	si, 9
	inc	si
	cmp	si, di
	jbe	_read1
	mov	si, di		; di < si
_read1:	push	ax
	xor	dx, dx
	mov	cx, 0x0024
	div	cx		; ax = track number
	xchg	dx, ax
	mov	cl, 0x12
	div	cl		; ah = sector number, al = head	number
	sub	cl, ah
	cmp	cx, si
	jbe	.read1
	mov	cx, si		; si < cx
.read1:	mov	ch, dl		; ch = track number
	mov	dh, al		; dh = head number
	mov	al, cl		; al = number of sectors to read
	mov	cl, ah		; cl = sector number
	inc	cl
	xor	dl, dl		; dl = drive number
	mov	ah, 0x02
	int	0x13
	jc	_read_error
	mov	dx, ax
	mov	cl, 0x09
	shl	ax, cl
	add	bx, ax
	pop	ax
	add	ax, dx
	sub	di, dx
	sub	si, dx
	jnz	_read1
	mov	cx, es
	add	ch, 0x10
	mov	es, cx
	xor	bx, bx
	or	di, di
	jnz	_read0
	;
	pop	es
	popa
	ret

_read_error:
	xor	ax, ax
	xor	dl, dl
	int	0x13
	pop	ax
	jmp	_read1

;-----------------------------------------------------------------------------------------
; SVGA�ɐ؂�ւ�
;-----------------------------------------------------------------------------------------
vesa_mode:
	xor	ax, ax
	mov	es, ax
	mov	gs, ax
	mov	di, VESA_INFO		; 0x0000:VESA_INFO
	mov	ax, 0x4F00		; function 00h
	int	0x10
	cmp	ax, 0x004F
	jne	vesa_fail
vesa_search_mode:
	mov	si, vesa_mode_list
vesa_try_mode:
	mov	cx, [cs:si]
	cmp	cx, 0xFFFF
	je	vesa_fail
vesa_get_mode:
	push	si
	mov	si, testmodemsg
	call	print
	pop	si
	mov	ax, 0x4F01		; function 01h
	mov	di, VESA_INFO_DETAIL	; 0x0000:VESA_INFO_DETAIL
	int	0x10
	cmp	ax, 0x004F
	jne	vesa_next_mode
vesa_check_mode:
	mov	ax, [cs:vesares]
	cmp	ax, [es:VESA_INFO_DETAIL+0x12]
	ja	vesa_next_mode
	mov	ax, [cs:vesabpp]
	cmp	al, [es:VESA_INFO_DETAIL+0x19]
	ja	vesa_next_mode
vesa_switch_mode:
;************************************************
; ����2�s��L��������ƃe�L�X�g���[�h�ŋN�����܂�
;------------------------------------------------
;	mov	dword [es:VESA_INFO_DETAIL+0x28], 0
;	ret
;************************************************
	mov	ax, 0x4F02		; functon 02h
	mov	bx, cx			; set video mode
	or	bx, 0x4000		; set linear mode
	int	0x10
	cmp	ax, 0x004F
	jne	vesa_next_mode
	ret
vesa_next_mode:
	add	si, 2
	jmp	vesa_try_mode
vesa_fail:
	mov	si, vesa_fail_msg
	call	print
	jmp	stop

;-----------------------------------------------------------------------------------------
; A-20���C����L���ɂ���
;-----------------------------------------------------------------------------------------
a20enable:
	mov	si, a20msg
	call	print
a20enable_0:
        in      al,0x64
        test    al,0x02
        jnz     a20enable_0
        mov     al,0xD1
        out     0x64,al
a20enable_1:
        in      al,0x64
        test    al,0x02
        jnz     a20enable_1    ;wait every KBC cmd.
        mov     al,0xDF
        out     0x60,al
a20enable_2:
        in      al,0x64
        test    al,0x02
        jnz     a20enable_2
        mov     al,0xFF
        out     0x64,al
a20enable_3:
        in      al,0x64
        test    al,0x02
        jnz     a20enable_3

	mov	si, cmplmsg
	call	print
	ret

;---------------------------------

enable_a20:
	call	wait8042
	mov	al, 0xd1
	out	064h, al		; ����f�[�^�𑗂邱�Ƃ�m�点��
	call	wait8042
	mov	al, 0xdf
	out	060h, al		; A20���C����L��
	call	wait8042
	mov	al, 0xff
	out	064h, al		; A20���C����L��
	call	wait8042
        ret
; 8042�𒲂ׂăL�[�{�[�h�o�b�t�@����ɂȂ�̂�҂�
wait8042:
	mov	cx, 0ffffh
waitloop:
	;jmp	$+2
	in	al, 064h 		; 64h��ǂݍ���
;	test	al, 1			; bit0==1?(�o�̓o�b�t�@�͋�)
;	jz	checkinbuf		; �����Ȃ���̓o�b�t�@���m�F
	;jmp	$+2
;	in	al, 060h 		; �ǂݍ���œ��̓o�b�t�@���N���A
;	jmp	checkloop
;checkinbuf:
	test	al, 2			; bit1==1?(���̓o�b�t�@�͋�)
	jz	wait8042_end 		; �����Ȃ烋�[�v�I��
	in	al, 060h 		; �ǂݍ���œ��̓o�b�t�@���N���A
checkloop:
	loop	waitloop		; cx-=1����0�ȏ�Ȃ�waitloop��
wait8042_end:
	ret

;-----------------------------------------------------------------------------------------
; �v���e�N�g���[�h�ֈڍs
;-----------------------------------------------------------------------------------------
enter_protectmode:
	mov	si, modemsg
	call	print
	mov	ax, cs			; �Z�O�����g���W�X�^�̏�����
	mov	ds, ax			; CS = DS
	cli 				; ���荞�݂��~
	lgdt	[gdtr] 			; GDT�̐ݒ�
	mov	eax, cr0
	or	eax, 1
	mov	cr0, eax		; PE�r�b�g��1�ɃZ�b�g���ăv���e�N�g���[�h��
	jmp	near far_jump
far_jump:
	jmp	dword 0x08:on_protectmode+0x10000

;-----------------------------------------------------------------------------------------
; �����̒�~
;-----------------------------------------------------------------------------------------
stop:
	cli
forever:
	hlt
	jmp	short forever

;*****************************************************************************************
; 32 bit �R�[�h
;*****************************************************************************************

[bits 32]
on_protectmode:
	mov	ax, 0x10		; ds & es selector
	mov	ds, ax			; is 0x10
	mov	es, ax			;
	mov	ax, 0x18		; ss selector
	mov	ss, ax			; is 0x18
	mov	ax, 0x00		; null gdt
	mov	fs, ax
	mov	gs, ax
	lidt	[idtr+0x10000]		; IDT�̐ݒ�
	mov	esp, 1024*1024*4	; sp is 4MB
	; �J�[�l���R�[�h�̃R�s�[
	mov	eax, BASE_SEGMENT_OF_KERNEL_IMAGE
	shl	eax, 4
	mov	esi, eax
	mov	eax, 0x101000
	mov	edi, eax
	mov	eax, [0x7c00]
	mov	ecx, eax
	cld
	rep	movsb
	; 1ch
	mov	eax, [lastmsg+0x10000]
	mov	[0xb8000], eax
	; �J�[�l���ɃW�����v
;	jmp	stop
	lss	esp, [stack+0x10000]
	jmp	0x8:0x101000

stack	dd	0x00400000
	dw	0x0018

;-------------------------------------------------------------------------------
;	GDT definitions
;-------------------------------------------------------------------------------
gdtr:
	dw	gdt_end-gdt0-1  	; gdt limit
	dd	gdt0+0x00010000

gdt0:			; segment 00
	dw 0			; segment limitL
	dw 0			; segment baseL
	db 0			; segment baseM
	db 0			; segment type
	db 0			; segment limitH, etc.
	db 0			; segment baseH
gdt08:			; segment 08(code segment)
	dw 0xffff		; segment limitL
	dw 0x0000		; segment baseL
	db 0			; segment baseM
	db 0x9a 		; Type Code
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt10:			; segment 10(data segment)
	dw 0xffff		; segment limitL
	dw 0x0000		; segment baseL
	db 0			; segment baseM
	db 0x92 		; Type Data
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt18:			; segment 18(stack segment)
	dw 0x20			; segment limitL (2MB)
	dw 0x0000		; segment baseL
	db 0			; segment baseM
	db 0x96 		; Type Stack
	db 0xC0 		; segment limitH, etc.
	db 0			; segment baseH
gdt20:			; VESA DataArea data segment
	dw 0xffff		; segment limitL
	dw 0x7c00		; segment baseL
	db 0			; segment baseM
	db 0x92 		; Type Data
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt28:			; VESA 0xA0000 data segment
	dw 0xffff		; segment limitL
	dw 0x0000		; segment baseL
	db 0x0A			; segment baseM
	db 0x92 		; Type Data
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt30:			; VESA 0xB0000 data segment
	dw 0xffff		; segment limitL
	dw 0x0000		; segment baseL
	db 0x0B			; segment baseM
	db 0x92 		; Type Data
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt38:			; VESA 0xB8000 data segment
	dw 0xffff		; segment limitL
	dw 0x8000		; segment baseL
	db 0x0B			; segment baseM
	db 0x92 		; Type Data
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt40:			; VESA loaded BIOS image data segment
	dw 0xffff		; segment limitL
	dw 0x1000		; segment baseL
	db 0x30			; segment baseM
	db 0x92 		; Type Data
	db 0xdf 		; segment limitH, etc.
	db 0			; segment baseH
gdt48:			; VESA 16bit code segment
	dw 0xffff		; segment limitL
	dw 0x0000		; segment baseL
	db 0			; segment baseM
	db 0x9a 		; Type Code
	db 0x9F 		; segment limitH, etc. (16bit)
	db 0			; segment baseH

gdt_end:			; end of gdt

;-------------------------------------------------------------------------------
;	IDT definitions
;-------------------------------------------------------------------------------
idtr:
	dw	idt_end-idt_begin-1	; IDT�̃T�C�Y
	dd	0x10000+idt_begin	; IDT�̃A�h���X

%macro	makeidt	1
	dw	ig%1
	dw	0x08
	db	0
	db	2+4+8+0+0x80
	dw	0x0001
%endmacro

idt_begin:
%assign	i 0
%rep	20
	makeidt	i
%assign	i i+1
%endrep
idt_end:

;-------------------------------------------------------------------------------
;	Interrupt Handlers
;-------------------------------------------------------------------------------

%macro	makeig	1
ig%1:
	mov	ebx, eax
	mov	eax, %1
	sti
igb%1:
	;hlt
	jmp	igb%1
	iret
%endmacro

%assign	i 0
%rep	20
	makeig	i
%assign	i i+1
%endrep

;-------------------------------------------------------------------------------
;	���b�Z�[�W
;-------------------------------------------------------------------------------

bname		db      "KERNEL  IMG"
cmplmsg		db	'OK', 0x0d, 0x0a, 0
loadmsg		db	'loading kernel', 0x00
readmsg		db	'.', 0
overseg		db	':', 0
not_found	db	"kernel image was not found", 0x00
a20msg		db	'a20 line enable...', 0
testmodemsg	db	'Testing VESA mode', 0x0d, 0x0a, 0
modemsg		db	'entering protect mode', 0
lastmsg		db	'>', 0x0F
		db	'-', 0x0F
vesa_mode_list	dw	0x118	; 1024x768x24
		dw	0x115	; 800x600x24
		dw	0x11b	; 1280x1024x24
		dw	0x141	; VMware 1024x768x24
		dw	0x142	; VMware 1152x864x24
		dw	0x140	; VMware 800x600x24
		dw	0x143	; VMware 1280x1024x24
		dw	0xFFFF	; end of list
vesa_fail_msg	db	'FATAL ERROR with VESA VBE', 0

lenvres dw      16              ; length of strvres
lenvbpp dw      9               ; length of strvbpp
vesares dw      800
vesabpp dw      24

times 512*4 - ($-$$) db 0
