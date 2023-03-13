using System;
using System.IO;
using CooS.Architectures;
using System.Runtime.InteropServices;

namespace CooS.Drivers.Controllers {

	public enum DMAIOModes {
		Verify		= 0x00,
		IOToMemory	= 0x04,
		MemoryToIO	= 0x08,
		Forbidden	= 0x0C,
	};

	public class DMAController {

		static DMAController master;
		static DMAController slave;
		static IOPort1[] highaddr = new IOPort1[8];
		static IOPort1 highaddr_ram_refresh;
		IOPort1[] cur_address = new IOPort1[4];
		IOPort1[] cur_counter = new IOPort1[4];
		IOPort1 status;
		IOPort1 command;
		IOPort1 request;
		IOPort1 single_mask;
		IOPort1 mode;
		IOPort1 clear_byte_ptr_flipflop;
		IOPort1 temporary;
		IOPort1 master_clear;
		IOPort1 clear_mask;
		IOPort1 all_mask;

		static DMAController() {
			highaddr[0] = new IOPort1(0x0087);
			highaddr[1] = new IOPort1(0x0083);
			highaddr[2] = new IOPort1(0x0081);
			highaddr[3] = new IOPort1(0x0082);
			highaddr[4] = null;
			highaddr[5] = new IOPort1(0x008B);
			highaddr[6] = new IOPort1(0x0089);
			highaddr[7] = new IOPort1(0x008A);
			highaddr_ram_refresh = new IOPort1(0x008F);
			//Console.WriteLine("Initialized DMAController");
		}

		public static DMAController Master {
			get {
				if(master==null) {
					master = new DMAController(
						0x00C0, 0x00C4, 0x00C8, 0x00CC,
                        0x00C2, 0x00C6, 0x00CA, 0x00CE,
                        0x00D0, 0x00D0, 0x00D2, 0x00D4,
                        0x00D6, 0x00D8, 0x00DA, 0x00DA,
                        0x00DC, 0x00DE);
					// (cascade, verify, ch0), (all mask except for Slave)
					master.Initialize(0xC0, 0x0E);
				}
				return master;
			}
		}

		public static DMAController Slave {
			get {
				if(slave==null) {
					slave = new DMAController(
						0x0000, 0x0002, 0x0004, 0x0006,
                        0x0001, 0x0003, 0x0005, 0x0007,
                        0x0008, 0x0008, 0x0009, 0x000A,
                        0x000B, 0x000C, 0x000D, 0x000D,
						0x000E, 0x000F);
					// (single, verify, ch0), (all mask)
					slave.Initialize(0x40, 0x0F);
				}
				return slave;
			}
		}

		private DMAController(
			ushort cadr0, ushort cadr1, ushort cadr2, ushort cadr3,
			ushort ccnt0, ushort ccnt1, ushort ccnt2, ushort ccnt3,
			ushort psts, ushort pcmd, ushort preq, ushort psmask,
			ushort pmode, ushort pcbpf, ushort ptmp, ushort pmclr,
			ushort pcmask, ushort pamask)
		{
			this.cur_address[0] = new IOPort1(cadr0);
			this.cur_address[1] = new IOPort1(cadr1);
			this.cur_address[2] = new IOPort1(cadr2);
			this.cur_address[3] = new IOPort1(cadr3);
			this.cur_counter[0] = new IOPort1(ccnt0);
			this.cur_counter[1] = new IOPort1(ccnt1);
			this.cur_counter[2] = new IOPort1(ccnt2);
			this.cur_counter[3] = new IOPort1(ccnt3);
			this.status = new IOPort1(psts);
			this.command = new IOPort1(pcmd);
			this.request = new IOPort1(preq);
			this.single_mask = new IOPort1(psmask);
			this.mode = new IOPort1(pmode);
			this.clear_byte_ptr_flipflop = new IOPort1(pcbpf);
			this.temporary = new IOPort1(ptmp);
			this.master_clear = new IOPort1(pmclr);
			this.clear_mask = new IOPort1(pcmask);
			this.all_mask = new IOPort1(pamask);
		}

		private void Initialize(byte mode, byte mask) {
			this.master_clear.Write(0x00);	// master clear (reset)
			this.command.Write(0x00);		// cmd. reg.
			this.mode.Write(mode);			// mode reg.
			this.all_mask.Write(mask);		// all mask reg.
		}

