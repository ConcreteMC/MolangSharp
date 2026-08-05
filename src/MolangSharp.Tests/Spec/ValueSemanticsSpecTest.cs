using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the "Values" section of the Molang syntax guide.
/// </summary>
/// <remarks>
///		"All numerical values are floats." <br/>
///		"Boolean values such as actor flags are converted and stored as a float value of either 0.0 or 1.0 for
///		values of false or true respectively." <br/>
///		"For boolean tests, a float value equivalent to 0.0 is false, and anything not equal to 0.0 is true."
/// </remarks>
[TestClass]
public class ValueSemanticsSpecTest
{
	[TestMethod("Booleans are stored as 1.0 and 0.0")]
	public void BooleansAreFloats()
	{
		Assert.AreEqual(1d, Mo.Eval("true"));
		Assert.AreEqual(0d, Mo.Eval("false"));
		Assert.AreEqual(1d, Mo.Eval("1 == 1"));
		Assert.AreEqual(0d, Mo.Eval("1 == 2"));
	}

	/// <summary>
	///		"anything not equal to 0.0 is true" - so negative and fractional values are truthy too.
	/// </summary>
	[TestMethod("Any value other than 0.0 is true in a boolean test")]
	public void NonZeroIsTrue()
	{
		Assert.AreEqual(10d, Mo.Eval("1 ? 10 : 20"), "1 is true");
		Assert.AreEqual(10d, Mo.Eval("2 ? 10 : 20"), "2 is true");
		Assert.AreEqual(10d, Mo.Eval("0.5 ? 10 : 20"), "0.5 is true");
		Assert.AreEqual(10d, Mo.Eval("-1 ? 10 : 20"), "-1 is true");
		Assert.AreEqual(10d, Mo.Eval("-0.5 ? 10 : 20"), "-0.5 is true");
		Assert.AreEqual(20d, Mo.Eval("0 ? 10 : 20"), "0 is false");
		Assert.AreEqual(20d, Mo.Eval("0.0 ? 10 : 20"), "0.0 is false");
	}

	[TestMethod("Negative values are true for logical operators")]
	public void NonZeroIsTrueForLogicalOperators()
	{
		Assert.AreEqual(1d, Mo.Eval("-1 && 1"));
		Assert.AreEqual(1d, Mo.Eval("-1 || 0"));
		Assert.AreEqual(1d, Mo.Eval("2 && 3"));
		Assert.AreEqual(0d, Mo.Eval("!-1"));
		Assert.AreEqual(0d, Mo.Eval("!2"));
		Assert.AreEqual(1d, Mo.Eval("!0"));
	}

	[TestMethod("Logical operators always yield 1.0 or 0.0")]
	public void LogicalOperatorsNormalizeToOneOrZero()
	{
		Assert.AreEqual(1d, Mo.Eval("2 && 3"));
		Assert.AreEqual(1d, Mo.Eval("5 || 0"));
		Assert.AreEqual(0d, Mo.Eval("0 && 5"));
	}

	/// <summary>
	///		"Like C, Molang statements terminate with a semicolon (;), but in simple cases, the terminating ;
	///		should be omitted, and the result of the single expression is directly returned."
	/// </summary>
	[TestMethod("A simple expression returns its value directly")]
	public void SimpleExpressionReturnsItsValue()
	{
		Assert.AreEqual(3d, Mo.Eval("1 + 2"));
		Assert.AreEqual(1d, Mo.Eval("math.sin(90)"), 1e-9);
	}

	/// <summary>
	///		"The last statement in a complex set of subexpressions should be a return statement that returns the final
	///		value to evaluate. If you leave off the return statement, the return value will be 0.0".
	/// </summary>
	[TestMethod("A complex expression without a return statement evaluates to 0.0")]
	public void ComplexExpressionWithoutReturnIsZero()
	{
		Assert.AreEqual(0d, Mo.Eval("t.a = 5;"));
		Assert.AreEqual(0d, Mo.Eval("t.a = 5; t.b = 6;"));
		Assert.AreEqual(0d, Mo.Eval("t.a = 5; t.a + 1;"));
		Assert.AreEqual(0d, Mo.Eval("t.a = 5; t.a + 1"));
	}

	[TestMethod("A complex expression stops at the return statement")]
	public void ReturnStopsExecution()
	{
		// The doc's own example: temp.moo/temp.baa then a return.
		Assert.AreEqual(6d, Mo.Eval("t.a = 1; t.b = 2; return t.a + t.b + 3; t.a = 100;"));
		Assert.AreEqual(1d, Mo.Eval("t.a = 1; return t.a; t.a = 2;"));
	}

	[TestMethod("The documented multi-statement example evaluates correctly")]
	public void SyntaxGuideComplexExample()
	{
		var runtime = new MoLangRuntime();
		runtime.Environment.SetValue(new MoPath("variable.anim_time"), new DoubleValue(30));
		runtime.Environment.SetValue(new MoPath("variable.life_time"), new DoubleValue(10));

		var result = Mo.Eval(
			runtime, @"
temp.moo = math.sin(v.anim_time * 1.23);
temp.baa = math.cos(v.life_time + 2.0);
return temp.moo * temp.moo + temp.baa;");

		var moo = System.Math.Sin(30 * 1.23 * (System.Math.PI / 180d));
		var baa = System.Math.Cos((10 + 2.0) * (System.Math.PI / 180d));

		Assert.AreEqual(moo * moo + baa, result, 1e-12);
	}

	/// <summary>
	///		"Errors (such as divide by zero, missing variables, null references, etc) generally return a value of 0.0."
	/// </summary>
	[TestMethod("Reading a variable that was never assigned yields 0.0")]
	public void MissingVariableIsZero()
	{
		Assert.AreEqual(0d, Mo.Eval("v.never_assigned"));
		Assert.AreEqual(0d, Mo.Eval("t.never_assigned"));
		Assert.AreEqual(0d, Mo.Eval("c.never_provided"));
		Assert.AreEqual(0d, Mo.Eval("v.never.assigned.at.all"));
	}

	/// <summary>
	///		"this - The current value that this expression will ultimately write to (context specific)".
	/// </summary>
	[TestMethod("'this' resolves to the value the expression writes to")]
	public void ThisResolvesToTheTargetValue()
	{
		var runtime = new MoLangRuntime();
		runtime.Environment.ThisVariable = new DoubleValue(12.5);

		Assert.AreEqual(12.5d, Mo.Eval(runtime, "this"));
		Assert.AreEqual(13.5d, Mo.Eval(runtime, "this + 1"));
	}
}
