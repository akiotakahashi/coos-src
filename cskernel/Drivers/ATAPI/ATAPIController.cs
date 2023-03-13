using System;
using System.IO;
using CooS.Architectures;
using CooS.Drivers.Controllers;

namespace CooS.Drivers.ATAPI {
	using ATAPI;

	public class ATAPIController {

		const int TIMEOUT = 100000;

		int irq;

		IOPort2 reg_dtr;
		IOPort1 reg_err;
		IOPort1 reg_ftr;	// Feature register
		IOPort1 reg_scr;	// Sector Count register
		IOPort1 reg_irr;	// Interrupt Reason register
		IOPort1 reg_snr;
		IOPort1 reg_clr;
		IOPort1 reg_blr;
		internal IOPort1 reg_chr;
		IOPort1 reg_bhr;
		IOPort1 reg_dhr;
		IOPort1 reg_str;	// Status register
		IOPort1 reg_cmr;	// Command register
		IOPort1 reg_asr;	// Alternative Status register
		IOPort1 reg_dcr;	// Device Control register

		DeviceBase dev0;
		DeviceBase dev1;
		int active_device = -1;

		int mode_pio = -1;
		int mode_dma = -1;

		volatile InterruptState atapi_interrupt_state;
		volatile byte[] atapi_interrupt_buffer;
		volatile int atapi_interrupt_index;
		volatile int atapi_interrupt_limit;
		volatile int atapi_datatransfer;
		volatile byte[] atapi_message;

		public ATAPIController(int irq, int reg, int ctl) {
			this.irq = irq;
			this.reg_dtr = new IOPort2(reg+0);
			this.reg_err = new IOPort1(reg+1);
			this.reg_ftr = new IOPort1(reg+1);
			this.reg_scr = new IOPort1(reg+2);
			this.reg_irr = new IOPort1(reg+2);
			this.reg_snr = new IOPort1(reg+3);
			this.reg_clr = new IOPort1(reg+4);
			this.reg_blr = new IOPort1(reg+4);
			this.reg_chr = new IOPort1(reg+5);
			this.reg_bhr = new IOPort1(reg+5);
			this.reg_dhr = new IOPort1(reg+6);
			this.reg_str = new IOPort1(reg+7);
			this.reg_cmr = new IOPort1(reg+7);
			this.reg_asr = new IOPort1(ctl+0);
			this.reg_dcr = new IOPort1(ctl+0);
			InterruptController.Register(irq, new InterruptHandler(HandleInterrupt));
		}

		public DeviceBase Master {
			get {
				return this.dev0;
			}
		}

		public DeviceBase Slave {
			get {
				return this.dev1;
			}
		}

		private void HandleInterrupt(ref IntPtr sp) {
			InterruptReason irr = (InterruptReason)reg_irr.Read();
			StatusRegister str = (StatusRegister)reg_str.Read();
			//Console.WriteLine("{0}, {1}", irr, str);
			if(str.bsy) {
				//getConsole() << "{ATAPI:INT:BSY}";
			} else if(!str.drq) {
				if(!irr.io && !irr.cd) {
					// �o�X�����[�X
					// �I�[�o�[���b�v�R�}���h���g�p
					atapi_interrupt_state = InterruptState.RELEASE;
				} else if(irr.io && irr.cd) {
					if(!str.drdy) {
						throw new IOException();
					} else {
						// �R�}���h���s�I��
						atapi_interrupt_state = InterruptState.STATUS;
					}
				} else if(!irr.cd && irr.io && str.drdy && !str.err) {
					if(str.df) {
						throw new IOException("{ATAPI:INT:FAULT?}");
					} else {
						throw new IOException("{ATAPI:INT:BREAK?}");
					}
				} else {
					throw new IOException("ATA�R���g���[�����\�����Ȃ���ԂŊ��荞�݂��N�����܂����B");
				}
			} else {
				// �f�[�^�]���v��
				int datasize = ((int)reg_bhr.Read()<<8)|reg_blr.Read();
				//Console.WriteLine("Datasize = {0} (0x{0:X}) bytes", datasize);
				if(irr.cd) {
					if(!irr.io) {
						// �R�}���h�p�P�b�g��M�ҋ@���
						atapi_interrupt_state = InterruptState.COMMAND;
						// �p�P�b�g�R�}���h�͔񊄂荞�݃��[�`�����Ń|�[�����O�ɂ��]��
					} else {
						atapi_message = new byte[datasize];
						ReadData(atapi_message, 0, datasize);
					}
				} else {
					if(!irr.io) {
						// �z�X�g���f�o�C�X�f�[�^�]��
						atapi_interrupt_state = InterruptState.DATAFROMHOST;
						if(datasize>atapi_interrupt_limit) {
							// �]���\��o�C�g�����]���o�C�g���������ꍇ
							throw new IOException("ATA�R���g���[�����\����傫���T�C�Y�̏������݂�v�����܂����B");
						}
						WriteData(atapi_interrupt_buffer, atapi_interrupt_index, datasize);
					} else {
						// �f�o�C�X���z�X�g�f�[�^�]��
						atapi_interrupt_state = InterruptState.DATATOHOST;
						if(atapi_datatransfer+datasize>atapi_interrupt_limit) {
							// �]���\��o�C�g�����]���o�C�g���������ꍇ
							throw new ATAPIException("ATA�R���g���[�����\����傫���T�C�Y�̓ǂݎ���v�����܂����B");
						}
						// Data���W�X�^�ǂݏo��
						ReadData(atapi_interrupt_buffer, atapi_interrupt_index, datasize);
					}
					atapi_interrupt_index += datasize;
					atapi_datatransfer = datasize;
				}
			}
			InterruptController.NotifyEndOfInterrupt(irq);
		}

