using System;
using System.Collections.Generic;
using System.Globalization;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the Molang math library against the official math function reference.
/// </summary>
/// <remarks>
///		https://learn.microsoft.com/en-us/minecraft/creator/reference/content/molangreference/examples/molangconcepts/mathfunctions
///		<br/>
///		Molang's trigonometry works in degrees: "math.sin(value) - Sine (in degrees) of value".
/// </remarks>
[TestClass]
public class MathFunctionsSpecTest
{
	private const double Tolerance = 1e-9;

	/// <summary>
	///		Every function listed in the official math function reference, called with arguments whose correct result
	///		is non-zero. An unregistered <c>math.*</c> member resolves to 0.0, so a zero result means "not implemented".
	/// </summary>
	private static readonly string[] DocumentedFunctions =
	{
		"math.abs(-3)", "math.acos(0)", "math.asin(1)", "math.atan(1)", "math.atan2(1, 1)", "math.ceil(1.2)",
		"math.clamp(5, 0, 10)", "math.copy_sign(3, -1)", "math.cos(0)", "math.die_roll(3, 5, 5)",
		"math.die_roll_integer(3, 5, 5)", "math.exp(0)", "math.floor(1.5)", "math.hermite_blend(1)",
		"math.inverse_lerp(0, 10, 5)", "math.lerp(0, 10, 0.5)", "math.lerprotate(0, 90, 0.5)", "math.ln(2)",
		"math.max(1, 2)", "math.min(1, 2)", "math.min_angle(370)", "math.mod(7, 4)", "math.pi", "math.pow(2, 3)",
		"math.random(1, 1)", "math.random_integer(5, 5)", "math.round(1.5)", "math.sign(1)", "math.sin(90)",
		"math.sqrt(9)", "math.trunc(1.7)"
	};

	/// <summary>
	///		The easing functions documented in the math function reference. All of them take (start, end, 0_to_1).
	/// </summary>
	private static readonly string[] EasingFunctions =
	{
		"ease_in_back", "ease_in_bounce", "ease_in_circ", "ease_in_cubic", "ease_in_elastic", "ease_in_expo",
		"ease_in_quad", "ease_in_quart", "ease_in_quint", "ease_in_sine", "ease_out_back", "ease_out_bounce",
		"ease_out_circ", "ease_out_cubic", "ease_out_elastic", "ease_out_expo", "ease_out_quad", "ease_out_quart",
		"ease_out_quint", "ease_out_sine", "ease_in_out_back", "ease_in_out_bounce", "ease_in_out_circ",
		"ease_in_out_cubic", "ease_in_out_elastic", "ease_in_out_expo", "ease_in_out_quad", "ease_in_out_quart",
		"ease_in_out_quint", "ease_in_out_sine"
	};

	[TestMethod("Every documented math function is registered")]
	public void AllDocumentedFunctionsExist()
	{
		var missing = new List<string>();

		foreach (var call in DocumentedFunctions)
		{
			if (Mo.Eval(call) == 0d)
				missing.Add(call);
		}

		foreach (var easing in EasingFunctions)
		{
			var call = $"math.{easing}(0, 10, 1)";

			if (Mo.Eval(call) == 0d)
				missing.Add(call);
		}

		Assert.AreEqual(0, missing.Count, $"Unimplemented math functions: {string.Join(", ", missing)}");
	}

	[TestMethod("math.abs returns the absolute value")]
	public void Abs()
	{
		Assert.AreEqual(3d, Mo.Eval("math.abs(-3)"));
		Assert.AreEqual(3d, Mo.Eval("math.abs(3)"));
		Assert.AreEqual(0d, Mo.Eval("math.abs(0)"));
	}

	[TestMethod("math.sin and math.cos work in degrees")]
	public void SinAndCos()
	{
		Assert.AreEqual(0d, Mo.Eval("math.sin(0)"), Tolerance);
		Assert.AreEqual(1d, Mo.Eval("math.sin(90)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.sin(180)"), Tolerance);
		Assert.AreEqual(-1d, Mo.Eval("math.sin(270)"), Tolerance);

		Assert.AreEqual(1d, Mo.Eval("math.cos(0)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.cos(90)"), Tolerance);
		Assert.AreEqual(-1d, Mo.Eval("math.cos(180)"), Tolerance);
	}

