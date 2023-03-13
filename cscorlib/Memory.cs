using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CooS;
using CooS.Reflection;
using CooS.Reflection.CLI;
using CooS.Architectures;

namespace CooS {
	using nint = UInt32;

	public class Memory {

		static nint StartAddress;
		static nint LimitAddress;
		static nint Current;

		[StructLayout(LayoutKind.Sequential)]
		private struct Header {
			public nint Size;
			public nint Type;
		};

		public static void Initialize(IntPtr startaddr, IntPtr limitaddr) {
			StartAddress = (nint)startaddr;
			LimitAddress = (nint)limitaddr;
			Current = StartAddress;
		}

		public static void BeSynchronized(IntPtr start, IntPtr limit, IntPtr current) {
			StartAddress = (nint)start.ToInt32();
			LimitAddress = (nint)limit.ToInt32();
			Current = (nint)current.ToInt32();
		}

		private static unsafe Header* AllocateBlock(int size) {
			size = (size+3)&~3;
			Header* p0;
			while((p0 = (Header*)Current)->Size!=0) {
				Current = Current+p0->Size;
			}
			Header* p1 = (Header*)((byte*)(p0+1)+size);
			p1->Size = 0;
			p1->Type = 0;
			p0->Type = 0;
			p0->Size = (nint)(size+sizeof(Header));
			CooS.Tuning.Memory.Clear(p0+1, size);
			return p0;
		}

		static bool prepare_slots_when_allocate = false;

		public static unsafe object Allocate(TypeImpl type) {
			if(type.IsArray) throw new ArgumentException("Allocating type must not be a kind of array.");
			int size = Architecture.GetStackingSize(type.InstanceSize);
			if(prepare_slots_when_allocate) {
				type.PrepareSlots();
			}
			Header* hdr = AllocateBlock(size);
			hdr->Type = (nint)Kernel.ObjectToValue(type);
			return Kernel.ValueToObject(new IntPtr(hdr+1));
		}

		public static unsafe string AllocateString(TypeImpl strtype, int length) {
			int size = strtype.VariableSize+length*2+2;	// add extra padding
			Header* hdr = AllocateBlock(size);
			hdr->Type = (nint)Kernel.ObjectToValue(strtype);
			++hdr;
			*(int*)hdr = length;
			*((char*)hdr+2+length) = '\0';
			return (string)Kernel.ValueToObject(hdr);
		}

		public static unsafe Array AllocateArray(TypeImpl elemtype, int length) {
			TypeImpl arrtype = elemtype.GetSzArrayType();
			int size = /*typeof(SzArray).InstanceSize*/12+elemtype.VariableSize*length;
			Header* hdr = AllocateBlock(size);
			hdr->Type = (nint)Kernel.ObjectToValue(arrtype);
			++hdr;
			((int*)hdr)[0] = Kernel.ObjectToValue(elemtype).ToInt32();
			((int*)hdr)[1] = elemtype.VariableSize;
			((int*)hdr)[2] = length;
			return (Array)Kernel.ValueToObject(hdr);
		}

		public static object AllocateArray(TypeImpl elemtype, int[] sizes) {
			throw new NotImplementedException();
		}

		public static object AllocateArray(TypeImpl elemtype, int[] bases, int[] sizes) {
			throw new NotImplementedException();
		}

		public static unsafe object BoxValue(TypeImpl type, byte[] buf, int index) {
			int size = type.VariableSize;
			Header* hdr = Memory.AllocateBlock(size);
			hdr->Type = (nint)Kernel.ObjectToValue(type);
			fixed(byte* p = buf) {
				Tuning.Memory.Copy(hdr+1, p+index, size);
			}
			return Kernel.ValueToObject(new IntPtr(hdr+1));
		}

		public static unsafe object BoxValue(TypeImpl type, IntPtr obj, int offset) {
			int size = type.VariableSize;
			Header* hdr = Memory.AllocateBlock(size);
			hdr->Type = (nint)Kernel.ObjectToValue(type);
			Tuning.Memory.Copy(hdr+1, (byte*)obj.ToPointer()+offset, size);
			return Kernel.ValueToObject(new IntPtr(hdr+1));
		}

		public static unsafe void PrepareSlotsOfAll() {
			Console.WriteLine("PrepareSlotsOfAll [0x{0:X8}-0x{1:X8}] ... please wait for a while.", StartAddress, Current);
			byte* p =(byte*)StartAddress;
			byte* p1 = (byte*)Current;
			while(p<p1) {
				Header* hdr = (Header*)p;
				TypeImpl type = (TypeImpl)Kernel.ValueToObject((void*)hdr->Type);
				if(!type.SlotAvailable) {
					Console.Write("\r0x{0:X8} {1}  ", (int)p, type.FullName);
					type.PrepareSlots();
				}
				p += hdr->Size;
			}
			Console.WriteLine("...OK");
			prepare_slots_when_allocate = true;
		}

	}

}
