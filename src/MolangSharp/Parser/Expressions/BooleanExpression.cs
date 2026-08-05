using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class BooleanExpression : Expression
	{
		private readonly IMoValue _value;
		private readonly double _asDouble;

		public BooleanExpression(bool value) : base()
		{
			_value = value ? DoubleValue.One : DoubleValue.Zero;
			_asDouble = value ? 1d : 0d;
		}

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
	}
}