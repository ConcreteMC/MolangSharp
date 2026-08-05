using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the "Null-coalescing operator" section of the Molang syntax guide.
/// </summary>
/// <remarks>
///		"The null-coalescing operator, ??, lets you work with variables that might not have been set yet by providing
///		a default value to use when the variable hasn't been initialized." <br/>
///		"In other words, a ?? b will return a if it can be resolved to a value; otherwise, it will return b."
/// </remarks>
[TestClass]
public class NullCoalescingSpecTest
{
	[TestMethod("An uninitialized variable falls back to the right hand side")]
	public void UninitializedVariableFallsBack()
	{
		Assert.AreEqual(1.2d, Mo.Eval("v.never_set ?? 1.2"), 1e-9);
		Assert.AreEqual(1.2d, Mo.Eval("t.never_set ?? 1.2"), 1e-9);
		Assert.AreEqual(1.2d, Mo.Eval("c.never_set ?? 1.2"), 1e-9);
		Assert.AreEqual(1.2d, Mo.Eval("v.never.set.at.all ?? 1.2"), 1e-9);
	}

	/// <summary>
	///		The syntax guide's own example: <c>variable.x = (variable.y ?? 1.2) + 0.3;</c>
	/// </summary>
	[TestMethod("The documented example uses the default when the variable is missing")]
	public void DocumentedExampleWithMissingVariable()
	{
		Assert.AreEqual(1.5d, Mo.Eval("variable.x = (variable.y ?? 1.2) + 0.3; return variable.x;"), 1e-9);
	}

	[TestMethod("The documented example uses the variable when it has a value")]
	public void DocumentedExampleWithInitializedVariable()
	{
		Assert.AreEqual(
			5.3d, Mo.Eval("variable.y = 5; variable.x = (variable.y ?? 1.2) + 0.3; return variable.x;"), 1e-9);
	}

	/// <summary>
	///		A variable holding 0.0 has been resolved to a value, so it must be returned rather than replaced by the
	///		default. Only unresolvable operands fall through to the right hand side.
	/// </summary>
	[TestMethod("An initialized variable is returned even when it holds a falsy value")]
	public void InitializedFalsyValueIsReturned()
	{
		Assert.AreEqual(0d, Mo.Eval("v.zero = 0; return v.zero ?? 1.2;"));
		Assert.AreEqual(0d, Mo.Eval("t.zero = 0; return t.zero ?? 1.2;"));
		Assert.AreEqual(0d, Mo.Eval("v.zero = 0.0; return v.zero ?? 5;"));
		Assert.AreEqual(-1d, Mo.Eval("v.neg = -1; return v.neg ?? 5;"));
	}

	[TestMethod("A literal on the left hand side is always resolved")]
	public void LiteralsAreAlwaysResolved()
	{
		Assert.AreEqual(0d, Mo.Eval("0 ?? 5"));
		Assert.AreEqual(1d, Mo.Eval("1 ?? 5"));
		Assert.AreEqual(0d, Mo.Eval("false ?? 5"));
		Assert.AreEqual("value", Mo.Value("'value' ?? 5").AsString());
		Assert.AreEqual(string.Empty, Mo.Value("'' ?? 'fallback'").AsString());
	}

	[TestMethod("An initialized variable holding a string is returned")]
	public void InitializedStringIsReturned()
	{
		Assert.AreEqual("pig", Mo.Value("v.name = 'pig'; return v.name ?? 'cow';").AsString());
		Assert.AreEqual("cow", Mo.Value("return v.name ?? 'cow';").AsString());
	}

	[TestMethod("A resolvable entity reference is returned rather than the default")]
	public void EntityReferenceIsResolvable()
	{
		var runtime = new MoLangRuntime();
		var pig = new MoLangEnvironment();
		pig.SetValue(new MoPath("variable.oink"), new DoubleValue(3));
		runtime.Environment.Structs["variable"].Set(new MoPath("pigpig"), pig);

		Assert.AreEqual(3d, Mo.Eval(runtime, "(v.pigpig ?? v.pigpig)->v.oink"));
	}

	/// <summary>
	///		"if it's a reference to a deleted entity ... the expression will use 1.2 in place of it".
	/// </summary>
	[TestMethod("An unresolvable entity reference falls back to the right hand side")]
	public void StaleEntityReferenceFallsBack()
	{
		// v.not_an_entity holds a number, so the arrow operator cannot resolve a variable through it.
		Assert.AreEqual(1.2d, Mo.Eval("v.not_an_entity = 1; return (v.not_an_entity->v.oink) ?? 1.2;"), 1e-9);
	}

	/// <summary>
	///		Chained defaults must fall all the way through, so <c>a ?? b ?? c</c> groups as <c>a ?? (b ?? c)</c>.
	/// </summary>
	[TestMethod("Coalescing chains through every default")]
	public void CoalescingChains()
	{
		Assert.AreEqual(3d, Mo.Eval("v.a ?? v.b ?? 3"));
		Assert.AreEqual(2d, Mo.Eval("v.b = 2; return v.a ?? v.b ?? 3;"));
		Assert.AreEqual(1d, Mo.Eval("v.a = 1; v.b = 2; return v.a ?? v.b ?? 3;"));
	}

	[TestMethod("A coalesced value can be used in further arithmetic")]
	public void CoalescedValueParticipatesInArithmetic()
	{
		Assert.AreEqual(12d, Mo.Eval("(v.missing ?? 10) + 2"));
		Assert.AreEqual(20d, Mo.Eval("v.present = 10; return (v.present ?? 1) * 2;"));
	}
}
