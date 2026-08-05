using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class ArrayAccessExpression : Expression
	{
		public IExpression Array => Parameters[0];
		public IExpression Index => Parameters[1];

		public ArrayAccessExpression(IExpression array, IExpression index) : base(array, index) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			var index = (int) Index.EvaluateDouble(scope, environment);
			var array = environment.GetValue(GetPath(scope, environment));

			if (array is ArrayStruct asArray)
				return asArray[index];

			return array;
		}

		/// <inheritdoc />
		public override void Assign(MoScope scope, MoLangEnvironment environment, IMoValue value)
		{
			var index = (int) Index.EvaluateDouble(scope, environment);
			var array = environment.GetValue(GetPath(scope, environment));

			if (array is ArrayStruct asArray)
			{
				asArray[index] = value;
			}
		}

		private MoPath GetPath(MoScope scope, MoLangEnvironment environment)
		{
			// The common case is indexing a named array, whose path was already resolved while parsing.
			if (Array is NameExpression nameExpression)
				return nameExpression.Name;

			return new MoPath(Array.Evaluate(scope, environment).AsString());
		}
	}
}