		public StatusRegister WaitForIdle() {
			for(int l=0; l<TIMEOUT; l++){
				StatusRegister asr = (StatusRegister)reg_asr.Read();
				if(asr.bsy) continue;
				return asr;
			}
			throw new IOException("ATA�R���g���[�����r�W�[����񕜂��܂���B");
		}

		public StatusRegister WaitForSuccess(string errmsg) {
			for(int l=0; l<TIMEOUT; l++){
				StatusRegister asr = (StatusRegister)reg_asr.Read();
				if(asr.bsy) continue;
				if(asr.err) {
					throw new ATAPIException(errmsg);
				}
				return asr;
			}
			throw new IOException("ATA�R���g���[�����r�W�[����񕜂��܂���B");
		}

		public StatusRegister WaitForSuccess() {
			return this.WaitForSuccess("ATA�R���g���[�����G���[��񍐂��܂����B");
		}

		public void WaitToCommand() {
			StatusRegister asr = new StatusRegister();
			for(int l=0; l<TIMEOUT; l++) {
				asr = (StatusRegister)reg_asr.Read();
				if(asr.bsy) continue;
				if(asr.drq) continue;
				if(!asr.drdy) continue;
				return;
			}
			throw new ATAPITimeoutException(asr);
		}

		public StatusRegister WaitForData() {
			StatusRegister asr = new StatusRegister();
			for(int l=0; l<TIMEOUT; l++) {
				asr = (StatusRegister)reg_asr.Read();
				if(asr.bsy) continue;
				if(asr.err) throw new ATAPIException("�f�[�^�]�����̃G���[�ł��B");
				if(!asr.drq) continue;
				return asr;
			}
			throw new ATAPITimeoutException(asr);
		}

		void Reset() {
			this.reg_dcr.Write(6);	// SRST & nIEN(Neg. Interrupt Enabled)
			Kernel.Delay(0,30,0);	// >25us, Wait for software reset
			this.reg_dcr.Write(2);	// nIEN
			Kernel.Delay(20,0,0);	// >20ms
			this.WaitForIdle();
		}

		bool IsDeviceConnected(int devnum) {
			lock(this) {
				this.active_device = -1;
				reg_dhr.Write(devnum<<4);
				Kernel.Delay(0,0,400);
				if(0xFF==reg_str.Read() || 0xFF==reg_str.Read()) {
					/* ���ڑ��Ɣ��� */
					return false;
				}
				try {
					this.WaitForIdle();
					this.active_device = devnum;
					return ((reg_dhr.Read()>>4)&1)==devnum;
				} catch {
					return false;
				}
			}
		}

