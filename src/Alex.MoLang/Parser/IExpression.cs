using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Alex.MoLang.Parser.Tokenizer;
using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Parser
{
	public interface IExpression
	{
		ExpressionMeta Meta { get; }

		IMoValue Evaluate(MoScope scope, MoLangEnvironment environment);

		void Assign(MoScope scope, MoLangEnvironment environment, IMoValue value);

		IExpression[] Parameters { get; set; }
	}

	public class ExpressionMeta
	{
		public Token Token { get; set; }
		public IExpression Parent { get; set; }
		public IExpression Previous { get; set; }
		public IExpression Next { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(255);
			bool includeFileInfoIfAvailable;

			if (Token != null)
			{
				sb.Append(Token.Text);
				includeFileInfoIfAvailable = true;
			}
			else
			{
				includeFileInfoIfAvailable = false;
			}

			if (includeFileInfoIfAvailable)
			{
				//	sb.Append(" at offset ");

				//	if (_nativeOffset == OFFSET_UNKNOWN)
				//		sb.Append("<offset unknown>");
				//	else
				//		sb.Append(_nativeOffset);

				sb.Append(" in file:line:column ");
				sb.Append("<filename unknown>");
				sb.Append(':');
				sb.Append(Token.Position.LineNumber);
				sb.Append(':');
				sb.Append(Token.Position.Index);
			}
			else
			{
				sb.Append("<null>");
			}

			sb.AppendLine();

			return sb.ToString();
		}
	}

	public abstract class Expression : IExpression
	{
		public IExpression[] Parameters { get; set; }

		/// <inheritdoc />
		public ExpressionMeta Meta { get; } = new ExpressionMeta();

		public Expression(params IExpression[] parameters)
		{
			Parameters = parameters;
		}
		
		/// <inheritdoc />
		public abstract IMoValue Evaluate(MoScope scope, MoLangEnvironment environment);

		/// <inheritdoc />
		public virtual void Assign(MoScope scope, MoLangEnvironment environment, IMoValue value)
		{
			throw new Exception("Cannot assign a value to " + this.GetType());
		}
	}

	public abstract class Expression<T> : Expression
	{
		protected Expression(T value) : base()
		{
			Value = value;
		}

		public T Value { get; set; }
	}
}