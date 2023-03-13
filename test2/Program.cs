using System;
using System.Collections.Generic;
using System.Text;

namespace test2 {

	public class Program {

		static int Main(string[] args) {
			Program p = new Program();
			return p.add<int>(1,2);
		}

		public T add<T>(T v1, T v2) where T : struct {
			return v1;
		}

		public double CompileTest() {
			int a = 3;
			int b = 7;
			return (int)Math.Pow(a,b);
		}

	}

}