		DeviceBase CreateDeviceIfConnected(int devnum) {
			if(!IsDeviceConnected(devnum)) {
				return null;
			} else {
				// �f�o�C�X�V�O�l�`���擾
				byte s0l = reg_clr.Read();
				byte s0h = reg_chr.Read();
				Console.WriteLine("Device #{0} Signature: {1:X2} {2:X2}", devnum, s0l, s0h);
				//
				DeviceBase device;
				if ((s0l==0)&&(s0h==0)) {
					// �f�o�C�Xn��ATA�f�o�C�X�̉\������
					return null;	// �ǂ���ATA�Ή����ĂȂ����B
					//device = new ATADevice(this, devnum);
				} else if ((s0l==0x14)&&(s0h==0xEB)) {
					this.WaitForIdle();
					// �f�o�C�Xn��ATAPI�f�o�C�X�̉\������
					device = new ATAPIDevice(this, devnum);
				} else {
					return null;
				}
				// �f�o�C�X�m��
				try {
					this.WaitForIdle();
					Console.WriteLine("VALIDATE: "+(StatusRegister)this.reg_asr.Read());
					device.ValidateDeviceType();
					return device;
				} catch {
					return null;
				}
			}
		}

		public void InitializeDevice() {
			// ���Z�b�g���Ȃ��ƃf�o�C�X�V�O�j�`���͑����Ă��Ȃ�
			Console.WriteLine("Resetting...");
			Reset();
			// �f�o�C�X�ڑ��`�F�b�N
			this.dev0 = CreateDeviceIfConnected(0);
			Console.WriteLine("Master Device Connected: {0}", dev0!=null);
			this.dev1 = CreateDeviceIfConnected(1);
			Console.WriteLine("Slave Device Connected : {0}", dev1!=null);
			if(dev0==null && dev1==null) return;
			//
			if(dev0!=null) {
				this.SelectDevice(0, true);
			} else {
				this.SelectDevice(1, true);
			}
			// ���[�h�̐ݒ�
			if(dev1==null) {
				this.mode_pio = dev0.PIOMode;
				this.mode_dma = dev0.DMAMode;
			} else if(dev0==null) {
				this.mode_pio = dev1.PIOMode;
				this.mode_dma = dev1.DMAMode;
			} else {
				this.mode_pio = Math.Min(dev0.PIOMode, dev1.PIOMode);
				this.mode_dma = Math.Min(dev0.DMAMode, dev1.DMAMode);
			}
			Console.WriteLine("Controller Modes: PIO={0}, DMA={1}", mode_pio, mode_dma);
			// �f�o�C�X���[�h������
			if(this.dev0!=null) {
				this.SelectDevice(0, true);
				dev0.SetTransferMode(this.mode_pio, this.mode_dma);
			}
			if(this.dev1!=null) {
				this.SelectDevice(1, true);
				dev1.SetTransferMode(this.mode_pio, this.mode_dma);
			}
		}

		public void SelectDevice(int devnum, bool force) {
			if(this.active_device!=devnum) {
				if(!force) this.WaitToCommand();
				reg_dhr.Write(devnum << 4);
				Kernel.Delay(0,400,0);
				this.WaitToCommand();
				this.active_device = devnum;
			}
		}

		public void WriteCommand(int cmd) {
			reg_cmr.Write(cmd);
			Kernel.Delay(0,0,400);		// 400ns�E�F�C�g
		}

		public void SendCommand(int command, int feature, int sectorcount, int sectornum, int cylinder, bool waitDRDY) {
			if(waitDRDY) this.WaitToCommand();
			reg_dcr.Write(0x2);				/* ���荞�ݖ��g�p */
			reg_ftr.Write(feature);			/* �t�B�[�`���[ */
			reg_scr.Write(sectorcount);		/* �Z�N�^�J�E���g */
			reg_snr.Write(sectornum);		/* �Z�N�^�i���o */
			reg_clr.Write(cylinder&0xFF);	/* �V�����_Lo */
			reg_chr.Write(cylinder>>8);		/* �V�����_Hi */
			WriteCommand(command);
		}

