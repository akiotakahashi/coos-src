using System;
using System.IO;
using NUnit.Framework;

namespace IA32Toolkit.Assembler {

	[TestFixture]
	public class IA32AssemblerTest {

		IA32Assembler assembler = new IA32Assembler(new MemoryStream());

		public void Validate(params byte[] expected) {
			byte[] src = ((MemoryStream)this.assembler.BaseStream).ToArray();
			try {
				Assert.AreEqual(expected.Length, src.Length, "Both length are different.");
				for(int i=0; i<src.Length; ++i) {
					Assert.AreEqual(expected[i], src[i], "Differ at "+i);
				}
			} catch {
				Console.Error.Write("expected: ");
				foreach(byte e in expected) {
					Console.Error.Write("{0:X2} ", e);
				}
				Console.Error.WriteLine();
				Console.Error.Write("  actual: ");
				foreach(byte e in src) {
					Console.Error.Write("{0:X2} ", e);
				}
				Console.Error.WriteLine();
				throw;
			} finally {
				this.assembler.BaseStream.SetLength(0);
			}
		}

		public void Validate(string expected) {
			byte[] buf = new byte[expected.Length/2];
			for(int i=0; i<expected.Length; i+=2) {
				buf[i/2] = Convert.ToByte(expected.Substring(i,2),16);
			}
			this.Validate(buf);
		}

		[SetUp]
		public void Init() {
			this.assembler.BaseStream.SetLength(0);
		}

		[Test]
		public void Add1() {
			this.assembler.Add_EAX(0x100);
			this.Validate(0x05,0x00,0x01,0x00,0x00);
			this.assembler.Add(RegMem.Indirect(Register32.EAX), 0x100);
			this.Validate(0x81,0x00,0x00,0x01,0x00,0x00);
		}

		[Test]
		public void Add2() {
			this.assembler.Add(RegMem.Indirect(Register32.EAX, 127), 0x100);
			this.Validate(0x81,0x40,0x7F,0x00,0x01,0x00,0x00);
			this.assembler.Add(RegMem.Indirect(Register32.EAX, 128), 0x100);
			this.Validate(0x81,0x80,0x80,0x00,0x00,0x00,0x00,0x01,0x00,0x00);
		}

		[Test]
		public void Add3() {
			this.assembler.Add(RegMem.Indirect(Register32.EAX, Register32.ESI,2, 127), 0x100);
			this.Validate("8144707F00010000");
			this.assembler.Add(RegMem.Indirect(Register32.EAX, Register32.ESI,2, 128), 0x100);
			this.Validate("8184708000000000010000");
			this.assembler.Add(RegMem.Indirect(Register32.ESP, Register32.EBP,4, 128), 0x100);
			this.Validate("8184AC8000000000010000");
		}

		[Test]
		public void Add4() {
			this.assembler.Add((RegMem8)RegMem.Indirect(Register32.EBP), 8);
			this.Validate("80450008");
		}

		[Test]
		public void And1() {
			this.assembler.And_EAX(8);
			this.Validate("2508000000");
			this.assembler.And_EAX(127);
			this.Validate("257F000000");
			this.assembler.And_EAX(128);
			this.Validate("2580000000");
		}

		[Test]
		public void And2() {
			this.assembler.And(Register32.ECX, 8);
			this.Validate("81E108000000");
			this.assembler.And(Register32.ESP, 127);
			this.Validate("81E47F000000");
			this.assembler.And(Register32.EBP, 128);
			this.Validate("81E580000000");
		}

		[Test]
		public void And3() {
			this.assembler.And(RegMem.Indirect(Register32.ECX), 8);
			this.Validate("812108000000");
			this.assembler.And(RegMem.Indirect(Register32.ESP,10), 127);
			this.Validate("8164240A7F000000");
			this.assembler.And(RegMem.Indirect(Register32.EBP,Register32.EBX,2), 128);
			this.Validate("81645D0080000000");
		}

		[Test]
		public void Cmp1() {
			this.assembler.Cmp(RegMem.Indirect(Register32.ECX), 8);
			this.Validate("813908000000");
			this.assembler.Cmp(RegMem.Indirect(Register32.ESP,10), 127);
			this.Validate("817C240A7F000000");
			this.assembler.Cmp(RegMem.Indirect(Register32.EBP,Register32.EBX,2), 128);
			this.Validate("817C5D0080000000");
		}

		[Test]
		public void Jmp1() {
			this.assembler.Jmp(8);
			this.Validate("E908000000");
			this.assembler.Jmp((sbyte)8);
			this.Validate("EB08");
		}

		[Test]
		public void Call1() {
			this.assembler.Call(Register32.EAX);
			this.Validate("FFD0");
			this.assembler.Call((RegMem32)RegMem.Indirect(Register32.EAX));
			this.Validate("FF10");
			this.assembler.Call((RegMem32)RegMem.Indirect(Register32.ESP));
			this.Validate("FF1424");
			this.assembler.Call((RegMem32)RegMem.Indirect(Register32.EBP,Register32.EAX,2));
			this.Validate("FF544500");
			this.assembler.Call(0x100);
			this.Validate("E800010000");
		}

		[Test]
		public void Test() {
			this.assembler.Test_AL(8);
			this.Validate("A808");
			this.assembler.Test_EAX(unchecked((int)0xFF00AA00));
			this.Validate("A900AA00FF");
			this.assembler.Test(Register32.EAX, Register32.EAX);
			this.Validate("85C0");
		}

	}

}
