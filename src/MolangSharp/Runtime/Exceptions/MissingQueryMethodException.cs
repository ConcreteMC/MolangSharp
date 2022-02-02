using System;
using ConcreteMC.MolangSharp.Parser;

namespace ConcreteMC.MolangSharp.Runtime.Exceptions
{
	public class MissingQueryMethodException : MoLangRuntimeException
	{
		/// <inheritdoc />
		public MissingQueryMethodException(string message, Exception baseException) : base(message, baseException) { }

		/// <inheritdoc />
		public MissingQueryMethodException(IExpression expression, string message, Exception baseException) : base(
			expression, message, baseException) { }
	}
}