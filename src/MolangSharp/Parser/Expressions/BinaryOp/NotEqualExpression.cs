using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class NotEqualExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public NotEqualExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		/// <remarks>
		///		Compares the values rather than their numeric form, because <c>!=</c> also has to work on strings.
		///		Returns the shared 1.0/0.0 instances, so a comparison never allocates.
		/// </remarks>
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return IsEqual(scope, environment) ? DoubleValue.Zero : DoubleValue.One;
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return IsEqual(scope, environment) ? 0d : 1d;
		}

		private bool IsEqual(MoScope scope, MoLangEnvironment environment)
		{
			return Left.Evaluate(scope, environment).Equals(Right.Evaluate(scope, environment));
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "!=";
		}
	}
}