using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests;

[TestClass]
public class BinaryOpTest
{
	private MoLangRuntime _runtime;

	[TestInitialize]
	public void Setup()
	{
		_runtime = new MoLangRuntime();
	}

	[TestMethod]
	public void ZeroReturnsFalse()
	{
		var parsed = MoLangParser.Parse("return 0 ?? 'test'");
		var result = _runtime.Execute(parsed);
		
		Assert.AreEqual(result.AsString(), "test");
	}
	
	[TestMethod]
	public void ZeroVariableReturnsFalse()
	{
		_runtime.Environment.SetValue(new MoPath("variable.lastNumber"), new DoubleValue(0));
		var parsed = MoLangParser.Parse("return variable.lastNumber ?? 'test'");
		var result = _runtime.Execute(parsed);
		
		Assert.AreEqual(result.AsString(), "test");
	}
}