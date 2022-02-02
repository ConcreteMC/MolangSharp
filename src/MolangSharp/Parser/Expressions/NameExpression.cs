using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class NameExpression : Expression
	{
		public MoPath Name { get; set; }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return environment.GetValue(Name);
		}

		/// <inheritdoc />
		public override void Assign(MoScope scope, MoLangEnvironment environment, IMoValue value)
		{
			environment.SetValue(Name, value);
		}

		/// <inheritdoc />
		public NameExpression(string value)
		{
			Name = new MoPath(value);
		}

		public NameExpression(MoPath path)
		{
			Name = path;
		}
	}
}