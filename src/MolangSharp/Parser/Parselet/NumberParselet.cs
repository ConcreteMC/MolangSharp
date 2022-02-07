using System.Globalization;
using ConcreteMC.MolangSharp.Parser.Exceptions;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements number parsing
	/// </summary>
	public class NumberParselet : PrefixParselet
	{
		private const NumberStyles NumberStyle = System.Globalization.NumberStyles.AllowDecimalPoint;
		private static readonly CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (double.TryParse(token.Text, NumberStyle, Culture, out var result))
			{
				return new NumberExpression(result);
			}

			throw new MoLangParserException($"Could not parse \'{token.Text.ToString()}\' as a double");
		}
	}
}