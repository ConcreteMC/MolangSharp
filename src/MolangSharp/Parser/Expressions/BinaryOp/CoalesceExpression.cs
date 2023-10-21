using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class CoalesceExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public CoalesceExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			IMoValue evalLeft = Left.Evaluate(scope, environment);

			//IMoValue value = environment.GetValue(new MoPath(evalLeft.AsString()));

			if (evalLeft == null || !evalLeft.AsBool())
			{
				return Right.Evaluate(scope, environment);
			}
			else
			{
				return evalLeft;
			}
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "??";
		}
	}
}