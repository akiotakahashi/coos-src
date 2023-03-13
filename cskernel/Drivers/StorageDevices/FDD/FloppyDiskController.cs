using System;
using CooS;
using CooS.FileSystem;
using CooS.Architectures;
using CooS.Architectures.IA32;
using CooS.Drivers.Controllers;

namespace CooS.Drivers.StorageDevices.FDD {

	public class FloppyDiskController {
		
		const byte FDC_MSR_D0B			= 0x01;
		const byte FDC_MSR_D1B			= 0x02;
		const byte FDC_MSR_D2B			= 0x04;
		const byte FDC_MSR_D3B			= 0x08;
        const byte FDC_MSR_CB			= 0x10;
		const byte FDC_MSR_NDM			= 0x20;
		const byte FDC_MSR_DATA_READ	= 0x40;
		const byte FDC_MSR_RQM			= 0x80;

		static FloppyDiskController master;
		static FloppyDiskController slave;
		static bool InterruptFlag;
		IOPort1 dorp;
		IOPort1 msrp;
		IOPort1 dsrp;
		IOPort1 ccrp;
		IOPort1 datap;

		static void HandleInterrupt(ref IntPtr sp) {
			InterruptFlag = true;
			InterruptController.NotifyEndOfInterrupt(6);
		}

		static FloppyDiskController() {
			InterruptHandler handler = new InterruptHandler(HandleInterrupt);
			InterruptController.Register(6, handler);
			InterruptController.LetEnabled(6, true);
			/*
			fdc0 = new FloppyDiskController();
			if(!fdc0->InitializeAsMaster()) {
				console << "Failed to initialize FDC#0" << endl;
			} else {
				console << "Detecte FD Drives" << endl;
				fdc0->DetectDrives();
			}
			*/
			/*
			fdc1 = new FloppyDiskController();
			if(!fdc1->InitializeAsSlave()) {
				getConsole() << "Failed to initialize FDC#1" << endl;
			} else {
				fdc1.DetectDrives();
			}
			*/
			//Console.WriteLine("Initialized FloppyDiskController");
		}

		public static FloppyDiskController Master {
			get {
				if(master==null) {
					master = new FloppyDiskController(0x3f2,0x3f4,0x3f4,0x3f7,0x3f5);
				}
				return master;
			}
		}
		
		public static FloppyDiskController Slave {
			get {
				if(slave==null) {
					slave = new FloppyDiskController(0x372,0x374,0x374,0x377,0x375);
				}
				return slave;
			}
		}
		
		public FloppyDiskController(ushort dor, ushort msr, ushort dsr, ushort ccr, ushort data) {
			this.dorp = new IOPort1(dor);
			this.msrp = new IOPort1(msr);
			this.dsrp = new IOPort1(dsr);
			this.ccrp = new IOPort1(ccr);
			this.datap = new IOPort1(data);
		}

		bool checkStatus(byte mask, byte expected) {
			return expected==(msrp.Read()&mask);
		}

		public bool RequiredToRead {
			get {
				return checkStatus(FDC_MSR_DATA_READ,FDC_MSR_DATA_READ);
			}
		}

		public bool ReadyToSend {
			get {
				return checkStatus(FDC_MSR_RQM|FDC_MSR_DATA_READ,FDC_MSR_RQM);
			}
		}

		void discardData() {
			while(RequiredToRead) {
				datap.Read();
				Console.Write(".");
			}
		}

		bool waitStatus(byte mask, byte expected) {
			int limit = 10;
			while(!checkStatus(mask,expected)) {
				Kernel.Delay(0,1,0);
			}
			return limit>0;
		}

		private bool waitForIdle() {
			discardData();
			return waitStatus(FDC_MSR_RQM|FDC_MSR_DATA_READ|FDC_MSR_CB,FDC_MSR_RQM);
		}

		public void SelectDrive(int drvno) {
			if(drvno<0 || 3<drvno) throw new ArgumentOutOfRangeException();
			byte dor = dorp.Read();
			dor &= unchecked((byte)~3);
			dor |= (byte)drvno;
			dorp.Write(dor);
		}

		public bool Seek(uint cylinder) {
			byte[] command = new byte[]{0x0F,0,(byte)cylinder};
			InterruptFlag = false;
			if(!waitForIdle()) {
				return false;
			} else if(!sendCommand(command)) {
				discardData();
				return false;
			} else {
				while(!InterruptFlag);
				return SenseInterrupt();
			}
		}

		bool sendCommand(byte[] command) {
			for(int i=0; i<command.Length; /* Don't increment here */) {
				if(this.ReadyToSend) {
					this.datap.Write(command[i++]);
				} else if(RequiredToRead) {
					return false;
				}
			}
			return true;
		}

		byte getResult() {
			if(!RequiredToRead) {
				Console.Write("{FDC-REPLY-TIMEOUT}");
				return 0xFF;
			} else {
				return this.datap.Read();
			}
		}
	
		bool SenseInterrupt() {
			byte[] command = new byte[]{0x08};
			if(!waitForIdle()) {
				return false;
			} else if(!sendCommand(command)) {
				discardData();
				return false;
			} else {
				ST0 st0;
				byte pcn;
				st0.Value = getResult();	/* ST0 */
				pcn = getResult();			/* PCN */
				return st0.ic==00;
			}
		}

		public ReadDataResult ReadData(byte[] buf, int index, int drvno, byte cylinder, byte head, byte sector, uint count) {
			if(count==0) {
				ReadDataResult result;
				result.st0.Value = 0x0;
				result.st1 = 0;
				result.st2 = 0;
				result.c = cylinder;
				result.h = head;
				result.r = sector;
				result.n = 2;
				return result;
			} else {
				SelectDrive(drvno);
				byte[] command = new byte[]{
											   0xE6,
											   (byte)(head<<2 | drvno),
											   cylinder,					// C
											   head,						// H
											   (byte)(sector+1),			// R
											   0x02,						// N
											   (byte)(sector+1+count-1),	// EOT
											   0x1B,						// GSL
											   0xFF							// DTL
										   };
				InterruptFlag = false;
				if(!waitForIdle()) {
					ReadDataResult result = new ReadDataResult();
					result.st0.us = drvno;
					result.st0.ic = 0x1;
					return result;
				} else {
					// transfer
					DMAController.BeginTransfer(2, DMAIOModes.IOToMemory, buf, index, (ushort)(count*512));
					if(!sendCommand(command)) {
						DMAController.EndTransfer(2);
						discardData();
						ReadDataResult result = new ReadDataResult();
						result.st0.us = drvno;
						result.st0.ic = 0x1;
						return result;
					} else {
						while(!InterruptFlag) {
							Kernel.Delay(0,1,0);
						}
						DMAController.EndTransfer(2);
						// receive result data
						ReadDataResult result;
						result.st0.Value = getResult();
						result.st1 = getResult();
						result.st2 = getResult();
						result.c = getResult();
						result.h = getResult();
						result.r = getResult();
						result.n = getResult();
						return result;
					}
				}
			}
		}

	}

}
