using System;
using CooS.Reflection;
using System.Collections.Generic;

namespace CooS.Execution {

	public struct Address {

		public readonly TypeInfo Type;
		public readonly int Value;

		public bool OnHeap {
			get {
				return 0==(0x80000000&(uint)this.Value);
			}
		}

		public bool OnStack {
			get {
				return 0!=(0x80000000&(uint)this.Value);
			}
		}

		public int Index {
			get {
				return 0xFFFF&this.Value;
			}
		}

		public int Offset {
			get {
				return 0x7FFF&(this.Value>>16);
			}
		}

		public Address(int address, TypeInfo type) {
			this.Type = type;
			this.Value = address;
		}

		private Address(bool stack, int index, int offset, TypeInfo type) {
			if(index<0 || 0xFFFF<index) throw new ArgumentOutOfRangeException("index");
			if(offset<0 || 0x7FFF<index) throw new ArgumentOutOfRangeException("offset");
			this.Type = type;
			this.Value = (offset<<16) | index;
			if(stack) this.Value |= unchecked((int)0x80000000);
		}

		public static Address AtHeap(int index, int offset, TypeInfo type) {
			return new Address(false, index, offset, type);
		}

		public static Address AtHeap(int index, TypeInfo type) {
			return new Address(false, index, 0, type);
		}

		public static Address AtStack(int index, int offset, TypeInfo type) {
			return new Address(true, index, offset, type);
		}

		public static Address AtStack(int index, TypeInfo type) {
			return new Address(true, index, 0, type);
		}

	}

}