		public int SendPacketCommand(byte[] buf, int index, int feature, int limit, byte[] atapi_cmd) {
			if(limit>0xFFFF) throw new ArgumentOutOfRangeException("limit");
			// �R�}���h���M
			this.WaitToCommand();
			atapi_interrupt_state = InterruptState.IDLE;
			reg_dcr.Write(0);				// ���荞�ݎg�p
			reg_ftr.Write(feature);			// Overlapped I/O
			reg_scr.Write(0);
			reg_blr.Write(limit&0xff);		// �o�C�g�J�E���g low
			reg_bhr.Write(limit>>8);		// �o�C�g�J�E���g high
			this.WriteCommand(0xA0);		// PACKET�R�}���h
			this.WaitForSuccess("PACKET�R�}���h���s�G���[");
			// �R�}���h���s�I��
			// PACKET �R�}���h�ł́A�R�}���h�f�[�^���M�J�n��DRQ�Ŕ��f����B
			for(int i=0; i<TIMEOUT; ++i) {
				StatusRegister asr = this.WaitForSuccess("PACKET�R�}���h���s�G���[");
				InterruptReason irr = (InterruptReason)reg_irr.Read();
				if(asr.drq && !irr.io && irr.cd) break;
			}
			// �p�P�b�g�R�}���h���s
			atapi_interrupt_state = InterruptState.IDLE;
			atapi_interrupt_buffer = buf;
			atapi_interrupt_limit = limit;
			atapi_interrupt_index = index;
			atapi_datatransfer = 0;
			if(atapi_cmd.Length!=12) throw new ArgumentException();
			//Console.WriteLine("Send packet data to device.");
			WriteData(atapi_cmd, 0, atapi_cmd.Length);
			// �p�P�b�g�R�}���h���s�I���҂�
			this.WaitForSuccess("ATAPI�p�P�b�g�R�}���h�����s���܂���");
			while(atapi_interrupt_state!=InterruptState.STATUS) {
				switch(atapi_interrupt_state) {
				case InterruptState.IDLE:
				case InterruptState.DATATOHOST:
					Kernel.Delay(0,1,0);
					break;
				case InterruptState.STATUS:
					break;
				default:
					throw new ATAPIException("Unexpected interrupt state: "+(int)atapi_interrupt_state);
				}
			}
			//Console.WriteLine("Recognized completion of command");
			// �p�P�b�g�R�}���h����I��
			atapi_interrupt_state = InterruptState.IDLE;
			return this.atapi_datatransfer;
		}

		public void DiscardData() {
			int count = 0;
			for(;;) {
				this.WaitForIdle();
				StatusRegister status = (StatusRegister)this.reg_asr.Read();
				if(!status.drq) {
					break;
				}
				++count;
				this.reg_dtr.Read();
			}
			if(count>0) Console.WriteLine("DISCARDED {0} BYTES", count*2);
		}

		public void ReadBlocks(byte[] buf, int count) {
			/* �ǂݏo���u���b�N�����[�v */
			for(int i=0; i<count; i++) {
				/* �u���b�N�ǂݏo��(256���[�h) */
				this.ReadData(buf, i*512, 512);
			}
		}

		unsafe void ReadData(byte[] buf, int index, int size) {
			if((size&1)!=0) throw new ArgumentException("size must be scaled by 2: "+size);
			if(index<0 || index+size>buf.Length) throw new ArgumentOutOfRangeException();
			//*
			fixed(byte* p = &buf[index]) {
				CooS.Architectures.IA32.Instruction.inw(this.reg_dtr.Port, p, size>>1
					, this.reg_asr.Port, (byte)(StatusBits.BSY|StatusBits.DRQ),(byte)StatusBits.DRQ);
			}
			//*/
			/*
			for(int i=0; i<size; i+=2) {
				this.WaitForData();
				ushort value = this.reg_dtr.Read();
				buf[index+i+0] = (byte)value;
				buf[index+i+1] = (byte)(value>>8);
			}
			//*/
		}

		void WriteData(byte[] buf, int index, int size) {
			if(index<0 || index+size>buf.Length) throw new ArgumentOutOfRangeException();
			if((size&1)!=0) throw new ArgumentException();
			for(int i=0; i<size; i+=2) {
				this.WaitForData();
				reg_dtr.Write(BitConverter.ToUInt16(buf, index+i));
			}
		}

		//**********************************************************************************************
		//**********************************************************************************************
		//
		//		�������牺�͎d�l�����Ă����ƍ�����B
		//
		//**********************************************************************************************
		//**********************************************************************************************

