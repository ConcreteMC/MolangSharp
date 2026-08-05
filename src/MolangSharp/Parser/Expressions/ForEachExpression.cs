using System.Collections.Generic;
using System.Linq;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	/// <summary>
	///		Iterates over an array
	/// </summary>
	public class ForEachExpression : Expression
	{
		public IExpression Variable => Parameters[0];
		public IExpression Array => Parameters[1];
		public IExpression Body => Parameters[2];

		public ForEachExpression(IExpression variable, IExpression array, IExpression body) : base(
			variable, array, body) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			IEnumerable<IMoValue> values = GetValues(Array.Evaluate(scope, environment));

			if (values == null)
				return DoubleValue.Zero;

			MoScope subScope = new MoScope(scope.Runtime);

			foreach (IMoValue value in values)
			{
				subScope.IsContinue = false;
				subScope.IsBreak = false;

				Variable.Assign(
					subScope, environment, value is VariableStruct vss ? vss.Map.FirstOrDefault().Value : value);

				Body.Evaluate(subScope, environment);

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

		private static IEnumerable<IMoValue> GetValues(IMoValue array)
		{
			switch (array)
			{
				case ArrayStruct arrayStruct:
					return (IMoValue[]) arrayStruct.Value;

				case VariableStruct variableStruct:
					return variableStruct.Map.Values;

				default:
					return null;
			}
		}
	}
}