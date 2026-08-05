using System;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	/// <summary>
	///		Repeats an expression 'n' times
	/// </summary>
	public class LoopExpression : Expression
	{
		/// <summary>
		///		The highest number of iterations a single loop may run
		/// </summary>
		/// <remarks>
		///		"The maximum loop counter is 1024 for safety reasons."
		/// </remarks>
		public const int MaxIterations = 1024;

		public IExpression Count => Parameters[0];
		public IExpression Body => Parameters[1];

		public LoopExpression(IExpression count, IExpression body) : base(count, body) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			int loop = Math.Min((int) Count.Evaluate(scope, environment).AsDouble(), MaxIterations);
			MoScope subScope = new MoScope(scope.Runtime) {Runtime = scope.Runtime};

			while (loop > 0)
			{
				subScope.IsContinue = false;
				subScope.IsBreak = false;

				Body.Evaluate(subScope, environment);
				loop--;

				if (subScope.ReturnValue != null)
				{
					// "return ... stops execution of the expression", not just of the loop body.
					scope.ReturnValue = subScope.ReturnValue;

					return subScope.ReturnValue;
				}
				else if (subScope.IsBreak)
				{
					break;
				}
			}

			return DoubleValue.Zero;
		}
	}
}