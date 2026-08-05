using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class BooleanAndExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public BooleanAndExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		/// <remarks>
		///		Returns the shared 1.0/0.0 instances, so a logical operator never allocates.
		/// </remarks>
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return IsTrue(scope, environment) ? DoubleValue.One : DoubleValue.Zero;
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return IsTrue(scope, environment) ? 1d : 0d;
		}

		private bool IsTrue(MoScope scope, MoLangEnvironment environment)
		{
			return Left.EvaluateDouble(scope, environment) != 0d
			       && Right.EvaluateDouble(scope, environment) != 0d;
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "&&";
		}
	}
}