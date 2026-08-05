using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Shorthands shared by the Molang specification conformance tests.
/// </summary>
/// <remarks>
///		The tests in this namespace assert the behaviour described by the official Molang documentation: <br/>
///		- https://learn.microsoft.com/en-us/minecraft/creator/documents/molang/syntax-guide <br/>
///		- https://learn.microsoft.com/en-us/minecraft/creator/reference/content/molangreference/examples/molangconcepts/mathfunctions <br/>
///		- https://bedrock.dev/docs/stable/Molang
/// </remarks>
internal static class Mo
{
	/// <summary>
	///		Parses and executes <paramref name="expression"/> on a fresh runtime.
	/// </summary>
	public static IMoValue Value(string expression)
	{
		return Value(new MoLangRuntime(), expression);
	}

	/// <summary>
	///		Parses and executes <paramref name="expression"/> on <paramref name="runtime"/>.
	/// </summary>
	public static IMoValue Value(MoLangRuntime runtime, string expression)
	{
		return runtime.Execute(MoLangParser.Parse(expression));
	}

	/// <summary>
	///		Parses and executes <paramref name="expression"/> on a fresh runtime and returns the numeric result.
	/// </summary>
	public static double Eval(string expression)
	{
		return Value(expression).AsDouble();
	}

	/// <summary>
	///		Parses and executes <paramref name="expression"/> on <paramref name="runtime"/> and returns the numeric result.
	/// </summary>
	public static double Eval(MoLangRuntime runtime, string expression)
	{
		return Value(runtime, expression).AsDouble();
	}
}
