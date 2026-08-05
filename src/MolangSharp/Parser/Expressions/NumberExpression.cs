using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class NumberExpression : Expression
	{
		private readonly IMoValue _value;
		private readonly double _asDouble;

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return _value;
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return _asDouble;
		}

		/// <inheritdoc />
		public NumberExpression(double value) : base()
		{
			_value = new DoubleValue(value);
			_asDouble = value;
		}

		public NumberExpression(IMoValue value) : base()
		{
			_value = value;
			_asDouble = value.AsDouble();
		}
	}
}