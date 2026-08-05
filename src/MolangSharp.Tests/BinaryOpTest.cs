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

	/// <summary>
	///		A literal can always be resolved, so '??' returns it even when it is falsy:
	///		"a ?? b will return a if it can be resolved to a value; otherwise, it will return b."
	/// </summary>
	[TestMethod]
	public void ZeroIsAResolvedValue()
	{
		var parsed = MoLangParser.Parse("return 0 ?? 'test'");
		var result = _runtime.Execute(parsed);

		Assert.AreEqual(0d, result.AsDouble());
	}

	/// <summary>
	///		A variable that was assigned 0 has been initialized, so '??' must not substitute its default.
	/// </summary>
	[TestMethod]
	public void ZeroVariableIsAResolvedValue()
	{
		_runtime.Environment.SetValue(new MoPath("variable.lastNumber"), new DoubleValue(0));
		var parsed = MoLangParser.Parse("return variable.lastNumber ?? 'test'");
		var result = _runtime.Execute(parsed);

		Assert.AreEqual(0d, result.AsDouble());
	}

	/// <summary>
	///		An uninitialized variable cannot be resolved, so '??' falls back to its right hand side.
	/// </summary>
	[TestMethod]
	public void UninitializedVariableFallsBack()
	{
		var parsed = MoLangParser.Parse("return variable.lastNumber ?? 'test'");
		var result = _runtime.Execute(parsed);

		Assert.AreEqual("test", result.AsString());
	}
}
