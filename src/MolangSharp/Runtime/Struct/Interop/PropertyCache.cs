using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ConcreteMC.MolangSharp.Attributes;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime.Struct.Interop
{
	public class PropertyCache
	{
		public readonly IReadOnlyDictionary<string, ValueAccessor> Properties;

		public readonly IReadOnlyDictionary<string, Func<object, MoParams, IMoValue>> Functions;

		public PropertyCache(Type arg)
		{
			var properties = new Dictionary<string, ValueAccessor>(StringComparer.OrdinalIgnoreCase);
			var functions = new Dictionary<string, Func<object, MoParams, IMoValue>>(StringComparer.OrdinalIgnoreCase);

			ProcessMethods(arg, functions);
			ProcessProperties(arg, properties);

			Functions = functions;
			Properties = properties;
		}

		private static void ProcessMethods(IReflect type,
			IDictionary<string, Func<object, MoParams, IMoValue>> functions)
		{
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach (var method in methods)
			{
				var functionAttribute = method.GetCustomAttribute<MoFunctionAttribute>();

				if (functionAttribute == null)
					continue;

				// Resolve everything reflective once per type instead of on every call.
				var methodParameters = method.GetParameters();
				var plan = BuildParameterPlan(methodParameters);
				var invoker = AccessorCompiler.CompileInvoker(method, methodParameters);

				foreach (var name in functionAttribute.Name)
				{
					if (functions.ContainsKey(name))
					{
						Debug.WriteLine($"Duplicate function \'{name}\' in {type.ToString()}");

						continue;
					}

					IMoValue ExecuteMolangFunction(object instance, MoParams mo)
					{
						object[] parameters = plan.Length == 0 ? Array.Empty<object>() : new object[plan.Length];

						for (var index = 0; index < plan.Length; index++)
						{
							var parameter = plan[index];

							// A MoParams parameter receives the whole argument list, so it is always satisfiable.
							if (parameter.Kind == ArgumentKind.Parameters)
							{
								parameters[index] = mo;

								continue;
							}

							if (!mo.Contains(index))
							{
								if (!parameter.IsOptional)
									throw new MissingMethodException($"Missing parameter: {parameter.Name}");

								// Fill this and every remaining parameter with its declared default.
								for (var remaining = index; remaining < plan.Length; remaining++)
									parameters[remaining] = plan[remaining].MissingValue;

								break;
							}

							parameters[index] = ReadArgument(parameter.Kind, mo, index);
						}

						var result = invoker != null ? invoker(instance, parameters) : method.Invoke(instance, parameters);

						if (result != null)
						{
							if (result is IMoValue moValue) return moValue;

							return MoValue.FromObject(result);
						}

						return DoubleValue.Zero;
					}

					functions.Add(name, ExecuteMolangFunction);
				}
			}
		}

		/// <summary>
		///		How a single method parameter is filled from a <see cref="MoParams"/>
		/// </summary>
		private enum ArgumentKind
		{
			Parameters,
			Int,
			Double,
			Float,
			String,
			Struct,
			Environment
		}

		/// <summary>
		///		Everything needed to fill one parameter, resolved once when the type is cached
		/// </summary>
		private readonly struct ParameterPlan
		{
			public readonly ArgumentKind Kind;
			public readonly bool IsOptional;
			public readonly string Name;

			/// <summary>
			///		The value to pass when the expression did not supply this optional argument
			/// </summary>
			public readonly object MissingValue;

			public ParameterPlan(ArgumentKind kind, ParameterInfo parameter)
			{
				Kind = kind;
				IsOptional = parameter.IsOptional;
				Name = parameter.Name;

				if (parameter.HasDefaultValue)
					MissingValue = parameter.DefaultValue;
				else if (parameter.ParameterType.IsValueType)
					MissingValue = Activator.CreateInstance(parameter.ParameterType);
				else
					MissingValue = null;
			}
		}

		private static ParameterPlan[] BuildParameterPlan(ParameterInfo[] parameters)
		{
			if (parameters.Length == 0)
				return Array.Empty<ParameterPlan>();

			var plan = new ParameterPlan[parameters.Length];

			for (var index = 0; index < parameters.Length; index++)
				plan[index] = new ParameterPlan(GetArgumentKind(parameters[index].ParameterType), parameters[index]);

			return plan;
		}

		private static ArgumentKind GetArgumentKind(Type type)
		{
			if (type == typeof(MoParams))
				return ArgumentKind.Parameters;

			if (type == typeof(int))
				return ArgumentKind.Int;

			if (type == typeof(double))
				return ArgumentKind.Double;

			if (type == typeof(float))
				return ArgumentKind.Float;

			if (type == typeof(string))
				return ArgumentKind.String;

			if (typeof(IMoStruct).IsAssignableFrom(type))
				return ArgumentKind.Struct;

			if (typeof(MoLangEnvironment).IsAssignableFrom(type))
				return ArgumentKind.Environment;

			throw new Exception("Unknown parameter type.");
		}

		private static object ReadArgument(ArgumentKind kind, MoParams mo, int index)
		{
			switch (kind)
			{
				case ArgumentKind.Int:
					return mo.GetInt(index);

				case ArgumentKind.Double:
					return mo.GetDouble(index);

				case ArgumentKind.Float:
					return (float) mo.GetDouble(index);

				case ArgumentKind.String:
					return mo.GetString(index);

				case ArgumentKind.Struct:
					return mo.GetStruct(index);

				case ArgumentKind.Environment:
					return mo.GetEnv(index);

				default:
					return mo;
			}
		}

		private static void ProcessProperties(IReflect type, IDictionary<string, ValueAccessor> valueAccessors)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in properties)
			{
				foreach (var functionAttribute in prop.GetCustomAttributes<MoPropertyAttribute>())
				{
					if (valueAccessors.ContainsKey(functionAttribute.Name))
						continue;

					var accessor = new PropertyAccessor(prop);

					if (prop.GetCustomAttribute<MoObservableAttribute>() != null)
						accessor.Observable = true;

					valueAccessors.Add(functionAttribute.Name, accessor);
				}
			}

			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in fields)
			{
				foreach (var functionAttribute in prop.GetCustomAttributes<MoPropertyAttribute>())
				{
					if (valueAccessors.ContainsKey(functionAttribute.Name))
						continue;

					var accessor = new FieldAccessor(prop);

					if (prop.GetCustomAttribute<MoObservableAttribute>() != null)
						accessor.Observable = true;

					valueAccessors.Add(functionAttribute.Name, accessor);
				}
			}
		}
	}
}