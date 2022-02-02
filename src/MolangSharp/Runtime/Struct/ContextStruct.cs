using System;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Runtime.Struct
{
	public class ContextStruct : VariableStruct
	{
		internal IDictionary<string, IMoValue> Container
		{
			get
			{
				return Map;
			}
			set
			{
				Map = value;
			}
		}

		public ContextStruct() : base() { }

		public ContextStruct(IEnumerable<KeyValuePair<string, IMoValue>> values) : base(values) { }

		/// <inheritdoc />
		public override void Set(MoPath key, IMoValue value)
		{
			throw new NotSupportedException("Read-only context");
		}
	}
}