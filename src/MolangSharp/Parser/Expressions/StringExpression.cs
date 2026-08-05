using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class StringExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return _value;
		}

		private readonly StringValue _value;

		/// <inheritdoc />
		public StringExpression(string value) : base()
		{
			// A literal never changes, so the wrapper is built once instead of on every evaluation.
			_value = new StringValue(value);
		}
	}
}