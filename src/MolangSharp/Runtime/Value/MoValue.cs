using System;
using System.Collections;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime.Struct;

namespace ConcreteMC.MolangSharp.Runtime.Value
{
	/// <summary>
	///		Resembles a MoLang compatible value. 
	/// </summary>
	public interface IMoValue : IEquatable<IMoValue>
	{
		object Value { get; }

		virtual string AsString() => Value.ToString();

		/// <remarks>
		///		Booleans are folded to 1.0/0.0 here as well, so that a numeric test and
		///		<see cref="AsBool"/> always agree.
		/// </remarks>
		virtual double AsDouble() => Value is double db ? db : Value is bool bl ? (bl ? 1d : 0d) : 0d;

		virtual float AsFloat() => Value is float flt ? flt : (float) AsDouble();

		/// <remarks>
		///		"For boolean tests, a float value equivalent to 0.0 is false, and anything not equal to 0.0 is true."
		/// </remarks>
		virtual bool AsBool() => Value is bool b ? b : AsDouble() != 0d;
	}

	public static class MoValue
	{
		public static IMoValue FromObject(object value)
		{
			if (value is IMoValue moValue)
			{
				return moValue;
			}

			if (value is string str)
			{
				return new StringValue(str);
			}

			if (value is IEnumerable enumerable)
			{
				List<IMoValue> values = new List<IMoValue>();

				foreach (var enumObject in enumerable)
				{
					values.Add(FromObject(enumObject));
				}

				return new ArrayStruct(values);
			}

			return new DoubleValue(value);
		}
	}
}