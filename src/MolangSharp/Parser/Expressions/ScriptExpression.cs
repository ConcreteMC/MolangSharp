using System;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class ScriptExpression : Expression
	{
		public ScriptExpression(IExpression[] expressions) : base(expressions) { }

		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			IMoValue result = DoubleValue.Zero;
			// MoScope scope = new MoScope(this);

			foreach (IExpression expression in Parameters)
			{
				if (expression == null)
					continue;

				try
				{
					result = expression.Evaluate(scope, environment);

					if (scope.ReturnValue != null)
					{
						result = scope.ReturnValue;

						break;
					}
				}
				catch (Exception ex)
				{
					throw new MoLangRuntimeException(
						expression, "An error occured while evaluating the expression", ex);
				}
			}

			return result;
		}
	}
}