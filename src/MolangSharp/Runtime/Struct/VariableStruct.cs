using System;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Runtime.Struct
{
	/// <summary>
	///		Represents a dynamic variable
	/// </summary>
	public class VariableStruct : IMoStruct
	{
		public IDictionary<string, IMoValue> Map { get; protected set; }

		/// <inheritdoc />
		public object Value => Map;

		public VariableStruct()
		{
			Map = new Dictionary<string, IMoValue>(StringComparer.OrdinalIgnoreCase);
		}

		public VariableStruct(IEnumerable<KeyValuePair<string, IMoValue>> values)
		{
			if (values != null)
			{
				Map = new Dictionary<string, IMoValue>(values, StringComparer.OrdinalIgnoreCase);
			}
		}

		protected virtual IMoStruct CreateNew()
		{
			return new VariableStruct();
		}

		/// <inheritdoc />
		public virtual void Set(MoPath key, IMoValue value)
		{
			//var index = key.IndexOf('.');

			if (!key.HasChildren)
			{
				Map[key.Value] = value;

				return;
			}

			string main = key.Value;

			if (!string.IsNullOrWhiteSpace(main))
			{
				//object vstruct = Get(main, MoParams.Empty);

				if (!Map.TryGetValue(main, out var container))
				{
					if (!key.HasChildren)
					{
						//Map.TryAdd(main, container = new VariableStruct());
						throw new MoLangRuntimeException($"Variable was not a struct: {key}", null);
					}

					Map.TryAdd(main, container = CreateNew());
				}

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
		public virtual IMoValue Get(MoPath key, MoParams parameters)
		{
			//var index = key.IndexOf('.');

			if (key.HasChildren)
			{
				string main = key.Value;

				if (!string.IsNullOrWhiteSpace(main))
				{
					IMoValue value = null; //Map[main];

					if (!Map.TryGetValue(main, out value))
					{
						//		Log.Info($"Unknown variable map: {key}");
						return DoubleValue.Zero;
					}

					if (value is IMoStruct moStruct)
					{
						return moStruct.Get(key.Next, parameters);
					}

					return value;
				}
			}

			if (Map.TryGetValue(key.Value, out var v))
				return v;

			//
			//	Log.Info($"Unknown variable: {key}");

			return DoubleValue.Zero;
		}

		/// <inheritdoc />
		public void Clear()
		{
			Map.Clear();
		}
	}
}