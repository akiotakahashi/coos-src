%include "define.mac"
%define BASE_SEGMENT_OF_KERNEL_IMAGE	0x1500
%define BASE_SEGMENT_OF_FAT		0x6000
%define MEMORY_SIZE_KB			0x1000
%define VESA_INFO			0x0800
%define VESA_INFO_DETAIL		0x0900

[ORG 0]
[BITS 16]
	jmp	start

; メッセージ表示
print:
	push	ax
	push	bx
.print:
	lodsb				; ds:siの位置からalへ値を取り出す。SIは+1される
	cmp	al, 0			; ALが0だったら終了
	je	printend
	mov	ah, 0eh 		; alの文字をBIOSで表示
	mov	bx, 7
	int	10h
	jmp	.print			; 次の文字を表示
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
; エントリポイント
;-----------------------------------------------------------------------------------------
start:
	; セグメントレジスタの初期化
	mov	ax, cs
	mov	ds, ax		; CS = DS = 0x10000
	; カーソルを消去
	mov	ah, 0x01
	mov	ch, 0x20
	int	0x10
	; 物理メモリサイズ取得
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
	; ロードシーケンス
	call	loadkernel	; カーネルロード
	call	a20enable	; A20ラインを有効にする
	;call	enable_a20
	call	vesa_mode	; SVGAに移行
	; プロテクトモードに移行
	jmp	enter_protectmode

;-----------------------------------------------------------------------------------------
; カーネルロード
;-----------------------------------------------------------------------------------------
loadkernel:
	mov	si, loadmsg
	call	print		; メッセージの表示

	; まずはルートディレクトリエントリを読み込みます。
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

	; ルートディレクトリエントリの中からカーネルイメージを探します。
	xor	di, di
	mov	bx, RootEntryCount
kernel_next:
	mov	si, bname
	mov	cx, 0x000b
	push	di
	rep	cmpsb
	pop	di
	je	kernel_found
	add	di, 32				; 次のエントリへ移動します。
	dec	bx				; 残エントリカウンタを減らします。
	jnz	kernel_next
	jmp	file_not_found

	; ルートディレクトリエントリの中にカーネルイメージを見つけました。
kernel_found:
	mov	ecx, [es:di+28]			; ファイルサイズ(+28-4)を保存しておきます。
	mov	ax, 0
	mov	gs, ax
	mov	[gs:0x7c00], ecx
	push	word [es:di+26]			; クラスタ位置(+26-2)を保存しておきます。

	; FAT領域を読み込みます。
	mov	bx, 0				; このアドレスに読み込みます。
	mov	ax, BASE_SEGMENT_OF_FAT
	mov	es, ax
	mov	ax, ReservedSectorCount		; 予約領域の次がFAT領域です。
	mov	di, SectorsPerFAT
	call	readsector

	; カーネルイメージを読み込みます。
	mov	ax, BASE_SEGMENT_OF_KERNEL_IMAGE
	mov	es, ax
	mov	bx, 0
	pop	cx			; クラスタ位置を復帰します。
kernel_load:
	mov	ax, cx			; 読み込むクラスタ位置です。
	sub	ax, 2
	mov	si, SectorsPerCluster
	mul	si
	add	ax, FirstDataSector	; 読み込みセクタ位置です。
	mov	di, SectorsPerCluster	; １クラスタ読み込みます。
	call	readsector
	mov	si, readmsg
	call	print
	; 後続のクラスタがあるか調べます。
	push	bx
	mov	bx, cx
	call	get_fat
	pop	bx
	cmp	ax, 0x0ff8
	jge	end_of_kernel
	; 後続のクラスタを読み込みます。
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

; BX: クラスタ番号
get_fat:
	mov	si, bx
	shr	si, 1
	mov	ax, BASE_SEGMENT_OF_FAT
	mov	gs, ax
	mov	ax, [gs:si+bx]
	jnc	get_fat_with_even_cluster
	; 読み込むのは奇数クラスタエントリです。
	shr	ax, 4
get_fat_with_even_cluster:
	and	ah, 0x0f
	ret

file_not_found:
	; カーネルイメージがディレクトリエントリの中に見つかりませんでした。
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
; SVGAに切り替え
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
; 次の2行を有効化するとテキストモードで起動します
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
; A-20ラインを有効にする
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
	out	064h, al		; 制御データを送ることを知らせる
	call	wait8042
	mov	al, 0xdf
	out	060h, al		; A20ラインを有効
	call	wait8042
	mov	al, 0xff
	out	064h, al		; A20ラインを有効
	call	wait8042
        ret
; 8042を調べてキーボードバッファが空になるのを待つ
wait8042:
	mov	cx, 0ffffh
waitloop:
	;jmp	$+2
	in	al, 064h 		; 64hを読み込む
;	test	al, 1			; bit0==1?(出力バッファは空き)
;	jz	checkinbuf		; そうなら入力バッファを確認
	;jmp	$+2
;	in	al, 060h 		; 読み込んで入力バッファをクリア
;	jmp	checkloop
;checkinbuf:
	test	al, 2			; bit1==1?(入力バッファは空き)
	jz	wait8042_end 		; そうならループ終了
	in	al, 060h 		; 読み込んで入力バッファをクリア
checkloop:
	loop	waitloop		; cx-=1して0以上ならwaitloopへ
wait8042_end:
	ret

;-----------------------------------------------------------------------------------------
; プロテクトモードへ移行
;-----------------------------------------------------------------------------------------
enter_protectmode:
	mov	si, modemsg
	call	print
	mov	ax, cs			; セグメントレジスタの初期化
	mov	ds, ax			; CS = DS
	cli 				; 割り込みを停止
	lgdt	[gdtr] 			; GDTの設定
	mov	eax, cr0
	or	eax, 1
	mov	cr0, eax		; PEビットを1にセットしてプロテクトモードへ
	jmp	near far_jump
far_jump:
	jmp	dword 0x08:on_protectmode+0x10000

;-----------------------------------------------------------------------------------------
; 処理の停止
;-----------------------------------------------------------------------------------------
stop:
	cli
forever:
	hlt
	jmp	short forever

;*****************************************************************************************
; 32 bit コード
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
	lidt	[idtr+0x10000]		; IDTの設定
	mov	esp, 1024*1024*4	; sp is 4MB
	; カーネルコードのコピー
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
	; カーネルにジャンプ
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
	dw	idt_end-idt_begin-1	; IDTのサイズ
	dd	0x10000+idt_begin	; IDTのアドレス

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
;	メッセージ
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
