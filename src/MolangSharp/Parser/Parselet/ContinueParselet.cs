using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the "continue" instruction parser
	/// </summary>
	public class ContinueParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			return new ContinueExpression();
		}
	}
}