	/// <summary>
	///		Molang's trigonometry is expressed in degrees, so the inverse functions return degrees as well.
	/// </summary>
	[TestMethod("The inverse trigonometry functions return degrees")]
	public void InverseTrigonometry()
	{
		Assert.AreEqual(90d, Mo.Eval("math.asin(1)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.asin(0)"), Tolerance);
		Assert.AreEqual(-90d, Mo.Eval("math.asin(-1)"), Tolerance);

		Assert.AreEqual(0d, Mo.Eval("math.acos(1)"), Tolerance);
		Assert.AreEqual(90d, Mo.Eval("math.acos(0)"), Tolerance);
		Assert.AreEqual(180d, Mo.Eval("math.acos(-1)"), Tolerance);

		Assert.AreEqual(45d, Mo.Eval("math.atan(1)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.atan(0)"), Tolerance);

		Assert.AreEqual(45d, Mo.Eval("math.atan2(1, 1)"), Tolerance);
		Assert.AreEqual(90d, Mo.Eval("math.atan2(1, 0)"), Tolerance);
		Assert.AreEqual(-45d, Mo.Eval("math.atan2(-1, 1)"), Tolerance);

		// sin/asin and cos/acos must round-trip.
		Assert.AreEqual(30d, Mo.Eval("math.asin(math.sin(30))"), Tolerance);
		Assert.AreEqual(60d, Mo.Eval("math.acos(math.cos(60))"), Tolerance);
	}

	[TestMethod("math.ceil rounds up and math.floor rounds down")]
	public void CeilAndFloor()
	{
		Assert.AreEqual(2d, Mo.Eval("math.ceil(1.2)"));
		Assert.AreEqual(-1d, Mo.Eval("math.ceil(-1.2)"));
		Assert.AreEqual(1d, Mo.Eval("math.floor(1.8)"));
		Assert.AreEqual(-2d, Mo.Eval("math.floor(-1.2)"));
	}

	/// <summary>
	///		"math.clamp(value, min, max) - Clamp value to between min and max inclusive".
	/// </summary>
	[TestMethod("math.clamp clamps its first argument between min and max")]
	public void Clamp()
	{
		Assert.AreEqual(5d, Mo.Eval("math.clamp(5, 0, 10)"));
		Assert.AreEqual(0d, Mo.Eval("math.clamp(-5, 0, 10)"));
		Assert.AreEqual(10d, Mo.Eval("math.clamp(15, 0, 10)"));
		Assert.AreEqual(0d, Mo.Eval("math.clamp(0, 0, 10)"));
		Assert.AreEqual(10d, Mo.Eval("math.clamp(10, 0, 10)"));
		Assert.AreEqual(-3d, Mo.Eval("math.clamp(-3, -10, -1)"));
	}

	/// <summary>
	///		"math.copy_sign(A, B) - Compose a floating-point value with the magnitude of x and the sign of y."
	/// </summary>
	[TestMethod("math.copy_sign takes the magnitude of A and the sign of B")]
	public void CopySign()
	{
		Assert.AreEqual(-3d, Mo.Eval("math.copy_sign(3, -1)"));
		Assert.AreEqual(3d, Mo.Eval("math.copy_sign(-3, 1)"));
		Assert.AreEqual(3d, Mo.Eval("math.copy_sign(3, 5)"));
		Assert.AreEqual(-3d, Mo.Eval("math.copy_sign(-3, -5)"));
	}

	/// <summary>
	///		"math.die_roll(num, low, high) - Returns the sum of 'num' random numbers, each with a value from low to high."
	/// </summary>
	[TestMethod("math.die_roll sums 'num' random values in range")]
	public void DieRoll()
	{
		Assert.AreEqual(15d, Mo.Eval("math.die_roll(3, 5, 5)"), Tolerance);

		for (var i = 0; i < 50; i++)
		{
			var result = Mo.Eval("math.die_roll(3, 1, 6)");
			Assert.IsTrue(result >= 3d && result <= 18d, $"math.die_roll(3, 1, 6) returned {result}");
		}
	}

