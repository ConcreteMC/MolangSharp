using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class MinusExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public MinusExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(EvaluateDouble(scope, environment));
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return Left.EvaluateDouble(scope, environment) - Right.EvaluateDouble(scope, environment);
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "-";
		}
	}
}