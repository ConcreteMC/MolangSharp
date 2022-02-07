using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;
using ConcreteMC.MolangSharp.Parser.Visitors;
using ConcreteMC.MolangSharp.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests;

[TestClass]
public class MathOptimizationVisitorTest
{
    [TestMethod]
    public void Constants()
    {
        MoLangParser parser = new MoLangParser(new TokenIterator("7 + 2 * (6 + 3) / 3 - 7"));
        parser.ExpressionTraverser.Visitors.Add(new MathOptimizationVisitor());
        
        var expr = parser.Parse();
        Assert.IsInstanceOfType(expr, typeof(NumberExpression));

        MoLangRuntime runtime = new MoLangRuntime();
        var result = runtime.Execute(expr);
        
        Assert.AreEqual(6, result.AsDouble());
    }
    
    [TestMethod]
    public void MathFunctions()
    {
        MoLangParser parser = new MoLangParser(new TokenIterator("math.floor(10.1 + 20.1)"));
        parser.ExpressionTraverser.Visitors.Add(new MathOptimizationVisitor());
        
        var expr = parser.Parse();
        Assert.IsInstanceOfType(expr, typeof(NumberExpression));
        
        MoLangRuntime runtime = new MoLangRuntime();
        var result = runtime.Execute(expr);
        
        Assert.AreEqual(30, result.AsDouble());
    }
}