	[TestMethod("math.die_roll_integer sums 'num' random integers in range")]
	public void DieRollInteger()
	{
		for (var i = 0; i < 50; i++)
		{
			var result = Mo.Eval("math.die_roll_integer(3, 1, 6)");
			Assert.IsTrue(result >= 3d && result <= 18d, $"math.die_roll_integer(3, 1, 6) returned {result}");
			Assert.AreEqual(result, Math.Truncate(result), "math.die_roll_integer must return whole numbers");
		}
	}

	[TestMethod("math.exp and math.ln are inverses")]
	public void ExpAndLn()
	{
		Assert.AreEqual(1d, Mo.Eval("math.exp(0)"), Tolerance);
		Assert.AreEqual(Math.E, Mo.Eval("math.exp(1)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.ln(1)"), Tolerance);
		Assert.AreEqual(1d, Mo.Eval("math.ln(math.exp(1))"), Tolerance);
	}

	/// <summary>
	///		"math.hermite_blend(value) - Interpolate points on a smooth curve using one of the Hermite Basis
	///		functions: 3t^2 - 2t^3."
	/// </summary>
	[TestMethod("math.hermite_blend evaluates 3t^2 - 2t^3")]
	public void HermiteBlend()
	{
		Assert.AreEqual(0d, Mo.Eval("math.hermite_blend(0)"), Tolerance);
		Assert.AreEqual(1d, Mo.Eval("math.hermite_blend(1)"), Tolerance);
		Assert.AreEqual(0.5d, Mo.Eval("math.hermite_blend(0.5)"), Tolerance);
		Assert.AreEqual(0.15625d, Mo.Eval("math.hermite_blend(0.25)"), Tolerance);
		Assert.AreEqual(0.84375d, Mo.Eval("math.hermite_blend(0.75)"), Tolerance);
	}

	/// <summary>
	///		"math.inverse_lerp(start, end, value) - Returns the normalized progress between start and end given value".
	/// </summary>
	[TestMethod("math.inverse_lerp returns the normalized progress")]
	public void InverseLerp()
	{
		Assert.AreEqual(0.5d, Mo.Eval("math.inverse_lerp(10, 20, 15)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.inverse_lerp(10, 20, 10)"), Tolerance);
		Assert.AreEqual(1d, Mo.Eval("math.inverse_lerp(10, 20, 20)"), Tolerance);
		Assert.AreEqual(0.25d, Mo.Eval("math.inverse_lerp(0, 4, 1)"), Tolerance);

		// math.lerp and math.inverse_lerp must round-trip.
		Assert.AreEqual(15d, Mo.Eval("math.lerp(10, 20, math.inverse_lerp(10, 20, 15))"), Tolerance);
	}

