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

; メッセージ表示
print:
	lodsb	    			; ds:siの位置からalへ値を取り出す。SIは+1される
	cmp	al, 0			; ALが0だったら終了
	je	printend
	mov	ah, 0eh			; alの文字をBIOSで表示
	mov	bx, 7
	int	10h
	jmp	print			; 次の文字を表示
printend:
	ret

; エラー処理
error:
	mov	si, errormsg
	call	print
	ret


; ブートローダースタート
start:
	jmp	0x07c0:refresh_cs
refresh_cs:
	mov	ax, cs			; セグメントレジスタの初期化
	mov	ds, ax
	mov	es, ax			; CS = ES = DS = 07C0h
	mov	ax, 0x4000
	mov	ss, ax
	mov	ax, 0x8000
	mov	sp, ax
	; スタートメッセージの表示
	mov	si, startmsg
	call	print

; FDCリセット
fdc_reset:
	mov	ax, 0
	mov	dl, 0			; Drive=0(Aドライブ/FDD)を指定
	int	13h
	jc	fdc_reset		; エラーが起きたらfdc_resetへ移ってもう一度

; カーネルローダの読み込み
	mov	si, 1
kernel_read:
	cmp	si, ReservedSectorCount
	jge	kernel_readend
	mov	di, si
	mov	si, progressmsg
	call	print
	mov	si, di
	dec	di			; ブートセクタ分引く
	sal	di, 5
	add	di, 0x1000
	mov	es, di			;
	mov	bx, 0			; ES:BX = 1000+si*0x20:0000
	mov	ax, si			; ax=読み込むセクタ番号
	mov	dl, SectorsPerTrack*NumberOfHeads	; sectors per track
	div	dl			; al=div, ah=mod
	mov	cl, ah			; ヘッダを区別しないセクタの位置
	mov	ch, al			; シリンダ
	mov	ah, 0
	mov	al, cl
	mov	dl, SectorsPerTrack
	div	dl
	mov	dh, al			; ヘッド=cl/18
	mov	cl, ah			; セクタの位置=cl%18
	inc	cl			; 1ベース
	mov	dl, 0			; ドライブ=0
	mov	al, 1			; 1セクタ読み込む
	mov	ah, 2			; function: READ DIAGNOSTIC
	int	13h			; 読み込み
	jc	kernel_readerror
	inc	si			; 次のセクタへ
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
	mov	si, endmsg		; 読み込みメッセージの表示
	call	print

kernel_start:
	jmp	0x1000:0000		;カーネルローダへジャンプ

stop:
	hlt
	jmp	stop			; 処理の停止

;メッセージ
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

;残りを0で埋める
	times 512 - 2 - ($-$$) db 0

;ブートローダーのマジックナンバー
	dw	0x0AA55
