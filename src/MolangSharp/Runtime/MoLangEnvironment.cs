using System;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Runtime
{
	/// <summary>
	///		Provides an execution environment for the <see cref="MoLangRuntime"/>
	/// </summary>
	/// <remarks>
	///		An example of a MoLangEnvironment would be a minecraft entity.
	/// </remarks>
	public class MoLangEnvironment : IMoValue
	{
		/// <inheritdoc />
		public object Value => Structs;

		public Dictionary<string, IMoStruct> Structs { get; } =
			new Dictionary<string, IMoStruct>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		///		The value that should be returned when an expression tries to access "this"
		/// </summary>
		public IMoValue ThisVariable { get; set; } = DoubleValue.Zero;

		/// <summary>
		///		Creates a new instance of the MoLangEnvironment class
		/// </summary>
		public MoLangEnvironment()
		{
			Structs.TryAdd("math", MoLangMath.Library);
			Structs.TryAdd("temp", new VariableStruct());
			Structs.TryAdd("variable", new VariableStruct());
			Structs.TryAdd("array", new VariableArrayStruct());

			Structs.TryAdd("context", new ContextStruct());
		}

		public IMoValue GetValue(MoPath name)
		{
			return GetValue(name, MoParams.Empty);
		}

		public IMoValue GetValue(MoPath name, MoParams param)
		{
			try
			{
				return Structs[name.Value].Get(name.Next, param);
			}
			catch (Exception ex)
			{
				// Experimental
				if (MoLangRuntimeConfiguration.UseDummyValuesInsteadOfExceptions)
					return DoubleValue.Zero;

				throw new MoLangRuntimeException($"Cannot retrieve struct: {name}", ex);
			}
		}

		public void SetValue(MoPath name, IMoValue value)
		{
			if (!Structs.TryGetValue(name.Value, out var v))
			{
				throw new MoLangRuntimeException($"Invalid path: {name.Path}", null);
			}

			try
			{
				v.Set(name.Next, value);
			}
			catch (Exception ex)
			{
				throw new MoLangRuntimeException($"Cannot set value on struct: {name}", ex);
			}
		}

		/// <inheritdoc />
		public bool Equals(IMoValue b)
		{
			return Equals((object) b);
		}
	}
}