using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	public class ArrayAccessParselet : InfixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token, IExpression leftExpr)
		{
			IExpression index = parser.ParseExpression(Precedence);
			parser.ConsumeToken(TokenType.ArrayRight);

			return new ArrayAccessExpression(leftExpr, index);
		}

		/// <inheritdoc />
		public ArrayAccessParselet() : base(Precedence.ArrayAccess) { }
	}
}