	[TestMethod("math.lerp interpolates linearly between start and end")]
	public void Lerp()
	{
		Assert.AreEqual(0d, Mo.Eval("math.lerp(0, 10, 0)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.lerp(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(10d, Mo.Eval("math.lerp(0, 10, 1)"), Tolerance);
		Assert.AreEqual(-5d, Mo.Eval("math.lerp(-10, 0, 0.5)"), Tolerance);
	}

	/// <summary>
	///		"math.lerprotate(start, end, 0_to_1) - Computes a linear interpolation around a circle in the shortest
	///		direction from a degrees to b degrees".
	/// </summary>
	[TestMethod("math.lerprotate interpolates around a circle")]
	public void LerpRotate()
	{
		Assert.AreEqual(45d, Mo.Eval("math.lerprotate(0, 90, 0.5)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.lerprotate(0, 90, 0)"), Tolerance);
		Assert.AreEqual(90d, Mo.Eval("math.lerprotate(0, 90, 1)"), Tolerance);

		// Crossing 0/360 must take the short way round: 350 -> 10 passes through 0, not through 180.
		Assert.AreEqual(0d, Mo.Eval("math.lerprotate(350, 10, 0.5)"), Tolerance);
	}

	[TestMethod("math.max and math.min pick the higher and lower value")]
	public void MaxAndMin()
	{
		Assert.AreEqual(2d, Mo.Eval("math.max(1, 2)"));
		Assert.AreEqual(2d, Mo.Eval("math.max(2, 1)"));
		Assert.AreEqual(1d, Mo.Eval("math.min(1, 2)"));
		Assert.AreEqual(-2d, Mo.Eval("math.min(-1, -2)"));
	}

	/// <summary>
	///		"math.min_angle(value) - Minimize angle magnitude (in degrees) into the range [-180, 180)".
	/// </summary>
	[TestMethod("math.min_angle folds an angle into [-180, 180)")]
	public void MinAngle()
	{
		Assert.AreEqual(0d, Mo.Eval("math.min_angle(0)"), Tolerance);
		Assert.AreEqual(10d, Mo.Eval("math.min_angle(370)"), Tolerance);
		Assert.AreEqual(-170d, Mo.Eval("math.min_angle(190)"), Tolerance);
		Assert.AreEqual(170d, Mo.Eval("math.min_angle(-190)"), Tolerance);
		Assert.AreEqual(-180d, Mo.Eval("math.min_angle(180)"), Tolerance);
		Assert.AreEqual(-180d, Mo.Eval("math.min_angle(-180)"), Tolerance);
		Assert.AreEqual(-90d, Mo.Eval("math.min_angle(270)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.min_angle(720)"), Tolerance);
	}

	/// <summary>
	///		"math.mod(value, denominator) - Return the remainder of value / denominator".
	/// </summary>
	[TestMethod("math.mod returns the remainder")]
	public void Mod()
	{
		Assert.AreEqual(3d, Mo.Eval("math.mod(7, 4)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.mod(8, 4)"), Tolerance);
		Assert.AreEqual(0.5d, Mo.Eval("math.mod(2.5, 1)"), Tolerance);
	}

	[TestMethod("math.pi is the constant pi and needs no parentheses")]
	public void Pi()
	{
		Assert.AreEqual(Math.PI, Mo.Eval("math.pi"), Tolerance);
		Assert.AreEqual(Math.PI * 2, Mo.Eval("math.pi * 2"), Tolerance);
	}

	[TestMethod("math.pow raises base to the exponent")]
	public void Pow()
	{
		Assert.AreEqual(1024d, Mo.Eval("math.pow(2, 10)"), Tolerance);
		Assert.AreEqual(1d, Mo.Eval("math.pow(2, 0)"), Tolerance);
		Assert.AreEqual(0.25d, Mo.Eval("math.pow(2, -2)"), Tolerance);
	}

	/// <summary>
	///		"math.random(low, high) - Random value between low and high inclusive".
	/// </summary>
	[TestMethod("math.random stays in range and is re-evaluated every time")]
	public void Random()
	{
		var runtime = new MoLangRuntime();
		var expression = MoLangParser.Parse("math.random(0, 1)");

		var distinct = new HashSet<double>();

		for (var i = 0; i < 200; i++)
		{
			var value = runtime.Execute(expression).AsDouble();
			Assert.IsTrue(value >= 0d && value <= 1d, $"math.random(0, 1) returned {value}");
			distinct.Add(value);
		}

		Assert.IsTrue(
			distinct.Count > 1,
			"math.random must produce a new value on every evaluation, it must not be folded into a constant");
	}

	/// <summary>
	///		"math.random_integer(low, high) - Random integer value between low and high inclusive".
	/// </summary>
	[TestMethod("math.random_integer is inclusive of both bounds")]
	public void RandomInteger()
	{
		Assert.AreEqual(5d, Mo.Eval("math.random_integer(5, 5)"));

		var runtime = new MoLangRuntime();
		var expression = MoLangParser.Parse("math.random_integer(1, 3)");
		var seen = new HashSet<double>();

		for (var i = 0; i < 500; i++)
		{
			var value = runtime.Execute(expression).AsDouble();
			Assert.IsTrue(value >= 1d && value <= 3d, $"math.random_integer(1, 3) returned {value}");
			Assert.AreEqual(value, Math.Truncate(value), "math.random_integer must return whole numbers");
			seen.Add(value);
		}

		Assert.IsTrue(seen.Contains(1d), "the low bound must be reachable");
		Assert.IsTrue(seen.Contains(3d), "the high bound must be reachable (inclusive)");
	}

	/// <summary>
	///		Molang's math library is "modeled on the C math library", where round() rounds half away from zero.
	/// </summary>
	[TestMethod("math.round rounds halves away from zero")]
	public void Round()
	{
		Assert.AreEqual(1d, Mo.Eval("math.round(1.4)"));
		Assert.AreEqual(2d, Mo.Eval("math.round(1.5)"));
		Assert.AreEqual(3d, Mo.Eval("math.round(2.5)"));
		Assert.AreEqual(-3d, Mo.Eval("math.round(-2.5)"));
		Assert.AreEqual(-1d, Mo.Eval("math.round(-1.4)"));
	}

	/// <summary>
	///		"math.sign(value) - Returns 1 if value is positive, -1 otherwise".
	/// </summary>
	[TestMethod("math.sign returns 1 for positive values and -1 otherwise")]
	public void Sign()
	{
		Assert.AreEqual(1d, Mo.Eval("math.sign(5)"));
		Assert.AreEqual(1d, Mo.Eval("math.sign(0.001)"));
		Assert.AreEqual(-1d, Mo.Eval("math.sign(-5)"));
		Assert.AreEqual(-1d, Mo.Eval("math.sign(0)"));
	}

	[TestMethod("math.sqrt returns the square root")]
	public void Sqrt()
	{
		Assert.AreEqual(3d, Mo.Eval("math.sqrt(9)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.sqrt(0)"), Tolerance);
		Assert.AreEqual(1.5d, Mo.Eval("math.sqrt(2.25)"), Tolerance);
	}

	/// <summary>
	///		"math.trunc(value) - Round value towards zero".
	/// </summary>
	[TestMethod("math.trunc rounds towards zero")]
	public void Trunc()
	{
		Assert.AreEqual(1d, Mo.Eval("math.trunc(1.7)"));
		Assert.AreEqual(-1d, Mo.Eval("math.trunc(-1.7)"));
		Assert.AreEqual(0d, Mo.Eval("math.trunc(0.9)"));
		Assert.AreEqual(0d, Mo.Eval("math.trunc(-0.9)"));
		Assert.AreEqual(2d, Mo.Eval("math.trunc(2)"));
	}

	/// <summary>
	///		Every easing function is documented as "Output goes from start to end via 0_to_1", so all of them must
	///		hit exactly start at 0 and exactly end at 1.
	/// </summary>
	[TestMethod("Every easing function hits start at t=0 and end at t=1")]
	public void EasingEndpoints()
	{
		foreach (var easing in EasingFunctions)
		{
			Assert.AreEqual(3d, Mo.Eval($"math.{easing}(3, 9, 0)"), Tolerance, $"math.{easing} at t=0");
			Assert.AreEqual(9d, Mo.Eval($"math.{easing}(3, 9, 1)"), Tolerance, $"math.{easing} at t=1");
		}
	}

	[TestMethod("Every easing function clamps its interpolant to [0, 1]")]
	public void EasingClampsInterpolant()
	{
		foreach (var easing in EasingFunctions)
		{
			Assert.AreEqual(3d, Mo.Eval($"math.{easing}(3, 9, -1)"), Tolerance, $"math.{easing} below 0");
			Assert.AreEqual(9d, Mo.Eval($"math.{easing}(3, 9, 2)"), Tolerance, $"math.{easing} above 1");
		}
	}

	[TestMethod("The polynomial easing functions match their standard definitions")]
	public void PolynomialEasingCurves()
	{
		Assert.AreEqual(2.5d, Mo.Eval("math.ease_in_quad(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(7.5d, Mo.Eval("math.ease_out_quad(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_quad(0, 10, 0.5)"), Tolerance);

		Assert.AreEqual(1.25d, Mo.Eval("math.ease_in_cubic(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(8.75d, Mo.Eval("math.ease_out_cubic(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_cubic(0, 10, 0.5)"), Tolerance);

		Assert.AreEqual(0.625d, Mo.Eval("math.ease_in_quart(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(9.375d, Mo.Eval("math.ease_out_quart(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_quart(0, 10, 0.5)"), Tolerance);

		Assert.AreEqual(0.3125d, Mo.Eval("math.ease_in_quint(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(9.6875d, Mo.Eval("math.ease_out_quint(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_quint(0, 10, 0.5)"), Tolerance);
	}

	[TestMethod("The trigonometric, exponential and circular easing functions match their standard definitions")]
	public void TranscendentalEasingCurves()
	{
		Assert.AreEqual(10d * (1d - Math.Cos(Math.PI / 4d)), Mo.Eval("math.ease_in_sine(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(10d * Math.Sin(Math.PI / 4d), Mo.Eval("math.ease_out_sine(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_sine(0, 10, 0.5)"), Tolerance);

		Assert.AreEqual(10d * Math.Pow(2d, -5d), Mo.Eval("math.ease_in_expo(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(10d * (1d - Math.Pow(2d, -5d)), Mo.Eval("math.ease_out_expo(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_expo(0, 10, 0.5)"), Tolerance);

		Assert.AreEqual(
			10d * (1d - Math.Sqrt(1d - 0.25d)), Mo.Eval("math.ease_in_circ(0, 10, 0.5)"), Tolerance);

		Assert.AreEqual(10d * Math.Sqrt(1d - 0.25d), Mo.Eval("math.ease_out_circ(0, 10, 0.5)"), Tolerance);
		Assert.AreEqual(5d, Mo.Eval("math.ease_in_out_circ(0, 10, 0.5)"), Tolerance);
	}

	/// <summary>
	///		"math.ease_in_back ... overshooting backward before accelerating into the end" and
	///		"math.ease_out_back ... overshooting past the end before settling into it".
	/// </summary>
	[TestMethod("The back easing functions overshoot")]
	public void BackEasingOvershoots()
	{
		Assert.IsTrue(Mo.Eval("math.ease_in_back(0, 10, 0.2)") < 0d, "ease_in_back must undershoot the start");
		Assert.IsTrue(Mo.Eval("math.ease_out_back(0, 10, 0.8)") > 10d, "ease_out_back must overshoot the end");

		const double c1 = 1.70158d;
		const double c3 = c1 + 1d;
		var expected = 10d * (c3 * 0.125d - c1 * 0.25d);
		Assert.AreEqual(expected, Mo.Eval("math.ease_in_back(0, 10, 0.5)"), Tolerance);
	}

	/// <summary>
	///		"math.ease_out_bounce ... approaching the end with bounce oscillations that diminish over time".
	/// </summary>
	[TestMethod("The bounce easing functions stay within the animated range")]
	public void BounceEasingStaysInRange()
	{
		for (var i = 0; i <= 20; i++)
		{
			var t = i / 20d;

			foreach (var easing in new[] {"ease_in_bounce", "ease_out_bounce", "ease_in_out_bounce"})
			{
				var value = Mo.Eval($"math.{easing}(0, 10, {t.ToString(CultureInfo.InvariantCulture)})");
				Assert.IsTrue(value >= -Tolerance && value <= 10d + Tolerance, $"math.{easing} at t={t} was {value}");
			}
		}

		// The first bounce segment of ease_out_bounce is 7.5625 * t^2.
		Assert.AreEqual(10d * 7.5625d * 0.04d, Mo.Eval("math.ease_out_bounce(0, 10, 0.2)"), Tolerance);
	}

	[TestMethod("The elastic easing functions oscillate outside the animated range")]
	public void ElasticEasingOscillates()
	{
		var overshoots = false;

		for (var i = 0; i <= 40; i++)
		{
			var t = (i / 40d).ToString(CultureInfo.InvariantCulture);

			if (Mo.Eval($"math.ease_out_elastic(0, 10, {t})") > 10d)
				overshoots = true;
		}

		Assert.IsTrue(overshoots, "ease_out_elastic must overshoot the end value at least once");
		Assert.IsTrue(Mo.Eval("math.ease_in_elastic(0, 10, 0.3)") < 0d, "ease_in_elastic must undershoot the start");
	}

	[TestMethod("Easing functions interpolate between arbitrary start and end values")]
	public void EasingRespectsStartAndEnd()
	{
		// ease_in_out_quad(0.25) == 0.125, so the output is -10 + 20 * 0.125.
		Assert.AreEqual(-7.5d, Mo.Eval("math.ease_in_out_quad(-10, 10, 0.25)"), Tolerance);
		Assert.AreEqual(7.5d, Mo.Eval("math.ease_in_out_quad(-10, 10, 0.75)"), Tolerance);
		Assert.AreEqual(0d, Mo.Eval("math.ease_in_out_quad(-10, 10, 0.5)"), Tolerance);

		// A descending range still eases from start to end: 10 + (0 - 10) * 0.25.
		Assert.AreEqual(7.5d, Mo.Eval("math.ease_in_quad(10, 0, 0.5)"), Tolerance);
	}
}
