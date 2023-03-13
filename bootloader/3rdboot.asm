[ORG 0x00001000]

;--------------------------------------------------------
; �v���e�N�g���[�h�ֈڍs
;--------------------------------------------------------
[BITS 16]
enter_protectmode:
	mov	si, ax		; AX = segment of kernel.img
	mov	ax, cs
	mov	ds, ax
	mov	es, ax
	cli
	lgdt	[gdtr]
	mov	eax, cr0
	or	eax, 1
	mov	cr0, eax	; �v���e�N�g���[�h��
	jmp	near far_jump
far_jump:
	jmp	dword 0x08:on_protectmode

;********************************************************
; 32 bit �R�[�h
;********************************************************

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
	and	esi, 0x0000FFFF		; SI = segment of kernel.img
	shl	esi, 4
	mov	edi, 0x00101000
	mov	ecx, [0x06F8]
	cld
	rep	movsb
	; 1ch
	mov	eax, [lastmsg]
	mov	[0xb8000], eax
	;------------------------------------------------
	; CR4�̑���������ł���
	; ����ɂ����SSE/SSE2���������
	; ���Ή���CPU�ł��ꂷ��Ƃǂ��Ȃ邩�͕s��
	mov	eax, cr4
	or	eax, 0x200
	mov	cr4, eax
	;------------------------------------------------
	; �J�[�l���ɃW�����v
;	jmp	stop
	lss	esp, [stack]
	jmp	0x8:0x101000

stack	dd	0x00400000
	dw	0x0018

;--------------------------------------------------------
; �����̒�~
;--------------------------------------------------------
stop:
	cli
forever:
	hlt
	jmp	short forever

;--------------------------------------------------------
;	GDT definitions
;--------------------------------------------------------
gdtr:
	dw	gdt_end-gdt0-1  	; gdt limit
	dd	gdt0

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

gdt_end:			; end of gdt

;--------------------------------------------------------
;	IDT definitions
;--------------------------------------------------------
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
%rep	2
	makeidt	i
%assign	i i+1
%endrep
idt_end:

;--------------------------------------------------------
;	Interrupt Handlers
;--------------------------------------------------------

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
%rep	2
	makeig	i
%assign	i i+1
%endrep

;--------------------------------------------------------
;	���b�Z�[�W
;--------------------------------------------------------

lastmsg		db	'>', 0x0F
		db	'-', 0x0F

times 512 - ($-$$) db 0
