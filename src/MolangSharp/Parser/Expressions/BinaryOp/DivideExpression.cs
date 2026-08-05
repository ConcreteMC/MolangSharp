using System;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class DivideExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public DivideExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(EvaluateDouble(scope, environment));
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			try
			{
				return Left.EvaluateDouble(scope, environment) / Right.EvaluateDouble(scope, environment);
			}
			catch (Exception ex)
			{
				throw new MoLangRuntimeException(this, "An unexpected error occured.", ex);
			}
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "/";
		}
	}
}