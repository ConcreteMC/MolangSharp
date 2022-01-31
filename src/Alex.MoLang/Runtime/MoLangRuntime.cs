using System;
using System.Collections.Generic;
using Alex.MoLang.Parser;
using Alex.MoLang.Runtime.Exceptions;
using Alex.MoLang.Runtime.Struct;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Runtime
{
	public class MoLangRuntime
	{
		public MoLangEnvironment Environment { get; }

		public MoLangRuntime() : this(new MoLangEnvironment()) { }

		public MoLangRuntime(MoLangEnvironment environment)
		{
			Environment = environment;
		}
		
		public IMoValue Execute(params IExpression[] expressions)
		{
			return Execute(expressions, null);
		}

		public IMoValue Execute(IExpression[] expressions, IDictionary<string, IMoValue> context)
		{
			if (expressions == null)
				return DoubleValue.Zero;
			//try
			//{

			//expressions = _exprTraverser.Traverse(expressions);

			if (Environment.Structs.TryGetValue("context", out IMoStruct cont) && cont is ContextStruct contextStruct)
			{
				contextStruct.Container = context;
			}

			IMoValue result = null;
			MoScope scope = new MoScope();

			foreach (IExpression expression in expressions)
			{
				if (expression == null)
					continue;

				try
				{
					result = expression.Evaluate(scope, Environment);

					if (scope.ReturnValue != null)
					{
						result = scope.ReturnValue;

						break;
					}
				}
				catch (Exception ex)
				{
					throw new MoLangRuntimeException(
						expression, "An error occured while evaluating the expression", ex);
				}
			}

			Environment.Structs["temp"].Clear();
			//Environment.Structs.Remove("context", out _);

			return result ?? DoubleValue.Zero;
		}
	}
}