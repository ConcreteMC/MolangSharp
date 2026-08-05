using System;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Parser.Tokenizer;
using ConcreteMC.MolangSharp.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Runs the math function conformance checks against <see cref="MoLangMathImpl"/>, the reflection based
///		alternative to <see cref="MoLangMath.Library"/>, so the two implementations cannot drift apart.
/// </summary>
[TestClass]
public class InteropMathFunctionsSpecTest
{
	private const double Tolerance = 1e-9;

	[TestMethod("The interop math library registers every documented function")]
	public void AllDocumentedFunctionsExist()
	{
		string[] calls =
		{
			"math.abs(-3)", "math.acos(0)", "math.asin(1)", "math.atan(1)", "math.atan2(1, 1)", "math.ceil(1.2)",
			"math.clamp(5, 0, 10)", "math.copy_sign(3, -1)", "math.cos(0)", "math.die_roll(3, 5, 5)",
			"math.die_roll_integer(3, 5, 5)", "math.exp(0)", "math.floor(1.5)", "math.hermite_blend(1)",
			"math.inverse_lerp(0, 10, 5)", "math.lerp(0, 10, 0.5)", "math.lerprotate(0, 90, 0.5)", "math.ln(2)",
			"math.max(1, 2)", "math.min(1, 2)", "math.min_angle(370)", "math.mod(7, 4)", "math.pi", "math.pow(2, 3)",
			"math.random(1, 1)", "math.random_integer(5, 5)", "math.round(1.5)", "math.sign(1)", "math.sin(90)",
			"math.sqrt(9)", "math.trunc(1.7)"
		};

		var missing = new List<string>();

		foreach (var call in calls)
		{
			if (Eval(call) == 0d)
				missing.Add(call);
		}

		foreach (var easing in EasingNames())
		{
			var call = $"math.{easing}(0, 10, 1)";

			if (Eval(call) == 0d)
				missing.Add(call);
		}

		Assert.AreEqual(0, missing.Count, $"Unimplemented math functions: {string.Join(", ", missing)}");
	}

	[TestMethod("The interop math library matches the documented behaviour")]
	public void BehaviourMatchesTheReference()
	{
		Assert.AreEqual(3d, Eval("math.abs(-3)"));

		Assert.AreEqual(1d, Eval("math.sin(90)"), Tolerance);
		Assert.AreEqual(-1d, Eval("math.cos(180)"), Tolerance);
		Assert.AreEqual(90d, Eval("math.asin(1)"), Tolerance);
		Assert.AreEqual(90d, Eval("math.acos(0)"), Tolerance);
		Assert.AreEqual(45d, Eval("math.atan(1)"), Tolerance);
		Assert.AreEqual(45d, Eval("math.atan2(1, 1)"), Tolerance);

		Assert.AreEqual(5d, Eval("math.clamp(5, 0, 10)"));
		Assert.AreEqual(0d, Eval("math.clamp(-5, 0, 10)"));
		Assert.AreEqual(10d, Eval("math.clamp(15, 0, 10)"));

		Assert.AreEqual(-3d, Eval("math.copy_sign(3, -1)"));
		Assert.AreEqual(3d, Eval("math.copy_sign(-3, 1)"));

		Assert.AreEqual(0.5d, Eval("math.hermite_blend(0.5)"), Tolerance);
		Assert.AreEqual(0.5d, Eval("math.inverse_lerp(10, 20, 15)"), Tolerance);
		Assert.AreEqual(45d, Eval("math.lerprotate(0, 90, 0.5)"), Tolerance);
		Assert.AreEqual(-170d, Eval("math.min_angle(190)"), Tolerance);

		Assert.AreEqual(3d, Eval("math.round(2.5)"));
		Assert.AreEqual(-3d, Eval("math.round(-2.5)"));

		Assert.AreEqual(1d, Eval("math.sign(5)"));
		Assert.AreEqual(-1d, Eval("math.sign(-5)"));
		Assert.AreEqual(-1d, Eval("math.sign(0)"));

		Assert.AreEqual(-1d, Eval("math.trunc(-1.7)"));
		Assert.AreEqual(1d, Eval("math.trunc(1.7)"));

		Assert.AreEqual(Math.PI, Eval("math.pi"), Tolerance);
		Assert.AreEqual(5d, Eval("math.random_integer(5, 5)"));
	}

	[TestMethod("Every interop easing function hits start at t=0 and end at t=1")]
	public void EasingEndpoints()
	{
		foreach (var easing in EasingNames())
		{
			Assert.AreEqual(3d, Eval($"math.{easing}(3, 9, 0)"), Tolerance, $"math.{easing} at t=0");
			Assert.AreEqual(9d, Eval($"math.{easing}(3, 9, 1)"), Tolerance, $"math.{easing} at t=1");
		}

		Assert.AreEqual(2.5d, Eval("math.ease_in_quad(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(7.5d, Eval("math.ease_out_quad(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Eval("math.ease_in_out_quad(0, 10, 0.5)"), Tolerance);
	}

	private static IEnumerable<string> EasingNames()
	{
		return MoLangMath.Easings.Keys;
	}

	/// <summary>
	///		Parses without the <see cref="ConcreteMC.MolangSharp.Parser.Visitors.MathOptimizationVisitor"/> so that
	///		constant math calls are not folded using the other math implementation, then evaluates against
	///		<see cref="MoLangMathImpl"/>.
	/// </summary>
	private static double Eval(string expression)
	{
		var parsed = new MoLangParser(new TokenIterator(expression)).Parse();

		var runtime = new MoLangRuntime();
		runtime.Environment.Structs["math"] = MoLangMathImpl.Library;

		return runtime.Execute(parsed).AsDouble();
	}
}
