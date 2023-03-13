%include "define.mac"

[bits 16]
[org 0]
	jmp	short start
	nop

; boot parameter block(bpb)
oem	db	"CLIOS   "
bps	dw	BytesPerSector			; Bytes per Sector
spc	db	SectorsPerCluster		; Sectors per Cluster
rsc	dw	ReservedSectorCount		; Reserved sector count: spec dictates this should be 1.
nof	db	NumberOfFAT			; Number of FAT
rec	dw	RootEntryCount			; Root Entry Count
nos	dw	NumberOfSectors			; Total sector count
med	db	0xf0				; MediaType
spf	dw	SectorsPerFAT			; Sectors per FAT
spt	dw	SectorsPerTrack			; Sectors per Track
noh	dw	NumberOfHeads			; Number of heads
	dd	0
	dd	0
	db	0
	db	0
	db	0x29
	dd	0x00000000
	db	"clios boot "
	db	"FAT12   "

; ���b�Z�[�W�\��
print:
	lodsb	    			; ds:si�̈ʒu����al�֒l�����o���BSI��+1�����
	cmp	al, 0			; AL��0��������I��
	je	printend
	mov	ah, 0eh			; al�̕�����BIOS�ŕ\��
	mov	bx, 7
	int	10h
	jmp	print			; ���̕�����\��
printend:
	ret

; �G���[����
error:
	mov	si, errormsg
	call	print
	ret


; �u�[�g���[�_�[�X�^�[�g
start:
	jmp	0x07c0:refresh_cs
refresh_cs:
	mov	ax, cs			; �Z�O�����g���W�X�^�̏�����
	mov	ds, ax
	mov	es, ax			; CS = ES = DS = 07C0h
	mov	ax, 0x4000
	mov	ss, ax
	mov	ax, 0x8000
	mov	sp, ax
	; �X�^�[�g���b�Z�[�W�̕\��
	mov	si, startmsg
	call	print

; FDC���Z�b�g
fdc_reset:
	mov	ax, 0
	mov	dl, 0			; Drive=0(A�h���C�u/FDD)���w��
	int	13h
	jc	fdc_reset		; �G���[���N������fdc_reset�ֈڂ��Ă�����x

; �J�[�l�����[�_�̓ǂݍ���
	mov	si, 1
kernel_read:
	cmp	si, ReservedSectorCount
	jge	kernel_readend
	mov	di, si
	mov	si, progressmsg
	call	print
	mov	si, di
	dec	di			; �u�[�g�Z�N�^������
	sal	di, 5
	add	di, 0x1000
	mov	es, di			;
	mov	bx, 0			; ES:BX = 1000+si*0x20:0000
	mov	ax, si			; ax=�ǂݍ��ރZ�N�^�ԍ�
	mov	dl, SectorsPerTrack*NumberOfHeads	; sectors per track
	div	dl			; al=div, ah=mod
	mov	cl, ah			; �w�b�_����ʂ��Ȃ��Z�N�^�̈ʒu
	mov	ch, al			; �V�����_
	mov	ah, 0
	mov	al, cl
	mov	dl, SectorsPerTrack
	div	dl
	mov	dh, al			; �w�b�h=cl/18
	mov	cl, ah			; �Z�N�^�̈ʒu=cl%18
	inc	cl			; 1�x�[�X
	mov	dl, 0			; �h���C�u=0
	mov	al, 1			; 1�Z�N�^�ǂݍ���
	mov	ah, 2			; function: READ DIAGNOSTIC
	int	13h			; �ǂݍ���
	jc	kernel_readerror
	inc	si			; ���̃Z�N�^��
	jmp	kernel_read

kernel_readerror:
	push	si
	xor	cx, cx
	mov	cl, ah
	mov	si, cx
	shr	si, 4
	add	si, num0
	call	print
	mov	si, cx
	and	si, 0x0F
	add	si, num0
	call	print
	pop	si
	inc	si
	jmp	kernel_read

kernel_readend:
	mov	si, endmsg		; �ǂݍ��݃��b�Z�[�W�̕\��
	call	print

kernel_start:
	jmp	0x1000:0000		;�J�[�l�����[�_�փW�����v

stop:
	hlt
	jmp	stop			; �����̒�~

;���b�Z�[�W
startmsg	db  'loading kernel loader', 0x00
progressmsg	db  '.', 0x00
endmsg		db  'OK', 0x0d, 0x0a, 0x00
errormsg	db  '*', 0x00;'read error.',  0x00
num0		db  '0', 0x00
		db  '1', 0x00
		db  '2', 0x00
		db  '3', 0x00
		db  '4', 0x00
		db  '5', 0x00
		db  '6', 0x00
		db  '7', 0x00
		db  '8', 0x00
		db  '9', 0x00
		db  'A', 0x00
		db  'B', 0x00
		db  'C', 0x00
		db  'D', 0x00
		db  'E', 0x00
		db  'F', 0x00

;�c���0�Ŗ��߂�
	times 512 - 2 - ($-$$) db 0

;�u�[�g���[�_�[�̃}�W�b�N�i���o�[
	dw	0x0AA55
