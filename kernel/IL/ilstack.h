#pragma once


namespace IL {

	enum STACK_TYPE {
		STACK_TYPE_Unk,		// Native Int
		STACK_TYPE_N,		// Native Int
		STACK_TYPE_I4,		// Int32
		STACK_TYPE_I8,		// Int64
		STACK_TYPE_R4,		// Float
		STACK_TYPE_R8,		// Double
		STACK_TYPE_O,		// Object
		STACK_TYPE_P,		// Pointer
		STACK_TYPE_V,		// ValueType
	};

	class ILStack {
		nint* mem;
		unint sp;
		STACK_TYPE top0type;
		STACK_TYPE top1type;
		STACK_TYPE top2type;
		STACK_TYPE top3type;
		STACK_TYPE top4type;
		STACK_TYPE top5type;
		STACK_TYPE top6type;
		STACK_TYPE top7type;
		STACK_TYPE top8type;
		STACK_TYPE top9type;
		STACK_TYPE top10type;
		STACK_TYPE top11type;
		STACK_TYPE top12type;
		STACK_TYPE top13type;
		STACK_TYPE top14type;
		STACK_TYPE top15type;
	public:
		ILStack(uint size);
		~ILStack();
	public:
		static STACK_TYPE getStackType(ELEMENT_TYPE type);
	public:
		static uint getSizeOnStack(uint size);
		static uint getLengthOnStack(uint size);
		STACK_TYPE getDataType() const;
		void setDataType(STACK_TYPE type);
		void setDataType(ELEMENT_TYPE type);
	private:
		void* alloc(uint count);
		void pushtype(STACK_TYPE type);
		void poptype();
	public:
		void pushi(int32 n);
		void pushl(int64 n);
		void pushn(nint n);
		void pushn(const void* p);
		void pushr4(float n);
		void pushr8(double n);
		void pusho(void* p);
		void pushp(void* p);
	private:
		void pushmem(const void* p, uint size);
	public:
		int32 popi();
		int64 popl();
		nint popn();
		float popr4();
		double popr8();
		void* popo();
		void* popp();
	public:
		void pushmem(const void* p, ELEMENT_TYPE type, uint size);
		void pushmem(const void* p, STACK_TYPE type, uint size);
		void popmem(void* p, uint size);
		void* allocmem(uint size);
		void releasemem(uint size);
	public:
		const nint& top() const;
		const nint& operator [](uint index) const;
	public:
		Console& Dump(Console& console) const;
	public:
		void buildFrame(const Reflection::Method& method, const void* p);
		void releaseFrame(const void* ptoparg, int argsize, int retsize, const Signature::MethodSig& signature);
		void releaseFrame(const void* ptoparg, const Signature::MethodSig& signature, const Reflection::Assembly& assembly);
		void releaseFrame(const void* ptoparg, const Reflection::Method& method);
	};

}
