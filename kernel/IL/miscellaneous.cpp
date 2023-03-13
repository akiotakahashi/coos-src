#include "..\ilengine.h"

using namespace Reflection;
using namespace IL;


#define DUMMY_INST(name) \
	extern Method* execute_ ## name(IL::ILMachine& machine, Frame& frame, const byte* operand) \
	{ machine.panic("Dummy routine of " __FUNCTION__); }

//DUMMY_INST(ret);
DUMMY_INST(add_ovf_un);
DUMMY_INST(arglist);
DUMMY_INST(ckfinite);
DUMMY_INST(constrained_);
DUMMY_INST(conv_ovf_i8);
DUMMY_INST(conv_ovf_i8_un);
DUMMY_INST(conv_ovf_u8);
DUMMY_INST(conv_ovf_u8_un);
DUMMY_INST(endfilter);
DUMMY_INST(endfinally);
DUMMY_INST(jmp);
DUMMY_INST(mkrefany);
DUMMY_INST(mul_ovf_un);
DUMMY_INST(no_);
DUMMY_INST(prefixref);
DUMMY_INST(readonly_);
DUMMY_INST(refanytype);
DUMMY_INST(refanyval);
DUMMY_INST(rethrow);
DUMMY_INST(stelem);
DUMMY_INST(sub_ovf_un);
DUMMY_INST(tail_);
DUMMY_INST(unaligned_);
DUMMY_INST(unbox_any);

extern Method* execute_volatile_(ILMachine& machine, Frame& frame, const byte* operand) {
	frame.pc += 2;
	return NULL;
}

/*
extern Method* execute_prefixref(ILMachine& machine, Frame& frame, const byte* operand) {
	// ?
	frame.pc += 1;
	return NULL;
}
*/
