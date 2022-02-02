using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class StringExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new StringValue(_value);
		}

		private string _value;

		/// <inheritdoc />
		public StringExpression(string value) : base()
		{
			_value = value;
		}
	}
}