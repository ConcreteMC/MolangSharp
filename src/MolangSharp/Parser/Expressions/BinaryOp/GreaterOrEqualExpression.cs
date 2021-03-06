using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class GreaterOrEqualExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public GreaterOrEqualExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(
				Left.Evaluate(scope, environment).AsDouble() >= Right.Evaluate(scope, environment).AsDouble());
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return ">=";
		}
	}
}