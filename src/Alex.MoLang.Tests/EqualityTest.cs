using Alex.MoLang.Parser;
using Alex.MoLang.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alex.MoLang.Tests;

[TestClass]
public class EqualityTest
{
    private MoLangRuntime _runtime;

    [TestInitialize]
    public void Setup()
    {
        _runtime = new MoLangRuntime();
    }

    [TestMethod("Break Execution (Bigger or Equal)")]
    public void BiggerOrEqualTest()
    {
        var parsed = MoLangParser.Parse(@"
t.a = 1;
loop(10, {
t.a = t.a + 1;
t.a >= 5 ? break : continue;
});
return t.a;
");

        Assert.IsNotNull(parsed);

        var result = _runtime.Execute(parsed);
        Assert.AreEqual(5, result.AsDouble());
    }

    [TestMethod("Break Execution (Smaller or Equal)")]
    public void SmallerOrEqualTest()
    {
        var parsed = MoLangParser.Parse(@"
t.a = 10;
loop(10, {
t.a = t.a - 1;
t.a <= 5 ? break : continue;
});
return t.a;
");

        Assert.IsNotNull(parsed);

        var result = _runtime.Execute(parsed);
        Assert.AreEqual(5, result.AsDouble());
    }

    [TestMethod("Break Execution (Equal)")]
    public void EqualityOperatorTest()
    {
        var parsed = MoLangParser.Parse(@"
t.a = 10;
loop(10, {
t.a = t.a - 1;
t.a == 5 ? break : continue;
});
return t.a;
");

        Assert.IsNotNull(parsed);

        var result = _runtime.Execute(parsed);
        Assert.AreEqual(5, result.AsDouble());
    }
}