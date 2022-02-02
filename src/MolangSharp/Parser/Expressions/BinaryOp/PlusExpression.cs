using System;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class PlusExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public PlusExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			try
			{
				return new DoubleValue(
					Left.Evaluate(scope, environment).AsDouble() + Right.Evaluate(scope, environment).AsDouble());
			}
			catch (Exception ex)
			{
				throw new MoLangRuntimeException("An unexpected error occured.", ex);
			}
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "+";
		}
	}
}