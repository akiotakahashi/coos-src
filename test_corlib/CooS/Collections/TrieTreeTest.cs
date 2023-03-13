using System;
using System.Reflection;
using CooS.Reflection;
using NUnit.Framework;

namespace CooS.Collections {

	[TestFixture]
	public class TrieTreeTest {

		TrieTree tree = new TrieTree();
		object obj1 = new object();
		object obj2 = new object();
		object obj3 = new object();

		public TrieTreeTest() {
		}

		[SetUp]
		public void SetUp() {
			tree = new TrieTree();
		}

		[TearDown]
		public void TearDown() {
			tree.Dump();
		}

		[Test]
		public void Add() {
			tree.Add("www.coos.jp", obj1);
			Assert.AreSame(obj1, tree["www.coos.jp"]);
		}

		[Test]
		public void Add2() {
			tree.Add("www.coos.jp", obj1);
			tree.Add("www.coos.jp.obj2", obj2);
			Assert.AreSame(obj1, tree["www.coos.jp"]);
			Assert.AreSame(obj2, tree["www.coos.jp.obj2"]);
		}

		[Test]
		public void Add3() {
			tree.Add("www.coos.jp.obj1", obj1);
			tree.Add("www.coos.jp.obj2", obj2);
			Assert.AreSame(obj1, tree["www.coos.jp.obj1"]);
			Assert.AreSame(obj2, tree["www.coos.jp.obj2"]);
		}

		[Test]
		public void Add4() {
			tree.Add("www.coos.jp.obj1", obj1);
			tree.Add("www.coos.jp", obj2);
			Assert.AreSame(obj1, tree["www.coos.jp.obj1"]);
			Assert.AreSame(obj2, tree["www.coos.jp"]);
		}

		[Test]
		public void Add5() {
			tree.Add("www.coos.jp.obj1", obj1);
			tree.Add("www.coos.jp", obj2);
			tree.Add("www.coos.jp.o", obj3);
			tree.Add("www.coos.jp.obj1.sub", obj1);
			Assert.AreSame(obj1, tree["www.coos.jp.obj1"]);
			Assert.AreSame(obj2, tree["www.coos.jp"]);
			Assert.AreSame(obj3, tree["www.coos.jp.o"]);
			Assert.AreSame(obj1, tree["www.coos.jp.obj1.sub"]);
		}

		[Test,Ignore("")]
		public void Add6() {
			AssemblyManager manager = TestUtility.CreateManager();
			foreach(Type type in manager.ResolveAssembly("cscorlib",true).GetTypes(false)) {
				tree.Add(type.FullName, type);
			}
		}

		[Test]
		public void Remove1() {
			tree.Add("www.coos.jp.obj1", obj1);
			tree.Add("www.coos.jp", obj2);
			tree.Remove("www.coos.jp");
			tree.Add("www.coos.jp", obj3);
			Assert.AreSame(obj1, tree["www.coos.jp.obj1"]);
			Assert.AreSame(obj3, tree["www.coos.jp"]);
		}

	}

}
