using System;
using System.Linq.Expressions;
using System.Reflection;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime.Struct.Interop
{
	/// <summary>
	///		Turns reflection metadata into compiled delegates
	/// </summary>
	/// <remarks>
	///		<see cref="PropertyInfo.GetValue(object)"/> and <see cref="MethodBase.Invoke(object, object[])"/> cost
	///		hundreds of nanoseconds per call, which dominates the cost of reading a query in an animation expression.
	///		Compiling the access once per type removes that overhead, and building the <see cref="IMoValue"/> inside
	///		the expression tree means a numeric property never gets boxed on the way out.
	///		<br/>
	///		Every method here falls back to plain reflection when the runtime cannot compile expression trees.
	/// </remarks>
	internal static class AccessorCompiler
	{
		/// <summary>
		///		Compiles a delegate that reads <paramref name="member"/> and wraps the result in an
		///		<see cref="IMoValue"/>
		/// </summary>
		/// <returns>The compiled getter, or null when one could not be built</returns>
		public static Func<object, IMoValue> CompileGetter(MemberInfo member, Type valueType)
		{
			try
			{
				var declaringType = member.DeclaringType;

				if (declaringType == null)
					return null;

				var instance = Expression.Parameter(typeof(object), "instance");
				Expression access = Expression.MakeMemberAccess(Expression.Convert(instance, declaringType), member);

				return Expression.Lambda<Func<object, IMoValue>>(WrapAsMoValue(access, valueType), instance).Compile();
			}
			catch (Exception)
			{
				// Platforms without runtime code generation fall back to reflection.
				return null;
			}
		}

		/// <summary>
		///		Compiles a delegate that writes a boxed CLR value to <paramref name="member"/>
		/// </summary>
		/// <returns>The compiled setter, or null when one could not be built</returns>
		public static Action<object, object> CompileSetter(MemberInfo member, Type valueType)
		{
			try
			{
				var declaringType = member.DeclaringType;

				if (declaringType == null)
					return null;

				var instance = Expression.Parameter(typeof(object), "instance");
				var value = Expression.Parameter(typeof(object), "value");

				var assignment = Expression.Assign(
					Expression.MakeMemberAccess(Expression.Convert(instance, declaringType), member),
					Expression.Convert(value, valueType));

				return Expression.Lambda<Action<object, object>>(assignment, instance, value).Compile();
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		///		Compiles a delegate that invokes <paramref name="method"/> with an argument array
		/// </summary>
		/// <returns>The compiled invoker, or null when one could not be built</returns>
		public static Func<object, object[], object> CompileInvoker(MethodInfo method, ParameterInfo[] parameters)
		{
			try
			{
				var declaringType = method.DeclaringType;

				if (declaringType == null)
					return null;

				var instance = Expression.Parameter(typeof(object), "instance");
				var arguments = Expression.Parameter(typeof(object[]), "arguments");

				var argumentExpressions = new Expression[parameters.Length];

				for (var i = 0; i < parameters.Length; i++)
				{
					argumentExpressions[i] = Expression.Convert(
						Expression.ArrayIndex(arguments, Expression.Constant(i)), parameters[i].ParameterType);
				}

				Expression call = Expression.Call(
					Expression.Convert(instance, declaringType), method, argumentExpressions);

				// A void method still has to hand something back to the caller.
				call = method.ReturnType == typeof(void)
					? (Expression) Expression.Block(call, Expression.Constant(null, typeof(object)))
					: Expression.Convert(call, typeof(object));

				return Expression.Lambda<Func<object, object[], object>>(call, instance, arguments).Compile();
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		///		Builds the expression that turns a CLR value into an <see cref="IMoValue"/>
		/// </summary>
		/// <remarks>
		///		The well known primitive types get their wrapper constructed inline so the value is never boxed.
		/// </remarks>
		private static Expression WrapAsMoValue(Expression value, Type valueType)
		{
			if (typeof(IMoValue).IsAssignableFrom(valueType))
				return Expression.Convert(value, typeof(IMoValue));

			if (valueType == typeof(string))
				return Expression.New(StringValueConstructor, value);

			if (valueType == typeof(bool))
				return Expression.New(DoubleValueFromBool, value);

			if (valueType == typeof(double))
				return Expression.New(DoubleValueFromDouble, value);

			if (valueType == typeof(float) || valueType == typeof(int) || valueType == typeof(long)
			    || valueType == typeof(short) || valueType == typeof(byte) || valueType == typeof(decimal))
				return Expression.New(DoubleValueFromDouble, Expression.Convert(value, typeof(double)));

			return Expression.Call(FromObjectMethod, Expression.Convert(value, typeof(object)));
		}

		private static readonly ConstructorInfo StringValueConstructor =
			typeof(StringValue).GetConstructor(new[] {typeof(string)});

		private static readonly ConstructorInfo DoubleValueFromBool =
			typeof(DoubleValue).GetConstructor(new[] {typeof(bool)});

		private static readonly ConstructorInfo DoubleValueFromDouble =
			typeof(DoubleValue).GetConstructor(new[] {typeof(double)});

		private static readonly MethodInfo FromObjectMethod =
			typeof(MoValue).GetMethod(nameof(MoValue.FromObject), new[] {typeof(object)});
	}
}
