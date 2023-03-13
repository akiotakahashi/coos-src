00000000  C85C0000          enter 0x5c,0x0
00000004  FFB524000000      push dword [ebp+0x24]
0000000A  FFB51C000000      push dword [ebp+0x1c]
00000010  59                pop ecx
00000011  58                pop eax
00000012  39C8              cmp eax,ecx
00000014  0F8D11000000      jnl near 0x2b
0000001A  6801000000        push dword 0x1
0000001F  58                pop eax
00000020  8985FCFFFFFF      mov [ebp+0xfffffffc],eax
00000026  E90C000000        jmp 0x37
0000002B  68FFFFFFFF        push dword 0xffffffff
00000030  58                pop eax
00000031  8985FCFFFFFF      mov [ebp+0xfffffffc],eax
00000037  FFB520000000      push dword [ebp+0x20]
0000003D  FFB518000000      push dword [ebp+0x18]
00000043  59                pop ecx
00000044  58                pop eax
00000045  39C8              cmp eax,ecx
00000047  0F8D11000000      jnl near 0x5e
0000004D  6801000000        push dword 0x1
00000052  58                pop eax
00000053  8985F8FFFFFF      mov [ebp+0xfffffff8],eax
00000059  E90C000000        jmp 0x6a
0000005E  68FFFFFFFF        push dword 0xffffffff
00000063  58                pop eax
00000064  8985F8FFFFFF      mov [ebp+0xfffffff8],eax
0000006A  FFB51C000000      push dword [ebp+0x1c]
00000070  FFB524000000      push dword [ebp+0x24]
00000076  59                pop ecx
00000077  58                pop eax
00000078  29C8              sub eax,ecx
0000007A  50                push eax
0000007B  E8C4220000        call 0x2344
00000080  50                push eax
00000081  58                pop eax
00000082  8985F4FFFFFF      mov [ebp+0xfffffff4],eax
00000088  FFB518000000      push dword [ebp+0x18]
0000008E  FFB520000000      push dword [ebp+0x20]
00000094  59                pop ecx
00000095  58                pop eax
00000096  29C8              sub eax,ecx
00000098  50                push eax
00000099  E8A6220000        call 0x2344
0000009E  50                push eax
0000009F  58                pop eax
000000A0  8985F0FFFFFF      mov [ebp+0xfffffff0],eax
000000A6  FFB5F4FFFFFF      push dword [ebp+0xfffffff4]
000000AC  FFB5F0FFFFFF      push dword [ebp+0xfffffff0]
000000B2  59                pop ecx
000000B3  58                pop eax
000000B4  39C8              cmp eax,ecx
000000B6  0F8F0B000000      jg near 0xc7
000000BC  FFB5F0FFFFFF      push dword [ebp+0xfffffff0]
000000C2  E906000000        jmp 0xcd
000000C7  FFB5F4FFFFFF      push dword [ebp+0xfffffff4]
000000CD  58                pop eax
000000CE  8985ECFFFFFF      mov [ebp+0xffffffec],eax
000000D4  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000000DA  6801000000        push dword 0x1
000000DF  59                pop ecx
000000E0  58                pop eax
000000E1  39C8              cmp eax,ecx
000000E3  0F8D88000000      jnl near 0x171
000000E9  FFB528000000      push dword [ebp+0x28]
000000EF  5B                pop ebx
000000F0  FF33              push dword [ebx]
000000F2  FFB524000000      push dword [ebp+0x24]
000000F8  FFB520000000      push dword [ebp+0x20]
000000FE  89E8              mov eax,ebp
00000100  0508000000        add eax,0x8
00000105  81EC10000000      sub esp,0x10
0000010B  89C6              mov esi,eax
0000010D  89E7              mov edi,esp
0000010F  B904000000        mov ecx,0x4
00000114  F3A5              rep movsd
00000116  FFB42418000000    push dword [esp+0x18]
0000011D  68EC9DCB02        push dword 0x2cb9dec
00000122  E89D370000        call 0x38c4
00000127  50                push eax
00000128  58                pop eax
00000129  FFD0              call eax
0000012B  FFB528000000      push dword [ebp+0x28]
00000131  5B                pop ebx
00000132  FF33              push dword [ebx]
00000134  FFB51C000000      push dword [ebp+0x1c]
0000013A  FFB518000000      push dword [ebp+0x18]
00000140  89E8              mov eax,ebp
00000142  0508000000        add eax,0x8
00000147  81EC10000000      sub esp,0x10
0000014D  89C6              mov esi,eax
0000014F  89E7              mov edi,esp
00000151  B904000000        mov ecx,0x4
00000156  F3A5              rep movsd
00000158  FFB42418000000    push dword [esp+0x18]
0000015F  68EC9DCB02        push dword 0x2cb9dec
00000164  E85B370000        call 0x38c4
00000169  50                push eax
0000016A  58                pop eax
0000016B  FFD0              call eax
0000016D  C9                leave
0000016E  C22400            ret 0x24
00000171  FFB528000000      push dword [ebp+0x28]
00000177  5B                pop ebx
00000178  FF33              push dword [ebx]
0000017A  89E8              mov eax,ebp
0000017C  0508000000        add eax,0x8
00000181  81EC10000000      sub esp,0x10
00000187  89C6              mov esi,eax
00000189  89E7              mov edi,esp
0000018B  B904000000        mov ecx,0x4
00000190  F3A5              rep movsd
00000192  FFB42410000000    push dword [esp+0x10]
00000199  6800D4B202        push dword 0x2b2d400
0000019E  E821370000        call 0x38c4
000001A3  50                push eax
000001A4  58                pop eax
000001A5  FFD0              call eax
000001A7  50                push eax
000001A8  58                pop eax
000001A9  8985E8FFFFFF      mov [ebp+0xffffffe8],eax
000001AF  FFB528000000      push dword [ebp+0x28]
000001B5  5B                pop ebx
000001B6  FF33              push dword [ebx]
000001B8  FF3424            push dword [esp]
000001BB  6868CDBE02        push dword 0x2becd68
000001C0  E8FF360000        call 0x38c4
000001C5  50                push eax
000001C6  58                pop eax
000001C7  FFD0              call eax
000001C9  50                push eax
000001CA  58                pop eax
000001CB  8985E4FFFFFF      mov [ebp+0xffffffe4],eax
000001D1  6800000000        push dword 0x0
000001D6  58                pop eax
000001D7  8985E0FFFFFF      mov [ebp+0xffffffe0],eax
000001DD  6800000000        push dword 0x0
000001E2  58                pop eax
000001E3  8985DCFFFFFF      mov [ebp+0xffffffdc],eax
000001E9  FFB528000000      push dword [ebp+0x28]
000001EF  5B                pop ebx
000001F0  FF33              push dword [ebx]
000001F2  FF3424            push dword [esp]
000001F5  681414BF02        push dword 0x2bf1414
000001FA  E8C5360000        call 0x38c4
000001FF  50                push eax
00000200  58                pop eax
00000201  FFD0              call eax
00000203  50                push eax
00000204  6804000000        push dword 0x4
00000209  59                pop ecx
0000020A  58                pop eax
0000020B  39C8              cmp eax,ecx
0000020D  0F850C020000      jnz near 0x41f
00000213  FFB528000000      push dword [ebp+0x28]
00000219  5B                pop ebx
0000021A  FF33              push dword [ebx]
0000021C  FF3424            push dword [esp]
0000021F  68B44ACD02        push dword 0x2cd4ab4
00000224  E89B360000        call 0x38c4
00000229  50                push eax
0000022A  58                pop eax
0000022B  FFD0              call eax
0000022D  50                push eax
0000022E  58                pop eax
0000022F  8985D8FFFFFF      mov [ebp+0xffffffd8],eax
00000235  FFB5D8FFFFFF      push dword [ebp+0xffffffd8]
0000023B  FF3424            push dword [esp]
0000023E  68506FCD02        push dword 0x2cd6f50
00000243  E87C360000        call 0x38c4
00000248  50                push eax
00000249  58                pop eax
0000024A  FFD0              call eax
0000024C  50                push eax
0000024D  58                pop eax
0000024E  8985A8FFFFFF      mov [ebp+0xffffffa8],eax
00000254  89E8              mov eax,ebp
00000256  2D58000000        sub eax,0x58
0000025B  50                push eax
0000025C  E803560000        call 0x5864
00000261  50                push eax
00000262  58                pop eax
00000263  8985D4FFFFFF      mov [ebp+0xffffffd4],eax
00000269  6800000000        push dword 0x0
0000026E  58                pop eax
0000026F  8985D0FFFFFF      mov [ebp+0xffffffd0],eax
00000275  E95B010000        jmp 0x3d5
0000027A  FFB5D4FFFFFF      push dword [ebp+0xffffffd4]
00000280  6804000000        push dword 0x4
00000285  FFB524000000      push dword [ebp+0x24]
0000028B  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000291  6808000000        push dword 0x8
00000296  59                pop ecx
00000297  58                pop eax
00000298  D3F8              sar eax,cl
0000029A  50                push eax
0000029B  59                pop ecx
0000029C  58                pop eax
0000029D  01C8              add eax,ecx
0000029F  50                push eax
000002A0  59                pop ecx
000002A1  58                pop eax
000002A2  F7E1              mul ecx
000002A4  50                push eax
000002A5  59                pop ecx
000002A6  58                pop eax
000002A7  01C8              add eax,ecx
000002A9  50                push eax
000002AA  FFB5E4FFFFFF      push dword [ebp+0xffffffe4]
000002B0  FFB520000000      push dword [ebp+0x20]
000002B6  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
000002BC  6808000000        push dword 0x8
000002C1  59                pop ecx
000002C2  58                pop eax
000002C3  D3F8              sar eax,cl
000002C5  50                push eax
000002C6  59                pop ecx
000002C7  58                pop eax
000002C8  01C8              add eax,ecx
000002CA  50                push eax
000002CB  59                pop ecx
000002CC  58                pop eax
000002CD  F7E1              mul ecx
000002CF  50                push eax
000002D0  59                pop ecx
000002D1  58                pop eax
000002D2  01C8              add eax,ecx
000002D4  50                push eax
000002D5  FFB5E8FFFFFF      push dword [ebp+0xffffffe8]
000002DB  58                pop eax
000002DC  59                pop ecx
000002DD  898100000000      mov [ecx+0x0],eax
000002E3  FFB5D4FFFFFF      push dword [ebp+0xffffffd4]
000002E9  6804000000        push dword 0x4
000002EE  FFB51C000000      push dword [ebp+0x1c]
000002F4  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
000002FA  6808000000        push dword 0x8
000002FF  59                pop ecx
00000300  58                pop eax
00000301  D3F8              sar eax,cl
00000303  50                push eax
00000304  59                pop ecx
00000305  58                pop eax
00000306  29C8              sub eax,ecx
00000308  50                push eax
00000309  59                pop ecx
0000030A  58                pop eax
0000030B  F7E1              mul ecx
0000030D  50                push eax
0000030E  59                pop ecx
0000030F  58                pop eax
00000310  01C8              add eax,ecx
00000312  50                push eax
00000313  FFB5E4FFFFFF      push dword [ebp+0xffffffe4]
00000319  FFB518000000      push dword [ebp+0x18]
0000031F  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
00000325  6808000000        push dword 0x8
0000032A  59                pop ecx
0000032B  58                pop eax
0000032C  D3F8              sar eax,cl
0000032E  50                push eax
0000032F  59                pop ecx
00000330  58                pop eax
00000331  29C8              sub eax,ecx
00000333  50                push eax
00000334  59                pop ecx
00000335  58                pop eax
00000336  F7E1              mul ecx
00000338  50                push eax
00000339  59                pop ecx
0000033A  58                pop eax
0000033B  01C8              add eax,ecx
0000033D  50                push eax
0000033E  FFB5E8FFFFFF      push dword [ebp+0xffffffe8]
00000344  58                pop eax
00000345  59                pop ecx
00000346  898100000000      mov [ecx+0x0],eax
0000034C  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000352  FFB5FCFFFFFF      push dword [ebp+0xfffffffc]
00000358  FFB5F4FFFFFF      push dword [ebp+0xfffffff4]
0000035E  6808000000        push dword 0x8
00000363  59                pop ecx
00000364  58                pop eax
00000365  D3E0              shl eax,cl
00000367  50                push eax
00000368  59                pop ecx
00000369  58                pop eax
0000036A  F7E1              mul ecx
0000036C  50                push eax
0000036D  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
00000373  59                pop ecx
00000374  58                pop eax
00000375  99                cdq
00000376  F7F9              idiv ecx
00000378  50                push eax
00000379  59                pop ecx
0000037A  58                pop eax
0000037B  01C8              add eax,ecx
0000037D  50                push eax
0000037E  58                pop eax
0000037F  8985E0FFFFFF      mov [ebp+0xffffffe0],eax
00000385  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
0000038B  FFB5F8FFFFFF      push dword [ebp+0xfffffff8]
00000391  FFB5F0FFFFFF      push dword [ebp+0xfffffff0]
00000397  6808000000        push dword 0x8
0000039C  59                pop ecx
0000039D  58                pop eax
0000039E  D3E0              shl eax,cl
000003A0  50                push eax
000003A1  59                pop ecx
000003A2  58                pop eax
000003A3  F7E1              mul ecx
000003A5  50                push eax
000003A6  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000003AC  59                pop ecx
000003AD  58                pop eax
000003AE  99                cdq
000003AF  F7F9              idiv ecx
000003B1  50                push eax
000003B2  59                pop ecx
000003B3  58                pop eax
000003B4  01C8              add eax,ecx
000003B6  50                push eax
000003B7  58                pop eax
000003B8  8985DCFFFFFF      mov [ebp+0xffffffdc],eax
000003BE  FFB5D0FFFFFF      push dword [ebp+0xffffffd0]
000003C4  6801000000        push dword 0x1
000003C9  59                pop ecx
000003CA  58                pop eax
000003CB  01C8              add eax,ecx
000003CD  50                push eax
000003CE  58                pop eax
000003CF  8985D0FFFFFF      mov [ebp+0xffffffd0],eax
000003D5  FFB5D0FFFFFF      push dword [ebp+0xffffffd0]
000003DB  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000003E1  6802000000        push dword 0x2
000003E6  59                pop ecx
000003E7  58                pop eax
000003E8  99                cdq
000003E9  F7F9              idiv ecx
000003EB  50                push eax
000003EC  59                pop ecx
000003ED  58                pop eax
000003EE  39C8              cmp eax,ecx
000003F0  0F8E84FEFFFF      jng near 0x27a
000003F6  FFB5D8FFFFFF      push dword [ebp+0xffffffd8]
000003FC  58                pop eax
000003FD  3D00000000        cmp eax,0x0
00000402  0F8417000000      jz near 0x41f
00000408  FFB5D8FFFFFF      push dword [ebp+0xffffffd8]
0000040E  FF3424            push dword [esp]
00000411  68482EEB02        push dword 0x2eb2e48
00000416  E8A9340000        call 0x38c4
0000041B  50                push eax
0000041C  58                pop eax
0000041D  FFD0              call eax
0000041F  FFB528000000      push dword [ebp+0x28]
00000425  5B                pop ebx
00000426  FF33              push dword [ebx]
00000428  FF3424            push dword [esp]
0000042B  681414BF02        push dword 0x2bf1414
00000430  E88F340000        call 0x38c4
00000435  50                push eax
00000436  58                pop eax
00000437  FFD0              call eax
00000439  50                push eax
0000043A  6802000000        push dword 0x2
0000043F  59                pop ecx
00000440  58                pop eax
00000441  39C8              cmp eax,ecx
00000443  0F851C020000      jnz near 0x665
00000449  FFB528000000      push dword [ebp+0x28]
0000044F  5B                pop ebx
00000450  FF33              push dword [ebx]
00000452  FF3424            push dword [esp]
00000455  68B44ACD02        push dword 0x2cd4ab4
0000045A  E865340000        call 0x38c4
0000045F  50                push eax
00000460  58                pop eax
00000461  FFD0              call eax
00000463  50                push eax
00000464  58                pop eax
00000465  8985CCFFFFFF      mov [ebp+0xffffffcc],eax
0000046B  FFB5CCFFFFFF      push dword [ebp+0xffffffcc]
00000471  FF3424            push dword [esp]
00000474  68506FCD02        push dword 0x2cd6f50
00000479  E846340000        call 0x38c4
0000047E  50                push eax
0000047F  58                pop eax
00000480  FFD0              call eax
00000482  50                push eax
00000483  58                pop eax
00000484  8985A8FFFFFF      mov [ebp+0xffffffa8],eax
0000048A  89E8              mov eax,ebp
0000048C  2D58000000        sub eax,0x58
00000491  50                push eax
00000492  E8CD530000        call 0x5864
00000497  50                push eax
00000498  58                pop eax
00000499  8985C8FFFFFF      mov [ebp+0xffffffc8],eax
0000049F  6800000000        push dword 0x0
000004A4  58                pop eax
000004A5  8985C4FFFFFF      mov [ebp+0xffffffc4],eax
000004AB  E96B010000        jmp 0x61b
000004B0  FFB5C8FFFFFF      push dword [ebp+0xffffffc8]
000004B6  6802000000        push dword 0x2
000004BB  FFB524000000      push dword [ebp+0x24]
000004C1  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
000004C7  6808000000        push dword 0x8
000004CC  59                pop ecx
000004CD  58                pop eax
000004CE  D3F8              sar eax,cl
000004D0  50                push eax
000004D1  59                pop ecx
000004D2  58                pop eax
000004D3  01C8              add eax,ecx
000004D5  50                push eax
000004D6  59                pop ecx
000004D7  58                pop eax
000004D8  F7E1              mul ecx
000004DA  50                push eax
000004DB  59                pop ecx
000004DC  58                pop eax
000004DD  01C8              add eax,ecx
000004DF  50                push eax
000004E0  FFB5E4FFFFFF      push dword [ebp+0xffffffe4]
000004E6  FFB520000000      push dword [ebp+0x20]
000004EC  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
000004F2  6808000000        push dword 0x8
000004F7  59                pop ecx
000004F8  58                pop eax
000004F9  D3F8              sar eax,cl
000004FB  50                push eax
000004FC  59                pop ecx
000004FD  58                pop eax
000004FE  01C8              add eax,ecx
00000500  50                push eax
00000501  59                pop ecx
00000502  58                pop eax
00000503  F7E1              mul ecx
00000505  50                push eax
00000506  59                pop ecx
00000507  58                pop eax
00000508  01C8              add eax,ecx
0000050A  50                push eax
0000050B  FFB5E8FFFFFF      push dword [ebp+0xffffffe8]
00000511  812424FFFF0000    and dword [esp],0xffff
00000518  58                pop eax
00000519  59                pop ecx
0000051A  66898100000000    mov [ecx+0x0],ax
00000521  FFB5C8FFFFFF      push dword [ebp+0xffffffc8]
00000527  6802000000        push dword 0x2
0000052C  FFB51C000000      push dword [ebp+0x1c]
00000532  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000538  6808000000        push dword 0x8
0000053D  59                pop ecx
0000053E  58                pop eax
0000053F  D3F8              sar eax,cl
00000541  50                push eax
00000542  59                pop ecx
00000543  58                pop eax
00000544  29C8              sub eax,ecx
00000546  50                push eax
00000547  59                pop ecx
00000548  58                pop eax
00000549  F7E1              mul ecx
0000054B  50                push eax
0000054C  59                pop ecx
0000054D  58                pop eax
0000054E  01C8              add eax,ecx
00000550  50                push eax
00000551  FFB5E4FFFFFF      push dword [ebp+0xffffffe4]
00000557  FFB518000000      push dword [ebp+0x18]
0000055D  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
00000563  6808000000        push dword 0x8
00000568  59                pop ecx
00000569  58                pop eax
0000056A  D3F8              sar eax,cl
0000056C  50                push eax
0000056D  59                pop ecx
0000056E  58                pop eax
0000056F  29C8              sub eax,ecx
00000571  50                push eax
00000572  59                pop ecx
00000573  58                pop eax
00000574  F7E1              mul ecx
00000576  50                push eax
00000577  59                pop ecx
00000578  58                pop eax
00000579  01C8              add eax,ecx
0000057B  50                push eax
0000057C  FFB5E8FFFFFF      push dword [ebp+0xffffffe8]
00000582  812424FFFF0000    and dword [esp],0xffff
00000589  58                pop eax
0000058A  59                pop ecx
0000058B  66898100000000    mov [ecx+0x0],ax
00000592  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000598  FFB5FCFFFFFF      push dword [ebp+0xfffffffc]
0000059E  FFB5F4FFFFFF      push dword [ebp+0xfffffff4]
000005A4  6808000000        push dword 0x8
000005A9  59                pop ecx
000005AA  58                pop eax
000005AB  D3E0              shl eax,cl
000005AD  50                push eax
000005AE  59                pop ecx
000005AF  58                pop eax
000005B0  F7E1              mul ecx
000005B2  50                push eax
000005B3  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000005B9  59                pop ecx
000005BA  58                pop eax
000005BB  99                cdq
000005BC  F7F9              idiv ecx
000005BE  50                push eax
000005BF  59                pop ecx
000005C0  58                pop eax
000005C1  01C8              add eax,ecx
000005C3  50                push eax
000005C4  58                pop eax
000005C5  8985E0FFFFFF      mov [ebp+0xffffffe0],eax
000005CB  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
000005D1  FFB5F8FFFFFF      push dword [ebp+0xfffffff8]
000005D7  FFB5F0FFFFFF      push dword [ebp+0xfffffff0]
000005DD  6808000000        push dword 0x8
000005E2  59                pop ecx
000005E3  58                pop eax
000005E4  D3E0              shl eax,cl
000005E6  50                push eax
000005E7  59                pop ecx
000005E8  58                pop eax
000005E9  F7E1              mul ecx
000005EB  50                push eax
000005EC  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000005F2  59                pop ecx
000005F3  58                pop eax
000005F4  99                cdq
000005F5  F7F9              idiv ecx
000005F7  50                push eax
000005F8  59                pop ecx
000005F9  58                pop eax
000005FA  01C8              add eax,ecx
000005FC  50                push eax
000005FD  58                pop eax
000005FE  8985DCFFFFFF      mov [ebp+0xffffffdc],eax
00000604  FFB5C4FFFFFF      push dword [ebp+0xffffffc4]
0000060A  6801000000        push dword 0x1
0000060F  59                pop ecx
00000610  58                pop eax
00000611  01C8              add eax,ecx
00000613  50                push eax
00000614  58                pop eax
00000615  8985C4FFFFFF      mov [ebp+0xffffffc4],eax
0000061B  FFB5C4FFFFFF      push dword [ebp+0xffffffc4]
00000621  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
00000627  6802000000        push dword 0x2
0000062C  59                pop ecx
0000062D  58                pop eax
0000062E  99                cdq
0000062F  F7F9              idiv ecx
00000631  50                push eax
00000632  59                pop ecx
00000633  58                pop eax
00000634  39C8              cmp eax,ecx
00000636  0F8E74FEFFFF      jng near 0x4b0
0000063C  FFB5CCFFFFFF      push dword [ebp+0xffffffcc]
00000642  58                pop eax
00000643  3D00000000        cmp eax,0x0
00000648  0F8417000000      jz near 0x665
0000064E  FFB5CCFFFFFF      push dword [ebp+0xffffffcc]
00000654  FF3424            push dword [esp]
00000657  68482EEB02        push dword 0x2eb2e48
0000065C  E863320000        call 0x38c4
00000661  50                push eax
00000662  58                pop eax
00000663  FFD0              call eax
00000665  FFB528000000      push dword [ebp+0x28]
0000066B  5B                pop ebx
0000066C  FF33              push dword [ebx]
0000066E  FF3424            push dword [esp]
00000671  681414BF02        push dword 0x2bf1414
00000676  E849320000        call 0x38c4
0000067B  50                push eax
0000067C  58                pop eax
0000067D  FFD0              call eax
0000067F  50                push eax
00000680  6803000000        push dword 0x3
00000685  59                pop ecx
00000686  58                pop eax
00000687  39C8              cmp eax,ecx
00000689  0F858E020000      jnz near 0x91d
0000068F  FFB528000000      push dword [ebp+0x28]
00000695  5B                pop ebx
00000696  FF33              push dword [ebx]
00000698  FF3424            push dword [esp]
0000069B  68B44ACD02        push dword 0x2cd4ab4
000006A0  E81F320000        call 0x38c4
000006A5  50                push eax
000006A6  58                pop eax
000006A7  FFD0              call eax
000006A9  50                push eax
000006AA  58                pop eax
000006AB  8985C0FFFFFF      mov [ebp+0xffffffc0],eax
000006B1  FFB5C0FFFFFF      push dword [ebp+0xffffffc0]
000006B7  FF3424            push dword [esp]
000006BA  68506FCD02        push dword 0x2cd6f50
000006BF  E800320000        call 0x38c4
000006C4  50                push eax
000006C5  58                pop eax
000006C6  FFD0              call eax
000006C8  50                push eax
000006C9  58                pop eax
000006CA  8985A8FFFFFF      mov [ebp+0xffffffa8],eax
000006D0  89E8              mov eax,ebp
000006D2  2D58000000        sub eax,0x58
000006D7  50                push eax
000006D8  E887510000        call 0x5864
000006DD  50                push eax
000006DE  58                pop eax
000006DF  8985BCFFFFFF      mov [ebp+0xffffffbc],eax
000006E5  6800000000        push dword 0x0
000006EA  58                pop eax
000006EB  8985B8FFFFFF      mov [ebp+0xffffffb8],eax
000006F1  E9DD010000        jmp 0x8d3
000006F6  6803000000        push dword 0x3
000006FB  FFB524000000      push dword [ebp+0x24]
00000701  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000707  6808000000        push dword 0x8
0000070C  59                pop ecx
0000070D  58                pop eax
0000070E  D3F8              sar eax,cl
00000710  50                push eax
00000711  59                pop ecx
00000712  58                pop eax
00000713  01C8              add eax,ecx
00000715  50                push eax
00000716  59                pop ecx
00000717  58                pop eax
00000718  F7E1              mul ecx
0000071A  50                push eax
0000071B  FFB5E4FFFFFF      push dword [ebp+0xffffffe4]
00000721  FFB520000000      push dword [ebp+0x20]
00000727  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
0000072D  6808000000        push dword 0x8
00000732  59                pop ecx
00000733  58                pop eax
00000734  D3F8              sar eax,cl
00000736  50                push eax
00000737  59                pop ecx
00000738  58                pop eax
00000739  01C8              add eax,ecx
0000073B  50                push eax
0000073C  59                pop ecx
0000073D  58                pop eax
0000073E  F7E1              mul ecx
00000740  50                push eax
00000741  59                pop ecx
00000742  58                pop eax
00000743  01C8              add eax,ecx
00000745  50                push eax
00000746  58                pop eax
00000747  8985B4FFFFFF      mov [ebp+0xffffffb4],eax
0000074D  6803000000        push dword 0x3
00000752  FFB51C000000      push dword [ebp+0x1c]
00000758  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
0000075E  6808000000        push dword 0x8
00000763  59                pop ecx
00000764  58                pop eax
00000765  D3F8              sar eax,cl
00000767  50                push eax
00000768  59                pop ecx
00000769  58                pop eax
0000076A  29C8              sub eax,ecx
0000076C  50                push eax
0000076D  59                pop ecx
0000076E  58                pop eax
0000076F  F7E1              mul ecx
00000771  50                push eax
00000772  FFB5E4FFFFFF      push dword [ebp+0xffffffe4]
00000778  FFB518000000      push dword [ebp+0x18]
0000077E  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
00000784  6808000000        push dword 0x8
00000789  59                pop ecx
0000078A  58                pop eax
0000078B  D3F8              sar eax,cl
0000078D  50                push eax
0000078E  59                pop ecx
0000078F  58                pop eax
00000790  29C8              sub eax,ecx
00000792  50                push eax
00000793  59                pop ecx
00000794  58                pop eax
00000795  F7E1              mul ecx
00000797  50                push eax
00000798  59                pop ecx
00000799  58                pop eax
0000079A  01C8              add eax,ecx
0000079C  50                push eax
0000079D  58                pop eax
0000079E  8985B0FFFFFF      mov [ebp+0xffffffb0],eax
000007A4  FFB5BCFFFFFF      push dword [ebp+0xffffffbc]
000007AA  FFB5B4FFFFFF      push dword [ebp+0xffffffb4]
000007B0  59                pop ecx
000007B1  58                pop eax
000007B2  01C8              add eax,ecx
000007B4  50                push eax
000007B5  FF3424            push dword [esp]
000007B8  58                pop eax
000007B9  FF30              push dword [eax]
000007BB  68000000FF        push dword 0xff000000
000007C0  59                pop ecx
000007C1  58                pop eax
000007C2  21C8              and eax,ecx
000007C4  50                push eax
000007C5  58                pop eax
000007C6  59                pop ecx
000007C7  898100000000      mov [ecx+0x0],eax
000007CD  FFB5BCFFFFFF      push dword [ebp+0xffffffbc]
000007D3  FFB5B0FFFFFF      push dword [ebp+0xffffffb0]
000007D9  59                pop ecx
000007DA  58                pop eax
000007DB  01C8              add eax,ecx
000007DD  50                push eax
000007DE  FF3424            push dword [esp]
000007E1  58                pop eax
000007E2  FF30              push dword [eax]
000007E4  68000000FF        push dword 0xff000000
000007E9  59                pop ecx
000007EA  58                pop eax
000007EB  21C8              and eax,ecx
000007ED  50                push eax
000007EE  58                pop eax
000007EF  59                pop ecx
000007F0  898100000000      mov [ecx+0x0],eax
000007F6  FFB5BCFFFFFF      push dword [ebp+0xffffffbc]
000007FC  FFB5B4FFFFFF      push dword [ebp+0xffffffb4]
00000802  59                pop ecx
00000803  58                pop eax
00000804  01C8              add eax,ecx
00000806  50                push eax
00000807  FF3424            push dword [esp]
0000080A  58                pop eax
0000080B  FF30              push dword [eax]
0000080D  FFB5E8FFFFFF      push dword [ebp+0xffffffe8]
00000813  59                pop ecx
00000814  58                pop eax
00000815  09C8              or eax,ecx
00000817  50                push eax
00000818  58                pop eax
00000819  59                pop ecx
0000081A  898100000000      mov [ecx+0x0],eax
00000820  FFB5BCFFFFFF      push dword [ebp+0xffffffbc]
00000826  FFB5B0FFFFFF      push dword [ebp+0xffffffb0]
0000082C  59                pop ecx
0000082D  58                pop eax
0000082E  01C8              add eax,ecx
00000830  50                push eax
00000831  FF3424            push dword [esp]
00000834  58                pop eax
00000835  FF30              push dword [eax]
00000837  FFB5E8FFFFFF      push dword [ebp+0xffffffe8]
0000083D  59                pop ecx
0000083E  58                pop eax
0000083F  09C8              or eax,ecx
00000841  50                push eax
00000842  58                pop eax
00000843  59                pop ecx
00000844  898100000000      mov [ecx+0x0],eax
0000084A  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000850  FFB5FCFFFFFF      push dword [ebp+0xfffffffc]
00000856  FFB5F4FFFFFF      push dword [ebp+0xfffffff4]
0000085C  6808000000        push dword 0x8
00000861  59                pop ecx
00000862  58                pop eax
00000863  D3E0              shl eax,cl
00000865  50                push eax
00000866  59                pop ecx
00000867  58                pop eax
00000868  F7E1              mul ecx
0000086A  50                push eax
0000086B  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
00000871  59                pop ecx
00000872  58                pop eax
00000873  99                cdq
00000874  F7F9              idiv ecx
00000876  50                push eax
00000877  59                pop ecx
00000878  58                pop eax
00000879  01C8              add eax,ecx
0000087B  50                push eax
0000087C  58                pop eax
0000087D  8985E0FFFFFF      mov [ebp+0xffffffe0],eax
00000883  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
00000889  FFB5F8FFFFFF      push dword [ebp+0xfffffff8]
0000088F  FFB5F0FFFFFF      push dword [ebp+0xfffffff0]
00000895  6808000000        push dword 0x8
0000089A  59                pop ecx
0000089B  58                pop eax
0000089C  D3E0              shl eax,cl
0000089E  50                push eax
0000089F  59                pop ecx
000008A0  58                pop eax
000008A1  F7E1              mul ecx
000008A3  50                push eax
000008A4  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000008AA  59                pop ecx
000008AB  58                pop eax
000008AC  99                cdq
000008AD  F7F9              idiv ecx
000008AF  50                push eax
000008B0  59                pop ecx
000008B1  58                pop eax
000008B2  01C8              add eax,ecx
000008B4  50                push eax
000008B5  58                pop eax
000008B6  8985DCFFFFFF      mov [ebp+0xffffffdc],eax
000008BC  FFB5B8FFFFFF      push dword [ebp+0xffffffb8]
000008C2  6801000000        push dword 0x1
000008C7  59                pop ecx
000008C8  58                pop eax
000008C9  01C8              add eax,ecx
000008CB  50                push eax
000008CC  58                pop eax
000008CD  8985B8FFFFFF      mov [ebp+0xffffffb8],eax
000008D3  FFB5B8FFFFFF      push dword [ebp+0xffffffb8]
000008D9  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
000008DF  6802000000        push dword 0x2
000008E4  59                pop ecx
000008E5  58                pop eax
000008E6  99                cdq
000008E7  F7F9              idiv ecx
000008E9  50                push eax
000008EA  59                pop ecx
000008EB  58                pop eax
000008EC  39C8              cmp eax,ecx
000008EE  0F8E02FEFFFF      jng near 0x6f6
000008F4  FFB5C0FFFFFF      push dword [ebp+0xffffffc0]
000008FA  58                pop eax
000008FB  3D00000000        cmp eax,0x0
00000900  0F8417000000      jz near 0x91d
00000906  FFB5C0FFFFFF      push dword [ebp+0xffffffc0]
0000090C  FF3424            push dword [esp]
0000090F  68482EEB02        push dword 0x2eb2e48
00000914  E8AB2F0000        call 0x38c4
00000919  50                push eax
0000091A  58                pop eax
0000091B  FFD0              call eax
0000091D  6824430070        push dword 0x70004324
00000922  E87DA10000        call 0xaaa4
00000927  50                push eax
00000928  FFB528000000      push dword [ebp+0x28]
0000092E  5B                pop ebx
0000092F  FF33              push dword [ebx]
00000931  FF3424            push dword [esp]
00000934  681414BF02        push dword 0x2bf1414
00000939  E8862F0000        call 0x38c4
0000093E  50                push eax
0000093F  58                pop eax
00000940  FFD0              call eax
00000942  50                push eax
00000943  58                pop eax
00000944  8985A4FFFFFF      mov [ebp+0xffffffa4],eax
0000094A  89E8              mov eax,ebp
0000094C  2D5C000000        sub eax,0x5c
00000951  50                push eax
00000952  E8ADAC0000        call 0xb604
00000957  50                push eax
00000958  E8A7B40000        call 0xbe04
0000095D  6800000000        push dword 0x0
00000962  58                pop eax
00000963  8985ACFFFFFF      mov [ebp+0xffffffac],eax
00000969  E961010000        jmp 0xacf
0000096E  FFB528000000      push dword [ebp+0x28]
00000974  5B                pop ebx
00000975  FF33              push dword [ebx]
00000977  FFB524000000      push dword [ebp+0x24]
0000097D  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000983  6808000000        push dword 0x8
00000988  59                pop ecx
00000989  58                pop eax
0000098A  D3F8              sar eax,cl
0000098C  50                push eax
0000098D  59                pop ecx
0000098E  58                pop eax
0000098F  01C8              add eax,ecx
00000991  50                push eax
00000992  FFB520000000      push dword [ebp+0x20]
00000998  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
0000099E  6808000000        push dword 0x8
000009A3  59                pop ecx
000009A4  58                pop eax
000009A5  D3F8              sar eax,cl
000009A7  50                push eax
000009A8  59                pop ecx
000009A9  58                pop eax
000009AA  01C8              add eax,ecx
000009AC  50                push eax
000009AD  89E8              mov eax,ebp
000009AF  0508000000        add eax,0x8
000009B4  81EC10000000      sub esp,0x10
000009BA  89C6              mov esi,eax
000009BC  89E7              mov edi,esp
000009BE  B904000000        mov ecx,0x4
000009C3  F3A5              rep movsd
000009C5  FFB42418000000    push dword [esp+0x18]
000009CC  68EC9DCB02        push dword 0x2cb9dec
000009D1  E8EE2E0000        call 0x38c4
000009D6  50                push eax
000009D7  58                pop eax
000009D8  FFD0              call eax
000009DA  FFB528000000      push dword [ebp+0x28]
000009E0  5B                pop ebx
000009E1  FF33              push dword [ebx]
000009E3  FFB51C000000      push dword [ebp+0x1c]
000009E9  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
000009EF  6808000000        push dword 0x8
000009F4  59                pop ecx
000009F5  58                pop eax
000009F6  D3F8              sar eax,cl
000009F8  50                push eax
000009F9  59                pop ecx
000009FA  58                pop eax
000009FB  29C8              sub eax,ecx
000009FD  50                push eax
000009FE  FFB518000000      push dword [ebp+0x18]
00000A04  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
00000A0A  6808000000        push dword 0x8
00000A0F  59                pop ecx
00000A10  58                pop eax
00000A11  D3F8              sar eax,cl
00000A13  50                push eax
00000A14  59                pop ecx
00000A15  58                pop eax
00000A16  29C8              sub eax,ecx
00000A18  50                push eax
00000A19  89E8              mov eax,ebp
00000A1B  0508000000        add eax,0x8
00000A20  81EC10000000      sub esp,0x10
00000A26  89C6              mov esi,eax
00000A28  89E7              mov edi,esp
00000A2A  B904000000        mov ecx,0x4
00000A2F  F3A5              rep movsd
00000A31  FFB42418000000    push dword [esp+0x18]
00000A38  68EC9DCB02        push dword 0x2cb9dec
00000A3D  E8822E0000        call 0x38c4
00000A42  50                push eax
00000A43  58                pop eax
00000A44  FFD0              call eax
00000A46  FFB5E0FFFFFF      push dword [ebp+0xffffffe0]
00000A4C  FFB5FCFFFFFF      push dword [ebp+0xfffffffc]
00000A52  FFB5F4FFFFFF      push dword [ebp+0xfffffff4]
00000A58  6808000000        push dword 0x8
00000A5D  59                pop ecx
00000A5E  58                pop eax
00000A5F  D3E0              shl eax,cl
00000A61  50                push eax
00000A62  59                pop ecx
00000A63  58                pop eax
00000A64  F7E1              mul ecx
00000A66  50                push eax
00000A67  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
00000A6D  59                pop ecx
00000A6E  58                pop eax
00000A6F  99                cdq
00000A70  F7F9              idiv ecx
00000A72  50                push eax
00000A73  59                pop ecx
00000A74  58                pop eax
00000A75  01C8              add eax,ecx
00000A77  50                push eax
00000A78  58                pop eax
00000A79  8985E0FFFFFF      mov [ebp+0xffffffe0],eax
00000A7F  FFB5DCFFFFFF      push dword [ebp+0xffffffdc]
00000A85  FFB5F8FFFFFF      push dword [ebp+0xfffffff8]
00000A8B  FFB5F0FFFFFF      push dword [ebp+0xfffffff0]
00000A91  6808000000        push dword 0x8
00000A96  59                pop ecx
00000A97  58                pop eax
00000A98  D3E0              shl eax,cl
00000A9A  50                push eax
00000A9B  59                pop ecx
00000A9C  58                pop eax
00000A9D  F7E1              mul ecx
00000A9F  50                push eax
00000AA0  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
00000AA6  59                pop ecx
00000AA7  58                pop eax
00000AA8  99                cdq
00000AA9  F7F9              idiv ecx
00000AAB  50                push eax
00000AAC  59                pop ecx
00000AAD  58                pop eax
00000AAE  01C8              add eax,ecx
00000AB0  50                push eax
00000AB1  58                pop eax
00000AB2  8985DCFFFFFF      mov [ebp+0xffffffdc],eax
00000AB8  FFB5ACFFFFFF      push dword [ebp+0xffffffac]
00000ABE  6801000000        push dword 0x1
00000AC3  59                pop ecx
00000AC4  58                pop eax
00000AC5  01C8              add eax,ecx
00000AC7  50                push eax
00000AC8  58                pop eax
00000AC9  8985ACFFFFFF      mov [ebp+0xffffffac],eax
00000ACF  FFB5ACFFFFFF      push dword [ebp+0xffffffac]
00000AD5  FFB5ECFFFFFF      push dword [ebp+0xffffffec]
00000ADB  6802000000        push dword 0x2
00000AE0  59                pop ecx
00000AE1  58                pop eax
00000AE2  99                cdq
00000AE3  F7F9              idiv ecx
00000AE5  50                push eax
00000AE6  59                pop ecx
00000AE7  58                pop eax
00000AE8  39C8              cmp eax,ecx
00000AEA  0F8E7EFEFFFF      jng near 0x96e
00000AF0  C9                leave
00000AF1  C22400            ret 0x24
00000AF4  CC                int3
