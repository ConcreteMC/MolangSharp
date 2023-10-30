using System.Diagnostics;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests;

[TestClass]
public class PerformanceTest
{
	private Stopwatch _sw;
	private MoLangEnvironment _environment;
	private MoLangRuntime _runtime;
	
	[TestInitialize]
	public void Init()
	{
		_sw = new Stopwatch();
		_environment = new MoLangEnvironment();
		_runtime = new MoLangRuntime(_environment);
		
		_environment.SetValue(new MoPath("variable.test"), DoubleValue.One);
		_environment.SetValue(new MoPath("variable.life_time"), DoubleValue.Zero);
		
		MoLangRuntimeConfiguration.UseDummyValuesInsteadOfExceptions = false;
		MoLangRuntimeConfiguration.UseMoLangStackTrace = true;
	}

	[TestCleanup]
	public void Cleanup()
	{
		MoLangRuntimeConfiguration.UseMoLangStackTrace = true;
		MoLangRuntimeConfiguration.UseDummyValuesInsteadOfExceptions = false;
	}

	[TestMethod]
	public void TestVariableReadingMath()
	{
		const int iterations = 5000000;
		long total = 0;

		var expression = MoLangParser.Parse("variable.test * 20");
		for (int i = 0; i < iterations; i++)
		{
			_sw.Restart();
			var a = _runtime.Execute(expression);
			total += _sw.ElapsedMilliseconds;
			Assert.AreEqual(20, a.AsDouble());
		}
		
		Debug.WriteLine($"Average: {((double)total / iterations):N10}ms");
	}
	
	[TestMethod]
	public void TestVariableReading()
	{
		const int iterations = 5000000;
		long total = 0;

		var expression = MoLangParser.Parse("variable.test");
		for (int i = 0; i < iterations; i++)
		{
			_sw.Restart();
			var a = _runtime.Execute(expression);
			total += _sw.ElapsedMilliseconds;
			Assert.AreEqual(1, a.AsDouble());
		}
		
		Debug.WriteLine($"Average: {((double)total / iterations):N10}ms");
	}

	[TestMethod]
	public void TestVariableWriting()
	{
		const int iterations = 5000000;
		long total = 0;

		var expression = MoLangParser.Parse("variable.test = 20");
		for (int i = 0; i < iterations; i++)
		{
			_sw.Restart();
			_runtime.Execute(expression);
			total += _sw.ElapsedMilliseconds;
		}
		
		Debug.WriteLine($"Average: {((double)total / iterations):N10}ms");
	}
	
	[TestMethod]
	public void TestInvalidPathPerformance()
	{
		const int iterations = 50000;
		long total = 0;
		
		var expression = MoLangParser.Parse("fr.test");
		for (int i = 0; i < iterations; i++)
		{
			_sw.Restart();

			try
			{
				_runtime.Execute(expression);
			}
			catch
			{
				
			}
			total += _sw.ElapsedMilliseconds;
		}
		
		Debug.WriteLine($"Average: {((double)total / iterations):N10}ms");
	}

	[TestMethod]
	public void TestDummyValuesConfiguration()
	{
		var expression = MoLangParser.Parse("return fr.test");
		MoLangRuntimeConfiguration.UseDummyValuesInsteadOfExceptions = true;
		var result = _runtime.Execute(expression);
		Assert.AreEqual(0d, result.AsDouble());
	}

	[TestMethod]
	public void RawPerformanceTest()
	{
		var parsed = MoLangParser.Parse("math.cos(v.life_time * 60) * -45.0");
		
		RunPerformanceTestAndOutput(parsed);
	}
	
	[TestMethod]
	public void InvalidPathPerformanceTest()
	{
		var parsed = MoLangParser.Parse("math.cos(a.life_time * 60) * -45.0");
		
		RunPerformanceTestAndOutput(parsed);
	}
	
	[TestMethod]
	public void InvalidPathPerformanceWithoutExceptionThrowingTest()
	{
		MoLangRuntimeConfiguration.UseDummyValuesInsteadOfExceptions = true;
		var parsed = MoLangParser.Parse("math.cos(a.life_time * 60) * -45.0");
		
		RunPerformanceTestAndOutput(parsed);
	}
	
	[TestMethod]
	public void InvalidPathPerformanceWithoutStackTraceThrowingTest()
	{
		MoLangRuntimeConfiguration.UseMoLangStackTrace = false;
		var parsed = MoLangParser.Parse("math.cos(a.life_time * 60) * -45.0");

		RunPerformanceTestAndOutput(parsed);
	}

		
	private void LogOutput(long iterations, long timespent)
	{
		Debug.WriteLine($"Iterations: {iterations}. Elapsed: {timespent}ms Avg: {timespent / (double)iterations}ms");
	}

	private void RunPerformanceTestAndOutput(IExpression expression)
	{
		var result = RunPerformanceTest(expression);
		LogOutput(result.iterations, result.timeSpent);
	}
	
	private (long iterations, long timeSpent) RunPerformanceTest(IExpression expression)
	{
		var iterations = 0L;
		var sw = Stopwatch.StartNew();
		while (sw.ElapsedMilliseconds < 1000)
		{
			try
			{
				var e = _runtime.Execute(expression);
			}
			catch
			{
				
			}

			iterations++;
		}
		sw.Stop();

		return (iterations, sw.ElapsedMilliseconds);
	}
}