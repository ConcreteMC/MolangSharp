using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	public class ThisParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			return new ThisExpression();
		}
	}
}