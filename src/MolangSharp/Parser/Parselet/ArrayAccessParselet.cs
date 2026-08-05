using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the indexer/array accessor parser
	/// </summary>
	/// <remarks>
	///		Parses expressions such as "array[0]"
	/// </remarks>
	public class ArrayAccessParselet : InfixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token, IExpression leftExpr)
		{
			// The index is delimited by the closing bracket, so it may contain any expression.
			IExpression index = parser.ParseExpression();
			parser.ConsumeToken(TokenType.ArrayRight);

			return new ArrayAccessExpression(leftExpr, index);
		}

		/// <inheritdoc />
		public ArrayAccessParselet() : base(Precedence.ArrayAccess) { }
	}
}