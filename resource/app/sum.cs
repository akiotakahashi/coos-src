using System;
using System.Reflection;
using CooS.Reflection;
using CooS.Architectures.IA32;

unsafe class sum {

	public static int Calculate(int x) {
		if(x==0) return 0;
		return x+Calculate(x-1);
	}

	static int Calculate1(int x) {
		if(x==0) return 0;
		return x+Calculate2(x-1);
	}

	static int Calculate2(int x) {
		if(x==0) return 0;
		return x+Calculate1(x-1);
	}

	static void Sum(int x) {
		ulong t0 = Instruction.rdtsc();
		int y = Calculate(x);
		ulong t1 = Instruction.rdtsc();
		Console.WriteLine("x={0,5}, y={1,10}, time={2,16}", x, y, t1-t0);
	}

	static void Main(string[] args) {
		if(args.Length>0 && bool.Parse(args[0])) {
			MethodInfo method = typeof(sum).GetMethod("Calculate");
			Engine.GenerateNativeCode((MethodInfoImpl)method);
		}
		Sum(0);
		Sum(1);
		Sum(0);
		Sum(1);
		Sum(10);
		Sum(100);
		Sum(1000);
		Sum(10000);
	}

}
