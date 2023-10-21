using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser
{
	public abstract class PrefixParselet
	{
		public abstract IExpression Parse(MoLangParser parser, Token token);
	}
}