namespace ConcreteMC.MolangSharp.Parser.Tokenizer
{
	public class TokenIterator : ITokenIterator
	{
		private readonly string _code;

		private int _index = 0;
		private int _currentLine = 0;
		private int _lastStep = 0;
		private int _lastStepLine = 0;

		public TokenIterator(string code)
		{
			this._code = code;
		}

		public Token Next()
		{
			while (_index < _code.Length)
			{
				var expr = _code[_index];

				if (_code.Length > _index + 1)
				{
					// check tokens with double chars
					TokenType token = TokenType.BySymbol(expr, _code[_index + 1]);

					if (token != null)
					{
						_index += 2;

						return new Token(token, GetPosition());
					}
				}

				TokenType tokenType = TokenType.BySymbol(expr);

				if (tokenType != null)
				{
					_index++;

					return new Token(tokenType, GetPosition());
				}
				else
				{
					if (expr.Equals('\''))
					{
						int stringStart = _index + 1;
						int stringLength = 0;

						while (stringStart + stringLength < _code.Length
						       && _code[stringStart + stringLength] != '\'')
						{
							stringLength++;
						}

						stringLength++;
						_index = stringStart + stringLength;

						return new Token(
							TokenType.String, _code.Substring(stringStart, stringLength - 1), GetPosition());
					}

					if (char.IsLetter(expr))
					{
						var nameStart = _index;
						int nameLength = 1;

						while (nameStart + nameLength < _code.Length)
						{
							var next = _code[nameStart + nameLength];

							if (!char.IsLetterOrDigit(next) && next != '_' && next != '.')
								break;

							nameLength++;
						}

						string value = _code.Substring(_index, nameLength);
						TokenType token = TokenType.BySymbol(value);

						if (token == null)
						{
							token = TokenType.Name;
						}

						_index = nameStart + nameLength;

						return new Token(token, value, GetPosition());
					}

					if (char.IsDigit(expr))
					{
						int numStart = _index;
						int numLength = 1;
						bool hasDecimal = false;
						bool isFloat = false;


						while (numStart + numLength < _code.Length)
						{
							var character = _code[numStart + numLength];

							if (!char.IsDigit(character))
							{
								if ((character == 'f' || character == 'F') && !isFloat)
								{
									isFloat = true;
								}
								else if (character == '.' && !hasDecimal)
								{
									hasDecimal = true;
								}
								else
								{
									break;
								}
							}

							numLength++;

							if (isFloat)
								break;
						}

						_index = numStart + numLength;

						if (isFloat)
							return new Token(
								TokenType.FloatingPointNumber, _code.Substring(numStart, numLength - 1), GetPosition());

						return new Token(TokenType.Number, _code.Substring(numStart, numLength), GetPosition());
					}

					if (expr.Equals('\n') || expr.Equals('\r'))
					{
						_currentLine++;
					}
				}

				_index++;
			}

			return new Token(TokenType.Eof, GetPosition());
		}

		public void Step()
		{
			_lastStep = _index;
			_lastStepLine = _currentLine;
		}

		private TokenPosition GetPosition()
		{
			return new TokenPosition(_lastStepLine, _currentLine, _lastStep, _index);
		}
	}
}