using System;
using System.Diagnostics;
using Alex.MoLang.Attributes;
using Alex.MoLang.Parser;
using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alex.MoLang.Tests;

[TestClass]
public class InteropTest
{
	[TestInitialize]
	public void Init()
	{
		
	}

	[TestMethod]
	public void PropertyRead()
	{
		var expression = MoLangParser.Parse("query.life_time");

		var expected = Environment.TickCount * 3.5d;
		MoLangRuntime runtime = new MoLangRuntime();

		runtime.Environment.Structs.TryAdd(
			"query", new ObjectStruct(new TestClass(expected)) {UseNLog = false, EnableDebugOutput = false});

		var result = runtime.Execute(expression);
		Assert.AreEqual(expected, result.AsDouble());
	}
	
	[TestMethod]
	public void PropertyWrite()
	{
		var expression = MoLangParser.Parse("query.life_time = 5");
		
		MoLangRuntime runtime = new MoLangRuntime();

		var testStruct = new TestClass(Environment.TickCount);
		runtime.Environment.Structs.TryAdd(
			"query", new ObjectStruct(testStruct) {UseNLog = false, EnableDebugOutput = false});

		runtime.Execute(expression);
		Assert.AreEqual(5d, testStruct.Lifetime);
	}

	public class TestClass
	{
		private double _value;

		public TestClass(double value)
		{
			_value = value;
		}

		[MoProperty("life_time")]
		public double Lifetime
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
	}
}