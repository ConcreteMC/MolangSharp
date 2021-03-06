using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the "break" instruction parser
	/// </summary>
	public class BreakParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			return new BreakExpression();
		}
	}
}