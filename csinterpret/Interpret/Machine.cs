using System;
using System.Collections.Generic;
using CooS.Reflection;
using CooS.Execution;

namespace CooS.Interpret {

	public class Machine {

#if true

		public sealed class HeapManager {

			private readonly Machine machine;
			private readonly List<Block> memory = new List<Block>();

			public HeapManager(Machine machine) {
				this.machine = machine;
			}

			public Address AllocateObject(TypeInfo type) {
				byte[] buf = new byte[type.InstanceSize];
				Block block;
				block.Type = type;
				block.Data = buf;
				memory.Add(block);
				return Address.AtHeap(memory.Count-1, type);
			}

			public Address AllocateString(TypeInfo type, int length) {
				throw new NotImplementedException();
			}

			public Address AllocateString(TypeInfo type, string s) {
				throw new NotImplementedException();
			}

			public Address AllocateArray(TypeInfo type, int length) {
				byte[] buf = new byte[this.machine.Engine.SzArrayOffsetToContents+type.VariableSize*length];
				this.machine.Engine.MakeSzArrayHeader(buf, type, length);
				Block block;
				block.Type = this.machine.Engine.Realize(type.MakeSzArrayType());
				block.Data = buf;
				memory.Add(block);
				return Address.AtHeap(memory.Count-1, block.Type);
			}

			public Block this[Address adr] {
				get {
					if(!adr.OnHeap) throw new ArgumentException();
					if(adr.Offset!=0) throw new ArgumentException();
					return this.memory[adr.Index];
				}
			}

			public void Dump() {
				Console.WriteLine("-----> Heap Dump");
				int i = 0;
				foreach(Block block in this.memory) {
					Console.WriteLine("[{0,3}] {1}", i++, block);
				}
			}

		}

		public sealed class StackManager {
	
			private Machine Machine;
			private readonly List<Block> stack = new List<Block>();

			public StackManager(Machine machine) {
				this.Machine = machine;
			}

			public Block this[int index] {
				get {
					return this.stack[this.stack.Count-1-index];
				}
			}

			public int BasePointer {
				get {
					return this.stack.Count-1;
				}
			}

			public Block this[int bp, int index] {
				get {
					return this.stack[bp-index];
				}
			}

			public Address GetAddress(int index) {
				return Address.AtStack(this.stack.Count-1-index, this[index].Type);
			}

			public Block this[Address adr] {
				get {
					if(!adr.OnStack) throw new ArgumentException();
					if(adr.Offset!=0) throw new ArgumentException();
					return this.stack[adr.Index];
				}
			}

			public Block Pop() {
				int i = this.stack.Count-1;
				Block blk = this.stack[i];
				this.stack.RemoveAt(i);
				return blk;
			}

			public void Pop(int count) {
				this.stack.RemoveRange(this.stack.Count-count, count);
			}

			public void Push(TypeInfo type, byte[] data, bool copy) {
				Block block;
				block.Type = type;
				if(data==null) {
					block.Data = null;
				} else if(copy) {
					block.Data = new byte[data.Length];
					Buffer.BlockCopy(data, 0, block.Data, 0, data.Length);
				} else {
					block.Data = data;
				}
				this.stack.Add(block);
			}

			public void Push(Block block, bool copy) {
				this.Push(block.Type, block.Data, copy);
			}

			public void Push(int i4) {
				this.Push(this.Machine.Engine.Resolve(PrimitiveTypes.I4), BitConverter.GetBytes(i4), false);
			}

			public void Push(long i8) {
				this.Push(this.Machine.Engine.Resolve(PrimitiveTypes.I8), BitConverter.GetBytes(i8), false);
			}

			public void Push(float r4) {
				this.Push(this.Machine.Engine.Resolve(PrimitiveTypes.R4), BitConverter.GetBytes(r4), false);
			}

			public void Push(double r8) {
				this.Push(this.Machine.Engine.Resolve(PrimitiveTypes.R8), BitConverter.GetBytes(r8), false);
			}

