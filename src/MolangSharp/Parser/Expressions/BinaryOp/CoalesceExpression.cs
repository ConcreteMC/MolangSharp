using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	/// <summary>
	///		The null coalescing operator
	/// </summary>
	/// <remarks>
	///		"a ?? b will return a if it can be resolved to a value; otherwise, it will return b."
	///		<br/>
	///		A variable holding a falsy value such as 0.0 or an empty string <em>has</em> been resolved, so it is
	///		returned as-is. Only an operand that cannot be resolved at all - an uninitialized variable or a stale
	///		entity reference - falls through to the right hand side.
	/// </remarks>
	public class CoalesceExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public CoalesceExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			if (TryResolveLeft(scope, environment, out var value))
				return value;

			return Right.Evaluate(scope, environment);
		}

		private bool TryResolveLeft(MoScope scope, MoLangEnvironment environment, out IMoValue value)
		{
			// Evaluating a variable that was never assigned hands back 0.0, which is indistinguishable from a
			// variable that holds 0.0. Ask the environment about the path itself instead.
			if (Left is NameExpression nameExpression)
				return environment.TryGetValue(nameExpression.Name, out value) && value != null;

			try
			{
				value = Left.Evaluate(scope, environment);
			}
			catch (MoLangRuntimeException)
			{
				// An invalid path or a stale reference cannot be resolved either.
				value = null;
			}

			return value != null;
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "??";
		}
	}
}
