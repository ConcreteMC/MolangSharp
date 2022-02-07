using System.Globalization;
using ConcreteMC.MolangSharp.Parser.Exceptions;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements Float parsing
	/// </summary>
	public class FloatParselet : PrefixParselet
	{
		private const NumberStyles NumberStyle = System.Globalization.NumberStyles.AllowDecimalPoint;
		private static readonly CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
		
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (float.TryParse(token.Text, NumberStyle, Culture, out var result))
			{
				return new NumberExpression(result);
			}

			throw new MoLangParserException($"Could not parse \'{token.Text.ToString()}\' as float");
		}
	}
}