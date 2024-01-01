using ConcreteMC.MolangSharp.Attributes;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Parser.Tokenizer;
using ConcreteMC.MolangSharp.Parser.Visitors.InteropOptimization;
using ConcreteMC.MolangSharp.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests;

[TestClass]
public class InteropOptimizationVisitorTest
{
	[TestMethod]
	public void VerifyFunctionCallOptimization()
	{
		var entity = new TestEntity();
		var parser = new MoLangParser(new TokenIterator("query.getHealth()"));
		parser.ExpressionTraverser.Visitors.Add(new InteropOptimizationVisitor(new InteropOptimizationVisitorOptions()
		{
			InteropEntries = new []
			{
				new InteropEntry("query", entity)
			}
		}));

		var expression = parser.Parse();
		Assert.IsInstanceOfType(expression, typeof(OptimizedFunctionCallExpression));

		var runtime = new MoLangRuntime();
		var result = runtime.Execute(expression);
		Assert.IsNotNull(result);
		Assert.AreEqual(20, result.AsDouble());
	}

	[TestMethod]
	public void VerifyPropertyAccessOptimization()
	{
		var entity = new TestEntity();
		var parser = new MoLangParser(new TokenIterator("query.health"));
		parser.ExpressionTraverser.Visitors.Add(new InteropOptimizationVisitor(new InteropOptimizationVisitorOptions()
		{
			InteropEntries = new []
			{
				new InteropEntry("query", entity)
			}
		}));

		var expression = parser.Parse();
		Assert.IsInstanceOfType(expression, typeof(OptimizedNameExpression));

		var runtime = new MoLangRuntime();
		var result = runtime.Execute(expression);
		Assert.IsNotNull(result);
		Assert.AreEqual(20, result.AsDouble());
	}
	
	[TestMethod]
	public void VerifyPropertyAssignOptimization()
	{
		var entity = new TestEntity();
		var parser = new MoLangParser(new TokenIterator("query.health = 21"));
		parser.ExpressionTraverser.Visitors.Add(new InteropOptimizationVisitor(new InteropOptimizationVisitorOptions()
		{
			InteropEntries = new []
			{
				new InteropEntry("query", entity)
			}
		}));

		var expression = parser.Parse();
		Assert.IsInstanceOfType(expression, typeof(OptimizedAssignExpression));

		var runtime = new MoLangRuntime();
		var result = runtime.Execute(expression);
		Assert.IsNotNull(result);
		Assert.AreEqual(21, entity.Health);
	}
	
	private class TestEntity
	{
		[MoProperty("health")]
		public double Health { get; set; }
		
		public TestEntity()
		{
			Health = 20;
		}
		
		[MoFunction("getHealth")]
		public double GetHealth()
		{
			return Health;
		}
	}
}