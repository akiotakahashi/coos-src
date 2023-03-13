using System;
using NUnit.Framework;
using CooS.Formats.CLI.Metadata;
using CooS.Formats.CLI.Metadata.Rows;

namespace CooS.Formats.CLI.IL {

	[TestFixture]
	public class InstructionTest {

		[Test]
		public void Read() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = CooS.Formats.CLI.Metadata.MetadataRootTest.LoadMetadata();
			foreach(MethodDefRow row in md.Tables.MethodDef) {
				if(MethodImplAttributes.IL!=(row.ImplFlags&MethodImplAttributes.CodeTypeMask)) { continue; }
				if(row.RVA==0) { continue; }
				MethodIL mil = md.GetMethodCode(row);
				Instruction[] insts = Instruction.Read(mil.ByteCode);
				Assert.Greater(insts.Length, 0);
			}
			Console.WriteLine("{0} assertions", Assert.Counter);
		}

		[Test]
		public void Rule() {
			CooS.Formats.CLI.Metadata.MetadataRoot md = CooS.Formats.CLI.Metadata.MetadataRootTest.LoadMetadata();
			foreach(MethodDefRow row in md.Tables.MethodDef) {
				if(MethodImplAttributes.IL!=(row.ImplFlags&MethodImplAttributes.CodeTypeMask)) { continue; }
				if(row.RVA==0) { continue; }
				MethodIL mil = md.GetMethodCode(row);
				Instruction[] insts = Instruction.Read(mil.ByteCode);
				foreach(Instruction inst in insts) {
					Assert.IsNotNull(inst.OpCode);
					Assert.GreaterOrEqual(inst.OpcodeAddress, 0);
					Assert.Less(inst.OpcodeAddress, mil.CodeSize);
					Assert.GreaterOrEqual(inst.GetOperandSize(mil.ByteCode), 0);
				}
			}
			Console.WriteLine("{0} assertions", Assert.Counter);
		}

		//[Test]
		public void Measure() {
			System.Reflection.Assembly assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(CooS.Formats.CLI.Metadata.MetadataRootTest.MscorlibPath);
			foreach(Type type in assembly.GetTypes()) {
				foreach(System.Reflection.MethodInfo method in type.GetMethods()) {
					System.Reflection.MethodBody mb = method.GetMethodBody();
					if(mb==null) { continue; }
					byte[] code = mb.GetILAsByteArray();
					Assert.Greater(code.Length, 0);
				}
			}
			Console.WriteLine("{0} assertions", Assert.Counter);
		}

	}

}
