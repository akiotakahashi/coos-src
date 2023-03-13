using System;

namespace CooS.Compile.CLI {

	partial class Synthesizer {

		public static void Dispatch_nop(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_nop(frame, inst); }
		public static void Dispatch_break(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_break(frame, inst); }
		public static void Dispatch_ldarg(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldarg(frame, inst); }
		public static void Dispatch_ldloc(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldloc(frame, inst); }
		public static void Dispatch_stloc(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_stloc(frame, inst); }
		public static void Dispatch_ldarga(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldarga(frame, inst); }
		public static void Dispatch_starg(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_starg(frame, inst); }
		public static void Dispatch_ldloca(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldloca(frame, inst); }
		public static void Dispatch_ldnull(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldnull(frame, inst); }
		public static void Dispatch_ldc(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldc(frame, inst); }
		public static void Dispatch_dup(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_dup(frame, inst); }
		public static void Dispatch_pop(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_pop(frame, inst); }
		public static void Dispatch_jmp(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_jmp(frame, inst); }
		public static void Dispatch_call(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_call(frame, inst); }
		public static void Dispatch_calli(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_calli(frame, inst); }
		public static void Dispatch_ret(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ret(frame, inst); }
		public static void Dispatch_br(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_br(frame, inst); }
		public static void Dispatch_brfalse(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_brfalse(frame, inst); }
		public static void Dispatch_brtrue(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_brtrue(frame, inst); }
		public static void Dispatch_beq(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_beq(frame, inst); }
		public static void Dispatch_bge(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_bge(frame, inst); }
		public static void Dispatch_bgt(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_bgt(frame, inst); }
		public static void Dispatch_ble(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ble(frame, inst); }
		public static void Dispatch_blt(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_blt(frame, inst); }
		public static void Dispatch_bne(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_bne(frame, inst); }
		public static void Dispatch_switch(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_switch(frame, inst); }
		public static void Dispatch_ldind(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldind(frame, inst); }
		public static void Dispatch_stind(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_stind(frame, inst); }
		public static void Dispatch_add(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_add(frame, inst); }
		public static void Dispatch_sub(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_sub(frame, inst); }
		public static void Dispatch_mul(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_mul(frame, inst); }
		public static void Dispatch_div(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_div(frame, inst); }
		public static void Dispatch_rem(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_rem(frame, inst); }
		public static void Dispatch_and(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_and(frame, inst); }
		public static void Dispatch_or(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_or(frame, inst); }
		public static void Dispatch_xor(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_xor(frame, inst); }
		public static void Dispatch_shl(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_shl(frame, inst); }
		public static void Dispatch_shr(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_shr(frame, inst); }
		public static void Dispatch_neg(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_neg(frame, inst); }
		public static void Dispatch_not(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_not(frame, inst); }
		public static void Dispatch_conv(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_conv(frame, inst); }
		public static void Dispatch_callvirt(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_callvirt(frame, inst); }
		public static void Dispatch_cpobj(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_cpobj(frame, inst); }
		public static void Dispatch_ldobj(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldobj(frame, inst); }
		public static void Dispatch_ldstr(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldstr(frame, inst); }
		public static void Dispatch_newobj(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_newobj(frame, inst); }
		public static void Dispatch_castclass(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_castclass(frame, inst); }
		public static void Dispatch_isinst(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_isinst(frame, inst); }
		public static void Dispatch_unbox(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_unbox(frame, inst); }
		public static void Dispatch_throw(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_throw(frame, inst); }
		public static void Dispatch_ldfld(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldfld(frame, inst); }
		public static void Dispatch_ldflda(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldflda(frame, inst); }
		public static void Dispatch_stfld(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_stfld(frame, inst); }
		public static void Dispatch_ldsfld(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldsfld(frame, inst); }
		public static void Dispatch_ldsflda(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldsflda(frame, inst); }
		public static void Dispatch_stsfld(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_stsfld(frame, inst); }
		public static void Dispatch_stobj(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_stobj(frame, inst); }
		public static void Dispatch_box(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_box(frame, inst); }
		public static void Dispatch_newarr(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_newarr(frame, inst); }
		public static void Dispatch_ldlen(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldlen(frame, inst); }
		public static void Dispatch_ldelema(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldelema(frame, inst); }
		public static void Dispatch_ldelem(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldelem(frame, inst); }
		public static void Dispatch_stelem(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_stelem(frame, inst); }
		public static void Dispatch_refanyval(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_refanyval(frame, inst); }
		public static void Dispatch_ckfinite(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ckfinite(frame, inst); }
		public static void Dispatch_mkrefany(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_mkrefany(frame, inst); }
		public static void Dispatch_ldtoken(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldtoken(frame, inst); }
		public static void Dispatch_endfinally(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_endfinally(frame, inst); }
		public static void Dispatch_leave(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_leave(frame, inst); }
		public static void Dispatch_prefix7(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix7(frame, inst); }
		public static void Dispatch_prefix6(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix6(frame, inst); }
		public static void Dispatch_prefix5(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix5(frame, inst); }
		public static void Dispatch_prefix4(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix4(frame, inst); }
		public static void Dispatch_prefix3(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix3(frame, inst); }
		public static void Dispatch_prefix2(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix2(frame, inst); }
		public static void Dispatch_prefix1(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefix1(frame, inst); }
		public static void Dispatch_prefixref(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_prefixref(frame, inst); }
		public static void Dispatch_arglist(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_arglist(frame, inst); }
		public static void Dispatch_ceq(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ceq(frame, inst); }
		public static void Dispatch_cgt(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_cgt(frame, inst); }
		public static void Dispatch_clt(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_clt(frame, inst); }
		public static void Dispatch_ldftn(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldftn(frame, inst); }
		public static void Dispatch_ldvirtftn(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_ldvirtftn(frame, inst); }
		public static void Dispatch_localloc(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_localloc(frame, inst); }
		public static void Dispatch_endfilter(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_endfilter(frame, inst); }
		public static void Dispatch_unaligned(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_unaligned(frame, inst); }
		public static void Dispatch_volatile(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_volatile(frame, inst); }
		public static void Dispatch_tail(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_tail(frame, inst); }
		public static void Dispatch_initobj(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_initobj(frame, inst); }
		public static void Dispatch_constrained(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_constrained(frame, inst); }
		public static void Dispatch_cpblk(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_cpblk(frame, inst); }
		public static void Dispatch_initblk(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_initblk(frame, inst); }
		public static void Dispatch_rethrow(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_rethrow(frame, inst); }
		public static void Dispatch_sizeof(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_sizeof(frame, inst); }
		public static void Dispatch_refanytype(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_refanytype(frame, inst); }
		public static void Dispatch_readonly(Synthesizer syn, EvaluationFrame frame, CompiledInstruction inst)
		{ syn.Eval_readonly(frame, inst); }

	}

}
