using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Alex.MoLang.Runtime.Exceptions;
using Alex.MoLang.Runtime.Value;
using Alex.MoLang.Utils;

namespace Alex.MoLang.Runtime.Struct
{
	public class ObjectStruct : IMoStruct
	{
		private object _instance;
		private readonly Dictionary<string, ValueAccessor> _properties;
		private readonly Dictionary<string, Func<object, MoParams, IMoValue>>
			_functions;

		private static readonly ConcurrentDictionary<Type, PropertyCache> PropertyCaches =
			new ConcurrentDictionary<Type, PropertyCache>();

		public ObjectStruct(object instance)
		{
			_instance = instance;

			var type = instance.GetType();

			var propCache = PropertyCaches.GetOrAdd(type, t => new PropertyCache(t));
			_properties = propCache.Properties;
			_functions = propCache.Functions;
		}

		/// <inheritdoc />
		public object Value => _instance;

		/// <inheritdoc />
		public void Set(MoPath key, IMoValue value)
		{
			//var index = key.IndexOf('.');

			if (!key.HasChildren)
			{
				if (_properties.TryGetValue(key.ToString(), out var accessor))
				{
					//Map.TryAdd(main, container = new VariableStruct());
					accessor.Set(_instance, value);

					return;
				}

				throw new MoLangRuntimeException($"Variable was not a struct: {key}", null);
			}

			string main = key.Value;

			if (!string.IsNullOrWhiteSpace(main))
			{
				//object vstruct = Get(main, MoParams.Empty);

				if (!_properties.TryGetValue(main, out var accessor))
				{
					//Map.TryAdd(main, container = new VariableStruct());
					throw new MoLangRuntimeException($"Variable was not a struct: {key}", null);
				}

				var container = accessor.Get(_instance);

				if (container is IMoStruct moStruct)
				{
					moStruct.Set(key.Next, value);
				}
				else
				{
					throw new MoLangRuntimeException($"Variable was not a struct: {key}", null);
				}

				//((IMoStruct) vstruct).Set(string.Join(".", segments), value);

				//Map[main] = (IMoStruct)vstruct;//.Add(main, (IMoStruct) vstruct);
			}
		}

		/// <inheritdoc />
		public IMoValue Get(MoPath key, MoParams parameters)
		{
			//var index = key.IndexOf('.');

			if (key.HasChildren)
			{
				string main = key.Value;

				if (!string.IsNullOrWhiteSpace(main))
				{
					IMoValue value = null; //Map[main];

					if (_properties.TryGetValue(main, out var accessor))
					{
						value = accessor.Get(_instance);
					}
					else if (_functions.TryGetValue(main, out var func))
					{
						value = func.Invoke(_instance, parameters);
					}
					else
					{
						return DoubleValue.Zero;
					}

					if (value is IMoStruct moStruct)
					{
						return moStruct.Get(key.Next, parameters);
					}

					return value;
				}
			}

			if (_properties.TryGetValue(key.ToString(), out var v))
				return v.Get(_instance);

			if (_functions.TryGetValue(key.ToString(), out var f))
				return f.Invoke(_instance, parameters);

			Debug.WriteLine($"({_instance.ToString()}) Unknown query: {key}");

			return DoubleValue.Zero;
		}

		/// <inheritdoc />
		public void Clear()
		{
			throw new NotImplementedException();
		}
	}
}