		[Flags]
		enum StatusBits : byte {
			ERR		= 0x01,
			DRQ		= 0x08,
			DF		= 0x20,
			DRDY	= 0x40,
			BSY		= 0x80,
		}

		[Flags]
		enum InterruptReasonBits : byte {
			CoD		= 0x01,
			IO		= 0x02,
			REL		= 0x04,
		}

		private StatusBits WaitStatus(StatusBits mask, StatusBits value) {
			StatusBits reg = 0;
			for(int i=0; i<TIMEOUT; ++i) {
				reg = (StatusBits)this.reg_asr.Read();
				if(value==(reg&mask)) {
					return reg;
				}
			}
			throw new ATAPITimeoutException((StatusRegister)(byte)reg);
		}

		private InterruptReasonBits WaitInterruptReason(InterruptReasonBits mask, InterruptReasonBits value) {
			InterruptReasonBits reg = 0;
			for(int i=0; i<TIMEOUT; ++i) {
				reg = (InterruptReasonBits)this.reg_irr.Read();
				if(value==(reg&mask)) {
					return reg;
				}
			}
			throw new ATAPIException("InterruptReason: "+(InterruptReason)(byte)reg);
		}

		public int ReadNonOverlapDMAData(bool dmadir, byte[] atapi_cmd, byte[] buf, int index, ushort size) {
			// [ATAPI SFF8020i] Flow of Non-overlap DMA Data commands
			if(atapi_cmd.Length!=12) throw new ArgumentException();

			/// 1. The host Polls for BSY=0, DRQ=0 then initializes the task file by writing the required parameters to the Features,
			/// Byte Count, and Drive/Head registers. The host must also initializes the DMA engine which will service the Devices
			/// requests.
			this.WaitStatus(StatusBits.BSY|StatusBits.DRQ, 0);
			this.reg_dcr.Write(0);				// ���荞�ݎg�p
			this.reg_ftr.Write(dmadir ? 5 : 1);	// DMA, Non-overlap, [opt] Transfer to the host
			this.reg_blr.Write(size&0xff);		// �o�C�g�J�E���g low
			this.reg_bhr.Write(size>>8);		// �o�C�g�J�E���g high
			
			/// 2. The host writes the Packet Command code (A0h) to the Command Register.
			this.WriteCommand(0xA0);
			
			/// 3. The Device sets BSY and prepares for Command Packet transfer.
			this.WaitInterruptReason(InterruptReasonBits.CoD|InterruptReasonBits.IO, InterruptReasonBits.CoD);
			
			/// 4. When the Device is ready to accept the Command Packet, the Device sets CoD and clears IO. DRQ shall then be
			/// asserted simultaneous or prior to the de-assertion of BSY. Some Devices will assert INTRQ following the assertion
			/// of DRQ. See section 7.1.7.1, "General Configuration Word (0)", on page 63 for command packet DRQ types
			/// and other related timing information.
			StatusRegister status = (byte)this.WaitStatus(StatusBits.BSY, 0);
			if(!status.drq) throw new ATAPIException("DRQ must be set to 1");
			
			/// 5. After detecting DRQ, the host writes the 12 bytes (6 words) of Command to the Data Register.
			this.WriteData(atapi_cmd, 0, atapi_cmd.Length);
			
			/// 6. The Device(1) clears DRQ (when the 12th byte is written), (2) sets BSY, (3) reads Features and Byte Count requested
			/// by the host system, (4) prepares for data transfer.
			this.WaitStatus(StatusBits.DRQ|StatusBits.BSY, StatusBits.BSY);

			/// 7. When ready to transfer data, the Device transfers via DMARQ/DMACK any amount that the Device can accommodate
			/// or has in its buffers at this time. This continues until all the data has been transferred.
			CooS.Drivers.Controllers.DMAController.BeginTransfer(0, DMAIOModes.IOToMemory, buf, index, size);

			/// 8. When the Device is ready to present the status, the Device places the completion status into the Status Register,
			/// and sets IO, CoD, DRDY and clears BSY, DRQ, prior to asserting INTRQ.
			status = (byte)this.WaitStatus(StatusBits.DRQ|StatusBits.BSY, 0);
			InterruptReason ir = this.reg_irr.Read();
			int iorest =CooS.Drivers.Controllers.DMAController.EndTransfer(0);
	
			//
			return size-iorest;
		}

	}

}
