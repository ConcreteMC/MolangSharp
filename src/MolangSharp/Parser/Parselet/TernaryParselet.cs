using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the Ternary expression parser
	/// </summary>
	public class TernaryParselet : InfixParselet
	{
		/// <inheritdoc />
		public TernaryParselet() : base(Precedence.Conditional) { }

		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token, IExpression leftExpr)
		{
			if (parser.MatchToken(TokenType.Colon))
			{
				return new TernaryExpression(leftExpr, null, ParseElseBranch(parser));
			}

			// The "then" branch is delimited by the ':' so it may contain any expression, including another
			// conditional: 'A ? B ? C : D : E' groups as 'A ? (B ? C : D) : E'.
			IExpression thenExpr = parser.ParseExpression(Precedence.Anything);

			if (!parser.MatchToken(TokenType.Colon))
			{
				// Binary conditional: '<test> ? <if true>'.
				return new TernaryExpression(leftExpr, thenExpr, null);
			}

			return new TernaryExpression(leftExpr, thenExpr, ParseElseBranch(parser));
		}

		/// <summary>
		///		Parses the "else" branch one level below the conditional operator's own precedence, which makes the
		///		operator right-associative.
		/// </summary>
		/// <remarks>
		///		Required by the Molang versioned change for 1.18.10: "Previously nested conditional expressions like
		///		<c>A ? B : C ? D : E</c> would evaluate as <c>(A ? B : C) ? D : E</c>. Now they evaluate as
		///		<c>A ? B : (C ? D : E)</c>."
		/// </remarks>
		private IExpression ParseElseBranch(MoLangParser parser)
		{
			return parser.ParseExpression(Precedence - 1);
		}
	}
}