			public void Push(Address adr) {
				this.Push(adr.Type, BitConverter.GetBytes(adr.Value), false);
			}

			public void PushAdr(Address adr) {
				this.Push(this.Machine.Engine.Resolve(PrimitiveTypes.I), BitConverter.GetBytes(adr.Value), false);
			}

			public void Dump() {
				Console.WriteLine("-----> Stack Dump");
				int i = 0;
				foreach(Block block in this.stack) {
					Console.WriteLine("[{0}] {1}", i++, block);
				}
			}

		}

		public readonly World World;
		public readonly Engine Engine;
		public readonly Dictionary<string,Interpreter> interpreters = new Dictionary<string,Interpreter>();
		public readonly HeapManager Heap;
		public readonly StackManager Stack;

		public Machine(Engine engine) {
			this.World = engine.World;
			this.Engine = engine;
			this.Heap = new HeapManager(this);
			this.Stack = new StackManager(this);
			this.interpreters.Add("CLI", new CLI.InterpreterImpl(this));
		}

		public unsafe void Run(AssemblyBase assembly, string[] args) {
			MethodBase entrypoint = assembly.EntryPoint;
			if(args==null) {
				args = new string[0];
			}
			TypeInfo strtype = this.Engine.Realize(this.World.Resolve(PrimitiveTypes.String));
			Address arr = this.Heap.AllocateArray(strtype, args.Length);
			List<Address> l = new List<Address>();
			int offset = this.Engine.SzArrayOffsetToContents;
			foreach(string arg in args) {
				Address str = this.Heap.AllocateString(strtype, arg);
				fixed(byte* p = &this.Heap[arr].Data[offset]) {
					*(int*)p = str.Value;
				}
				offset += Architecture.Target.AddressSize;
			}
			this.Stack.Push(arr);
			this.interpreters["CLI"].Execute(entrypoint);
		}

#else

		private byte[] memory;
		private int offset;

		public Machine() {
			memory = new byte[64*1024*1024];
			offset = 0;
		}

		public sbyte get1(int address) {
			return (sbyte)memory[address-offset];
		}

		public short get2(int address) {
			return BitConverter.ToInt16(memory, address-offset);
		}

		public int get4(int address) {
			return BitConverter.ToInt32(memory, address-offset);
		}

		public long get8(int address) {
			return BitConverter.ToInt64(memory, address-offset);
		}

		public void put(int address, sbyte value) {
			memory[address-offset] = (byte)value;
		}

		public void put(int address, byte value) {
			memory[address-offset] = value;
		}

		public void put(int address, short value) {
			memory[address-offset+0] = (byte)(value>>0);
			memory[address-offset+1] = (byte)(value>>8);
		}

		public void put(int address, ushort value) {
			memory[address-offset+0] = (byte)(value>>0);
			memory[address-offset+1] = (byte)(value>>8);
		}

		public void put(int address, int value) {
			address -= offset;
			memory[address+0] = (byte)(value>>0);
			memory[address+1] = (byte)(value>>8);
			memory[address+2] = (byte)(value>>16);
			memory[address+3] = (byte)(value>>24);
		}

		public void put(int address, uint value) {
			address -= offset;
			memory[address+0] = (byte)(value>>0);
			memory[address+1] = (byte)(value>>8);
			memory[address+2] = (byte)(value>>16);
			memory[address+3] = (byte)(value>>24);
		}

		public void put(int address, long value) {
			put(address+0, value);
			put(address+4, value>>32);
		}

		public void put(int address, ulong value) {
			put(address+0, value);
			put(address+4, value>>32);
		}

#endif

		public void Dump() {
			this.Stack.Dump();
			this.Heap.Dump();
		}

		public Block this[Address address] {
			get {
				if(address.OnHeap) {
					return this.Heap[address];
				} else if(address.OnStack) {
					return this.Stack[address];
				} else {
					throw new UnexpectedException();
				}
			}
		}

	}

}
