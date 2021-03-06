using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the unary plus parser
	/// </summary>
	public class UnaryPlusParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			return new UnaryPlusExpression(parser.ParseExpression(Precedence.Prefix));
		}
	}
}