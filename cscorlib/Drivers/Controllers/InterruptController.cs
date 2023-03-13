using System;
using CooS.Management;
using CooS.Architectures;

namespace CooS.Drivers.Controllers {

	/// <summary>
	/// InterruptController の概要の説明です。
	/// </summary>
	public class InterruptController {

		static InterruptController master;
		static InterruptController slave;
		byte baseintno;
		IOPort1 ctrlp;
		IOPort1 datap;

		static InterruptController() {
			// Don't reset interrupt mask because it's already initialized by kernel.
			master	= new InterruptController(0x0020,0x0021,0x60,4);
			slave	= new InterruptController(0x00A0,0x00A1,0x68,2);
			//Console.WriteLine("Initialized InterruptController");
		}

		public static InterruptController Master {
			get {
				return master;
			}
		}

		public static InterruptController Slave {
			get {
				return slave;
			}
		}

		private InterruptController(ushort irr, ushort imr, byte int_vector_base, byte icw3) {
			this.ctrlp = new IOPort1(irr);
			this.datap = new IOPort1(imr);
			/*
			// specify ICW1 and need ICW4
			ctrlp.Write(0x10 | 0x01);
			datap.Write(int_vector_base);
			datap.Write(icw3);
			datap.Write(0x01);
			*/
			baseintno = int_vector_base;
		}

		byte BaseInterruptNumber {
			get {
				return baseintno;
			}
		}

		byte Imr {
			get {
				return datap.Read();
			}
			set {
				datap.Write(value);
			}
		}

		byte Ocw1 {
			set {
				datap.Write(value);
			}
		}

		byte Ocw2 {
			set {
				ctrlp.Write(value);
			}
		}

		byte Ocw3 {
			set {
				ctrlp.Write((byte)(value|0x08));
			}
		}

		public static bool LetEnabled(int irq, bool enabled) {
			if(irq<0 || 15<irq) throw new ArgumentOutOfRangeException("irq");
			int prev = 0;
			if(irq<8) {
				irq = 1<<irq;
				prev = master.Imr&irq;
				if(enabled) {
					lock(master) {
						master.Imr &= (byte)~irq;
					}
				} else {
					lock(master) {
						master.Imr |= (byte)irq;
					}
				}
			} else if(irq<16) {
				irq = 1<<(irq-8);
				prev = slave.Imr&irq;
				if(enabled) {
					lock(slave) {
						slave.Imr &= (byte)~irq;
					}
				} else {
					lock(slave) {
						slave.Imr |= (byte)irq;
					}				
				}
			}
			return 0==prev;
		}

		public static void NotifyEndOfInterrupt(int irq) {
			if(irq<0) {
				throw new ArgumentOutOfRangeException("irq");
			} else if(irq<8) {
				// 通常モードにおいて、指定された割り込みを終了させる。
				master.Ocw2 = (byte)(0x60|irq);
			} else if(irq<16) {
				// 通常モードにおいて、指定された割り込みを終了させる。
				irq -= 8;
				master.Ocw2 = 0x62;
				slave.Ocw2 = (byte)(0x60|irq);
			} else {
				throw new ArgumentOutOfRangeException();
			}
		}

		public static void Register(int irq, InterruptHandler handler) {
			if(irq<0) {
				throw new ArgumentOutOfRangeException("irq");
			} else if(irq<8) {
				irq += master.BaseInterruptNumber;
			} else if(irq<16) {
				irq += slave.BaseInterruptNumber-8;
			} else {
				throw new ArgumentOutOfRangeException("irq",irq,"0 <= irq < 16");
			}
			InterruptManager.Register(irq, handler);
		}

		public static void MakeInterrupt(int irq) {
			if(irq<0 || 15<irq) throw new ArgumentOutOfRangeException();
			if(irq<8) {
				irq += master.BaseInterruptNumber;
			} else {
				irq -= 8;
				irq += slave.BaseInterruptNumber;
			}
			CooS.Architectures.IA32.Instruction.intn(irq);
		}

	}
}
