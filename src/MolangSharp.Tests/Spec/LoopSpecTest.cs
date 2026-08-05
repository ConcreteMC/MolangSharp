using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the "Loop", "break" and "continue" sections of the Molang syntax guide.
/// </summary>
[TestClass]
public class LoopSpecTest
{
	/// <summary>
	///		The syntax guide's Fibonacci example.
	/// </summary>
	[TestMethod("The documented Fibonacci loop produces the expected values")]
	public void FibonacciExample()
	{
		const string script = @"
v.x = 1;
v.y = 1;
loop(10, {
	t.x = v.x + v.y;
	v.x = v.y;
	v.y = t.x;
});
";

		Assert.AreEqual(89d, Mo.Eval(script + "return v.x;"));
		Assert.AreEqual(144d, Mo.Eval(script + "return v.y;"));
	}

	[TestMethod("loop repeats its body exactly 'count' times")]
	public void LoopRunsCountTimes()
	{
		Assert.AreEqual(10d, Mo.Eval("t.a = 0; loop(10, { t.a = t.a + 1; }); return t.a;"));
		Assert.AreEqual(1d, Mo.Eval("t.a = 0; loop(1, { t.a = t.a + 1; }); return t.a;"));
	}

	[TestMethod("A loop count of zero or less never executes the body")]
	public void LoopWithNonPositiveCountDoesNothing()
	{
		Assert.AreEqual(0d, Mo.Eval("t.a = 0; loop(0, { t.a = t.a + 1; }); return t.a;"));
		Assert.AreEqual(0d, Mo.Eval("t.a = 0; loop(-5, { t.a = t.a + 1; }); return t.a;"));
	}

	/// <summary>
	///		"For array indexes, floats are C-style-cast to integers" - the loop counter is treated the same way.
	/// </summary>
	[TestMethod("A fractional loop count is truncated towards zero")]
	public void LoopCountIsTruncated()
	{
		Assert.AreEqual(3d, Mo.Eval("t.a = 0; loop(3.9, { t.a = t.a + 1; }); return t.a;"));
	}

	/// <summary>
	///		"The maximum loop counter is 1024 for safety reasons."
	/// </summary>
	[TestMethod("A loop count above 1024 is capped at 1024")]
	public void LoopCountIsCappedAt1024()
	{
		Assert.AreEqual(1024d, Mo.Eval("t.a = 0; loop(1024, { t.a = t.a + 1; }); return t.a;"));
		Assert.AreEqual(1024d, Mo.Eval("t.a = 0; loop(2000, { t.a = t.a + 1; }); return t.a;"));
		Assert.AreEqual(1024d, Mo.Eval("t.a = 0; loop(100000, { t.a = t.a + 1; }); return t.a;"));
	}

	/// <summary>
	///		The syntax guide's break example.
	/// </summary>
	[TestMethod("break exits the loop early")]
	public void BreakExample()
	{
		const string script = @"
v.x = 1;
v.y = 1;
loop(10, {t.x = v.x + v.y; v.x = v.y; v.y = t.x; (v.y > 20) ? break;});
";

		Assert.AreEqual(13d, Mo.Eval(script + "return v.x;"));
		Assert.AreEqual(21d, Mo.Eval(script + "return v.y;"));
	}

	/// <summary>
	///		"If there are multiple loops, break exits the innermost active loop." The syntax guide walks the
	///		expected result through explicitly: "resulting in a final value of v.x of 6 + 1 + 1 + 1 + ... + 1 = 15".
	/// </summary>
	[TestMethod("break only exits the innermost loop")]
	public void NestedBreakExitsInnermostLoopOnly()
	{
		Assert.AreEqual(
			15d, Mo.Eval(
				@"
v.x = 0;
loop(10, {loop(10, {v.x = v.x + 1; (v.x > 5) ? break;});});
return v.x;"));
	}

	/// <summary>
	///		The syntax guide's continue example: "The following results in v.x becoming 6.0".
	/// </summary>
	[TestMethod("continue skips the rest of the current iteration")]
	public void ContinueExample()
	{
		Assert.AreEqual(
			6d, Mo.Eval(
				@"
v.x = 0;
loop(10, {
	(v.x > 5) ? continue;
	v.x = v.x + 1;
});
return v.x;"));
	}

	[TestMethod("continue only affects the innermost loop")]
	public void NestedContinueAffectsInnermostLoopOnly()
	{
		// The inner loop always completes its 10 iterations; only its body is skipped once v.x exceeds 5.
		Assert.AreEqual(
			6d, Mo.Eval(
				@"
v.x = 0;
loop(3, {loop(10, {(v.x > 5) ? continue; v.x = v.x + 1;});});
return v.x;"));
	}

	[TestMethod("A return inside a loop terminates the whole expression")]
	public void ReturnInsideLoop()
	{
		Assert.AreEqual(
			4d, Mo.Eval(
				@"
t.a = 0;
loop(10, {
	t.a = t.a + 1;
	(t.a >= 4) ? { return t.a; };
});
return 99;"));
	}

	[TestMethod("Loops can be nested")]
	public void NestedLoops()
	{
		Assert.AreEqual(
			12d, Mo.Eval("t.a = 0; loop(3, { loop(4, { t.a = t.a + 1; }); }); return t.a;"));
	}

	/// <summary>
	///		"for_each - For iterating over an array of entities".
	/// </summary>
	[TestMethod("for_each visits every element of an array")]
	public void ForEachVisitsEveryElement()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(
			60d, Mo.Eval(runtime, "t.sum = 0; for_each(t.e, v.arr, { t.sum = t.sum + t.e; }); return t.sum;"));
	}

	[TestMethod("for_each supports break")]
	public void ForEachSupportsBreak()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(
			30d, Mo.Eval(
				runtime,
				"t.sum = 0; for_each(t.e, v.arr, { t.sum = t.sum + t.e; (t.sum >= 30) ? break; }); return t.sum;"));
	}

	[TestMethod("for_each supports continue")]
	public void ForEachSupportsContinue()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(
			40d, Mo.Eval(
				runtime,
				"t.sum = 0; for_each(t.e, v.arr, { (t.e == 20) ? continue; t.sum = t.sum + t.e; }); return t.sum;"));
	}

	[TestMethod("A return inside for_each terminates the whole expression")]
	public void ForEachSupportsReturn()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(
			20d, Mo.Eval(runtime, "for_each(t.e, v.arr, { (t.e == 20) ? { return t.e; }; }); return 99;"));
	}

	[TestMethod("for_each over an empty array never runs the body")]
	public void ForEachOverEmptyArray()
	{
		var runtime = new MoLangRuntime();
		runtime.Environment.Structs["variable"].Set(new MoPath("arr"), new ArrayStruct());

		Assert.AreEqual(0d, Mo.Eval(runtime, "t.a = 0; for_each(t.e, v.arr, { t.a = t.a + 1; }); return t.a;"));
	}

	private static MoLangRuntime ArrayRuntime()
	{
		var runtime = new MoLangRuntime();

		runtime.Environment.Structs["variable"].Set(
			new MoPath("arr"), new ArrayStruct(
				new List<IMoValue>
				{
					new DoubleValue(10), new DoubleValue(20), new DoubleValue(30)
				}));

		return runtime;
	}
}