		void EnableChannelInternal(int chno) {
			this.single_mask.Write((byte)chno);
		}
		
		void DisableChannelInternal(int chno) {
			this.single_mask.Write((byte)(chno|0x4));
		}

		private static void EnableChannel(int chno) {
			if(chno<4) {
				Slave.EnableChannelInternal(chno);
			} else {
				Master.EnableChannelInternal(chno-4);
			}
		}

		private static void DisableChannel(int chno) {
			if(chno<4) {
				Slave.DisableChannelInternal(chno);
			} else {
				Master.DisableChannelInternal(chno-4);
			}
		}

		void SetDirection(int chno, DMAIOModes iomode) {
			this.mode.Write((byte)(0x40|chno|(byte)iomode));
		}
		
		void ClearBytePointerFlopflop() {
			this.clear_byte_ptr_flipflop.Write(0x00);
		}

		void SetCurrentAddress(int chno, ushort addr) {
			cur_address[chno].Write((byte)addr);
			cur_address[chno].Write((byte)(addr>>8));
		}

		void SetCurrentCounter(int chno, ushort size) {
			cur_counter[chno].Write((byte)size);
			cur_counter[chno].Write((byte)(size>>8));
		}

		ushort GetCurrentAddressInternal(int chno) {
			return (ushort)(cur_address[chno].Read() | ((ushort)cur_address[chno].Read()<<8));
		}
		
		ushort GetCurrentCounterInternal(int chno) {
			return (ushort)(cur_counter[chno].Read() | ((ushort)cur_counter[chno].Read()<<8));
		}

		public static ushort GetCurrentAddress(int chno) {
			if(chno<4) {
				return Slave.GetCurrentAddressInternal(chno);
			} else if(chno<8) {
				return Master.GetCurrentAddressInternal(chno-4);
			} else {
				throw new ArgumentOutOfRangeException();
			}
		}
		
		public static ushort GetCurrentCounter(int chno) {
			if(chno<4) {
				return Slave.GetCurrentCounterInternal(chno);
			} else if(chno<8) {
				return Master.GetCurrentCounterInternal(chno-4);
			} else {
				throw new ArgumentOutOfRangeException();
			}
		}

		static byte[][] buffers = new byte[8][];
		static GCHandle[] handles = new GCHandle[8];

		public static void BeginTransfer(int chno, DMAIOModes mode, byte[] buf, int index, ushort size) {
			if(buf.Length<index+size) throw new ArgumentException("Buffer is smaller than size.");
			if(size<2) throw new ArgumentException("size is zero or one.");
			DMAController dmac;
			int lchno;
			if(chno<4) {
				dmac = Slave;
				lchno = chno;
			} else if(chno<8) {
				dmac = Master;
				lchno = chno-4;
			} else {
				throw new ArgumentOutOfRangeException();
			}
			DisableChannel(chno);
			buffers[chno] = buf;
			handles[chno] = GCHandle.Alloc(buf);
			int addr = handles[chno].AddrOfPinnedObject().ToInt32()+index;
			if((addr&0xFF000000)!=0) throw new IOException("DMA Address must be less than 16MB");
			dmac.SetDirection(lchno, mode);
			dmac.ClearBytePointerFlopflop();
			dmac.SetCurrentCounter(lchno, (ushort)(size-1));
			dmac.SetCurrentAddress(lchno, (ushort)addr);
			highaddr[chno].Write((byte)(addr>>16));
			/*
			Console.WriteLine("adr0:0x{0:X08}",p.ToInt32());
			Console.WriteLine("adr0:0x00{0:X02}{1:X04}",highaddr[chno].Read(),GetCurrentAddress(chno));
			Console.WriteLine("cnt0:0x{0:X04}",GetCurrentCounter(chno));
			//*/
			EnableChannel(chno);
		}

		public static int EndTransfer(int chno) {
			DisableChannel(chno);
			//memcpy(buffers[chno], getDMABufferAddress(chno), sizes[chno]);
			handles[chno].Free();
			handles[chno] = new GCHandle();
			buffers[chno] = null;
			/*
			Console.WriteLine("adr1:0x{0:X04}",GetCurrentAddress(chno));
			Console.WriteLine("cnt1:0x{0:X04}",GetCurrentCounter(chno));
			*/
			return GetCurrentCounter(chno);
		}

	}
}
