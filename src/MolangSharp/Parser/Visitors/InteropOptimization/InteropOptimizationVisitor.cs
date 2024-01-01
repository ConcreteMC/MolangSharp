using System;
using System.Linq;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct.Interop;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Parser.Visitors.InteropOptimization
{
	/// <summary>
	///		Optimizes interop calls to call the methods or properties directly instead of going through the MoLang query system.
	/// </summary>
	/// <remarks>
	///		This is a very simple optimization that can be done to improve performance, it does come with some limitations however.<br/>
	///		These limitations are: <br/>
	///		- You need to pass in the object instance to the visitor options, this is because the visitor cannot know what object you are trying to access. <br/>
	///		- You cannot access properties or functions that are not directly on the object, for example: <code>query.myThing.blabla</code> will not work, but <code>query.blabla</code> will work. <br/>
	/// </remarks>
	public class InteropOptimizationVisitor : ExpressionVisitor
	{
		private readonly InteropOptimizationVisitorOptions _options;

		public InteropOptimizationVisitor(InteropOptimizationVisitorOptions options)
		{
			options.Validate();
			
			_options = options;
		}
		
		/// <inheritdoc />
		public override IExpression OnVisit(ExpressionTraverser traverser, IExpression expression)
		{
			switch (expression)
			{
				case FuncCallExpression functionCall:
					return _options.OptimizeFunctionCalls ? TryOptimizeFunctionCall(functionCall) : functionCall;

				case NameExpression nameExpression:
					return _options.OptimizeVariableAccess ? TryOptimizeNameExpression(nameExpression) : nameExpression;
				
				case AssignExpression assignExpression:
					return _options.OptimizeVariableAssignment ? TryOptimizeAssignExpression(assignExpression) : assignExpression;

				default:
					return expression;
			}
		}
		
		private IExpression TryOptimizeAssignExpression(AssignExpression expression)
		{
			//Optimize variable assignment to call the method directly.
			var variableToAccess = expression.Variable;

			if (variableToAccess is NameExpression nameExpression)
			{
				var name = nameExpression.Name.Value;
				var interopEntry = _options.InteropEntries.FirstOrDefault(x => x.Name == name);

				if (interopEntry == null)
					return expression;

				if (TryGetProperty(interopEntry.Cache, nameExpression.Name.Next, out var valueAccessor))
				{
					return new OptimizedAssignExpression(interopEntry.Instance, valueAccessor, expression.Expression);
				}
			}
			
			return expression;
		}
		
		private IExpression TryOptimizeNameExpression(NameExpression expression)
		{
			//Optimize variable access to call the method directly.
			
			var name = expression.Name.Value;
			var interopEntry = _options.InteropEntries.FirstOrDefault(x => x.Name == name);

			if (interopEntry == null)
				return expression;
			
			if (TryGetProperty(interopEntry.Cache, expression.Name.Next, out var valueAccessor))
				return new OptimizedNameExpression(interopEntry.Instance, valueAccessor);
			
			return expression;
		}

		private IExpression TryOptimizeFunctionCall(FuncCallExpression expression)
		{
			//Optimize function call to call the method directly.
			var name = expression.Name.Value;
			var interopEntry = _options.InteropEntries.FirstOrDefault(x => x.Name == name);

			if (interopEntry == null)
				return expression;

			if (TryGetFunction(interopEntry.Cache, expression.Name.Next, out var function))
				return new OptimizedFunctionCallExpression(interopEntry.Instance, function, expression.Parameters);
			
			return expression;
		}

		private bool TryGetFunction(PropertyCache cache, MoPath path, out Func<object, MoParams, IMoValue> function)
		{
			if (path.HasChildren)
			{
				//Possibly trying to access another level in, this is not currently supported.
				function = null;
				return false;
			}
			
			if (cache.Functions.TryGetValue(path.Value, out var f))
			{
				function = f;
				return true;
			}
			
			function = null;
			return false;
		}
		
		private bool TryGetProperty(PropertyCache cache, MoPath path, out ValueAccessor valueAccessor)
		{
			if (path.HasChildren)
			{
				//Possibly trying to access another level in, this is not currently supported.
				valueAccessor = null;
				return false;
			}
			
			if (cache.Properties.TryGetValue(path.Value, out var f))
			{
				valueAccessor = f;
				return true;
			}
			
			valueAccessor = null;
			return false;
		}
	}

	public class OptimizedAssignExpression : Expression
	{
		private readonly object _instance;
		private readonly ValueAccessor _valueAccessor;
		private readonly IExpression _expressionExpression;

		public OptimizedAssignExpression(object instance, ValueAccessor valueAccessor, IExpression expressionExpression)
		{
			_instance = instance;
			_valueAccessor = valueAccessor;
			_expressionExpression = expressionExpression;
		}

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			_valueAccessor.Set(_instance, _expressionExpression.Evaluate(scope, environment));

			return DoubleValue.Zero;
		}
	}

	public class OptimizedFunctionCallExpression : Expression
	{
		private readonly object _instance;
		private readonly Func<object, MoParams, IMoValue> _function;

		public OptimizedFunctionCallExpression(object instance, Func<object, MoParams, IMoValue> function, IExpression[] args) : base(args)
		{
			_instance = instance;
			_function = function;
		}
		
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			var arguments = new IMoValue[Parameters.Length];

			for (int i = 0; i < arguments.Length; i++)
			{
				arguments[i] = Parameters[i].Evaluate(scope, environment);
			}
			
			return _function.Invoke(_instance, new MoParams(arguments));
		}
	}
	
	public class OptimizedNameExpression : Expression
	{
		private readonly object _instance;
		private readonly ValueAccessor _valueAccessor;

		public OptimizedNameExpression(object instance, ValueAccessor valueAccessor) : base()
		{
			_instance = instance;
			_valueAccessor = valueAccessor;
		}
		
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return _valueAccessor.Get(_instance);
		}
	}
}