using System;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	/// <summary>
	///		A complex expression: a series of statements, each terminated by a semicolon
	/// </summary>
	/// <remarks>
	///		"The last statement in a complex set of subexpressions should be a return statement that returns the final
	///		value to evaluate. If you leave off the return statement, the return value will be 0.0", so the value of
	///		the last statement is deliberately discarded.
	/// </remarks>
	public class ScriptExpression : Expression
	{
		public ScriptExpression(IExpression[] expressions) : base(expressions) { }

		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			foreach (IExpression expression in Parameters)
			{
				if (expression == null)
					continue;

				try
				{
					expression.Evaluate(scope, environment);
				}
				catch (Exception ex)
				{
					throw new MoLangRuntimeException(
						expression, "An error occured while evaluating the expression", ex);
				}

				if (scope.ReturnValue != null)
					return scope.ReturnValue;
			}

			return DoubleValue.Zero;
		}
	}
}