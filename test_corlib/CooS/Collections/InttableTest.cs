using System;
using NUnit.Framework;

namespace CooS.Collections {

	[TestFixture]
	public class InttableTest {

		Inttable table = new Inttable();

		[SetUp]
		public void Setup() {
			this.table = new Inttable();
		}

		[Test]
		public void Add() {
			object obj1 = new object();
			object obj2 = new object();
			object obj3 = new object();
			table.Add(1, obj1);
			table.Add(2, obj2);
			table.Add(3, obj3);
			Assert.AreSame(obj1, table[1]);
			Assert.AreSame(obj2, table[2]);
			Assert.AreSame(obj3, table[3]);
		}

	}

}
