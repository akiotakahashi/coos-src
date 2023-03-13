using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Reflection.CLI {
	class MethodDef {

#if false
		public override MethodInfoImpl[] GetCallings()
		{
			if(!this.HasRVA)
				return new MethodInfoImpl[0];
			ArrayList callings = new ArrayList();
			ILStream ils = new ILStream(this.MyAssembly, this.OpenCodeBlock());
			while(!ils.AtEndOfStream) {
				Instruction inst = ils.Read();
				if(inst.OpCode.Value==System.Reflection.Emit.OpCodes.Call.Value
				|| inst.OpCode.Value==System.Reflection.Emit.OpCodes.Callvirt.Value) {
					callings.Add(this.assembly.ResolveMethod((MDToken)inst.Operand));
				} else if(inst.OpCode.Value==System.Reflection.Emit.OpCodes.Ldtoken.Value) {
					MDToken token = (MDToken)inst.Operand;
					switch(token.TableId) {
					case TableId.Method:
					case TableId.MemberRef:
						MethodInfo mi = this.assembly.ResolveMethod(token);
						callings.Add(mi);
						break;
					}
				}
			}
			ils.Close();
			return (MethodInfoImpl[])callings.ToArray(typeof(MethodInfoImpl));
		}
#endif

	}
}
