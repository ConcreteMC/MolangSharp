using ConcreteMC.MolangSharp.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the "Conditionals" and "Brace scope delimiters" sections of the Molang syntax guide.
/// </summary>
/// <remarks>
///		"The first way is to use ? by itself to conditionally execute part of an expression: A ? B. In this case,
///		B will only be executed if A evaluates to true." <br/>
///		"The second way is to use ? with a : as a conditional ternary: A ? B : C."
/// </remarks>
[TestClass]
public class ConditionalSpecTest
{
	[TestMethod("The ternary conditional returns the 'then' or 'else' branch")]
	public void TernaryReturnsTheSelectedBranch()
	{
		Assert.AreEqual(10d, Mo.Eval("1 ? 10 : 20"));
		Assert.AreEqual(20d, Mo.Eval("0 ? 10 : 20"));
	}

	[TestMethod("The binary conditional only executes its right hand side when the test is true")]
	public void BinaryConditionalOnlyRunsWhenTrue()
	{
		Assert.AreEqual(5d, Mo.Eval("t.a = 0; (1 > 0) ? { t.a = 5; }; return t.a;"));
		Assert.AreEqual(0d, Mo.Eval("t.a = 0; (1 < 0) ? { t.a = 5; }; return t.a;"));
	}

	[TestMethod("The binary conditional evaluates to 0.0 when the test is false")]
	public void BinaryConditionalIsZeroWhenFalse()
	{
		Assert.AreEqual(7d, Mo.Eval("1 ? 7"));
		Assert.AreEqual(0d, Mo.Eval("0 ? 7"));
	}

	/// <summary>
	///		The syntax guide's own example: <c>v.should_reset_a ? { v.a = 0; }</c>
	/// </summary>
	[TestMethod("The documented reset-and-max examples behave as specified")]
	public void SyntaxGuideConditionalExamples()
	{
		Assert.AreEqual(
			0d, Mo.Eval(
				@"
v.a = 42;
v.should_reset_a = 1;
v.should_reset_a ? { v.a = 0; };
return v.a;"));

		Assert.AreEqual(
			42d, Mo.Eval(
				@"
v.a = 42;
v.should_reset_a = 0;
v.should_reset_a ? { v.a = 0; };
return v.a;"));

		Assert.AreEqual(
			8d, Mo.Eval(
				@"
v.a = 3;
v.b = 8;
v.larger_value = (v.a > v.b) ? v.a : v.b;
return v.larger_value;"));
	}

	/// <summary>
	///		"You can group a series of statements into a single group by wrapping them in { and } symbols."
	/// </summary>
	[TestMethod("A brace scope executes every statement it contains")]
	public void BraceScopeRunsEveryStatement()
	{
		// The syntax guide's example, with q.life_time replaced by a variable so the test needs no entity.
		var result = Mo.Eval(
			@"
v.moo = 1;
v.life_time = 0;
(v.moo > 0) ? {
	v.x = math.sin(v.life_time * 45);
	v.x = v.x * v.x + 17.3;
	t.sin_x = math.sin(v.x);
	v.x = t.sin_x * t.sin_x + v.x * v.x;
	v.x = math.sqrt(v.x) * v.x * math.pi;
};
return v.x;");

		// math.sin(0) == 0, so v.x starts at 17.3.
		var x = 17.3d;
		var sinX = System.Math.Sin(x * (System.Math.PI / 180d));
		x = sinX * sinX + x * x;
		x = System.Math.Sqrt(x) * x * System.Math.PI;

		Assert.AreEqual(x, result, 1e-9);
	}

	[TestMethod("A brace scope evaluates to 0.0 unless it returns")]
	public void BraceScopeIsZeroWithoutReturn()
	{
		Assert.AreEqual(0d, Mo.Eval("1 ? { t.a = 5; t.a + 1; }"));
		Assert.AreEqual(6d, Mo.Eval("1 ? { t.a = 5; return t.a + 1; }"));
	}

	[TestMethod("A return inside a brace scope terminates the whole expression")]
	public void ReturnInsideScopeTerminatesTheExpression()
	{
		Assert.AreEqual(1d, Mo.Eval("t.a = 0; (1 > 0) ? { return 1; }; return 2;"));
		Assert.AreEqual(2d, Mo.Eval("t.a = 0; (1 < 0) ? { return 1; }; return 2;"));
	}

	[TestMethod("Conditionals can be nested inside brace scopes")]
	public void NestedConditionalsInScopes()
	{
		Assert.AreEqual(
			3d, Mo.Eval(
				@"
t.a = 0;
(1 > 0) ? {
	(2 > 1) ? { t.a = t.a + 1; };
	(2 > 3) ? { t.a = t.a + 10; };
	t.a = t.a + 2;
};
return t.a;"));
	}

	[TestMethod("A struct or entity reference is not a valid boolean test but does not crash")]
	public void ConditionalOnMissingValueTakesTheElseBranch()
	{
		var runtime = new MoLangRuntime();
		Assert.AreEqual(20d, Mo.Eval(runtime, "v.undefined ? 10 : 20"));
	}
}
