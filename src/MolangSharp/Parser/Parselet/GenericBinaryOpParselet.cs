using ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Generic binary operator parselet
	/// </summary>
	public class GenericBinaryOpParselet : InfixParselet
	{
		private readonly bool _rightAssociative;

		/// <inheritdoc />
		public GenericBinaryOpParselet(Precedence precedence) : this(precedence, false) { }

		/// <summary>
		///		Generic binary operator parselet
		/// </summary>
		/// <param name="precedence">The binding power of the operator</param>
		/// <param name="rightAssociative">
		///		When true, an operator of equal precedence to the right binds into this operator's right hand side, so
		///		that <c>a ?? b ?? c</c> groups as <c>a ?? (b ?? c)</c>
		/// </param>
		public GenericBinaryOpParselet(Precedence precedence, bool rightAssociative) : base(precedence)
		{
			_rightAssociative = rightAssociative;
		}

		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token, IExpression leftExpr)
		{
			IExpression rightExpr = parser.ParseExpression(_rightAssociative ? Precedence - 1 : Precedence);

			if (token.Type.Equals(TokenType.Arrow))
				return new ArrowExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.And))
				return new BooleanAndExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Or))
				return new BooleanOrExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Coalesce))
				return new CoalesceExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Slash))
				return new DivideExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.EqualsEquals))
				return new EqualExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Greater))
				return new GreaterExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.GreaterOrEquals))
				return new GreaterOrEqualExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Minus))
				return new MinusExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.NotEquals))
				return new NotEqualExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Plus))
				return new PlusExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Asterisk))
				return new PowExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.Smaller))
				return new SmallerExpression(leftExpr, rightExpr);

			if (token.Type.Equals(TokenType.SmallerOrEquals))
				return new SmallerOrEqualExpression(leftExpr, rightExpr);

			return null;
		}
	}
}