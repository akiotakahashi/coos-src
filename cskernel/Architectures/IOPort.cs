using System;

namespace CooS.Architectures {

	public sealed class IOPort1 {
		
		ushort port;

		public IOPort1(int port) {
			if(port<0 || 0xFFFF<port) throw new ArgumentOutOfRangeException();
			this.port = (ushort)port;
		}
		
		public ushort Port {
			get {
				return port;
			}
		}
	
		public void Write(byte data) {
			IA32.Instruction.outb(port, data);
		}
	
		public void Write(int data) {
			IA32.Instruction.outb(port, (byte)data);
		}
				
		public byte Read() {
			return IA32.Instruction.inb(port);
		}

	}

	public sealed class IOPort2 {
		
		ushort port;

		public IOPort2(int port) {
			if(port<0 || 0xFFFF<port) throw new ArgumentOutOfRangeException();
			this.port = (ushort)port;
		}
		
		public ushort Port {
			get {
				return port;
			}
		}
			
		public void Write(ushort data) {
			IA32.Instruction.outw(port, data);
		}
		
		public void Write(int data) {
			IA32.Instruction.outw(port, (ushort)data);
		}
		
		public ushort Read() {
			return IA32.Instruction.inw(port);
		}

	}

	public sealed class IOPort4 {
		
		ushort port;

		public IOPort4(int port) {
			if(port<0 || 0xFFFF<port) throw new ArgumentOutOfRangeException();
			this.port = (ushort)port;
		}
		
		public ushort Port {
			get {
				return port;
			}
		}
	
		public void Write(uint data) {
			IA32.Instruction.outd(port, data);
		}
	
		public void Write(int data) {
			IA32.Instruction.outd(port, (uint)data);
		}
		
		public uint Read() {
			return IA32.Instruction.ind(port);
		}

	}

}
