using System.Collections.Generic;
using Alex.MoLang.Parser;
using Alex.MoLang.Parser.Tokenizer;
using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Struct;
using Alex.MoLang.Runtime.Value;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alex.MoLang.Tests;

[TestClass]
public class MathTest
{
	[TestInitialize]
	public void Init()
	{
		
	}

	private MoLangRuntime Setup(double a, double b)
	{
		MoLangRuntime runtime = new MoLangRuntime();

		runtime.Environment.Structs["variable"] = new VariableStruct(
			new List<KeyValuePair<string, IMoValue>>()
			{
				new KeyValuePair<string, IMoValue>("a", new DoubleValue(a)),
				new KeyValuePair<string, IMoValue>("b", new DoubleValue(b))
			});

		return runtime;
	}
	
	[TestMethod]
	public void ArithmeticsAdding()
	{
		const int a = 1200;
		const int b = 800;
		const double expectedResult = a + b;
		
		var expression = MoLangParser.Parse("v.a + v.b");
		var runtime = Setup(a, b);

		var result = runtime.Execute(expression);
		Assert.AreEqual(expectedResult, result.AsDouble());
	}
	
	[TestMethod]
	public void ArithmeticsSubtracting()
	{
		const double a = 1200;
		const double b = 800;
		const double expectedResult = a - b;
		
		var expression = MoLangParser.Parse("v.a - v.b");
		var runtime = Setup(a, b);

		var result = runtime.Execute(expression);
		Assert.AreEqual(expectedResult, result.AsDouble());
	}
	
	[TestMethod]
	public void ArithmeticsMultiplication()
	{
		const double a = 1200;
		const double b = 800;
		const double expectedResult = a * b;
		
		var expression = MoLangParser.Parse("v.a * v.b");
		var runtime = Setup(a, b);

		var result = runtime.Execute(expression);
		Assert.AreEqual(expectedResult, result.AsDouble());
	}
	
	[TestMethod]
	public void ArithmeticsDivision()
	{
		const double a = 1200;
		const double b = 800;
		const double expectedResult = a / b;
		
		var expression = MoLangParser.Parse("v.a / v.b");
		var runtime = Setup(a, b);

		var result = runtime.Execute(expression);
		Assert.AreEqual( expectedResult, result.AsDouble());
	}
	
	[TestMethod]
	public void OrderOfOperations()
	{
		var runtime = new MoLangRuntime();

		var result = runtime.Execute(MoLangParser.Parse("7 + 2 * (6 + 3) / 3 - 7"));
		Assert.AreEqual(6, result.AsDouble());
		
		result = runtime.Execute(MoLangParser.Parse("10 * 2 - (7 + 9)"));
		Assert.AreEqual(4, result.AsDouble());
		
		result = runtime.Execute(MoLangParser.Parse("12 + 2 * 44"));
		Assert.AreEqual(100, result.AsDouble());
	}
}